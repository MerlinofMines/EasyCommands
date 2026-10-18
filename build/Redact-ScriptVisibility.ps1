#requires -Version 7.0
param(
    [Parameter(Mandatory = $true)]
    [string] $ScriptPath
)
$ErrorActionPreference = 'Stop'
# PowerShell 7 ships the Roslyn assemblies used by Add-Type. No extra package is needed.
Add-Type -Path (Join-Path $PSHOME 'Microsoft.CodeAnalysis.dll')
Add-Type -Path (Join-Path $PSHOME 'Microsoft.CodeAnalysis.CSharp.dll')
$ScriptPath = (Resolve-Path -LiteralPath $ScriptPath).Path
$scriptText = [IO.File]::ReadAllText($ScriptPath)
$prefix = "class Program {`n"
$parseOptions = [Microsoft.CodeAnalysis.CSharp.CSharpParseOptions]::Default.WithLanguageVersion(
    [Microsoft.CodeAnalysis.CSharp.LanguageVersion]::CSharp6)
$tree = [Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree]::ParseText($prefix + $scriptText + "`n}", $parseOptions)
if (@($tree.GetDiagnostics() | Where-Object Severity -eq Error).Count) {
    throw 'Cannot redact visibility: the packed script has C# syntax errors.'
}
$program = $tree.GetRoot().Members[0]
$preservedNames = @('Main', 'Save', 'LogLevel', 'updateFrequency', 'logLevel',
    'commandParseAmount', 'commandParameterParseAmount', 'maxAsyncThreads',
    'maxQueuedThreads', 'maxItemTransfers')
$edits = @()
foreach ($member in $program.Members) {
    # Only direct Program members are eligible. Public members inside nested types
    # remain accessible to other nested types, and host callbacks retain their API.
    if ($member -is [Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax] -or
        $member.Modifiers.Text -contains 'override' -or
        $member.Modifiers.Text -contains 'virtual' -or
        $member.Modifiers.Text -contains 'extern') { continue }
    $kind = $member.GetType().Name
    if ($kind -notin @('ClassDeclarationSyntax', 'StructDeclarationSyntax',
        'InterfaceDeclarationSyntax', 'EnumDeclarationSyntax', 'DelegateDeclarationSyntax',
        'FieldDeclarationSyntax', 'MethodDeclarationSyntax', 'PropertyDeclarationSyntax',
        'EventDeclarationSyntax', 'EventFieldDeclarationSyntax')) { continue }
    if ($kind -in @('FieldDeclarationSyntax', 'EventFieldDeclarationSyntax')) {
        if (@($member.Declaration.Variables | Where-Object { $_.Identifier.ValueText -in $preservedNames }).Count) { continue }
    } elseif ($member.Identifier.ValueText -in $preservedNames) { continue }
    foreach ($token in $member.Modifiers) {
        if ($token.Text -eq 'public') {
            $start = $token.SpanStart - $prefix.Length
            $length = $token.Span.Length
            if ([char]::IsWhiteSpace($scriptText[$start + $length])) { $length++ }
            $edits += [pscustomobject]@{ Start = $start; Length = $length; TokenPosition = $token.SpanStart }
        }
    }
}
$redacted = $scriptText
foreach ($edit in ($edits | Sort-Object Start -Descending)) {
    $redacted = $redacted.Remove($edit.Start, $edit.Length)
}
# Verify that every other syntax token (including literals) is unchanged before writing.
$removedPositions = [Collections.Generic.HashSet[int]]::new()
foreach ($edit in $edits) { [void]$removedPositions.Add($edit.TokenPosition) }
$expected = @($tree.GetRoot().DescendantTokens() | Where-Object { -not $removedPositions.Contains($_.SpanStart) } | ForEach-Object Text)
$after = [Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree]::ParseText($prefix + $redacted + "`n}", $parseOptions)
$actual = @($after.GetRoot().DescendantTokens() | ForEach-Object Text)
if (@($after.GetDiagnostics() | Where-Object Severity -eq Error).Count -or
    ($expected -join [char]0) -cne ($actual -join [char]0)) {
    throw 'Visibility redaction changed unexpected syntax; packed script was not written.'
}
if ($edits.Count) { [IO.File]::WriteAllText($ScriptPath, $redacted, [Text.UTF8Encoding]::new($false)) }
Write-Host "Visibility redaction: $($edits.Count) public modifiers removed; $($scriptText.Length - $redacted.Length) characters saved; $($redacted.Length) characters total."

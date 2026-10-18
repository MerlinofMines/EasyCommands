#requires -Version 7.0
$ErrorActionPreference = 'Stop'
$testRoot = Join-Path (Split-Path -Parent $PSScriptRoot) ('.build/visibility-tests/' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $testRoot | Out-Null
$scriptPath = Join-Path $testRoot 'script.cs'
$source = @'
#region preserved trivia
public Program() {}
public void Main(string argument) {}
public void Save() {}
public override string ToString() { return "public class Hidden{}"; }
public enum LogLevel { INFO }
public int updateFrequency, maxQueuedThreads;
public int state;
public T Resolve<T>(T value) { return value; }
public class Helper { public Helper() {} public int value; }
public interface IHelper { void Work(); }
public delegate int Getter();
public enum Kind { FIRST }
public /* keep this comment */ class Commented {}
public static implicit operator int(Program p) { return 0; }
#endregion
// final comment without a newline
'@
[IO.File]::WriteAllText($scriptPath, $source)
& (Join-Path $PSScriptRoot 'Redact-ScriptVisibility.ps1') -ScriptPath $scriptPath
$result = [IO.File]::ReadAllText($scriptPath)
foreach ($preserved in @('public Program()', 'public void Main(', 'public void Save()',
    'public override string ToString()', '"public class Hidden{}"', 'public enum LogLevel',
    'public int updateFrequency, maxQueuedThreads;', 'class Helper { public Helper() {} public int value; }',
    '/* keep this comment */ class Commented', 'public static implicit operator int',
    '#region preserved trivia', '// final comment without a newline')) {
    if (-not $result.Contains($preserved)) { throw "Expected preserved API or text: $preserved" }
}
foreach ($removed in @('public int state;', 'public T Resolve<', 'public class Helper',
    'public interface IHelper', 'public delegate int Getter', 'public enum Kind')) {
    if ($result.Contains($removed)) { throw "Unexpected public Program member: $removed" }
}
& (Join-Path $PSScriptRoot 'Redact-ScriptVisibility.ps1') -ScriptPath $scriptPath
if ([IO.File]::ReadAllText($scriptPath) -cne $result) { throw 'Redaction must be idempotent.' }
$invalid = 'public void Main('
[IO.File]::WriteAllText($scriptPath, $invalid)
$rejected = $false
try { & (Join-Path $PSScriptRoot 'Redact-ScriptVisibility.ps1') -ScriptPath $scriptPath }
catch { $rejected = $true }
if (-not $rejected -or [IO.File]::ReadAllText($scriptPath) -cne $invalid) {
    throw 'Invalid source must be rejected without modifying the file.'
}
Write-Host 'Visibility redaction tests passed: host API, nested access, literals/comments, idempotence, and invalid input.'

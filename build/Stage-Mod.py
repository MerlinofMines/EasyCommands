"""Package the shared engine and mod host. No source transformations."""
import argparse
import hashlib
import json
import re
from pathlib import Path
import shutil

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    parser.add_argument("--compile-items", type=Path, required=True)
    args = parser.parse_args()
    repo, output = args.repo.resolve(), args.output.resolve()
    target = output / "Mods/EasyCommands/Data/Scripts/EasyCommands"
    # The project is the authoritative source set for compilation AND packaging.
    items = json.loads(args.compile_items.read_text(encoding="utf-8-sig"))["Items"]["Compile"]
    sources = []
    for item in items:
        path = Path(item["FullPath"]).resolve()
        if path.is_relative_to(repo / "EasyCommands"):
            relative = path.relative_to(repo / "EasyCommands")
        elif path.is_relative_to(repo / "EasyCommands.Mod"):
            relative = path.relative_to(repo / "EasyCommands.Mod")
        else:
            raise RuntimeError("Unexpected compile source outside engine/host: " + str(path))
        sources.append((path, relative))
    manifest, seen = [], set()
    for source, relative in sources:
        if str(relative).lower() in seen:
            raise RuntimeError("Duplicate source destination: " + str(relative))
        seen.add(str(relative).lower())
        destination = target / relative
        destination.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(source, destination)
        digest = hashlib.sha256(source.read_bytes()).hexdigest()
        if hashlib.sha256(destination.read_bytes()).hexdigest() != digest:
            raise RuntimeError("Source copy mismatch: " + str(relative))
        manifest.append({"source": source.relative_to(repo).as_posix(), "destination": relative.as_posix(), "originalSha256": digest, "stagedSha256": digest})
    bridge_source = repo / "EasyCommands.Bridge/script.cs.txt"
    protocol = re.search(r'const string ApiId = "([^"]+)";', bridge_source.read_text()).group(1)
    if ('const string ApiId = "' + protocol + '";') not in (repo / "EasyCommands.Mod/ModSession.cs").read_text():
        raise RuntimeError("Mod and bridge protocols do not match")
    bridge = output / "IngameScripts/local/EasyCommands Bridge"
    bridge.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(bridge_source, bridge / "script.cs")
    shutil.copyfile(bridge_source, output / "bridge.cs.txt")
    for folder in (target.parents[2], bridge):
        shutil.copyfile(repo / "EasyCommands/thumb.png", folder / "thumb.png")
        shutil.copyfile(repo / "LICENSE", folder / "LICENSE")
    (output / "source-manifest.json").write_text(json.dumps(manifest, indent=2), encoding="utf-8")
    print(target)

if __name__ == "__main__":
    main()

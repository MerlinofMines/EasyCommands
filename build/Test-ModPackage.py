"""Validate the deployable mod and PB bridge archives against their source manifests."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import zipfile


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest().upper()


def verify_zip(path, folder, expected):
    with zipfile.ZipFile(path) as archive:
        names = [entry.filename for entry in archive.infolist() if not entry.is_dir()]
        if len(names) != len(set(names)) or set(names) != expected:
            raise RuntimeError("Unexpected archive contents: " + str(path))
        for name in names:
            if archive.read(name) != (folder / name).read_bytes():
                raise RuntimeError("Archive differs from staged input: " + name)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--repo", type=Path, required=True)
    parser.add_argument("--package", type=Path, required=True)
    args = parser.parse_args()
    repo, package = args.repo.resolve(), args.package.resolve()
    mod = package / "Mods/EasyCommands"
    bridge = package / "IngameScripts/local/EasyCommands Bridge"
    source_manifest = json.loads((package / "source-manifest.json").read_text())
    expected = {"thumb.png", "LICENSE"}
    for source in source_manifest:
        name = "Data/Scripts/EasyCommands/" + source["destination"]
        path = Path(name)
        if path.is_absolute() or ".." in path.parts or path.suffix != ".cs":
            raise RuntimeError("Invalid source destination: " + name)
        if name in expected:
            raise RuntimeError("Duplicate source destination: " + name)
        expected.add(name)
        original = (repo / source["source"]).resolve()
        if not original.is_relative_to(repo):
            raise RuntimeError("Source outside repository")
        if not (digest(original) == source["originalSha256"].upper() == digest(mod / name) == source["stagedSha256"].upper()):
            raise RuntimeError("Source hash mismatch: " + name)
    if not source_manifest:
        raise RuntimeError("No engine sources packaged")
    if set(p.relative_to(mod).as_posix() for p in mod.rglob("*") if p.is_file()) != expected:
        raise RuntimeError("Unexpected files in mod content")
    verify_zip(package / "EasyCommands.zip", mod, expected)
    verify_zip(package / "EasyCommands-Bridge.zip", bridge, {"script.cs", "thumb.png", "LICENSE"})
    for path in (bridge / "script.cs", package / "bridge.cs.txt"):
        if path.read_bytes() != (repo / "EasyCommands.Bridge/script.cs.txt").read_bytes():
            raise RuntimeError("Bridge differs from shipped source")
    for folder in (mod, bridge):
        if (folder / "thumb.png").read_bytes() != (repo / "EasyCommands/thumb.png").read_bytes() or (folder / "LICENSE").read_bytes() != (repo / "LICENSE").read_bytes():
            raise RuntimeError("Thumbnail/license differs from source")
    release = json.loads((package / "release-manifest.json").read_text(encoding="utf-8-sig"))
    protocol = re.search(r'const string ApiId = "([^"]+)";', (bridge / "script.cs").read_text()).group(1)
    if release["SchemaVersion"] != 1 or release["ApiId"] != protocol or ('const string ApiId = "' + protocol + '";') not in (mod / "Data/Scripts/EasyCommands/ModSession.cs").read_text():
        raise RuntimeError("Protocol/release identity mismatch")
    expected_artifacts = {"EasyCommands.zip", "EasyCommands-Bridge.zip", "bridge.cs.txt", "source-manifest.json"}
    if {item["File"] for item in release["Artifacts"]} != expected_artifacts or len(release["Artifacts"]) != len(expected_artifacts):
        raise RuntimeError("Release manifest has unexpected artifacts")
    for artifact in release["Artifacts"]:
        if digest(package / artifact["File"]) != artifact["Sha256"]:
            raise RuntimeError("Release artifact hash mismatch")
    print("Validated mod source archive, PB bridge archive, shared protocol and release hashes.")


if __name__ == "__main__":
    main()

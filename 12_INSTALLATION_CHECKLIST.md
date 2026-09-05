# 12 — Installation and Connection Checklist

## Inspection baseline — 2026-09-05

Observed hardware: Ryzen 7 5700X, RTX 4070 Ti, 32 GB RAM; approximately 243 GB free on C: at inspection.

Observed installation folders: Unity Hub, Unity 2022.3.25f1, Blender 4.3, Visual Studio 2022 and another Visual Studio version folder. Git reports 2.43.0.windows.1. Folder presence is not a successful launch/licensing test.

Reserve roughly 60 GB for tools, imports, assets and builds. This is an allowance, not a measured download size. Recheck free space before installing.

## Required setup at 1A

- [ ] Use existing Unity Hub; update it only if needed for editor installation.
- [ ] Install the latest available Unity 6000.3 LTS patch alongside 2022.3.25f1.
- [ ] Complete account/licence activation using the user's eligible licence.
- [ ] Record exact editor revision; create Universal 3D/URP project.
- [ ] Verify Windows x64 / DirectX 11 / Mono build support.
- [ ] Resolve released Input System, Cinemachine 3 and Test Framework versions; retain matching URP/Shader Graph and UI packages.
- [ ] Commit exact manifest and lock file.
- [ ] Install Blender 4.5 LTS alongside 4.3; record exact patch.
- [ ] Verify Visual Studio 2022 Game development with Unity workload.
- [ ] Verify Git LFS; add binary tracking before art assets.

Do not uninstall old versions or upgrade unrelated projects. Unreal, FMOD, paid packs and a third-party MCP bridge are not required.

## Connection smoke tests

- [ ] Unity batch editor runs a project-owned validation method and writes an interpretable log/exit code.
- [ ] A local Windows player builds and launches with graphics.
- [ ] Controller input and mouse input each reach the game.
- [ ] Blender background Python creates a one-metre reference object, exports FBX and renders a preview.
- [ ] Unity imports the reference with correct scale/orientation.
- [ ] A simple rig/animation export plays correctly before bird production.
- [ ] Confirm interactive Unity and batch processes cannot contend for the project.

These tests prove command-line/file automation. They do not establish native GUI control or prove game feel. Human setup may be required for installer prompts and account activation.

## Official references

- [Unity releases and LTS support](https://unity.com/releases/unity-6/support)
- [Unity Hub installation](https://docs.unity.com/hub/install-hub-win-mac)
- [Unity 6.3 editor command-line arguments](https://docs.unity3d.com/6000.3/Documentation/Manual/EditorCommandLineArguments.html)
- [Blender LTS downloads](https://www.blender.org/download/lts/)
- [Blender command-line scripting reference](https://docs.blender.org/manual/en/3.0/advanced/command_line/arguments.html)

The Blender command reference above documents the established background/Python workflow in an older manual. Validate scripts against the installed 4.5 runtime during the smoke test. Release/package patch versions are selected and pinned at setup rather than guessed in this plan.

# 12 — Installation and Connection Checklist

## Setup status — 2026-09-06

**Update:** the user has now completed installation and authorised subsequent setup, project creation and implementation. Unity is at `C:\Program Files\Unity 6000.3.23f1\Editor\Unity.exe`; Blender is at `C:\Program Files\Blender Foundation\Blender 4.5\blender.exe`. The download-only restriction below records the earlier handoff and is superseded for authorised project work.

User instruction on 2026-09-05: download required items, provide manual installation steps and check before actually installing anything. Do not run installers, change Visual Studio workloads, install project packages or activate a licence without prior user approval.

The toolchain is installed and its automated checks pass. See [checkpoint evidence](Setup/checkpoint1a-evidence.json) and [the playtest guide](CHECKPOINT_1A_REVIEW.md). Physical-controller testing and human flight-feel feedback remain pending. The items below preserve the initial download inspection.

- [x] Recheck disk: approximately 240 GiB free before downloads.
- [x] Inspect existing tools: Hub 3.8.0, Unity 2022.3.25f1 directory, Blender 4.3, Visual Studio 2022 17.14.13.
- [x] Verify Git and Git LFS executables: 2.43.0.windows.1 / 3.4.0.
- [x] Check VS 2022 Unity workload/component: neither query returned a matching installation.
- [x] Select official Unity 6000.3.23f1, revision 09d2ecc7fb28, and Blender 4.5.13 LTS x64 downloads.
- [x] Download both installers and verify byte counts, publisher checksums and valid Unity/Blender Authenticode signatures. Neither installer was run.
- [x] User completed installation and authorised subsequent project work.

Subsequent inspection detects the VS 2022 Unity workload. Unity created the URP project and resolved packages into the real manifest/lock file. No Hub update was required by the batch workflow.

## Inspection baseline — 2026-09-05

Observed hardware: Ryzen 7 5700X, RTX 4070 Ti, 32 GB RAM; approximately 243 GB free on C: at inspection.

Observed installation folders: Unity Hub, Unity 2022.3.25f1, Blender 4.3, Visual Studio 2022 and another Visual Studio version folder. Git reports 2.43.0.windows.1. Folder presence is not a successful launch/licensing test.

Reserve roughly 60 GB for tools, imports, assets and builds. This is an allowance, not a measured download size. Recheck free space before installing.

## Required setup at 1A

- [x] Retain existing Hub; no agent-installed update needed.
- [x] Unity 6000.3.23f1 installed alongside the existing version.
- [x] Batch editor successfully resolves the user's licence.
- [x] Record revision 09d2ecc7fb28 and create Universal 3D/URP project in Game.
- [x] Verify Windows x64 / DirectX 11 / Mono build and player launch.
- [x] Resolve Input System 1.20.0, Cinemachine 3.1.7, Test Framework 1.6.0, URP/Shader Graph 17.3.0 and UI 2.0.0.
- [x] Commit exact manifest and lock file on feature/checkpoint1a-flight.
- [x] Blender 4.5.13 LTS installed alongside 4.3.
- [x] Verify Visual Studio 2022 Game development with Unity workload.
- [x] Configure repository-local Git LFS hooks and binary tracking before generated source/export assets.

Do not uninstall old versions or upgrade unrelated projects. Unreal, FMOD, paid packs and a third-party MCP bridge are not required.

## Connection smoke tests

- [x] Unity batch editor runs a project-owned validation method and writes an interpretable log/exit code.
- [x] A local Windows player builds and launches with graphics; captures inspected.
- [x] Synthetic controller input and mouse input each reach the game; controller removal triggers mouse fallback.
- [ ] User verifies physical controller input and hot-plug behaviour.
- [x] Blender background Python creates a one-metre reference object, exports FBX and renders a preview.
- [x] Unity imports the reference with correct scale/orientation.
- [x] Imported rig animation deforms the mesh correctly before bird production.
- [x] Batch wrapper refuses to start while a Unity editor process is present; no competing project editor used in this run.

These tests prove command-line/file automation. They do not establish native GUI control or prove game feel. Human setup may be required for installer prompts and account activation.

## Official references

- [Unity releases and LTS support](https://unity.com/releases/unity-6/support)
- [Unity Hub installation](https://docs.unity.com/hub/install-hub-win-mac)
- [Unity 6.3 editor command-line arguments](https://docs.unity3d.com/6000.3/Documentation/Manual/EditorCommandLineArguments.html)
- [Blender LTS downloads](https://www.blender.org/download/lts/)
- [Blender command-line scripting reference](https://docs.blender.org/manual/en/3.0/advanced/command_line/arguments.html)

The Blender command reference above documents the established background/Python workflow in an older manual. Validate scripts against the installed 4.5 runtime during the smoke test. Release/package patch versions are selected and pinned at setup rather than guessed in this plan.

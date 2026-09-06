# Installation handoff — Windows x64

Prepared 2026-09-05. Downloads are authorised; installer execution is not. Nothing has been installed or activated by this setup pass. All steps below are for the user to perform when ready. Keep the existing Unity 2022 and Blender 4.3 installations.

## What is needed

| Item | Inspection | Action |
|---|---|---|
| Unity Hub | 3.8.0 executable present; launch/licence not tested | Use existing Hub; update only if it cannot locate/use the editor or sign in |
| Unity editor | Only 2022.3.25f1 found in Hub editor directory | Install downloaded 6000.3.23f1 x64, revision 09d2ecc7fb28 |
| Blender | 4.3 executable present | Install downloaded 4.5.13 LTS x64 alongside it |
| Visual Studio 2022 Community | 17.14.13; Unity workload and Unity component queries returned no match | Add **Game development with Unity** in the existing Visual Studio Installer |
| Git / LFS | Git 2.43.0.windows.1; Git LFS 3.4.0 executable works | No download required; repository LFS hooks/tracking still pending |
| Disk | About 240 GiB free before downloading | Adequate for the plan's 60 GB allowance; recheck before installation |

Visual Studio 2026 is also installed; this setup retains the planned 2022 toolchain. Folder/version inspection does not prove editor launch, licensing, Unity integration or build success.

## Downloaded files and verification

The installer cache is `C:\Users\User\source\repos\Wings\Downloads` and is excluded from Git. Both installers are downloaded: exact byte counts and publisher checksums match, and both Authenticode signatures are valid. Check [download-verification.json](download-verification.json) for the recorded results. Temporary transfer files have been removed.

- `UnitySetup64-6000.3.23f1.exe`: 4,125,667,560 bytes (about 3.84 GiB). [Official release](https://unity.com/releases/editor/whats-new/6000.3.23f1); selected from the official release API's latest 6000.3 Windows x64 result on this date.
- `blender-4.5.13-windows-x64.msi`: 359,968,768 bytes (about 343 MiB). [Official LTS page](https://www.blender.org/download/lts/), [download directory](https://download.blender.org/release/Blender4.5/). The publisher's SHA-256 list is retained here.

`Verify-Downloads.ps1` reads the files and verifies publisher checksums and signatures; it does not install or launch either package. Unity publishes an MD5 integrity value in its release metadata; a local SHA-256 is also recorded for future file comparison.

## Steps for you

1. Run `Downloads\UnitySetup64-6000.3.23f1.exe`. Choose a separate version folder, preferably `C:\Program Files\Unity\Hub\Editor\6000.3.23f1`. Keep the Unity editor component. Do not select unrelated platforms or a second Visual Studio installation. The Windows editor supplies the initial Windows Mono target; Windows IL2CPP, UWP and server support are not needed for this build. Actual Windows x64/Mono build support will be verified after installation.
2. Open Unity Hub. In **Installs**, use **Locate** (wording can vary by Hub version) and select the new `Editor\Unity.exe`. Sign in and activate your eligible Unity licence through Hub. If the existing Hub cannot do this, report the error before choosing an update; no Hub update has been downloaded or run speculatively.
3. Run `Downloads\blender-4.5.13-windows-x64.msi`. Use a separate `Blender 4.5` folder and leave Blender 4.3 installed. Launch 4.5 once to confirm it opens.
4. Open **Visual Studio Installer**, find **Visual Studio Community 2022**, choose **Modify**, and select **Game development with Unity**, including **Visual Studio Tools for Unity**. Deselect an optional Unity Hub installation if offered, since Hub is already present. Review the listed changes before applying them. The existing installer will download the matching workload payloads: these have **not** been cached, because that requires invoking the installer workflow. No separate Visual Studio bootstrapper is necessary. [Microsoft setup instructions](https://learn.microsoft.com/en-us/visualstudio/gamedev/unity/get-started/getting-started-with-visual-studio-tools-for-unity)
5. Tell me when these steps are complete, or send the text of any error. Project creation and package installation remain pending; no need to create a project yourself yet.

## After installation

With your authorisation to proceed, create the Universal 3D/URP project in `C:\Users\User\source\repos\Wings\Game`, pin the editor revision and resolve compatible released Input System, Cinemachine 3 and Test Framework packages. Preserve matching URP/Shader Graph/UI versions and record the real package lock. This package installation is also covered by the requirement to check before installing.

Configure repository-local Git LFS hooks and binary tracking before art production. The agent account encountered Git ownership checks; command-scoped `safe.directory` was used for inspection/branch creation without changing global Git settings.

Then run the installation checklist's Unity batch, graphics-enabled Windows build/input, Blender export/render, scale/orientation and rig smoke tests. Do not run Unity interactively and in batch against the same project. Only after evidence exists can 1A be presented as a playable checkpoint.

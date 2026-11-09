# Full repository contents and status

This repository includes the tracked source and documentation listed below. Unity scene generation is provided by an Editor menu command; panorama photos and authored hint prefabs are not included because no licensed art files were supplied.

## Tracked project files
- `Packages/manifest.json` and `ProjectSettings/ProjectVersion.txt` — Unity project settings scaffold.
- `Assets/GlobeLens/Editor/GlobeLensSceneBuilder.cs` — menu command to generate an editor demo scene after importing the folder into a Unity project.
- `Assets/GlobeLens/Scripts/` — gameplay, map, panorama, HUD, and ambience scripts.
- `Assets/GlobeLens/Resources/rounds.json` and `Assets/Resources/rounds.json` — sample round data.
- `docs/` and `devpost/` — scene setup and submission preparation.

## Generate the demo scene

1. Open a Unity project with the required packages and copy this repository's `Assets` folder into it.
2. Import TextMeshPro Essential Resources via **Window → TextMeshPro → Import TMP Essential Resources**.
3. In Unity, select **GlobeLens → Create Demo Scene**.
4. Open `Assets/GlobeLens/Scenes/GlobeLensDemo.unity`.
5. The generated scene uses a desktop EventSystem for editor smoke tests. Configure Meta XR hand-ray UI input and an XR rig from the Meta XR SDK samples before Quest testing.

The scene builder is a convenient starting point, not a verified Quest-ready scene. Verify in Unity and on headset before submission. The repository does not contain a Quest APK, licensed panoramas, clue prop prefabs, or the Meta XR SDK package itself.

# GlobeLens Quest

**GlobeLens Quest** is a hands-first, seated-friendly VR geography game for Meta Quest. Explore a curated destination scene, read a clue, point at the in-world map to guess, and score based on distance.

- **Competition track:** Gaming
- **Division:** Choose **Adapted / Significantly Updated** if this GeoGuessr project existed before September 24, 2026 and the competition-window changes are meaningful. Choose **New Experience** only if the project genuinely began on or after that date. See [`devpost/SUBMISSION.md`](devpost/SUBMISSION.md).
- **Target:** Meta Quest, Unity, Meta XR SDK v81 (install through the official Meta XR package workflow)
- **Play style:** seated, hand-first; controller input may be configured as a fallback

> **Project status:** this repo contains Unity C# source and planning/submission docs, not a finished Unity project. It does not yet include a `.unity` scene, panorama images, a Quest APK, or a tested hand-tracking rig. Follow the setup guide below, import licensed art, wire the scene, then test on-device before calling it submission-ready.

## Gameplay

1. Start a short session from a simple start screen.
2. Inspect a curated destination image/scene and a concise clue.
3. Point and tap the world map to place a guess.
4. Submit before the 20-second timer ends.
5. See the destination, distance, and points; play the next round or replay.

The game works offline once assets are imported. It does not require GPS, an account, or live street-view APIs.

## Source layout

```text
Assets/GlobeLens/
├── Resources/rounds.json
└── Scripts/
    ├── GeoGuessrGameManager.cs
    ├── GeoGuessrHUD.cs
    ├── MapGuessInput.cs
    ├── PanoramaDisplay.cs
    ├── RoundCatalog.cs
    ├── RoundDefinition.cs
    └── WorldMapGraphic.cs
devpost/
├── DEMO_SCRIPT.md
├── PLAN.md
├── SUBMISSION.md
└── SUBMISSION_CHECKLIST.md
```

## Create and run the Unity project

1. Install Unity 2022.3 LTS and create a 3D URP or 3D Core project. Keep the same editor version for the team.
2. Install Meta XR SDK v81 using Meta's official Unity setup instructions. Configure Android/Quest build support and OpenXR/Meta settings as required by that SDK release.
3. Copy this repository's `Assets/GlobeLens` folder into the Unity project's `Assets` folder.
4. In the scene, add a world-space Canvas in comfortable seated reach. Add TextMeshPro text, Buttons, a `WorldMapGraphic` UI Image, and a child marker `RectTransform`. Add an EventSystem and the Meta/XR UI input setup from the installed SDK sample so hand rays can click Unity UI. Do not rely on desktop mouse clicks as proof of Quest hand interaction.
5. Add a GameObject with `GeoGuessrGameManager` and assign the `RoundCatalog` component and its `rounds.json` TextAsset (or let it load `Resources/rounds.json`). Assign `GeoGuessrHUD`, buttons, text fields, and panels in the Inspector.
6. Add `MapGuessInput` to the map Image; assign the map `RectTransform`, guess marker, and game manager. Set the Image's **Raycast Target** on. Place the marker as a child of the map.
7. Add `PanoramaDisplay` to the scene and assign a Renderer with an unlit material. Import panoramas you have rights to use under `Assets/Resources/Panoramas/`, using the matching asset names in `rounds.json`.
8. Press Play to smoke-test in the Editor. Then test hand-first interaction on a Quest device or supported simulator; verify timer, pause/resume, scoring, next round, and replay.
9. Make a signed Quest APK using the event's required channel, upload it to the Meta VR Developer Dashboard, create an Invite URL, and keep it available for judging.

The scripts provide gameplay and UI hooks; they do not automatically generate a Unity scene or configure the Meta XR rig. Exact hand input setup depends on the Meta XR SDK sample and packages installed in your Unity project.

## Art, maps, and licensing

Use original or properly licensed panoramas and sounds. The sample round file references texture names, not bundled images. The map is a stylized, generated world-map graphic and does not use external tiles. Keep attribution and license records for every asset in the project. Do not commit API keys or unlicensed Street View/Mapillary downloads.

## Submission package

See `devpost/SUBMISSION.md` for a draft name, tagline, track/division guidance, and description; `devpost/DEMO_SCRIPT.md` for a sub-3-minute recording plan; and `devpost/SUBMISSION_CHECKLIST.md` for build and upload requirements.

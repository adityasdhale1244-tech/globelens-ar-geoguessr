# GlobeLens Quest

**GlobeLens Quest** is a hands-first, seated-friendly VR geography game for Meta Quest. Explore an immersive curated destination scene, read a clue, point at the in-world map to guess, and score based on distance.

- **Competition track:** Gaming
- **Division:** Choose **Adapted / Significantly Updated** if this GeoGuessr project existed before September 24, 2026 and the competition-window changes are meaningful. Choose **New Experience** only if the project genuinely began on or after that date. Check the event's official rules.
- **Target:** Meta Quest, Unity and the event-required Meta XR SDK version
- **Play style:** seated, hand-first; controller input may be configured as a fallback

> **Project status:** this repo contains Unity C# source and planning docs, not a finished Unity project. It does not include a `.unity` scene, panorama images, a Quest APK, or a tested hand-tracking rig. Create and wire the scene using [the premium scene guide](docs/PREMIUM_SCENE_SETUP.md), then test on device.

## Gameplay

1. Start a short session from a premium, uncluttered start screen.
2. Look around an immersive, locally bundled destination panorama and read the clue.
3. Point at the large in-world world map; a preview pin tracks aim. Pinch/tap to place or reposition it.
4. Lock in the guess separately before the 30-second timer ends.
5. See the revealed answer pin, distance line, and score; continue or replay.

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
    ├── RoundEnvironmentController.cs
    └── WorldMapGraphic.cs
docs/
├── ADVANCED_3D_AND_MAP_LOOP.md
└── PREMIUM_SCENE_SETUP.md
devpost/
├── DEMO_SCRIPT.md
├── PLAN.md
├── SUBMISSION.md
└── SUBMISSION_CHECKLIST.md
```

## Create and run the Unity project

1. Install the Unity version and Meta XR SDK version required by the event. Use Meta's current official Unity setup instructions.
2. Create a Unity project and configure Android/Quest build support and Meta XR/OpenXR settings per that SDK's setup guide.
3. Copy `Assets/GlobeLens` into the Unity project's `Assets` folder.
4. Follow [the premium scene setup guide](docs/PREMIUM_SCENE_SETUP.md) to create the seated XR scene, world-space UI, map, pins, and hand-ray UI input.
5. Import properly licensed panoramas into `Assets/Resources/Panoramas/` and optional hint prefabs into `Assets/Resources/Cues/`, matching names in `rounds.json`.
6. Assign scene references in the Inspector, enter Play mode, then validate hand-first interaction on a Quest device or supported simulator.
7. Build and upload the APK using the event's required Meta VR Developer Dashboard release channel, create an Invite URL, and keep the build accessible for judging.

The scripts provide gameplay and UI hooks; they do not automatically generate a Unity scene or install/configure the Meta XR rig. Exact hand input integration is determined by the Meta XR SDK setup used in your project.

## Visual direction and assets

Premium evergreen/celadon palette, warm high-contrast type, a calm panorama dome, subtle per-destination ambient tint, locally bundled 3D clue props, and a crisp graticule map with preview, guess, and answer pins. Use original or properly licensed panoramas and sounds. The sample round file references texture/prefab names; it does not bundle assets.

## Submission package

See `devpost/SUBMISSION.md` for the draft submission, `devpost/DEMO_SCRIPT.md` for a sub-3-minute recording plan, `devpost/SUBMISSION_CHECKLIST.md` for build/upload requirements, and `docs/ADVANCED_3D_AND_MAP_LOOP.md` for the added interaction and visual polish.

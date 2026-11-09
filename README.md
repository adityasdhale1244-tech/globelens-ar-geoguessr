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
3. Point at the large in-world world map and pinch/tap to place a guess.
4. Submit before the 30-second timer ends.
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
    └── WorldMapGraphic.cs
docs/PREMIUM_SCENE_SETUP.md
devpost/
├── DEMO_SCRIPT.md
├── PLAN.md
├── SUBMISSION.md
└── SUBMISSION_CHECKLIST.md
```

## Create and run the Unity project

1. Install the Unity version and Meta XR SDK version required by the event. Use Meta's current official Unity setup instructions rather than assuming a package version.
2. Create a Unity project and configure Android/Quest build support and Meta XR/OpenXR settings per that SDK's setup guide.
3. Copy `Assets/GlobeLens` into the Unity project's `Assets` folder.
4. Follow [the premium scene setup guide](docs/PREMIUM_SCENE_SETUP.md) to create the seated XR scene, world-space UI, map, pins, and hand-ray UI input.
5. Import properly licensed 2:1 panoramas into `Assets/Resources/Panoramas/`, matching the names in `rounds.json`.
6. Assign scene references in the Inspector, enter Play mode, then validate hand-first interaction on a Quest device or supported simulator.
7. Build and upload the APK using the event's required Meta VR Developer Dashboard release channel, create an Invite URL, and keep the build accessible for judging.

The scripts provide gameplay and UI hooks; they do not automatically generate a Unity scene or install/configure the Meta XR rig. Exact hand input integration is determined by the Meta XR SDK setup used in your project.

## Visual direction and assets

Premium evergreen/celadon palette, warm high-contrast type, a calm panorama dome, a crisp graticule map, oversized guess pin, distinct answer pin, and a connecting result line. Use original or properly licensed panoramas and sounds. The sample round file references texture names; it does not bundle images. The map is a stylized generated graphic and does not use external tiles.

## Submission package

See `devpost/SUBMISSION.md` for a draft name, tagline, track/division guidance, and description; `devpost/DEMO_SCRIPT.md` for a sub-3-minute recording plan; `devpost/SUBMISSION_CHECKLIST.md` for build/upload requirements; and `docs/PREMIUM_SCENE_SETUP.md` for scene assembly and interaction setup.

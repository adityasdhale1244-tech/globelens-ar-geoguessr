# Premium seated Quest scene setup

This guide upgrades the GeoGuessr prototype with an immersive panorama, a clear reachable map console, hand-ray interaction, and a polished result reveal. This repo is source scaffolding; create and wire the scene as described, then test on Quest.

## 1. Comfortable scene layout

Use a seated origin with the player's head near local `(0, 1.6, 0)`. Keep the main UI and map in front of the player, approximately 1.0–1.4 m away, centered around eye level or slightly below. Avoid requiring the player to turn more than about 30 degrees or reach outside a comfortable seated zone.

Suggested hierarchy:

```text
Main
├── XR Origin (Meta XR rig, from installed SDK sample)
├── Environment
│   ├── PanoramaDisplay
│   ├── RoundEnvironmentController
│   └── CueAnchor
├── GameUI (World Space Canvas)
│   ├── Header (brand / round / score / pause)
│   ├── ClueCard (large clue and short instruction)
│   ├── WorldMapPanel (WorldMapGraphic + MapGuessInput)
│   │   ├── LatitudeLongitudeLabels
│   │   ├── PreviewPin (translucent target, raycast off)
│   │   ├── GuessPin (lime ring, raycast off)
│   │   ├── AnswerPin (white/cyan target, hidden until reveal)
│   │   └── GuessToAnswerLine (Image child, raycast off)
│   ├── SubmitButton (large, high contrast)
│   ├── RevealPanel (answer, distance, points, next)
│   └── FinalPanel (total, replay)
└── GameSystems
    ├── RoundCatalog
    ├── GeoGuessrGameManager
    ├── GeoGuessrHUD
    └── MapGuessInput
```

Recommended Canvas reference size: 1600 × 900. Keep the map panel wide (about 680 × 360 reference units) so landforms, labels, and pins are easy to see. Put the clue in a separate card above the map.

## 2. Destination environment and 3D cues

- `PanoramaDisplay` creates an inward-facing panorama sphere and loads local textures from `Assets/Resources/Panoramas/`.
- Add `RoundEnvironmentController` to an environment GameObject. Assign a key light and an anchor positioned in the comfortable seated view (e.g., slightly left of map center, not attached to head rotation). The component eases ambient/key color and displays a gently floating clue beacon.
- Create authored hint prefabs at `Assets/Resources/Cues/` matching names in `rounds.json` (`LanternCue`, `ArchCue`, `NordicRoofCue`). Keep cues small, low-poly, static in position, with no required collisions. If a prefab is missing, a simple procedural beacon is used.
- Use only a slight light color tint per scene. No camera movement, forced rotation, zoom, locomotion, or rapid lighting changes.
- Import licensed 2:1 equirectangular panoramas at Quest-appropriate resolutions and compress for Android. Do not use street-view imagery unless redistribution/display rights explicitly permit it.

## 3. Stronger map loop

- Put `WorldMapGraphic` on the map Image. Add coordinate labels at −180°, −90°, 0°, 90°, 180° and 60°N, 30°N, 0°, 30°S, 60°S.
- Attach `MapGuessInput`; assign the game manager, map RectTransform, preview pin, committed guess pin, answer pin, and connector line. The line must be a child of the map with its Image Raycast Target off.
- Pointing at the map shows a soft preview pin. Pinch/tap commits or repositions the guess. The separate large Lock In button submits only when a pin is placed.
- Reveal draws the correct answer marker and connector; each next round clears all markers. Use an equirectangular map graphic so the tap-coordinate mapping matches the artwork.
- Configure the Meta XR documented hand-ray UI input sample to drive Unity EventSystem pointer movement and clicks. If the chosen module only emits clicks, the preview is optional; the committed guess loop still works.

## 4. Wire and test

- Assign `RoundCatalog` and `GeoGuessrGameManager`; place `rounds.json` in `Assets/Resources/` or assign it directly.
- Assign `GeoGuessrHUD` references for labels, panels, and buttons. Assign the `PanoramaDisplay` and `RoundEnvironmentController`.
- Ensure the XR UI event system is configured for the Meta XR SDK version in the project.
- Verify map tap places/repositions guess, Lock In reveals answer, line connects pins, new round clears markers, timeout/reveal and replay work.
- Test seated without a paired controller on Quest hardware; editor mouse testing alone does not validate hand-first interaction.

## 5. Comfort and performance

Keep controls within comfortable reach, minimize head turns, and let players pause/resume. Use one low-intensity cue light at most and disable its shadows. Use short fades and subtle pin pulse only; no camera shake, flashing, or forced head movement. Record asset licenses and validate frame rate, text legibility, and map selection on device.

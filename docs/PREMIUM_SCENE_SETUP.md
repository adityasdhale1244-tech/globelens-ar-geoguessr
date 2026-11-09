# Premium seated Quest scene setup

This guide upgrades the GeoGuessr prototype with an immersive panorama, a clear reachable map console, hand-ray interaction, and a polished result reveal. This repo is source scaffolding; create and wire the Unity scene as described, then test on Quest.

## 1. Comfortable scene layout

Use a seated origin with the player's head near local `(0, 1.6, 0)`. Keep the main UI and map in front of the player, approximately 1.0–1.4 m away, centered around eye level or slightly below. Avoid requiring the player to turn more than about 30 degrees or reach outside a comfortable seated zone.

Suggested hierarchy:

```text
Main
├── XR Origin (Meta XR rig, from installed SDK sample)
├── Environment
│   ├── PanoramaDisplay (add PanoramaDisplay.cs)
│   ├── SeatedStage (optional decorative frame/lighting)
│   └── Key Light
├── GameUI (World Space Canvas, Event Camera = XR camera)
│   ├── Header (brand / round / score / pause)
│   ├── ClueCard (large clue and short instruction)
│   ├── WorldMapPanel (Image + WorldMapGraphic)
│   │   ├── LongitudeLatitudeLabels
│   │   ├── GuessPin (lime ring + center dot)
│   │   ├── AnswerPin (white/cyan marker; hidden until reveal)
│   │   └── GuessToAnswerLine (thin Image; hidden until reveal)
│   ├── SubmitButton (large, high contrast)
│   ├── RevealPanel (answer, distance, points, next)
│   └── FinalPanel (total, replay)
└── GameSystems
    ├── RoundCatalog
    ├── GeoGuessrGameManager
    ├── GeoGuessrHUD
    └── MapGuessInput
```

Recommended Canvas reference size: 1600 × 900. Keep the map panel wide (about 680 × 360 reference units) so countries and pins are easy to see. Put the clue in a separate card above the map, not over the interactive area.

## 2. Build the visual environment

- Add `PanoramaDisplay` to a scene object. If its `Target Renderer` is empty, the script creates an inward-facing sphere and loads textures from `Assets/Resources/Panoramas/` by name.
- Use licensed 2:1 equirectangular panoramas (e.g. 4096 × 2048 source, compressed for Quest). Keep a non-identifying fallback image assigned so a missing panorama never leaves a blank scene.
- Keep the dome centered on the XR camera. The script follows the camera position, so it remains immersive without moving the player.
- Add a restrained dark translucent overlay behind text, a thin lime/cyan edge, a soft ambient/key light for optional foreground geometry, and a small set of non-moving destination markers. Avoid busy particles or rapid animation that can cause discomfort.
- For stable comfort, use a stationary horizon and no camera shake, forced rotation, zoom, or locomotion.

## 3. Make the map read clearly

- Use `WorldMapGraphic` on the map Image for a lightweight stylized base. Add longitude labels at −180°, −90°, 0°, 90°, 180° and latitude labels at 60°N, 30°N, 0°, 30°S, 60°S. If you have a licensed detailed map texture, it can replace the silhouette.
- Add a subtle dark map background and visible graticule. Make the guess pin a large lime target (minimum 24–32 reference units), not just a tiny dot. Use a distinct answer pin and connecting line after submission.
- Attach `MapGuessInput.cs` to the map Image. Assign the manager, map RectTransform, guess pin, answer pin, and connector. Set the map Image's Raycast Target on. Pin and line RectTransforms must be children of the map; set their anchors appropriately and ensure the connector Image is raycastable off.
- Input uses equirectangular mapping: left/right = −180°/180°, bottom/top = −90°/90°. Keep the map art in the same orientation/projection.
- Use the Meta XR Interaction SDK's documented hand-ray/UI input sample to drive Unity EventSystem pointer clicks. Configure the EventSystem input module as prescribed by the installed SDK; do not assume desktop mouse input validates hands-free use.
- Provide an always-visible instruction like “Point at the map, pinch to place your pin.” Make the submit button a separate, large target.

## 4. Hook the game and UI

- Assign `RoundCatalog` and `GeoGuessrGameManager` references in the Inspector.
- Add `GeoGuessrHUD` and assign all TMP labels, panels, and button references.
- Assign `MapGuessInput` to the HUD and assign `PanoramaDisplay` so each round loads its own panorama.
- Assign the start, submit, next, pause/resume, and replay buttons. Keep pause visible during play.
- Keep the timer at 30 seconds initially; tune only after playtesting. Pause should stop the countdown (the manager's phase-based timer does this).
- Test that new rounds clear both pins, the reveal shows the answer pin and connector, no answer coordinates are exposed before reveal, and the final screen can replay.

## 5. Premium presentation checklist

- Use a compact 3-step onboarding: LOOK → PIN → REVEAL.
- Keep one primary action per screen; use large, legible typography and high contrast.
- Animate panels with a short fade/scale only; avoid persistent motion.
- Add concise, licensed sound cues for pin placement, reveal, and completion, with an audio mute option if possible.
- Include pause/resume, clear session exit, and no mandatory tutorial.
- Verify play from seated position without controller pairing, then verify optional controller fallback.

## 6. Asset and performance notes

Use authored or properly licensed imagery; log source, creator, and license. Never assume a Google Street View or Mapillary image can be redistributed in an APK. Avoid runtime service calls for the judged build. Use texture compression appropriate for Android/Quest, avoid unnecessary real-time shadows, and validate frame rate, loading, text legibility, and hand interaction on hardware.

# Beyond the map: premium 3D cues and hand-first guessing

This guide describes the next polish layer for GlobeLens Quest. It complements `PREMIUM_SCENE_SETUP.md`. Code hooks provide a foundation, but scene assembly and Quest-device validation are still required.

## Destination atmosphere

Each round can optionally define:

- `ambienceHex`: a gentle color tint blended into scene ambient and key lighting.
- `cuePrefab`: a locally authored clue object in `Assets/Resources/Cues/`.
- `cueCaption`: a short, non-spoiler label shown beside the object.

`RoundEnvironmentController` creates a softly floating, slowly turning cue at its seated-safe anchor. If no prefab is assigned, it creates a simple low-poly beacon as a placeholder. Replace placeholders with small authored props that hint at geography—such as a lantern silhouette, a Nordic roofline, or a desert arch—without directly revealing the answer. Keep them static in position, modest in scale, and free of collision requirements. The script eases ambient and key-light color instead of moving the camera or changing the panorama.

Use a single low-intensity cue light at most; avoid real-time shadows on mobile. If the scene already uses baked lighting or a skybox, check how `RenderSettings.ambientLight` affects it and adjust the controller or disable ambient tinting if necessary.

## More tactile guessing loop

1. **Aim:** The Meta XR hand-ray/UI setup points at the world map. Pointer movement shows a translucent preview marker.
2. **Place:** Pinch/click sets the guess pin. The player can reposition it as many times as desired before locking in.
3. **Commit:** A separate, large `LOCK IN GUESS` button confirms the choice. This prevents accidental guesses while aiming.
4. **Reveal:** Locking in disables further map input, reveals the answer pin and connecting line, and shows distance and score.
5. **Continue:** A clear next-round action starts the next scene, resets map markers, and keeps cumulative score visible.

The preview interaction is implemented with Unity UI pointer callbacks in `MapGuessInput`. Make sure the XR EventSystem input module emits pointer move, click, and exit events. If it only emits clicks, the committed-pin flow still works; hide the preview marker.

## Map hierarchy and setup

```text
WorldMapPanel (Image + WorldMapGraphic + MapGuessInput)
├── Grid labels (latitude / longitude)
├── GuessPreview (translucent target; raycast target OFF)
├── GuessPin (lime ring + center dot; raycast target OFF)
├── AnswerPin (white/cyan target; hidden until reveal; raycast target OFF)
└── GuessToAnswerLine (Image; child of map; raycast target OFF)
```

Assign the marker/line RectTransforms to `MapGuessInput`. `GuessToAnswerLine` must be a child of the map; the script positions it between the two markers on reveal. Keep its Image raycast target off and place it behind the pins. Use an equirectangular map image/artwork because input maps the rectangle to longitude and latitude linearly. Include coordinate labels at a few clear intervals and put the orientation (north-up) in the UI.

Make the map about 680 × 360 canvas reference units, positioned 1.0–1.4 m from the seated player and tilted only slightly toward them. Do not require reaching to its edges; use hand-ray/pinch selection. Use the actual XR Interaction SDK UI setup for the project version and test it on headset.

## Visual polish without comfort risk

- Use one selected guess, a hover preview, and a distinct answer marker; avoid too many animated map elements.
- Use a 0.2–0.35 second ease on cards and reveal, and a subtle marker pulse only after placement.
- Keep ambient tint changes slow (about one second), modest, and peripheral.
- No camera shake, forced head movement, locomotion, flashing, or full-screen strobing.
- Add sound only if licensed, spatially gentle, and independently muteable.
- Keep clue text, timer, pause, and submit action at a stable, reachable position.

## Test pass

- Hover preview follows pointer; moving out hides it.
- Clicking places/repositions the pin; submit remains separate.
- Reveal marker and connector align with both guess and answer on the map.
- New round clears preview, guess, answer, and connector.
- Cue prefab loads when assigned; fallback beacon works when not assigned.
- Lighting transitions are subtle and restore original ambient when controller disables.
- Complete a full game while seated, hands-first, with no controller paired.

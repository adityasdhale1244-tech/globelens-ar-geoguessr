# Competition-window build plan

## Hackathon fit
- **Track:** Gaming: the core product is a playable guessing game, not a media viewer or productivity tool.
- **Division:** Because this GeoGuessr concept/prototype existed in earlier work, select **Adapted / Significantly Updated** if it predates the competition window and the competition work is substantial. Select New Experience only if it genuinely meets the event's new-project rules.
- **Target:** Meta Quest, using the event-approved Unity + Meta XR setup.

## Premium update
1. Replace the flat web mockup presentation with an immersive, stationary panorama dome using bundled, licensed location art.
2. Rebuild the map as a large, seated-reachable world-space panel with visible graticules and coordinate labels.
3. Add hand-ray/pinch map selection, a large guess pin, and a separate answer marker with a connecting line on reveal.
4. Add a calm visual hierarchy: title/round/score strip, readable clue card, map action prompt, pause, reveal, and final/replay screens.
5. Keep the 3-round session offline-capable and comfortable while seated.

## Player loop
Start → inspect scene and clue → point/pinch the map → submit → reveal answer and distance → next round → final score/replay.

## Visual direction
Dark evergreen surfaces, warm white typography, restrained lime/cyan accents, crisp map graticule, calm panoramas, one strong primary action, and only brief transitions. Avoid forced camera motion and busy effects.

## Risks and fallback
- Exact hand UI input depends on the Meta XR SDK sample and EventSystem module used; verify on device early and retain optional controller fallback.
- If a 360° panorama is unavailable, use a licensed static image on a curved/world-space panel.
- If the stylized map is not sufficiently legible, substitute a licensed map texture while retaining the same tap-coordinate logic.
- Do not depend on live street-view calls for judging; verify imagery redistribution rights.
- This repository is code/documentation scaffold only until a Unity scene, XR rig, assets, and APK are built and tested.

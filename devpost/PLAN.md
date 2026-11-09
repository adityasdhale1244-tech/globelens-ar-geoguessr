# GlobeLens Quest — Build plan

## Hackathon fit
- **Track:** Gaming. The core product is a playable guessing game, not a media viewer or productivity tool.
- **Division:** Because this GeoGuessr concept/prototype existed in earlier work, select **Adapted / Significantly Updated** if it existed before September 24, 2026. Describe only substantial features built during the competition window. Select **New Experience** only if the project truly began within the permitted window and the event rules confirm eligibility. Do not claim a new-project division based solely on a port or visual refresh.
- **Target:** Meta Quest, using the event-approved Unity + Meta XR SDK setup.

## Competition-window changes to deliver
1. Replace the browser-only prototype with a native Quest build.
2. Add hand-first world-map guessing via an XR UI ray/pinch interaction; keep controller input as an optional fallback.
3. Make the experience seated, with all controls in comfortable reach and no required room-scale movement.
4. Add clean pause/resume, three short offline rounds, answer reveal, and replay.
5. Use locally bundled licensed scenes; avoid API/network failure during judging.

Only list these as completed in the submission after they are implemented and tested.

## Player loop
Start → inspect scene and clue → tap map → submit → reveal score → next round → final score/replay.

## Premium design direction
Dark evergreen background, warm-white typography, lime/cyan accent, restrained map grid, one high-contrast pin, large clue text, concise controls, and subtle transitions. Avoid excessive motion and keep text stable and readable in VR.

## Risks and fallback
- Hand interaction may need device-specific Meta XR Interaction SDK wiring; test on Quest early. Keep controller fallback available.
- If a 360° scene is too costly or unavailable, use a static licensed image on a curved/world-space panel.
- If the decorative map graphic is unclear, use a licensed map texture or simplified continent regions; preserve tap-to-guess scoring.
- Do not depend on Google Street View/Mapillary live calls for the judged build. Check redistribution and display terms for any location imagery.

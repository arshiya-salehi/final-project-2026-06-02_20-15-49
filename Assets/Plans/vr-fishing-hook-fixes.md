# Project Overview

- **Game Title:** (VR Fishing game — title TBD)
- **High-Level Concept:** A relaxing VR fishing experience. The player stands at the water's edge, holds a fishing rod, dangles a hook into the water to catch fish swimming below, and reels the catch in to deposit it into a storage cup.
- **Players:** Single player (VR).
- **Inspiration / Reference Games:** Casual VR fishing sims (e.g. *Real VR Fishing*, *Bait!*).
- **Tone / Art Direction:** Calm, open ocean, stylized low-poly assets (dock, rod, colorful fish).
- **Target Platform:** StandaloneOSX (per current settings; XR Device Simulator used for in-editor testing).
- **Screen Orientation / Resolution:** VR HMD (XR).
- **Render Pipeline:** URP (`PC_RPAsset`).
- **Input:** New Input System + XR Interaction Toolkit 3.4.1. `XRI Default Input Actions` is loaded by an `InputActionManager` in the scene.

> Note: Project metadata above is inferred from the scene. Correct any details if needed.

# Game Mechanics

## Core Gameplay Loop
1. Player grabs the fishing rod (`FishingRod_Lvl5`, an `XRGrabInteractable`) and aims it over the water.
2. A hook hangs straight down from the rod tip on a line. Nearby fish are attracted to the hook.
3. When a fish reaches the hook it is caught and hangs vertically (head-up) from the hook.
4. Player **reels the line in** (right thumbstick up) to raise the hook + fish, then moves it over the `Cup` (`FishStorage`) to store the catch.
5. Loop repeats.

## Controls and Input Methods
- **Grab rod:** XR grip (existing `XRGrabInteractable`).
- **Reel in / let out line:** **Right controller thumbstick — push up to reel in (shorten line), pull down to let out (lengthen line).** Read from the `XRI Right/Thumbstick` action (Vector2, Y axis).
- **Catch:** Automatic when a fish enters the hook trigger (attraction already pulls the nearest fish in).

# UI
No new UI required for these fixes. (Optional future HUD: caught-fish counter on the cup.)

# Root-Cause Analysis (confirmed via scene inspection)

| # | Symptom | Confirmed cause |
|---|---|---|
| 1 | Fish doesn't hang vertically | `CatchableFish.Catch()` parents the fish to the freely-rotating hook and sets a **local** rotation `Euler(90,0,0)`. The hook's `ConfigurableJoint` angular motion is Free, so the parent is tilted → fish is never world-vertical. |
| 2 | Big gap between hook and fish | The **hook is scaled 24.3×**. `Catch()` sets fish `localPosition = (0,-0.2,0)` → 0.2 × 24.3 = **~4.9 m below the hook**. |
| 3 | Hook is far away near the horizon | The **rod is scaled 11.93×**. Its `Tip` child is at local z = 1.80 → world z ≈ **21.8 m** (horizon). The hook's `ConfigurableJoint` connectedAnchor (rod-local `(0,0,1.80)`) is dragged to that scaled point at runtime. Rod mesh actually ends at local z ≈ **0.07**. |
| 4 | No way to pull hook closer | No reel/cast logic exists; the hook only dangles from a static joint. |

# Chosen Approach (per user decisions)
- **Hook suspension:** *Scripted kinematic line* — remove the `ConfigurableJoint`, make the hook kinematic, and drive it from a script that hangs it a controllable distance straight down from the rod tip. Immune to the scale bugs and makes reeling precise.
- **Reel input:** *Right thumbstick up/down*.
- **Caught fish pose:** *Head-up (natural catch)* — fish nose points up toward the hook, body hangs below.

# Key Asset & Context

**Scene objects (current state):**
- `FishingRod_Lvl5` — `XRGrabInteractable`, kinematic `Rigidbody`, `LineRenderer` (useWorldSpace = true), `FishingLine` (tip→hook). localScale 11.93. Child `Tip` at localPos (0,0,1.80). Rod mesh bounds: center z 0.03, size z 0.08 → physical tip at local z ≈ 0.07.
- `FishingHook` (root object, localScale 24.3) — `Rigidbody` (mass 1, gravity on), trigger `SphereCollider` (radius 0.1 → world 2.43 m), `FishingHook` script, `ConfigurableJoint` (connected to rod).
- `Fish_School/*` — `CatchableFish` + `FishSwim`, localScale 0.1, no Rigidbody, forward = +Z, bounds long-axis ≈ 0.5 m. At depth y ≈ −0.4…−0.8.
- `Cup` — `FishStorage` (stores fish when a hook carrying a caught fish enters its trigger).

**Scripts to modify/create:**
- `Assets/Scripts/Fishing/CatchableFish.cs` (modify) — vertical head-up hang, no gap, attach via unscaled point.
- `Assets/Scripts/Fishing/FishingHook.cs` (modify) — expose an attach point, remove the 10-second force-catch test hack (keep attraction).
- `Assets/Scripts/Fishing/HookReel.cs` (**new**) — drives hook position below the tip and handles reel input.

**Reference values for implementation:**
- Tip fix: `Tip.localPosition = (0, 0, 0.07)` (rod mesh end). [= 1.80 ÷ 11.93 also ≈ 0.151 if a longer rod is desired; 0.07 matches the actual mesh.]
- Hook unscaled attach child: `localScale = 1/24.3 ≈ 0.04115` so its `lossyScale ≈ 1`.
- Default line length ≈ 1.2 m (tip y ≈ 0.66 → hook y ≈ −0.5, fish depth). Min ≈ 0.15, max ≈ 3.0, reel speed ≈ 1.5 m/s. (Tune in play test.)
- Head-up rotation: `Quaternion.Euler(-90,0,0)` maps fish +Z (nose) → world +Y.

# Implementation Steps

### Step 1 — Create `HookReel.cs` (hook suspension + reel)
- **Description:** New `MonoBehaviour` on `FishingRod_Lvl5`. Serialized fields: `Transform tip`, `Transform hook`, `InputActionReference reelAction`, `float lineLength = 1.2f`, `minLength = 0.15f`, `maxLength = 3f`, `reelSpeed = 1.5f`.
  - `OnEnable`: enable `reelAction.action`.
  - `Update`: read `reelAction.action.ReadValue<Vector2>().y`; `lineLength -= y * reelSpeed * Time.deltaTime`; clamp to [min,max].
  - `LateUpdate`: `hook.position = tip.position + Vector3.down * lineLength; hook.rotation = Quaternion.identity;`
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes (with Step 2)

### Step 2 — Modify `CatchableFish.cs` (vertical, head-up, no gap)
- **Description:** Change `Catch(Transform attachPoint)` to:
  - Disable `FishSwim`; keep/ensure no rogue physics.
  - `transform.SetParent(attachPoint, true);`
  - Compute `halfHeight` from the fish's renderer bounds (world size, long axis).
  - `transform.rotation = Quaternion.Euler(-90, 0, 0);` (head-up)
  - `transform.position = attachPoint.position - Vector3.up * halfHeight;` (head/mouth at the hook, body hanging down — no gap)
  - Disable the fish collider as before.
- **Assigned role:** developer
- **Dependencies:** None
- **Parallelizable:** Yes (with Step 1)

### Step 3 — Modify `FishingHook.cs`
- **Description:** Add `[SerializeField] private Transform attachPoint;`. In `OnTriggerEnter`, call `fish.Catch(attachPoint != null ? attachPoint : transform)`. Remove (or gate behind a `debugAutoCatch` bool defaulting false) the 10-second `ForceCatch` test logic; keep the fish-attraction logic intact. Optionally reduce trigger catch radius (see Step 6).
- **Assigned role:** developer
- **Dependencies:** Step 2 (Catch signature)
- **Parallelizable:** No

### Step 4 — Scene fixes: rod tip & hook physics
- **Description (Editor changes via an editor script / manual):**
  - Set `FishingRod_Lvl5/Tip` `localPosition = (0, 0, 0.07)` so the line/hook attach at the real rod tip (instead of 21 m out).
  - On `FishingHook`: **remove the `ConfigurableJoint`**; set `Rigidbody.isKinematic = true`, `useGravity = false` (HookReel now fully controls it).
- **Assigned role:** developer
- **Dependencies:** Step 1 (HookReel must exist before disabling joint)
- **Parallelizable:** No

### Step 5 — Scene wiring: attach point & HookReel references
- **Description:**
  - Create an empty child `AttachPoint` under `FishingHook`, `localPosition = (0,0,0)`, `localScale = (0.04115, 0.04115, 0.04115)` (so lossyScale ≈ 1).
  - Assign `FishingHook.attachPoint = AttachPoint`.
  - Add `HookReel` to `FishingRod_Lvl5`; assign `tip = Tip`, `hook = FishingHook`, and `reelAction = XRI Right/Thumbstick` (from `XRI Default Input Actions`).
- **Assigned role:** developer
- **Dependencies:** Steps 1, 3, 4
- **Parallelizable:** No

### Step 6 — Tuning pass (optional but recommended)
- **Description:** In play test, tune `lineLength` default so the hook rests at fish depth; tune `reelSpeed`, min/max. Optionally reduce the hook `SphereCollider` radius (e.g. 0.1 → ~0.02 local ≈ 0.5 m world) so catches feel intentional, and reduce `LineRenderer` width to ~0.01–0.02 m for a thin line.
- **Assigned role:** developer
- **Dependencies:** Step 5
- **Parallelizable:** No

# Verification & Testing

**Edit-mode checks (read-only script):**
- `Tip.position` ≈ (−0.04, 0.66, ~1.1) — no longer ~21 m out.
- `FishingHook` has no `ConfigurableJoint`; `Rigidbody.isKinematic == true`.
- `AttachPoint.lossyScale` ≈ (1,1,1).

**Play-mode checks (XR Device Simulator):**
1. **Issue 3:** On Play, the hook hangs straight down from the rod tip, into the water near the fish — not at the horizon. The line is short and vertical.
2. **Reel (Issue 4):** Push right thumbstick up → hook rises toward the rod tip; pull down → line extends. Length clamps at min/max.
3. **Catch + vertical (Issue 1):** Let a fish be caught. The fish hangs **vertically, head-up**, regardless of how the rod is tilted.
4. **No gap (Issue 2):** The caught fish's head sits right at the hook (no multi-meter gap).
5. **Storage:** Reel the hook up and move it over the `Cup`; the fish is stored and the hook is freed for the next catch.

**Edge cases:**
- Reeling fully in while a fish is attached should not detach or misalign the fish.
- Aiming the rod in different directions: hook always hangs straight down (world −Y) from the tip.
- No console errors from the removed joint or the new input action.

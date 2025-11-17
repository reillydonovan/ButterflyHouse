# What to Expect When You Press Play

## Expected Behavior

When you press **Play** in the Unity Editor, here's what should happen:

### Immediate (0-2 seconds)
1. **Scene loads** - All managers initialize
2. **GameController** waits 2 seconds (introDelay), then logs: `"Butterfly House Experience Started"`
3. **AudioManager** starts playing ambience (if you assigned an ambient clip)

### After 2 seconds
1. **Chrysalises** begin their spawn timers
   - Each Chrysalis pulses/glows (energy increases from 0 to 1 over spawn interval)
   - Default spawn interval is **20 seconds** per butterfly
   - If `Spawn On Start` is enabled, first butterfly spawns immediately

### When Butterflies Spawn (~2-22 seconds depending on settings)
1. **Emerging Phase (2 seconds)**
   - Butterfly appears at Chrysalis position
   - Starts at scale 0, grows to full size over 2 seconds
   - Visual: small glowing point expanding

2. **Flying Phase (begins after emerging)**
   - Butterfly starts moving with procedural flight path
   - Uses Perlin noise for wandering behavior
   - Flies within min/max radius from spawn point
   - Audio starts playing (if AudioClip assigned to ButterflyArchetype)
   - Trail renderer creates glowing trail behind butterfly
   - Visual: butterfly moving organically through space

3. **Potential Landing (randomly, based on settings)**
   - Butterfly checks for nearby LandingTargets every few seconds
   - If found, butterfly moves toward target
   - Lands on target (hands, plants, or environment objects)
   - Audio volume/intensity reduces while landed
   - After a moment, takes off again

4. **Dissipating Phase (after lifetime expires)**
   - Default lifetime: **60 seconds** (configurable in ButterflyArchetype)
   - Butterfly fades out over 2 seconds
   - Trail fades away
   - Butterfly despawns
   - New butterfly can spawn from Chrysalis

### Plants
- **GenerativePlants** should be visible
- They have a subtle breathing/swaying animation
- Touch them (via collider) to trigger:
  - Visual pulse at touch point
  - Audio one-shot plays (if AudioClip assigned)

### Audio
- **Ambience** plays continuously (if assigned to AudioManager)
- **Butterfly audio** - each butterfly plays its own audio clip (if assigned)
  - Volume modulated by flight speed
  - Pitch varies slightly with movement
- **Plant audio** - plays one-shots when touched

## Timeline Example

```
0:00  - Play pressed
0:02  - Experience starts ("Butterfly House Experience Started" in console)
0:02  - Chrysalises start pulsing/timer begins
0:02  - First butterfly spawns (if Spawn On Start enabled)
0:04  - First butterfly finishes emerging, starts flying
0:15  - Second butterfly may spawn (if spawn interval is 20s and Spawn On Start)
0:20  - First Chrysalis spawns second butterfly
1:04  - First butterfly reaches 60s lifetime, starts dissipating
1:06  - First butterfly despawns
...continues...
```

## What You Should See

### Visual
- ✅ Chrysalises pulsing/glowing periodically
- ✅ Butterflies appearing and growing from chrysalises
- ✅ Butterflies flying in organic, wandering paths
- ✅ Glowing trails following butterflies (if trails enabled)
- ✅ Butterflies occasionally landing on targets
- ✅ Plants breathing/swaying gently

### Audio
- ✅ Ambient background sound (if assigned)
- ✅ Butterfly sounds - melodic tones following butterflies
- ✅ Plant sounds when touched

### Console Messages
- ✅ `"Butterfly House Experience Started"` (after 2 seconds)
- ⚠️ Warnings if ButterflyArchetype missing on Chrysalis
- ⚠️ Warnings if Butterfly Prefab not assigned to ButterflyManager
- ⚠️ Warnings if AudioClips missing (butterflies/plants won't have sound)

## Troubleshooting - If Nothing Happens

### No Butterflies Spawning?

**Check:**
1. **ButterflyManager** - Is Butterfly Prefab assigned?
   - Select ButterflyManager in Hierarchy
   - Inspector → Butterfly Prefab field should have a reference
   - If empty, drag Butterfly prefab from Project window

2. **Chrysalis** - Is ButterflyArchetype assigned?
   - Select a Chrysalis object in Hierarchy
   - Inspector → Archetype field should have a reference
   - If empty, drag ButterflyArchetype asset (from ScriptableObjects/Butterflies/)

3. **ButterflyManager** - Is Max Butterflies set correctly?
   - Should be > 0 (default: 20)

4. **Chrysalis** - Check spawn settings:
   - Spawn Interval should be > 0 (default: 20 seconds)
   - If "Spawn On Start" is enabled, first spawn happens immediately

5. **Check Console** for error messages:
   - "Cannot spawn butterfly: prefab is not assigned"
   - "Cannot spawn butterfly: archetype is null"
   - "Max butterflies reached"

### Butterflies Spawn But Don't Move?

**Check:**
1. **Butterfly Prefab** - Does it have all components?
   - Butterfly.cs component
   - ButterflyVisualController
   - ButterflyAudio
   - MeshRenderer with material
   - TrailRenderer (optional)

2. **ButterflyArchetype** - Check flight parameters:
   - Flight Speed Curve should have values > 0
   - Noise Scale should be > 0
   - Turn Speed should be > 0

### No Audio?

**Check:**
1. **ButterflyArchetype** - Is Base Tone assigned?
   - Select ButterflyArchetype asset
   - Inspector → Base Tone should have AudioClip

2. **AudioManager** - Is Ambient Clip assigned?
   - Select AudioManager in Hierarchy
   - Inspector → Ambient Clip field

3. **GenerativePlant** - Are Touch Clips assigned?
   - Select plant in Hierarchy
   - Inspector → Touch Clips array should have AudioClips

4. **Audio Sources** - Check they're enabled:
   - Butterfly prefab should have AudioSource component
   - Plants should have AudioSource component

### Butterflies Are Invisible?

**Check:**
1. **Materials** - Are they using valid shaders?
   - Materials should not be magenta (invalid shader)
   - If magenta, shaders need to be assigned

2. **Butterfly Prefab** - Does it have a mesh?
   - Should have a MeshRenderer with a mesh
   - Check if mesh is assigned

3. **Camera** - Can camera see the butterflies?
   - Check camera position/rotation
   - Butterflies spawn around chrysalis positions

### Console Errors?

**Common errors:**
- `"ButterflyManager Instance is null"` - ButterflyManager not in scene
- `"Cannot initialize butterfly: archetype is null"` - Check Chrysalis archetype assignment
- `"Cannot spawn butterfly: prefab is not assigned"` - Check ButterflyManager prefab assignment

## Quick Setup Checklist

Before pressing Play, ensure:

- [ ] ButterflyManager has Butterfly Prefab assigned
- [ ] At least one Chrysalis has ButterflyArchetype assigned
- [ ] Butterfly prefab exists and has Butterfly component
- [ ] ButterflyArchetype asset exists (created by setup script or manually)
- [ ] All managers are in scene (GameController, ButterflyManager, AudioManager, InteractionManager)
- [ ] (Optional) AudioClips assigned for audio to work
- [ ] (Optional) Materials/shaders assigned for visual appearance

## Next Steps

Once everything is working:
1. **Create more ButterflyArchetypes** with different colors/behaviors
2. **Assign AudioClips** for full audio experience
3. **Create custom shaders** for butterflies, plants, chrysalises
4. **Adjust parameters** in ButterflyArchetypes for different flight patterns
5. **Add more Chrysalises** for more butterflies
6. **Set up hand tracking** for interaction
7. **Create custom meshes** for butterflies and plants

## Performance

With default settings:
- Max butterflies: 20
- Spawn rate: ~1 every 20 seconds per chrysalis
- Each butterfly lives ~60 seconds
- Expected active butterflies: ~3-6 at steady state (with 3 chrysalises)

Adjust in:
- **ButterflyManager** → Max Butterflies
- **ButterflyArchetype** → Lifetime
- **Chrysalis** → Spawn Interval


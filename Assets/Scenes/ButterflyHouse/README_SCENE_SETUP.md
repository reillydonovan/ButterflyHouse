# Butterfly House Sample Scene Setup

## Quick Setup

1. **Open Unity Editor**

2. **Run the Setup Script:**
   - In Unity menu bar: `Butterfly House` → `Setup Sample Scene`
   - This will create a new scene with all necessary objects

3. **Assign Prefabs and References:**
   - Select `ButterflyManager` in the Hierarchy
   - In Inspector, drag the `Butterfly` prefab (from `Assets/Prefabs/`) to the `Butterfly Prefab` field

4. **Assign ButterflyArchetypes:**
   - Select each `Chrysalis` object in the Hierarchy
   - In Inspector, drag a `ButterflyArchetype` asset (from `Assets/ScriptableObjects/Butterflies/`) to the `Archetype` field
   - If you don't have archetypes yet, they will be created by the setup script

5. **Assign Audio Clips (Optional):**
   - Select `ButterflyArchetype` assets in Project
   - Assign AudioClips to the `Base Tone` field
   - Select `GenerativePlant` objects
   - Assign AudioClips to the `Touch Clips` array

6. **Save the Scene:**
   - File → Save As
   - Save to `Assets/Scenes/ButterflyHouse/ButterflyHouseScene.unity`

## What Gets Created

The setup script creates:

- **Managers:**
  - `GameController` - Main orchestrator
  - `ButterflyManager` - Butterfly management
  - `AudioManager` - Audio mixing
  - `InteractionManager` - Interaction handling

- **Environment:**
  - `GroundPlane` - Ground plane for reference
  - `Directional Light` - Main lighting
  - Ambient lighting configured

- **Butterflies:**
  - `Butterfly.prefab` - Butterfly prefab (if it doesn't exist)
  - `Archetype_Sample.asset` - Sample ButterflyArchetype (if none exist)
  - 3x `Chrysalis` objects positioned around the scene

- **Plants:**
  - 2x `GenerativePlant` objects

- **Landing Targets:**
  - 3x `LandingTarget` objects positioned around the scene

## Next Steps

1. **Create Shaders:**
   - Create butterfly shaders with properties: `_BaseColor`, `_EmissionStrength`, `_WaveAmplitude`, `_WaveFrequency`, `_FlapFrequency`
   - Create plant shaders with properties: `_Oscillation`, `_PulseIntensity`, `_PulseCenter`
   - Create chrysalis shaders with properties: `_PulseIntensity`, `_PulseScale`

2. **Create Materials:**
   - Assign shaders to butterfly, plant, and chrysalis meshes

3. **Import Audio:**
   - Import butterfly audio clips to `Assets/Audio/Butterflies/`
   - Import plant audio clips to `Assets/Audio/Plants/`
   - Assign to archetypes and plants

4. **Set up XR/VR (if needed):**
   - Configure XR Origin/Camera
   - Enable MR passthrough
   - Connect hand tracking to InteractionManager

5. **Test:**
   - Press Play
   - Watch butterflies spawn from chrysalises
   - Interact with plants
   - Observe butterflies landing on landing targets

## Troubleshooting

**Butterflies not spawning?**
- Check Chrysalis objects have ButterflyArchetype assigned
- Check ButterflyManager has Butterfly Prefab assigned
- Check ButterflyManager max butterflies setting

**No audio?**
- Check AudioClips assigned to ButterflyArchetypes
- Check AudioSource components on prefabs
- Check AudioManager is in scene

**Butterflies not visible?**
- Check materials assigned to meshes
- Check cameras are properly set up
- Check lighting


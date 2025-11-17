# Quick Setup Guide

## Getting Started

This guide will help you quickly set up the Psychedelic Butterfly House experience in Unity.

## Step 1: Create Core Managers

1. **Create an empty GameObject** named `GameController`
   - Add the `GameController` component
   - Drag other managers into its references (or they'll auto-find)

2. **Create an empty GameObject** named `ButterflyManager`
   - Add the `ButterflyManager` component
   - Leave the Butterfly Prefab field empty for now (we'll create it next)

3. **Create an empty GameObject** named `AudioManager`
   - Add the `AudioManager` component
   - Optionally assign AudioMixerGroups if you have an AudioMixer

4. **Create an empty GameObject** named `InteractionManager`
   - Add the `InteractionManager` component

## Step 2: Create Butterfly Prefab

1. **Create a new GameObject** named `Butterfly`:
   - Add a simple mesh (Cube, Sphere, or import butterfly mesh)
   - Add `Renderer` component (MeshRenderer)
   - Add `Collider` component (SphereCollider or BoxCollider)
   - Add `TrailRenderer` component (child object or on main)

2. **Add Scripts:**
   - Add `Butterfly` component
   - Add `ButterflyVisualController` component
   - Add `ButterflyAudio` component (requires AudioSource - Unity will auto-add)

3. **Setup TrailRenderer:**
   - Set time: 1-3 seconds
   - Set start width: 0.1
   - Set end width: 0

4. **Save as Prefab** in `Assets/Prefabs/Butterfly.prefab`

5. **Assign to ButterflyManager:**
   - Select ButterflyManager in scene
   - Drag Butterfly prefab to "Butterfly Prefab" field

## Step 3: Create ButterflyArchetype ScriptableObject

1. **Right-click** in Project → Create → Butterfly → Archetype
2. **Name it** something like `Archetype_BlueWings`
3. **Configure:**
   - Set ID (unique identifier)
   - Set Display Name
   - Create a Gradient for wing colors (double-click the gradient)
   - Set base scale (1.0 default)
   - Create AnimationCurves for flap frequency and flight speed
   - Assign an AudioClip to Base Tone (if you have one)
   - Set Base Pitch (1.0 = standard)
   - Set Audio Volume (0.6 = 60%)
   - Configure flight behavior (radii, noise scale, turn speed)
   - Set Lifetime (60 seconds default)

4. **Repeat** for additional archetypes

## Step 4: Create Chrysalis Prefab

1. **Create a new GameObject** named `Chrysalis`:
   - Add a mesh (Sphere or capsule works well)
   - Add `Renderer` component

2. **Add Scripts:**
   - Add `Chrysalis` component

3. **Configure:**
   - Assign a ButterflyArchetype from Step 3
   - Set Spawn Interval (20 seconds default)
   - Check "Spawn On Start" if desired

4. **Save as Prefab** in `Assets/Prefabs/Chrysalis.prefab`

5. **Place in Scene:**
   - Instantiate Chrysalis prefab in scene
   - Position it where you want butterflies to spawn

## Step 5: Create Plant Prefab

1. **Create a new GameObject** named `GenerativePlant`:
   - Add a mesh (Tree, custom plant mesh, or simple cylinder)
   - Add `Renderer` component
   - Add `Collider` components on touchable parts (SphereCollider or BoxCollider)

2. **Add Scripts:**
   - Add `GenerativePlant` component
   - Add `PlantVisualController` component

3. **Add Audio:**
   - AudioSource should auto-add (or add manually)
   - Assign AudioClips to Touch Clips array
   - Optionally assign Arpeggio Clip

4. **Configure:**
   - Set Audio Volume (0.8 = 80%)
   - Set Max Oscillation Amplitude (0.1 = 10% sway)
   - Set Oscillation Speed (1.0 = normal speed)

5. **Save as Prefab** in `Assets/Prefabs/GenerativePlant.prefab`

6. **Place in Scene:**
   - Instantiate GenerativePlant prefab in scene
   - Position in accessible location

## Step 6: Create Landing Targets

1. **Create a new GameObject** named `LandingTarget`:
   - Add `Collider` component (SphereCollider works well)
   - Set Is Trigger = true (for detection)
   - Adjust size to desired landing area

2. **Add Script:**
   - Add `LandingTarget` component

3. **Configure:**
   - Set Target Type (Hand, Plant, or Environment)
   - Set Max Concurrent Butterflies (1 = one at a time)

4. **Save as Prefab** in `Assets/Prefabs/LandingTarget.prefab`

5. **Place in Scene:**
   - Instantiate at desired landing locations
   - Or attach to plants/hands (see below)

## Step 7: Setup Hand Tracking (Optional)

1. **Create Hand Proxy GameObjects:**
   - Create empty GameObject named `LeftHandProxy`
   - Add `HandProxy` component
   - Set Hand Type to Left
   - Repeat for `RightHandProxy`

2. **Or use HandProxy as child of tracked hand:**
   - Attach `HandProxy` component to hand tracking skeleton
   - HandProxy will auto-create landing targets

3. **Update InteractionManager:**
   - Assign LeftHandProxy and RightHandProxy references
   - Enable Hand Tracking

4. **Extend InteractionManager.UpdateHandTracking()** to query your XR SDK:
   ```csharp
   // Example pseudo-code:
   if (TryGetHandPose(HandType.Left, out Vector3 pos, out Quaternion rot))
   {
       leftHandProxy.UpdateHandPose(pos, rot);
       leftHandProxy.SetTracked(true);
   }
   ```

## Step 8: Create Settings (Optional but Recommended)

1. **Right-click** in Project → Create → Butterfly House → Settings
2. **Name it** `ButterflyHouseSettings`
3. **Configure:**
   - Max Butterflies: 20
   - Enable Trails: true
   - Enable Post Processing: true
   - Global Bloom Intensity: 0.8
   - Master Volume: 0.7
   - Butterfly Volume: 0.6
   - Plant Volume: 0.8
   - Ambience Volume: 0.5

4. **For Singleton Access:**
   - Place in `Assets/Resources/` folder
   - Access via `Settings.Instance`

## Step 9: Test the Scene

1. **Press Play**
2. **Watch chrysalises spawn butterflies**
3. **Observe butterflies flying with procedural paths**
4. **Test plant interaction** (touch with collider)
5. **Watch butterflies land** on landing targets

## Shader Setup (Next Steps)

To enable full visual effects, create shaders with these exposed properties:

### Butterfly Shader
- `_BaseColor` (Color)
- `_EmissionStrength` (Float)
- `_WaveAmplitude` (Float)
- `_WaveFrequency` (Float)
- `_FlapFrequency` (Float)

### Plant Shader
- `_Oscillation` (Float)
- `_PulseIntensity` (Float)
- `_PulseCenter` (Vector3)

### Chrysalis Shader
- `_PulseIntensity` (Float)
- `_PulseScale` (Float)

Use Unity Shader Graph or write custom HLSL shaders with these parameter names.

## Audio Setup

1. **Import Audio Clips** to `Assets/Audio/Butterflies/` and `Assets/Audio/Plants/`
2. **Assign to ButterflyArchetypes** (Base Tone field)
3. **Assign to GenerativePlants** (Touch Clips array)
4. **Create ambience clip** and assign to AudioManager

## Troubleshooting

**Butterflies not spawning?**
- Check Chrysalis has ButterflyArchetype assigned
- Check ButterflyManager has Butterfly Prefab assigned
- Check maxButterflies limit

**No audio?**
- Check AudioClip assigned to ButterflyArchetype
- Check AudioSource on Butterfly prefab
- Check AudioManager is in scene

**Butterflies not landing?**
- Check LandingTarget has collider
- Check LayerMask on Butterfly landing detection
- Check landing radius in Butterfly script

**Plants not responding to touch?**
- Check colliders on plant are enabled
- Check GenerativePlant has AudioClips assigned
- Check InteractionManager for hand tracking setup

## Next Steps

- Create multiple butterfly archetypes
- Design butterfly meshes and shaders
- Create plant meshes and shaders
- Set up MR passthrough environment
- Integrate hand tracking SDK
- Tune audio and visual parameters
- Add post-processing effects (bloom, etc.)


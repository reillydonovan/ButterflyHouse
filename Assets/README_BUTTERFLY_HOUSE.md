# Psychedelic Butterfly House (MR)

A meditative, psychedelic Mixed Reality experience built with Unity, featuring generative butterflies, interactive plants, and synesthetic audio-visual mapping.

## Project Overview

### Core Experience

You're standing inside a meditative, psychedelic butterfly house. Chrysalises pulse with energy. Butterflies emerge, unfold into shimmering, generative forms, and take flight. As they fly, they morph into oscillating waveforms, leaving glowing tracers in their path, each mapped to an evolving sound. Generative plants sway and vibrate when you touch them, adding tones and textures to the ambient symphony.

### Experience Pillars

- **Meditative Ecosystem** – Alive, slow, breathing; not frantic
- **Synesthetic Mapping** – Visual motion maps to sound; waveforms, oscillations, and material changes reflect the audio
- **Embodied Interaction** – Your hands and subtle gestures influence butterflies and plants
- **Emergence & Generativity** – Each butterfly is unique; patterns feel algorithmic rather than canned

## Project Structure

```
Assets/
  Scripts/
    Core/
      GameController.cs       - Main orchestrator
      Settings.cs             - Global settings (ScriptableObject)
    Butterflies/
      Butterfly.cs            - Main butterfly behavior & lifecycle
      ButterflyManager.cs     - Central butterfly management
      ButterflyArchetype.cs   - Butterfly type definitions (ScriptableObject)
      ButterflyVisualController.cs - Visual properties & shader control
      ButterflyAudio.cs       - Per-butterfly audio
      Chrysalis.cs            - Spawn point for butterflies
    Plants/
      GenerativePlant.cs      - Interactive plant behavior
      PlantVisualController.cs - Plant visual effects
    Interaction/
      LandingTarget.cs        - Landing spots for butterflies
      HandProxy.cs            - Hand tracking proxy
      InteractionManager.cs   - Interaction system manager
    Audio/
      AudioManager.cs         - Central audio mixer/orchestrator
  ScriptableObjects/
    Butterflies/              - ButterflyArchetype assets
  Art/
    Butterflies/              - Meshes, Materials, Shaders
    Plants/                   - Meshes, Materials, Shaders
    Environment/              - Dome, Ground, Landing spots
  Audio/
    Butterflies/              - Butterfly audio clips
    Plants/                   - Plant interaction sounds
    Ambience/                 - Ambient audio
  Prefabs/
    Butterfly.prefab          - Butterfly prefab
    Chrysalis.prefab          - Chrysalis prefab
    GenerativePlant.prefab    - Plant prefab
    LandingTarget.prefab      - Landing target prefab
    EnvDome.prefab            - Environment dome prefab
```

## System Architecture

### 1. Butterfly Lifecycle System

Butterflies progress through states:
- **Emerging** → Spawn from chrysalis, grow in scale
- **Flying** → Procedural flight path with noise-based wandering
- **Landing** → Seek and land on hands/plants
- **Dissipating** → Fade out and despawn

**Key Components:**
- `ButterflyArchetype`: ScriptableObject defining butterfly types (colors, speeds, audio)
- `ButterflyManager`: Manages spawning, pooling, and lifecycle
- `Chrysalis`: Periodic spawn points for butterflies

### 2. Procedural Flight Paths

Butterflies use Perlin noise for wandering behavior, constrained by min/max flight radii. Vertical oscillation is linked to audio frequency for synesthetic effect.

### 3. Generative Materials & Visual FX

Shader-based visual system with MaterialPropertyBlock API:
- Color gradients mapped to butterfly age
- Waveform deformation for audio-reactive visuals
- Emission intensity tied to audio amplitude
- Trail renderers with gradient-based colors

### 4. Psychedelic Plant System

Interactive plants that respond to touch:
- Visual pulse at touch point
- Audio playback (single notes, arpeggios, textures)
- Procedural swaying/breathing animations

### 5. Audio & Synthesis Mapping

- **AudioManager**: Central mixer with density-based reverb
- **ButterflyAudio**: Per-butterfly audio with speed-based volume modulation
- **PlantAudio**: Event-based one-shots

### 6. Interaction & Hand Tracking

- `HandProxy`: Represents tracked hands
- `LandingTarget`: Landing spots (hands, plants, environment)
- `InteractionManager`: Bridges XR SDK with interaction systems

## Setup Instructions

### Prerequisites

- Unity 2022+ (Universal Render Pipeline recommended)
- XR Interaction Toolkit (for hand tracking)
- OpenXR or Quest SDK (for MR passthrough)

### Initial Setup

1. **Create a new Unity scene** with:
   - XR Origin/Camera setup
   - MR Passthrough enabled
   - Lighting configured

2. **Create ScriptableObjects:**
   - Create ButterflyArchetype assets in `Assets/ScriptableObjects/Butterflies/`
   - Assign gradients, audio clips, and flight parameters

3. **Set up prefabs:**
   - Create Butterfly prefab with components:
     - `Butterfly` script
     - `ButterflyVisualController`
     - `ButterflyAudio`
     - `TrailRenderer`
     - Mesh renderer with butterfly shader
   
   - Create Chrysalis prefab with:
     - `Chrysalis` script
     - Renderer with pulsing shader
     - ButterflyArchetype assigned

   - Create GenerativePlant prefab with:
     - `GenerativePlant` script
     - `PlantVisualController`
     - Colliders for touch detection
     - Audio clips assigned

4. **Initialize Managers:**
   - Add `GameController` to scene
   - Add `ButterflyManager` (assign butterfly prefab)
   - Add `AudioManager` (optionally assign audio mixer groups)
   - Add `InteractionManager` (for hand tracking)

5. **Place Chrysalises:**
   - Instantiate Chrysalis prefabs around the scene
   - Assign ButterflyArchetype to each

6. **Place Plants:**
   - Instantiate GenerativePlant prefabs
   - Position in accessible locations

### Settings Configuration

Create a Settings asset:
1. Right-click in Project → Create → Butterfly House → Settings
2. Configure max butterflies, volumes, visual effects
3. Place in `Assets/Resources/` folder (optional, for `Settings.Instance` access)

## Shader Setup

### Butterfly Shader Parameters

The butterfly shaders should expose these properties (via Shader Graph or HLSL):
- `_BaseColor` (Color): Base wing color
- `_EmissionStrength` (Float): Emission intensity (0-1)
- `_WaveAmplitude` (Float): Waveform deformation amplitude
- `_WaveFrequency` (Float): Waveform frequency
- `_FlapFrequency` (Float): Wing flap frequency

### Plant Shader Parameters

- `_Oscillation` (Float): Sway/breathing amount
- `_PulseIntensity` (Float): Pulse intensity (0-1)
- `_PulseCenter` (Vector3): Pulse origin in local space

### Chrysalis Shader Parameters

- `_PulseIntensity` (Float): Energy level (0-1)
- `_PulseScale` (Float): Visual pulse scale

## Audio Setup

### Butterfly Audio

Each ButterflyArchetype should have:
- `baseTone`: Base audio clip (looping)
- `basePitch`: Pitch multiplier
- `audioVolume`: Volume level (0-1)

Audio is modulated by:
- Flight speed → Volume
- Movement → Pitch variation
- Landing state → Intensity reduction

### Plant Audio

Assign AudioClips to `touchClips` array in GenerativePlant:
- Single notes for basic touch
- Arpeggio clips for sequences
- Textural sweeps for atmosphere

## Hand Tracking Integration

To integrate hand tracking:

1. **Set up XR Interaction Toolkit** or platform SDK
2. **Extend InteractionManager** to query hand poses:
   ```csharp
   // Example in InteractionManager.UpdateHandTracking():
   if (TryGetHandPose(HandType.Left, out Vector3 pos, out Quaternion rot))
   {
       leftHandProxy.UpdateHandPose(pos, rot);
       leftHandProxy.SetTracked(true);
   }
   ```

3. **Configure HandProxy colliders** for touch detection
4. **Enable landing targets** on hands via HandProxy

## Performance Tuning

### Settings Adjustments

- `maxButterflies`: Limit active butterflies (10-25 recommended)
- `enableTrails`: Toggle trail rendering
- `enablePostProcessing`: Toggle bloom/effects
- `globalBloomIntensity`: Control bloom strength

### Optimization Tips

- Use object pooling for butterflies (future enhancement)
- Limit trail lifetime (1-3 seconds)
- Use LODs for butterfly meshes
- Reduce shader complexity if needed
- Disable post-processing on lower-end devices

## Milestone Roadmap

### Milestone 1 – Core Loop Prototype ✅
- [x] MR passthrough basic scene
- [x] One chrysalis spawning butterflies
- [x] Procedural flight paths
- [x] Audio mapping
- [x] Basic trails

### Milestone 2 – Interaction & Plants
- [ ] Hand tracking integration
- [ ] Butterfly landing on hands
- [ ] Generative plants with touch interaction

### Milestone 3 – Generativity & Visual Polish
- [ ] Multiple butterfly archetypes
- [ ] Shader-based waveform mode
- [ ] Color gradients + emission mapping
- [ ] Multiple chrysalises and landing spots

### Milestone 4 – Ecosystem & Tuning
- [ ] Dynamic density management
- [ ] Global audio mixing with reverb
- [ ] UX polish (intro, tempo, breathing)

## Notes

- The codebase uses namespaces for organization: `ButterflyHouse.Core`, `ButterflyHouse.Butterflies`, etc.
- Settings are accessed via `Settings.Instance` (requires asset in Resources) or direct references
- Most managers use Singleton pattern with Instance static property
- MaterialPropertyBlock is used for per-instance shader parameters (efficient for many butterflies)

## License

[Add your license here]


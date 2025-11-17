# 🦋 Psychedelic Butterfly House

A mixed reality (MR) meditative experience built in Unity, where you stand in a tranquil butterfly sanctuary. Watch as chrysalises spawn generative butterflies that morph into oscillating waveforms with evolving sounds. Touch generative plants that react with sounds and vibrations. Feel butterflies land on your hands, stabilizing the soundscape as the ecosystem evolves around you.

![Unity Version](https://img.shields.io/badge/Unity-2022+-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Quest%2FOpenXR-green.svg)
![License](https://img.shields.io/badge/License-MIT-yellow.svg)

## 🎮 Game Description

### The Experience

**Psychedelic Butterfly House** is an immersive mixed reality meditation experience that transforms your physical space into a living, breathing ecosystem. You are a visitor in a tranquil sanctuary where time moves slowly, and every interaction ripples through the environment.

The experience begins simply: you stand in an empty space that gradually transforms into a butterfly sanctuary. Chrysalises pulse with energy, birthing unique butterflies that take flight with organic, procedural movements. Each butterfly generates its own sound based on its flight pattern—creating a living soundscape that responds to your presence.

As you explore the space, you'll discover generative plants that respond to your touch with bioluminescent pulses, harmonic tones, and subtle vibrations. Butterflies may land on your outstretched hands, creating moments of connection that deepen your harmony with the ecosystem.

The longer you stay, and the more you interact, the more the space evolves. The sanctuary grows through five distinct stages, each more wondrous than the last. Butterflies transform into pure waveform entities. Plants achieve sentience and respond to your footsteps. Rare events—like a butterfly eclipse or chromatic storm—unfold organically, creating moments of synesthetic beauty.

### Core Philosophy

This is not a game to be won, but an experience to be felt. There are no objectives, no goals, no failure states. The experience rewards:
- **Stillness** - Meditative quietude allows serenity to bloom
- **Curiosity** - Exploration and interaction fuel discovery
- **Harmony** - Building connections between you, butterflies, and plants

The experience is designed for presence and mindfulness. It's a living art installation that responds to your body, your movements, and your attention.

## 🎯 Gameplay Mechanics

### Player Actions

#### Stillness & Movement
- **Standing Still**: Increases your **Serenity Level** over time. The ecosystem responds to your calm presence.
- **Moving Around**: Allows you to explore the space, discover new plants, and interact with butterflies.
- **Serenity Sustained**: Remaining still for extended periods (60+ seconds) can trigger rapid progression.

#### Touching Plants
- **Single Touch**: Plants emit a musical tone and visual pulse. Each touch increases your **Curiosity Level**.
- **Plant Growth**: Plants evolve through 4 growth stages based on touch count and butterfly visits:
  - **Sprout** → **Bloom** → **Fractal Bloom** → **Psychedelic Sentience**
- **Bloomed Plants**: Emit multiple notes in arpeggios instead of single tones.
- **Sentient Plants**: Respond to your footsteps, sway to audio frequencies, and emit complex chords.

#### Hand Interactions
- **Hand Presence**: Simply holding your hands out can attract butterflies.
- **Butterfly Landings**: When butterflies land on your hands:
  - Your **Harmony Level** increases
  - The soundscape stabilizes around you
  - Your hand aura evolves through levels
  
#### Gestures (Advanced)
Once your hand aura reaches certain levels:
- **Conductor Aura** (Level 2): Rotating gestures spawn micro-swarms of butterflies
- **Gesture Spellbook** (Level 3): Advanced spell gestures:
  - **Palm-Up Hold**: Summons a harmonic chord burst
  - **Circle Gestures**: Creates a butterfly vortex
  - **Hand-to-Hand Arc**: Connects butterflies with ribbons of light

### Progression System

#### Three Core Meters

1. **Serenity Level** (0-100)
   - Increases when you remain still
   - Decreases with movement
   - Sustained serenity (60+ seconds) unlocks advanced stages

2. **Curiosity Level** (0-100)
   - Increases when you touch plants
   - Increases with exploration and gestures
   - Opens up new plant types and interactions

3. **Harmony Level** (0-100)
   - Increases when butterflies land on you
   - Increases with butterfly-plant interactions (pollination)
   - Triggers rare events and unlocks final stages

#### Stage Progression

The experience evolves through 5 stages:

**Stage 0: Emergence** (Starting State)
- Basic butterflies spawn from chrysalises
- Minimal plants in sprout form
- Low audio density, thin trails
- *Unlocks*: 45 seconds elapsed OR first butterfly landing

**Stage 1: Expansion**
- More butterfly archetypes appear
- Trails become more luminescent
- Plants begin responding to your presence
- *Unlocks*: Harmony > 20 AND Serenity > 15

**Stage 2: Symbiosis**
- Butterflies "pollinate" plants on landing
- Plants glow where touched
- New visual bloom patterns emerge
- Audio gains subtle harmonics
- *Unlocks*: Curiosity > 30 AND 5+ plant touches

**Stage 3: Emergent Ecology**
- New plant types appear procedurally
- Butterflies flock in synchronized patterns
- Noise-based wind currents become visible
- Trails thicken and persist longer
- *Unlocks*: Sustained serenity (60 seconds) OR swarm event triggered

**Stage 4: Synesthetic Overgrowth**
- Ambient lighting cycles (Dawn/Noon/Dusk/Midnight)
- Butterflies shift into waveform-mode more often
- Plants generate chords instead of single notes
- Space feels alive and self-transforming
- *Unlocks*: All meters > 50 OR 12+ minutes elapsed

**Stage 5: Ascension** (Final State)
- The sanctuary reaches its "final form"
- Giant chrysalis events can occur
- Butterfly choir formations (synchronized audio)
- Environment can temporarily dissolve into pure waveforms
- Everything reforms into peaceful equilibrium

### Butterfly Mechanics

#### Lifecycle
1. **Emerging** - Butterfly spawns from chrysalis at scale 0, grows to full size
2. **Flying** - Procedural flight path using Perlin/curl noise
3. **Landing** - Seeks nearby landing targets (hands, plants, flowers, fruits)
4. **Dissipating** - After lifetime expires, fades out

#### Lifetime System
- **Random Lifespans**: Butterflies can live 0.8x to 3x their base lifetime
- **Immortal Butterflies**: 10% chance to live forever (never die naturally)
- **Lifetime Variety**: Ensures ecosystem diversity with butterflies of varying ages

#### Flight Patterns
- **Orbital Motion**: Butterflies orbit around central points with variable radii
- **Perlin Noise**: Smooth, organic movement patterns
- **Flocking** (Stage 3+): Butterflies synchronize movement in groups using Boids algorithm:
  - **Cohesion**: Move toward nearby butterflies
  - **Alignment**: Align with nearby butterfly velocities
  - **Separation**: Avoid crowding nearby butterflies
  - Butterflies can break out of flocks randomly, after time, or when distance increases
- **Waveform Transformation** (Stage 4+): Visual morphing into sine/saw/square/FM waveforms

#### Landing Behavior
- Butterflies randomly seek landing targets
- **Priority System**:
  - **Fruits** (when energy is low): Butterflies feed from fruits to restore energy
  - **Flowers** (when no pollen or low energy): Butterflies collect pollen and nectar from flowers
  - **Plants**: Secondary landing targets for rest
  - **Hands**: Landing on player hands increases harmony
- Remember last landing location (avoid immediate re-landing on same spot)
- Stay landed for random duration (2-8 seconds)
- Cooldown period prevents constant re-landing (10-30 seconds)

#### Energy System
- Butterflies have **energy levels** that decay over time (after 10 seconds)
- **Low Energy**: Butterflies actively seek fruits to feed and restore energy
- **Feeding**: Butterflies gain energy while landing on fruits
- **Energy-Based Behavior**: Flight speed and behavior affected by energy level

#### Pollination System
- Butterflies can **collect pollen** from flowers
- **Pollen Carrying**: Butterflies track pollen charge (up to max 3 units)
- **Pollen Deposition**: Butterflies deposit pollen on other flowers or fruits
- **Pollination Effects**: 
  - Increases **Harmony Level** when butterflies pollinate flowers
  - Flowers evolve to higher stages when pollinated
  - Meta-Flowers (stage 3) can spawn fruit seeds when fully pollinated
- **Pollen Decay**: Pollen slowly decays if not deposited

### Plant Mechanics

#### Growth Phases

**Level 0: Sprout**
- Minimal mesh, single note on touch
- Static, unresponsive

**Level 1: Bloom** (3+ touches)
- New tendrils grow
- Bioluminescent pulses
- Multiple notes per touch (arpeggios)
- Can spawn flowers

**Level 2: Fractal Bloom** (5+ butterfly visits)
- Branches subdivide procedurally
- Responds visually to butterflies landing
- More complex audio patterns
- Spawns more flowers

**Level 3: Psychedelic Sentience** (Harmony > 50 + 5 min in bloom)
- Fully responsive to environment
- Emits chords, not single notes
- Sways based on audio band energy
- Touch releases spores/light particles
- Responds to footsteps and hand proximity
- Flowers on plant evolve faster

### Flower Pollination System

#### Flower Lifecycle

Flowers grow from plants and serve as primary pollination targets for butterflies.

**Stage FL0: Bud**
- Small, closed flower
- Low emission, minimal visual presence
- Single pure tone when touched

**Stage FL1: Bloom** (First pollination)
- Petals open
- Emits 2-note motifs
- Attracts butterflies with nectar

**Stage FL2: Radiant** (3+ pollinations)
- Strong bioluminescence
- Emits 3-5 note phrases
- Higher nectar value and pollen yield

**Stage FL3: Meta** (5+ pollinations + progression stage 3+)
- Fractal visual patterns
- Evolving melodies
- Can spawn fruit seeds when fully pollinated
- Influences nearby fruits and plants

#### Pollination Loop

1. **Butterfly Approaches Flower**: When butterfly has low energy or no pollen, it seeks flowers
2. **Butterfly Lands on Flower**: 
   - Collects pollen (if flower has pollen)
   - Feeds on nectar (restores energy)
   - Triggers visual/audio feedback
3. **Butterfly Departs**: Carries pollen to next flower or fruit
4. **Pollination Deposit**: 
   - Increases flower's pollination count
   - Advances flower stage
   - Increases Harmony Level
   - Meta-Flowers can spawn fruit seeds

#### Flower Roles in Ecosystem

- **Energy Source**: Flowers provide nectar for butterfly energy
- **Pollen Collection**: Butterflies collect pollen from flowers
- **Ecosystem Metrics**: Pollination events increase Harmony Level
- **Fruit Spawning**: Meta-Flowers can spawn fruit seeds
- **Plant Growth**: Pollinated flowers signal plant growth system

### Fruit System

#### Fruit Growth Stages

Fruits are melodic energy orbs that butterflies feed from and evolve through stages.

**Stage F0: Seed**
- Small glowing orb
- Single pure tone
- Low energy output
- Triggers when harmony level rises or first butterfly feed

**Stage F1: Harmonic**
- Grows petals/facets
- Emits 2-3 note arpeggios
- Higher energy output
- Triggers when harmony > 20 or first butterfly feed

**Stage F2: Resonant**
- Complex geometry
- Emits chords and harmonic pads
- High energy output
- Triggers when 5+ butterfly feeds OR curiosity > 30

**Stage F3: Celestial**
- Levitates above ground
- Emits full-spectrum melodic sequences
- Maximum energy output
- Triggers when progression stage >= 4 (Synesthetic Overgrowth)

#### Fruit-Butterfly Interaction

- **Resonance Field**: Fruits emit energy field that butterflies detect
- **Energy Transfer**: Butterflies gain energy while feeding from fruits
- **Visual Feedback**: 
  - Wings intensify color
  - Sonification burst on landing
  - Trail brightness increases
  - Waveform tier can ascend after feeding
- **Feeding Behavior**: Low-energy butterflies prioritize fruits over flowers

#### Fruit Growth Triggers

- **Butterfly Feeds**: Each feed increments feed count
- **Pollen Deposition**: Pollen can accelerate fruit growth
- **Ecosystem Metrics**: Harmony, curiosity, and progression stage influence growth
- **Time-Based**: Fruits naturally evolve with ecosystem progression

### Rare Events

Events occur randomly every 10-20 minutes, or based on high meter levels:

**Butterfly Eclipse**
- All butterflies gather overhead
- Form a rotating disk/mandala
- Low harmonic drone plays
- Tracers overlap to create geometric patterns

**Harmonic Rain**
- Beams of light fall around you
- Touching rain beams plays notes
- Plants echo the melodies
- Creates cascading audio-visual sequences

**Chromatic Storm**
- All shaders increase emission dramatically
- Butterflies spiral in unison
- Ambient drones swell with harmony
- A wave of color passes through environment

**Great Chrysalis** (Stage 5 only)
- Giant chrysalis forms above player
- Pulsates with fractal patterns
- Emits low frequencies
- Cracks open, releasing dozens of waveform butterflies

**Butterfly Choir** (Stage 5 only)
- All butterfly voices synchronize
- Form multi-layer chord progression
- Geometric bloom visualizes around player

**Dissolution Into Frequency** (Stage 5 only)
- Geometry dissolves into waveforms
- Plants flatten into oscilloscopes
- Butterflies turn into pure sine ribbons
- Entire space becomes living audio-visual waveform
- Slowly reforms to Stage 0 or holds equilibrium

### Audio System

#### Per-Butterfly Audio
- Each butterfly generates its own audio voice
- Pitch modulated by flight speed and movement
- Volume responds to velocity (louder when moving fast)
- Frequency content varies by butterfly form (sine/saw/square/FM)

#### Plant Audio
- **Touch Sounds**: Musical tones that vary by plant type
- **Growth State Audio**: Different sounds for sprout/bloom/sentient states
- **Pollination Sounds**: Triggered when butterflies interact with plants

#### Environmental Audio
- **Audio Density**: Increases with progression stage (0.3 → 1.0)
- **Harmonic Layers**: Added in later stages
- **Spatial Audio**: 3D positioning with proper falloff
- **Global Mixing**: Managed by AudioManager for balance

### Hand Aura System

Your hands evolve through progression levels:

**Level 0: Neutral**
- No visual effects
- Standard butterfly interaction

**Level 1: Attractor Aura** (3 butterfly landings)
- Hands emit soft glow
- Butterflies approach more readily
- Increased landing probability

**Level 2: Conductor Aura** (7 landings + Serenity > 30)
- Trail particles align with hand movement
- Rotating gestures spawn micro-swarms
- Visual effects intensify

**Level 3: Gesture Spellbook** (Unlocked through gesture mastery)
- **Palm-Up Hold** (2+ seconds): Harmonic burst spell
- **Circle Gestures**: Butterfly vortex spell
- **Hand-to-Hand Arc**: Light ribbon connecting spell

## ✨ Features

### Core Experience
- **Generative Butterflies** - Procedurally spawned butterflies with unique flight patterns using Perlin/curl noise
- **Chrysalis System** - Pulsing spawn points that birth butterflies into the world
- **Interactive Plants** - Touch-reactive flora that responds with sounds, visual effects, and vibrations
- **Flower Pollination System** - Flowers that butterflies pollinate, creating a closed ecosystem loop
- **Fruit System** - Melodic energy orbs that butterflies feed from, evolving through stages
- **Butterfly Landing** - Butterflies can land on hands, plants, flowers, or fruits, creating dynamic interactions
- **Trail System** - Beautiful particle trails that follow butterfly movements
- **Audio-Visual Synthesis** - Butterfly motion maps to sound, creating an evolving soundscape
- **Flocking Behavior** - Butterflies form flocks using Boids algorithm when near each other
- **Energy & Pollination Systems** - Butterflies track energy and pollen, creating emergent behaviors

### Progression System
- **5-Stage Ecosystem Evolution** - The sanctuary evolves through stages based on your interactions:
  1. **Emergence** - Basic butterflies, minimal plants
  2. **Expansion** - More archetypes, luminescent trails
  3. **Symbiosis** - Butterfly-plant pollination, glowing interactions
  4. **Emergent Ecology** - Synchronized patterns, wind currents, new plant types
  5. **Ascension** - Synesthetic overgrowth, ambient light cycles, pure waveform entities

- **Progression Meters**:
  - **Serenity Level** - Increases when you remain still
  - **Curiosity Level** - Grows as you explore and touch plants
  - **Harmony Level** - Rises with butterfly landings and interactions

### Butterfly Transformations
- **Waveform Evolution Tiers**:
  - Standard → Sineform → Sawform → Squareform → FM-Modulated → Pure Waveform
- Butterflies visually morph based on progression stage
- Shader-based waveform deformation system

### Hand Aura System
- **3-Level Progression**:
  - **Neutral** - No aura
  - **Attractor** - Soft glow, butterflies approach (3 landings)
  - **Conductor** - Trails align with hand movement, rotating gestures spawn micro-swarms (7 landings + serenity)
  - **Gesture Spellbook** - Advanced gesture spells (palm-up, circles, hand-to-hand arc)

### Plant Growth System
- **4-Level Growth**:
  - **Sprout** - Minimal mesh, single note on touch
  - **Bloom** - New tendrils, bioluminescent pulses, multiple notes
  - **Fractal Bloom** - Procedural branch subdivision, responds to butterflies
  - **Psychedelic Sentience** - Fully responsive, emits chords, sways to audio

### Rare Events
- **Butterfly Eclipse** - All butterflies gather overhead into a rotating disk
- **Harmonic Rain** - Playable light beams fall around you
- **Chromatic Storm** - Synchronized spirals, cosmic chords, color waves
- **Great Chrysalis** (Stage 5) - Giant chrysalis with fractal patterns
- **Butterfly Choir** (Stage 5) - All voices synchronize into harmony
- **Dissolution Into Frequency** (Stage 5) - Space becomes pure waveform

### Environmental Phases
- **Light Cycles** - Dawn, Noon, Dusk, Midnight phases that affect:
  - Plant behavior
  - Butterfly flight speed
  - Audio layers
  - Shader emission levels

## 🎮 Requirements

- **Unity Version**: 2022.3 or later
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Platform**: Quest/OpenXR compatible (can also run in editor for testing)
- **XR SDK**: OpenXR or Oculus XR Plugin (for hand tracking)

## 🚀 Getting Started

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/ButterflyHouse.git
   cd ButterflyHouse
   ```

2. **Open in Unity**
   - Open Unity Hub
   - Add the `ButterflyHouse` folder as a project
   - Unity will import all assets and scripts

3. **Set up the Sample Scene**
   - In Unity, go to `Butterfly House > Setup Sample Scene`
   - This will automatically create a test scene with all managers and objects
   - Or open an existing scene from `Assets/Scenes/`

4. **Configure Settings**
   - Create a `Settings` ScriptableObject (`Create > Butterfly House > Settings`)
   - Adjust max butterflies, volume levels, and other global settings
   - Create `ButterflyArchetype` ScriptableObjects for butterfly types

5. **Test in Editor**
   - Press Play to test the experience
   - Use WASD to move (if not in VR)
   - Click/touch plants to interact

6. **Build for Quest** (Optional)
   - Switch platform to Android
   - Configure XR settings for Quest
   - Build and deploy to your headset

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Core/                          # Core systems
│   │   ├── GameController.cs         # Main orchestrator
│   │   ├── EcosystemStateController.cs # Progression tracking
│   │   ├── ProgressionStageManager.cs  # Stage transitions
│   │   ├── EventOrchestrator.cs       # Rare events
│   │   ├── HandAuraSystem.cs          # Hand aura evolution
│   │   ├── Settings.cs                # Global settings (SO)
│   │   └── LightCycle.cs              # Environmental phases
│   │
│   ├── Butterflies/                   # Butterfly systems
│   │   ├── Butterfly.cs              # Main behavior controller
│   │   ├── ButterflyManager.cs       # Pooling and management
│   │   ├── ButterflyArchetype.cs     # Butterfly type (SO)
│   │   ├── ButterflyVisualController.cs
│   │   ├── ButterflyAudio.cs         # Per-butterfly audio
│   │   ├── ButterflyFormEvolution.cs # Waveform transformations
│   │   └── Chrysalis.cs              # Spawn points
│   │
│   ├── Plants/                        # Plant systems
│   │   ├── GenerativePlant.cs        # Interactive plant
│   │   ├── PlantVisualController.cs  # Visual effects
│   │   ├── PlantGrowthSystem.cs      # Growth progression
│   │   ├── GenerativeFruit.cs        # Fruit objects
│   │   ├── FruitGrowthSystem.cs      # Fruit growth stages
│   │   ├── FruitVisualController.cs  # Fruit visual effects
│   │   └── PlantManager.cs           # Plant management
│   │
│   ├── Flowers/                       # Flower systems
│   │   ├── Flower.cs                 # Interactive flower
│   │   └── FlowerVisualController.cs # Flower visual effects
│   │
│   ├── Butterflies/                   # Butterfly systems
│   │   ├── Butterfly.cs              # Main behavior controller
│   │   ├── ButterflyManager.cs       # Pooling and management
│   │   ├── ButterflyArchetype.cs     # Butterfly type (SO)
│   │   ├── ButterflyVisualController.cs
│   │   ├── ButterflyAudio.cs         # Per-butterfly audio
│   │   ├── ButterflyFormEvolution.cs # Waveform transformations
│   │   ├── ButterflyEnergy.cs        # Energy tracking
│   │   ├── ButterflyPollination.cs   # Pollen tracking
│   │   └── Chrysalis.cs              # Spawn points
│   │
│   ├── Core/                          # Core systems
│   │   ├── GameController.cs         # Main orchestrator
│   │   ├── EcosystemStateController.cs # Progression tracking
│   │   ├── EcosystemOrchestrator.cs  # Central ecosystem brain
│   │   ├── ProgressionStageManager.cs # Stage transitions
│   │   ├── EventOrchestrator.cs      # Rare events
│   │   ├── HandAuraSystem.cs         # Hand aura evolution
│   │   ├── Settings.cs               # Global settings (SO)
│   │   └── LightCycle.cs             # Environmental phases
│   │
│   ├── Interaction/                   # Interaction systems
│   │   ├── InteractionManager.cs     # XR interaction bridge
│   │   ├── LandingTarget.cs          # Landing zones
│   │   └── HandProxy.cs              # Hand tracking proxy
│   │
│   └── Audio/                         # Audio systems
│       └── AudioManager.cs           # Central audio manager
│
├── Editor/                            # Editor tools
│   ├── ButterflyHouseSceneSetup.cs   # Auto scene setup
│   ├── AudioDiagnostics.cs           # Audio debugging
│   └── FixTrailAndAudio.cs           # Material fixes
│
├── Prefabs/                           # Prefab assets
├── ScriptableObjects/                 # SO assets
│   ├── Butterflies/                   # Butterfly archetypes
│   └── Plants/                        # Plant configurations
│
└── Scenes/                            # Scene files
    └── ButterflyHouse.unity          # Main scene
```

## 🎨 Key Systems

### Progression System
The `EcosystemOrchestrator` is the central brain that coordinates all systems:
- **EcosystemStateController**: Tracks three main meters:
  - **Serenity**: Increases when player is still (measured via head/body movement)
  - **Curiosity**: Increases with exploration, plant touches, gestures
  - **Harmony**: Increases with butterfly landings, pollination events, and plant interactions
- **ProgressionStageManager**: Evaluates current stage based on ecosystem metrics
- **Subsystem Coordination**: Informs ButterflyManager, FruitManager, PlantManager, HandAuraSystem, and EventOrchestrator

Stage progression is automatic based on meter thresholds and time spent in the experience.

### Butterfly Flight
Butterflies use procedural flight paths:
- **Perlin/curl noise** for smooth, organic motion
- **Orbit patterns** around central points
- **Flocking behaviors** (Boids algorithm in Stage 3+):
  - **Cohesion**: Move toward nearby butterflies
  - **Alignment**: Align with nearby butterfly velocities
  - **Separation**: Avoid crowding
- **Target seeking** for landing zones (fruits, flowers, plants, hands)
- **Energy-based behavior**: Low-energy butterflies prioritize fruits and flowers
- **Random lifetimes**: Butterflies can live 0.8x-3x base lifetime, or be immortal (10% chance)

### Audio System
- **Per-butterfly audio voices** that modulate based on movement
- **Plant sound effects** triggered on touch
- **Flower nectar melodies** played when butterflies land on flowers
- **Fruit melodic sequences** that evolve with fruit growth stages
- **Global audio density** adjusted by progression stage
- **Spatial 3D audio** with proper falloff

### Pollination & Energy Systems
- **Butterfly Energy**: Tracks energy levels that decay over time (after 10 seconds)
- **Energy Feeding**: Butterflies gain energy from fruits and flowers
- **Pollen Collection**: Butterflies collect pollen from flowers
- **Pollen Deposition**: Butterflies deposit pollen on other flowers or fruits
- **Pollination Effects**: Increases Harmony Level, advances flower stages, spawns fruits

### Material System
- **Shader Graph/HLSL shaders** for generative materials
- **MaterialPropertyBlock** for runtime modification
- **URP-compatible** with fallbacks for built-in pipeline
- **Waveform deformation** shaders for butterfly transformations

## 🛠️ Customization

### Creating Butterfly Archetypes

1. Right-click in Project window
2. `Create > Butterfly House > Butterfly Archetype`
3. Configure:
   - Color gradient
   - Flight speed curves
   - Audio parameters (pitch, volume, tone)
   - Scale and lifetime

### Creating Stage Configurations

1. `Create > Butterfly House > Stage Configuration`
2. Set stage-specific:
   - Trail settings (time, width, luminescence)
   - Audio density and harmonics
   - Behavior flags (flocking, wind currents, etc.)

### Adjusting Progression Thresholds

Edit `EcosystemStateController.cs` to modify:
- Stage transition requirements
- Meter decay/growth rates
- Serenity detection sensitivity

## 🐛 Troubleshooting

### No Sound from Butterflies
- Check `AudioDiagnostics` menu (`Butterfly House > Diagnose Audio Issues`)
- Ensure `AudioListener` is on Main Camera
- Verify `AudioSource` components are configured
- Check volume settings in `Settings` ScriptableObject

### Magenta Materials
- Ensure URP is set as the render pipeline
- Run `Butterfly House > Fix Trail Materials` menu item
- Check that shaders exist in project

### Butterflies Not Spawning
- Verify chrysalises have `ButterflyArchetype` assigned
- Check `ButterflyManager` has butterfly prefab assigned
- Ensure `maxButterflies` setting is not 0

## 📚 Documentation

- `Assets/README_BUTTERFLY_HOUSE.md` - Detailed system documentation
- `Assets/SETUP_GUIDE.md` - Setup instructions
- `Assets/WHAT_TO_EXPECT.md` - Runtime behavior guide

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Built for immersive meditation and relaxation experiences
- Inspired by generative art and procedural audio-visual systems
- Designed for Quest/OpenXR mixed reality platforms

---

**Note**: This is an experimental MR experience. Performance may vary depending on device capabilities. For best results, use on Quest 2 or later.

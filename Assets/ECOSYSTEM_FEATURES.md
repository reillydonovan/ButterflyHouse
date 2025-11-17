# Ecosystem Evolution Features

This document describes the comprehensive ecosystem evolution system for the Psychedelic Butterfly House.

## Overview

The ecosystem evolves dynamically based on:
- **Time spent** in the experience
- **Player interactions** (touching plants, gestures)
- **Butterfly-plant interactions** (pollination, symbiosis)
- **Butterflies landing on player** (affinity building)

## System Architecture

### Core Systems

1. **EcosystemManager** - Main orchestrator for ecosystem phases
2. **HandAuraSystem** - Hand aura evolution and gesture recognition
3. **EventSystem** - Rare spectacular events (storms, eclipses, etc.)
4. **LightCycle** - Environmental lighting phases (Dawn, Noon, Dusk, Midnight)
5. **PlantGrowthSystem** - Plant growth and evolution phases

### Progression Meters

All meters range from 0-100:

- **Harmony Level** - Butterfly-plant interactions
  - Increases when butterflies land on plants/fruits
  - Required for ecosystem phase progression

- **Serenity Meter** - Player stillness
  - Increases when player stays still
  - Decreases when player moves
  - Affects environmental calmness

- **Curiosity Meter** - Player exploration
  - Increases when player touches/interacts with objects
  - Encourages exploration and interaction

- **Affinity Level** - Butterflies landing on player
  - Increases when butterflies land on hands
  - Unlocks hand aura levels
  - Required for certain events

## Ecosystem Phases

### Phase 1: Emergence
- **Duration**: 1 minute
- **Features**:
  - Simple butterflies from chrysalises
  - Soft soundscape
  - Plants sway gently
  
### Phase 2: Territorial Patterns
- **Duration**: 2 minutes (after Phase 1)
- **Requirements**: None
- **Features**:
  - Butterflies form flocks around player
  - New butterfly archetypes spawn
  - Trails become longer and more luminescent

### Phase 3: Symbiotic Relations
- **Duration**: 3 minutes (after Phase 2)
- **Requirements**: Harmony Level > 30
- **Features**:
  - Butterflies "pollinate" plants
  - Plants charge with color
  - Plants grow new tendrils
  - Plant surfaces glow where butterflies land

### Phase 4: Emergent Ecosystem
- **Requirements**: Harmony Level > 60, Affinity Level > 40
- **Features**:
  - Plants spread procedurally
  - "Queen chrysalis" appears (spawns rare butterflies)
  - Wind currents become visible
  - Ribbons guide flight paths

## Hand Aura Evolution

### Level 1: Neutral
- No special effects
- Default state

### Level 2: Butterfly Magnet
- **Requirements**: 5 butterflies land on player hands
- **Features**:
  - Shimmering aura appears on hands
  - Aura color shifts with audio frequencies
  - Butterflies more likely to follow hands
  - Butterflies treat hands like conductor's baton

### Level 3: Gesture Summoning
- **Requirements**: 15 butterflies land on player hands
- **Features**:
  - Gestures trigger special effects:
    - **Circle** → Swarm burst
    - **Pulse** → Harmonic chord
    - **Hand-to-hand arc** → Spiral of waveform butterflies
  - Gestures can temporarily morph environment

## Plant Growth Phases

### Phase 1: Small Bulb
- Initial state
- Basic interactions

### Phase 2: Sprout
- **Requirements**: 3 touches
- Plants begin to grow

### Phase 3: Fractal Bloom
- **Requirements**: 5 butterfly visits
- **Features**:
  - Plants emit chords instead of single notes
  - More complex visual patterns

### Phase 4: Psychedelic Sentience
- **Requirements**: 5 minutes in Bloom phase
- **Features**:
  - Plants respond to footsteps
  - Plants respond to hand movement
  - Generate particle loops around themselves
  - Emit complex chords

## Light Cycle Phases

Cycles every 5 minutes through 4 phases:

### Dawn (Soft Pastels)
- **Effects**: New butterfly births increase
- Soft, gentle lighting
- Ambient: Pastel colors

### Noon (Bright Rainbow Hues)
- **Effects**: Flight speed increases
- Bright, refractive rainbow lighting
- Ambient: Bright colors

### Dusk (Deep Purples)
- **Effects**: Harmonics intensify
- Deep purple, firefly trails
- Ambient: Purple tones

### Midnight (Everything Glows)
- **Effects**: Plants whisper, butterflies become waveforms
- Internal glow from all objects
- Ambient: Dark with glowing elements

## Synesthetic Storm Events

Rare events that occur every 10-20 minutes or when triggered by conditions:

### Butterfly Eclipse
- **Requirements**: Harmony Level > 50
- **Effects**:
  - All butterflies gather overhead
  - Form rotating disk
  - Generate low harmonic drone
  - Project shapes on dome

### Harmonic Rain
- **Availability**: Phase 2+
- **Effects**:
  - Vertical beams of light fall like rain
  - Touching beam plays note
  - Releases trapped butterflies

### Chromatic Storm
- **Requirements**: Harmony Level > 70
- **Effects**:
  - Trails thicken dramatically
  - Butterflies fly in synchronized spirals
  - Plants release spores of light
  - Space "rings" with cosmic chord

## Meta-Level Ascension Events

Rare, one-time events for endgame experiences:

### Chrysalis of Consciousness
- **Requirements**: Harmony Level > 90, Affinity Level > 80
- **Effects**:
  - Giant chrysalis appears above player
  - Shows morphing fractal patterns
  - Silhouette of cosmic butterfly
  - Bursts into cathedral of light
  - Releases dozens of waveform butterflies
  - Re-seeds entire environment

### Butterfly Choir
- **Requirements**: Affinity Level > 70
- **Effects**:
  - All butterflies form choir formation
  - Each contributes tone
  - Together form coherent melody
  - Melody based on player interactions

### Dissolution into Pure Frequency
- **Ultimate transformation**:
  - Geometry dissolves
  - Butterflies become pure waveforms
  - Plants become oscilloscopes
  - Space becomes living audio-visual waveform
  - Then slowly reforms to normal

## Integration

### Adding to Scene

1. Add **EcosystemManager** to scene
2. Add **HandAuraSystem** to scene (or it will be created by EcosystemManager)
3. Add **EventSystem** to scene (or it will be created by EcosystemManager)
4. Add **LightCycle** to scene (or it will be created by EcosystemManager)

### Setup

The ecosystem systems automatically:
- Track player interactions
- Monitor butterfly-plant interactions
- Progress through phases based on time and metrics
- Trigger events based on conditions
- Update lighting cycles

### Customization

All phase durations, requirements, and thresholds can be adjusted in Inspector:
- **EcosystemManager**: Phase durations, meter thresholds
- **HandAuraSystem**: Landing requirements for aura levels
- **EventSystem**: Event intervals and requirements
- **LightCycle**: Cycle duration, lighting colors

## Next Steps

To fully implement all features:

1. **Butterfly Transformations**:
   - Implement waveform evolutions (Sine, Saw, Square, FM)
   - Add chord formation when butterflies gather
   - Create geometric formations

2. **Plant Rare Events**:
   - Chromatic Bloom effect
   - Bass Pulse synchronized butterfly beat
   - Harmonic Mirror reflective surface

3. **Flocking Behavior**:
   - Implement butterfly flocking around player
   - Add wind current visualization
   - Create flight path guidance

4. **Visual Effects**:
   - Implement aura visuals for hands
   - Create event visual effects
   - Add light cycle transitions

5. **Queen Chrysalis**:
   - Special chrysalis type
   - Spawns rare butterflies
   - Appears at Phase 4

## Status

✅ Core systems created:
- EcosystemManager
- HandAuraSystem
- EventSystem
- LightCycle
- PlantGrowthSystem

✅ Integration:
- Butterfly integration (tracks landings on player/plants)
- Plant integration (tracks touches and butterfly visits)
- Progression meters working

⏳ To implement:
- Visual effects for phases
- Flocking behavior
- Butterfly transformations
- Plant rare events
- Queen chrysalis
- Wind currents


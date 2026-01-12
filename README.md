# Braed

A strategic Match-3 game with roguelite progression, built in Unity.

## Overview

Braed combines puzzle mechanics with resource management and roguelite elements. Match tiles to accumulate Might and Blessing, then cast to convert them into score. Progress through increasingly difficult Cycles and Trials while collecting powerful Relics.

## Core Mechanics

### Tiles

| Tile | Effect |
|------|--------|
| Might | Increases raw power |
| Blessing | Multiplies effectiveness |
| Shard | Currency for shop purchases |
| Fury | Multiplies Might gains |
| Mirror | Multiplies Blessing gains |
| Totem | Grants passive bonuses |
| Blight | Reduces Might and Blessing |

### Progression

- **Score Calculation**: Total Score = Σ(Might × Blessing) across all casts
- **Trials**: Complete 3 trials per cycle
- **Cycles**: Difficulty scales with each cycle
- **Shop**: Purchase relics between trials using Shards

### Resources

- **Flow**: Number of tile swaps per trial
- **Whirl**: Board shuffle uses

## Project Structure

```
Assets/
├── Scripts/
│   ├── Match3/       # Core game logic
│   ├── Relic/        # Relic system
│   ├── Shop.cs       # Shop mechanics
│   ├── Save/         # Save/Load system
│   └── UI/           # UI controllers
├── Data/
│   └── Relic/        # Relic ScriptableObjects
└── Scenes/
    ├── Menu
    └── Game
```

## Requirements

- Unity 2022.3 LTS or later
- TextMeshPro package

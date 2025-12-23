# Braed - Match 3 RPG

**Braed** is a unique Match-3 strategy game that blends puzzle mechanics with RPG progression. Players must match tiles to gather resources, cast powerful spells, and overcome increasing difficulties through Cycles and Trials.

## 🎮 Game Mechanics

### Tiles & Resources
The game features 7 distinct tile types, each interacting with your core stats:
- **❤️ Might** (Red): Increases your raw power.
- **✨ Blessing** (Cyan): Multiplies your effectiveness.
- **💎 Shard** (Yellow): Currency for the shop/upgrades.
- **🌿 Fury** (Green): Multiplies your Might.
- **🔮 Mirror** (Magenta): Multiplies your Blessing.
- **🗿 Totem** (White): (Functionality to be discovered/implemented).
- **☣️ Blight** (Black): A hazardous tile that reduces Might and Blessing. Must be managed carefully!

### Progression System
- **Cycles & Trials**: Progress through increasingly difficult stages.
- **Score Requirement**: To pass a level, you must accumulate enough **Total Score** (Calculated as `Might * Blessing`) to meet the `RequireScore`.
- **Flow & Whirl**: Manage your turn counters to optimize your board state before casting.

### Gameplay Loop
1.  **Match Tiles**: Swap tiles to create linear matches of 3 or more.
2.  **Gather Stats**: Accumulate Might, Blessing, and Shards.
3.  **Cast**: Trigger the `Cast()` ability to calculate your Total Score (`Might * Blessing`).
4.  **Advance**: If your score exceeds the requirement, you advance to the next Trial or Cycle.

## 🛠️ Project Structure

- `Assets/Scripts/Match3/`: Core game logic including board state (`Match3Game.cs`), matching algorithms (`Match.cs`), and skin definitions.
- `Assets/Scripts/Tile/`: Tile definitions and behaviors.
- `Assets/Scripts/Relic/`: System for game-enhancing items (Relics).
- `Assets/Scenes/`:
  - `Menu`: Main entry point.
  - `Game`: The core gameplay loop.
  - `Shop`: Upgrade and item interface.

## 🚀 Getting Started

1.  **Prerequisites**: Unity 2021.3 or later (Check `ProjectVersion.txt` for exact version).
2.  **Installation**:
    ```bash
    git clone https://github.com/phamhung1701/match3.git
    ```
3.  **Open in Unity**: Add the project folder to Unity Hub and open.
4.  **Play**: Open `Assets/Scenes/Menu.unity` and press Play.

## 🤝 Contributing

This project is under active development. Feel free to submit Pull Requests or open Issues for bugs and feature suggestions.

## 📄 License

[MIT License](LICENSE) (Assuming standard open source perms, please verify)

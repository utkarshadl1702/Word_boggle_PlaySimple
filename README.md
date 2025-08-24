# Word Boggle PlaySimple

A Unity-based word puzzle game inspired by Boggle. Players form words by selecting adjacent letter tiles on a grid. The game supports both endless and level-based modes, with scoring, objectives, and special tiles.
Unity Version:- 2022.3.62f1

## Features

- **Endless Mode:** Random grid generation for continuous play.
- **Levels Mode:** Predefined levels with objectives (make X words, reach score, collect bugs, etc.).
- **Scoring System:** Each tile has a score; bonus and blocked tiles affect gameplay.
- **Word Validation:** Words are checked against a dictionary ([wordlist.txt](Assets/Resources/wordlist.txt)).
- **UI:** Displays current word, score, average score, words formed, objectives, and timer.
- **Special Tiles:** Bug and blocked tiles add variety and challenge.
- **Grid Refilling:** Tiles are removed and refilled after valid words in endless mode.

## Project Structure

- `Assets/Scripts/GameController/`  
  - [`GameManager`](Assets/Scripts/GameController/GameManager.cs): Main game logic and state.
  - [`WordManager`](Assets/Scripts/GameController/WordManager.cs): Word validation and scoring.
  - [`GridManager`](Assets/Scripts/GameController/GridManager.cs): Grid creation and management.
  - [`WordDragManager`](Assets/Scripts/GameController/WordDragManager.cs): Handles tile selection and word formation.
- `Assets/Scripts/Tile/`  
  - [`LetterTile`](Assets/Scripts/Tile/LetterTile.cs): Represents a letter tile.
  - [`GridCell`](Assets/Scripts/Tile/GridCell.cs): Visual and selection logic for grid cells.
  - [`ScoreIndicator`](Assets/Scripts/Tile/ScoreIndicator.cs), [`ScoreDot`](Assets/Scripts/Tile/ScoreDot.cs): Visual score indicators.
- `Assets/Scripts/Utils/`  
  - [`Leveldata`](Assets/Scripts/Utils/Leveldata.cs): Level data structures.
- `Assets/Scripts/UIManager.cs`: UI logic.
- `Assets/Resources/wordlist.txt`: Dictionary of valid words.

## How to Play

1. **Endless Mode:**  
   - Click "Endless" to start.  
   - Form words by dragging across adjacent tiles.  
   - Words must be at least 2 letters and in the dictionary.

2. **Levels Mode:**  
   - Click "Levels" to start a random level.  
   - Complete the objective (e.g., make X words, reach score, collect bugs) before time runs out.

## How to Build & Run

1. Open the project in Unity (2022.3.62f1 recommended).
2. Open [My project.sln](My%20project.sln) in Visual Studio or VS Code for code editing.
3. Press Play in the Unity Editor.

## Customization

- **Add Words:** Edit [wordlist.txt](Assets/Resources/wordlist.txt).
- **Level Data:** Modify level JSON in `Assets/Resources/test-unity-master-Assets/Assets/levelData`.
- **Grid Size & Settings:** Adjust in [`GameManager`](Assets/Scripts/GameController/GameManager.cs) and [`GridManager`](Assets/Scripts/GameController/GridManager.cs).



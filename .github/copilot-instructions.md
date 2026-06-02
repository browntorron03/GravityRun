# Gravity Run - AI Development Guide

## Project Overview
**Gravity Run** is a 3D platformer game built in Unity 6 (v6000.0.75f1) using C# with CharacterController-based movement. The game features level progression, collectible scoring, and pause mechanics. All gameplay scripts are located in `Assets/Scripts/` with a singleton `GameManager` orchestrating game state.

## Architecture & Core Components

### GameManager (Central Hub)
- **Path**: [Assets/Scripts/GameManager.cs](Assets/Scripts/GameManager.cs)
- **Role**: Singleton that manages all game state and UI transitions
- **Key States**: `Playing`, `Death`, `GameOver`, `BeatLevel`
- **Public API**:
  - `AddGold(int)`: Updates score (gold × 100)
  - `CollectTrophy()`: Triggers level completion
  - `BeatLevel()`: Shows win UI and pauses game
  - `playAgain()` / `nextLevel()`: Scene loading
- **Important**: Access via `GameManager.gm` singleton; always null-check when calling from non-manager scripts

### Player Movement System
- **Controller**: [PlayerController.cs](Assets/Scripts/PlayerController.cs)
- **Uses CharacterController** (not Rigidbody) for movement
- **Input**: WASD/Horizontal-Vertical axes for movement, Space for jump
- **Movement Formula**: 
  - Horizontal: camera-relative (forward/right vectors)
  - Vertical: gravity-affected with custom `gravityScale` multiplier
  - Animation: Speed parameter tied to input magnitude
- **Key Fields**: `moveSpeed`, `jumpForce`, `gravityScale`, `rotateSpeed`

### Camera System
- **Controller**: [CameraController.cs](Assets/Scripts/CameraController.cs)
- **LateUpdate Pattern**: Updates after player movement to prevent jitter
- **Pause Integration**: Returns early and unlocks cursor when `GameManager.gm.pauseGame = true`
- **Pivot-Based Rotation**: Uses separate Transform pivot for Y-axis rotation independent of player model

### Game Mechanics
1. **Health System** ([HealthManager.cs](Assets/Scripts/HealthManager.cs)): 
   - Destroys player GameObject when health ≤ 0
   - Used by `HurtPlayer` triggers
   
2. **Collectibles** ([GoldPickup.cs](Assets/Scripts/GoldPickup.cs)):
   - Trigger-based collection with GameManager.AddGold() call
   - Instantiates pickup effect and destroys itself
   - Value is configurable per prefab instance

3. **Level Win Condition** ([Trophy.cs](Assets/Scripts/Trophy.cs)):
   - Trigger collision with player calls GameManager.CollectTrophy()
   - Sets `canBeatLevel = true`
   - Pauses game and shows UI

4. **Damage Triggers** ([HurtPlayer.cs](Assets/Scripts/HurtPlayer.cs)):
   - HealthManager.HurtPlayer(damage) on player collision
   - Configurable damage per trap

5. **Temporary Effects** ([DestroyOverTime.cs](Assets/Scripts/DestroyOverTime.cs)):
   - Auto-destroy after `lifetime` seconds
   - Used for pickup effects, VFX cleanup

## Critical Patterns & Conventions

### Singleton Access
```csharp
// ALWAYS null-check GameManager access
if (GameManager.gm != null)
    GameManager.gm.AddGold(10);
```

### Pause State Management
- Game-wide pause controlled by `GameManager.pauseGame`
- CameraController respects pause (unlocks cursor, returns early)
- **TODO**: PlayerController & UI systems should also respect pause flag

### Collision Detection
- All interactive objects use **trigger colliders** (IsTrigger=true)
- Player must have tag "Player" for detection
- Pattern: `OnTriggerEnter()` → check tag → call GameManager/HealthManager methods

### FindObjectOfType Usage
- `GoldPickup`, `HurtPlayer` use `FindObjectOfType<GameManager/HealthManager>()` in Start()
- Works because GameManager is singleton, but slower than direct reference
- **Recommendation**: Assign via Inspector or use GameManager.gm for performance

### Audio System
- Two methods in GameManager:
  - `playAudioRepeat()`: Background music loop (via camera AudioSource)
  - `playAudioOneTime()`: SFX via PlayClipAtPoint
- **Note**: No audio is wired in current implementation (empty Update)

## Build & Deployment
- **Target Platform**: Standalone OSX (configurable in .csproj)
- **Built Output**: Gravity Run(Windows)/ contains D3D12 build for Windows platform
- **Assembly**: Compiled to `Temp/bin/Debug/Assembly-CSharp.dll`
- **Input System**: New Input System enabled via InputSystem_Actions.inputactions

## Scene Structure
- **Scenes**: Located in [Assets/Scenes/](Assets/Scenes/)
- **Navigation**: Uses build index sequential loading (`nextLevel()` → buildIndex + 1)
- **First Scene**: "Level 1" is hardcoded restart target

## Code Quality Notes

### Potential Issues to Fix
1. **CameraController**: `maxViewAngle`/`minViewAngle` defined but never used
2. **PlayerController**: Rigidbody code commented out (clean up or document why)
3. **GameManager.Update()**: Empty method (remove if no logic planned)
4. **Pause Logic Incomplete**: PlayerController doesn't check `pauseGame` flag
5. **Audio Not Initialized**: `playAudioRepeat()` never called in current code

### Recommended Improvements
- Add input validation (check Input axes vs GetButtonDown)
- Implement pooling for frequently instantiated effects
- Cache FindObjectOfType results instead of calling each Start()
- Add editor validation to ensure Player tag exists and is assigned

## Dependencies & Imports
- **Core**: UnityEngine, CharacterController
- **UI**: TextMeshPro (TMP_Text), Button
- **Scene Management**: SceneManager (build-index based)
- **Audio**: AudioSource, AudioClip
- **Animation**: Animator (speed parameter only)

## Common Tasks

### Adding a New Interactive Object
1. Create script inheriting MonoBehaviour
2. Use `OnTriggerEnter(Collider other)` with tag check
3. Call appropriate GameManager/HealthManager method
4. Example: See [GoldPickup.cs](Assets/Scripts/GoldPickup.cs)

### Modifying Player Movement
- Edit `PlayerController`: `moveSpeed`, `jumpForce`, `gravityScale`
- Test gravity scale against Physics.gravity.y setting
- Remember CharacterController ignores Rigidbody physics

### Adding Level-Specific Audio/VFX
- Reference in GameManager (Inspector-assigned)
- Call `playAudioRepeat()` or `playAudioOneTime()` at appropriate state transitions
- Use `DestroyOverTime` for temporary effects

---

**Last Updated**: June 2, 2026 | **Unity Version**: 6000.0.75f1 | **Language**: C# (.NET Standard 2.1)

# DomeFight

## 1. Title Page
#### Game Name: Dome Fight
#### Tag Line: "Didn't see that coming, huh?"
#### Team
- Lead Designer & Programmer: Amano Teru / Roy Yang

## 2. Game Overview
#### Purpose of the game
Dome Fight is a special planetarium-based arena brawler dedicated to funny and chaotic offline multiplayer combat offline in a 360-degree dome planetarium. The game uses the planetarium dome as the arena...

#### Intended Use
Dome Fight is intended as a prototype for dome planetarium display events and an experiment of implementing video games in planetariums.

#### Justification for the Use
A brawler game using the dome as the arena can bring very different experiences with conventional brawlers. This game is a salute to Stick Fight, a 2d multiplayer minimalist brawler in which players engage in chaotic combat with map mechanisms and randomly obtained weapons. The dome arena allows the players to go 360 degrees around, projectiles to fly across the top of the dome and hit another player on the other side, players to fly across if they gain enough momentum, etc. It brings extra chaos to the party and can be very entertaining. 

#### Target audience
Dome Fight is mainly targeted at players who enjoy local party games, particularly teenagers or young adults bringing their friends to the dome.

## 3. Gameplay
#### Objectives
Players fight to eliminate opponents by using guns or melee weapons and environmental tactics to gain points. Whoever scores the targeted number of scores wins.




## 4. Mechanics

#### Game Progression
##### demo Version
- Scoring
	- 2-4 Players can choose the total number of scores needed to win.
	- The players fight each other, and the last one that survives gain 1 score.
	- Whoever is the first to reach the chosen winning score, wins the game. (This guarantees 1 winner)
- Maps
	- A random map is chosen from the map pool for each score.
- Weapons
	- The player starts with a default weapon that they can choose before the start of the entire game
		- Pistol, Katana, SMG, Super Shorty
	- Weapons will be randomly generated throughout the map once a given time passes.

#### Player Actions 
- Combat
	- Primary Weapon
		- RT - fire
		- RB - reload
		- R joystick - aim
		- LT - block bullets if weapon permits, with cooldown
	- Melee
		- Y - melee attack
- Movement
	- L joystick - Left and Right
	- A - jump, double jump, wall jump



# Unity Project Naming Rules

### 1. Git Branch Naming
Format：`type/description-ID`  
Examples：
```
feature/player-movement-123  
bugfix/ui-overlap-456  
hotfix/login-crash-789  
release/v1.2.0  
```

| Prefixs    | Meaning                             |
|------------|-------------------------------------|
| `feature/` | New feature development             |
| `bugfix/`  | Bug fix                             |
| `hotfix/`  | Emergency hot fix                   |
| `release/` | Official release                    |
| `docs/`    | Documentation update                |

---

### 2. Git Commit Message
Format：`type(range): Description [ID]`  
Examples：
```git
feat(Player): Add double jump [123]
fix(UI): Fix healthbar overlap [456]
refactor(AI): Pathfinding optimization
docs: Update documentation
```

| Prefixs    | Meaning                                    |
|------------|--------------------------------------------|
| `feat`     | New feature                                |
| `fix`      | Bug fix                                    |
| `refactor` | Code refactoring                           |
| `docs`     | Documentation changes                      |
| `test`     | Testing                                    |
| `chore`    | Change of construct/configuration          |

---

### 3. C# Code Naming
#### Class/Interface/enum
Follow：`PascalCase`  
Examples：
```csharp
public class PlayerController { }  
public interface IDamageable { }  
public enum EnemyType { Melee, Ranged }
```

#### Methods
Follow：`PascalCase` with opening verb
Examples：
```csharp
public void Jump() { }  
private IEnumerator LoadSceneAsync() { }
```

#### Variables/Parameters
- public fields：`PascalCase`
- private fields：`_camelCase`
- variables：`camelCase`
- constants：`ALL_CAPS`

Examples：
```csharp
private float _moveSpeed;
public static int Mode;
public int MaxHealth { get; }  
const float GRAVITY = 9.81f;  

void SetDamage(int damageAmount) { 
    int temporaryDamage = damageAmount * 2; 
}
```


---

### 4. Project Hierarchy（Expandable）
```
Assets/
├─ Code/
│  ├─ Scripts/
│  │  ├─ Character/
│  │  │  ├─ XxxxxXxx.cs
│  │  ├─ Utils/
│  │  │  ├─ XxxxxXxx.cs
│  ├─ Tests/
│  │  ├─ XxxxXxxxTests.cs
├─ Prefabs/
├─ Scenes/
│  ├─ Level_1_Forest.unity
```

Scene naming：`type_index_description` (e.g.: `Level_2_Castle.unity`)

---
### Extras
1. Make sure naming is closely related to its function
2. Avoid all abbreviations, except extremely common ones like UI and AI
3. Use `[SerializeField]` instead of `public` to expose variables in editor
``` 

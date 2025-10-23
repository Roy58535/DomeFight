# DomeFight

## Unity Project Naming Rules

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

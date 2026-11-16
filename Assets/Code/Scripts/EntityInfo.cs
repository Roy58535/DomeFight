using System;
using UnityEngine;


public class EntityInfo : MonoBehaviour
{
    public event Action<EntityInfo> OnEntityDeath;
    public event Action<int, int> OnHealthChanged;

    [SerializeField] private int _hp = 100;
    [SerializeField, Min(1)] private int _maxHp = 100;

    public int CurrentHealth => _hp;
    public int MaxHealth => _maxHp;
    
    private bool _isDead = false;

    private void Awake()
    {
        // Keep the initial health within the configured range without showing combat UI.
        _hp = Mathf.Clamp(_hp, 0, _maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0)
            return;

        _hp = Mathf.Max(0, _hp - damage); // Ensure HP doesn't go below 0
        OnHealthChanged?.Invoke(_hp, _maxHp);
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining HP: " + _hp);
        if (_hp == 0)
        {
            _isDead = true;
            OnEntityDeath?.Invoke(this);
        }
    }
}

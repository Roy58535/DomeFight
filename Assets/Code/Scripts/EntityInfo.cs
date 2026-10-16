using System;
using UnityEngine;


public class EntityInfo : MonoBehaviour
{
    public event Action<EntityInfo> OnEntityDeath;

    [SerializeField] private int _hp = 100;
    
    private bool _isDead = false;

    private void Start()
    {
        _hp = 100;
    }

    public void TakeDamage(int damage)
    {
        if (_isDead || damage <= 0)
            return;

        _hp = Mathf.Max(0, _hp - damage); // Ensure HP doesn't go below 0
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining HP: " + _hp);
        if (_hp == 0)
        {
            _isDead = true;
            OnEntityDeath?.Invoke(this);
        }
    }
}

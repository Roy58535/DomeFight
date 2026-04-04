using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInfo : MonoBehaviour
{
    private int _hp = 100;
    private void Start()
    {
        _hp = 100;
    }
    private void Update()
    {
        if (_hp <= 0)
        {
            // Implement death logic
            Debug.Log(gameObject.name + " has died.");
        }
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining HP: " + _hp);
    }
}

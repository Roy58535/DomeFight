using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinSetup : MonoBehaviour
{
    [SerializeField] private ObjectPool _bulletPool;
    public void OnPlayerJoined(PlayerInput player)
    {
        foreach (Weapon weapon in player.GetComponentsInChildren<Weapon>(true))
        {
            weapon.Initialize(_bulletPool);
        }
    }
}

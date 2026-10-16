using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinSetup : MonoBehaviour
{
    [SerializeField] private ObjectPool _bulletPool;
    [SerializeField] private RoundManager _roundManager;
    public void OnPlayerJoined(PlayerInput player)
    {
        foreach (Weapon weapon in player.GetComponentsInChildren<Weapon>(true))
        {
            weapon.Initialize(_bulletPool);
        }

        _roundManager.AddPlayer(player.gameObject);
        if (!_roundManager.IsRoundActive)
        {
            _roundManager.StartRound();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinSetup : MonoBehaviour
{
    [SerializeField] private PlayerConfigurator _playerSpawner;
    [SerializeField] private RoundManager _roundManager;
    public void OnPlayerJoined(PlayerInput player)
    {
        if (!_playerSpawner.ConfigurePlayer(player))
            return;

        _roundManager.AddPlayer(player.gameObject);
        if (!_roundManager.IsRoundActive)
        {
            _roundManager.StartRound();
        }
    }
}

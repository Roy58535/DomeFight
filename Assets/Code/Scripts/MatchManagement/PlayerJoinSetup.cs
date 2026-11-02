using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJoinSetup : MonoBehaviour
{
    [SerializeField] private ObjectPool _bulletPool;
    [SerializeField] private RoundManager _roundManager;
    [SerializeField] private ColorPalette _colorPalette;
    public void OnPlayerJoined(PlayerInput player)
    {
        PlayerAppearance appearance = player.GetComponent<PlayerAppearance>();
        if (_colorPalette != null && appearance != null)
        {
            appearance.SetColor(_colorPalette.GetColor(player.playerIndex, Color.white));
        }
        else
        {
            Debug.LogError("Player color setup requires a palette and PlayerAppearance on the player.", this);
        }

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

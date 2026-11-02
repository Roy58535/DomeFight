using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerConfigurator : MonoBehaviour
{
    [SerializeField] private Transform players;
    [SerializeField] private ColorPalette colorPalette;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private ObjectPool _bulletPool;

    // PlayerInputManager owns instantiation and controller pairing.
    public bool ConfigurePlayer(PlayerInput player)
    {
        if (player == null)
            return false;

        int slot = player.playerIndex;
        if (_spawnPoints == null || slot < 0 || slot >= _spawnPoints.Length ||
            _spawnPoints[slot] == null || _spawnPoints[slot].position.sqrMagnitude < 0.0001f)
        {
            Debug.LogError($"Missing or invalid spawn point for player {slot + 1}.", this);
            return false;
        }

        ObjectOnSphere body = player.GetComponent<ObjectOnSphere>();
        PlayerAppearance appearance = player.GetComponent<PlayerAppearance>();
        if (body == null || appearance == null || colorPalette == null || _bulletPool == null)
        {
            Debug.LogError("Player setup requires ObjectOnSphere, PlayerAppearance, a palette, and a bullet pool.", this);
            return false;
        }

        if (players != null)
            player.transform.SetParent(players, true);

        player.name = $"Player {slot + 1}";
        body.SetSphericalPosition(SphericalCoordinatesUtils.CartesianToSpherical(_spawnPoints[slot].position));
        body.SetVelocity(Vector3.zero);
        appearance.SetColor(colorPalette.GetColor(slot, Color.white));

        foreach (Weapon weapon in player.GetComponentsInChildren<Weapon>(true))
            weapon.Initialize(_bulletPool);

        return true;
    }
}

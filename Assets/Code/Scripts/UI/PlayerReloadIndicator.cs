using UnityEngine;

[RequireComponent(typeof(WeaponController))]
public class PlayerReloadIndicator : MonoBehaviour
{
    [SerializeField] private GameObject _barRoot;
    [SerializeField] private RectTransform _fill;

    private WeaponController _weapons;

    private void Awake()
    {
        // Begin hidden until the selected weapon is reloading.
        _weapons = GetComponent<WeaponController>();
        _barRoot.SetActive(false);
    }

    private void LateUpdate()
    {
        // Read progress after weapon timers and weapon switching have updated.
        Weapon weapon = _weapons.CurrentWeapon;
        bool visible = weapon != null && weapon.isActiveAndEnabled && weapon.IsReloading;
        if (_barRoot.activeSelf != visible)
            _barRoot.SetActive(visible);

        if (visible)
            _fill.localScale = new Vector3(weapon.ReloadProgress, 1f, 1f);
    }

    private void OnDisable()
    {
        // Remove feedback when the player is disabled or despawned.
        _barRoot.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EntityInfo))]
public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject _barRoot;
    [SerializeField] private Image _fill;
    [SerializeField, Min(0f)] private float _visibleDuration = 3f;

    private EntityInfo _health;
    private float _hideAt;

    private void Awake()
    {
        // Cache the health source and begin with the bar hidden.
        _health = GetComponent<EntityInfo>();
        _barRoot.SetActive(false);
    }

    private void OnEnable()
    {
        // Listen only while this player's presentation is active.
        _health.OnHealthChanged += ShowHealth;
    }

    private void OnDisable()
    {
        _health.OnHealthChanged -= ShowHealth;
        _barRoot.SetActive(false);
    }

    private void ShowHealth(int current, int maximum)
    {
        // Update the fill and restart the display window on every health change.
        float fraction = maximum > 0 ? Mathf.Clamp01((float)current / maximum) : 0f;
        _fill.rectTransform.localScale = new Vector3(fraction, 1f, 1f);
        _hideAt = Time.time + _visibleDuration;
        _barRoot.SetActive(true);
    }

    private void LateUpdate()
    {
        if (!_barRoot.activeSelf)
            return;

        // Hide after the most recent health change has aged out.
        if (Time.time >= _hideAt)
        {
            _barRoot.SetActive(false);
            return;
        }
    }
}

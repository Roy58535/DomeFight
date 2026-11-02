using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{
    [SerializeField] private Renderer _bodyRenderer;

    private MaterialPropertyBlock _properties;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    public void SetColor(Color color)
    {
        if (_bodyRenderer == null)
        {
            Debug.LogError("PlayerAppearance requires a body renderer.", this);
            return;
        }

        if (_properties == null)
            _properties = new MaterialPropertyBlock();

        _bodyRenderer.GetPropertyBlock(_properties);
        _properties.SetColor(BaseColor, color);
        _properties.SetColor(ColorProperty, color);
        _bodyRenderer.SetPropertyBlock(_properties);
    }
}

using UnityEngine;

public class BallReturnVisual : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;

    private MaterialPropertyBlock _propertyBlock;
    private Color _baseColor = Color.white;
    private int _colorPropertyId;
    private bool _hasColorProperty;

    private void Awake()
    {
        if (_renderer == null)
        {
            _renderer = GetComponentInChildren<Renderer>();
        }

        if (_renderer == null || _renderer.sharedMaterial == null)
        {
            return;
        }

        Material material = _renderer.sharedMaterial;

        if (material.HasProperty("_BaseColor"))
        {
            _colorPropertyId = Shader.PropertyToID("_BaseColor");
        }
        else if (material.HasProperty("_Color"))
        {
            _colorPropertyId = Shader.PropertyToID("_Color");
        }
        else
        {
            return;
        }

        _hasColorProperty = true;
        _baseColor = material.GetColor(_colorPropertyId);
        _propertyBlock = new MaterialPropertyBlock();
    }

    public void SetReturning()
    {
        SetAlpha(0.35f);
    }

    public void ResetVisual()
    {
        SetAlpha(1f);
    }

    private void SetAlpha(float alpha)
    {
        if (_hasColorProperty == false || _renderer == null)
        {
            return;
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        Color color = _baseColor;
        color.a *= Mathf.Clamp01(alpha);
        _propertyBlock.SetColor(_colorPropertyId, color);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}
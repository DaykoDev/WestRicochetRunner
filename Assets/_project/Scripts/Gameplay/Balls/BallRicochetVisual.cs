using UnityEngine;

public sealed class BallRicochetVisual : MonoBehaviour
{
    [Header("Test Feature")]
    [SerializeField] private bool _ricochetVisualEnabled = true;

    [Header("Renderer")]
    [SerializeField] private Renderer _visualRenderer;
    [SerializeField] private string _colorPropertyName = "_Color";
    [SerializeField] private string _luminosityPropertyName = "_Lumin";

    [Header("Color")]
    [SerializeField] private Color _defaultColor = Color.white;

    [SerializeField]
    private Color _targetRicochetColor = new(1f, 0.55f, 0.05f, 1f);

    [SerializeField, Range(0.05f, 1f)]
    private float _colorChangePerRicochet = 0.25f;

    [Header("Luminosity")]
    [SerializeField, Min(0f)]
    private float _luminosityPerRicochet = 0.2f;

    [SerializeField, Min(0f)]
    private float _maxLuminosity = 1.2f;

    private MaterialPropertyBlock _propertyBlock;

    private int _colorPropertyId;
    private int _luminosityPropertyId;

    private Color _currentColor;
    private float _luminosity;

    private bool _hasColorProperty;
    private bool _hasLuminosityProperty;

    private void Awake()
    {
        if (_visualRenderer == null)
        {
            _visualRenderer = GetComponentInChildren<Renderer>(true);
        }

        if (_visualRenderer == null)
        {
            Debug.LogError(
                "BallRicochetVisual: renderer is not assigned.",
                this);

            enabled = false;
            return;
        }

        _propertyBlock = new MaterialPropertyBlock();

        _colorPropertyId = Shader.PropertyToID(_colorPropertyName);

        _luminosityPropertyId = Shader.PropertyToID(
            _luminosityPropertyName);

        CheckShaderProperties();

        ResetVisual();
    }

    public void ResetVisual()
    {
        _currentColor = _defaultColor;
        _luminosity = 0f;

        ApplyVisual();
    }

    public void OnRicochet()
    {
        if (_ricochetVisualEnabled == false)
        {
            return;
        }

        _currentColor = Color.Lerp(
            _currentColor,
            _targetRicochetColor,
            _colorChangePerRicochet);

        _luminosity = Mathf.Min(
            _luminosity + _luminosityPerRicochet,
            _maxLuminosity);

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (_visualRenderer == null || _propertyBlock == null)
        {
            return;
        }

        _visualRenderer.GetPropertyBlock(_propertyBlock);

        if (_hasColorProperty)
        {
            _propertyBlock.SetColor(
                _colorPropertyId,
                _currentColor);
        }

        if (_hasLuminosityProperty)
        {
            _propertyBlock.SetFloat(
                _luminosityPropertyId,
                _luminosity);
        }

        _visualRenderer.SetPropertyBlock(_propertyBlock);
    }

    private void CheckShaderProperties()
    {
        Material material = _visualRenderer.sharedMaterial;

        if (material == null)
        {
            Debug.LogError(
                "BallRicochetVisual: renderer has no material.",
                this);

            return;
        }

        _hasColorProperty = material.HasProperty(_colorPropertyId);

        _hasLuminosityProperty = material.HasProperty(
            _luminosityPropertyId);

        if (_hasColorProperty == false)
        {
            Debug.LogError(
                $"BallRicochetVisual: material '{material.name}' " +
                $"has no property '{_colorPropertyName}'.",
                this);
        }

        if (_hasLuminosityProperty == false)
        {
            Debug.LogError(
                $"BallRicochetVisual: material '{material.name}' " +
                $"has no property '{_luminosityPropertyName}'.",
                this);
        }
    }
}
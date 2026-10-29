using UnityEngine;

public sealed class BallRicochetVisual : MonoBehaviour
{
    [Header("Test Feature")]
    [SerializeField] private bool _ricochetVisualEnabled = true;

    [Header("Renderer")]
    [SerializeField] private Renderer _visualRenderer;
    [SerializeField] private string _colorPropertyName = "_BaseColor";
    [SerializeField] private string _emissionPropertyName = "_EmissionColor";

    [Header("Color")]
    [ColorUsage(false, true)]
    [SerializeField] private Color _defaultColor = Color.white;

    [ColorUsage(false, true)]
    [SerializeField]
    private Color _targetRicochetColor = new(1f, 0.55f, 0.05f, 1f);

    [SerializeField, Range(0.05f, 1f)]
    private float _colorChangePerRicochet = 0.25f;

    [Header("Emission")]
    [ColorUsage(false, true)]
    [SerializeField] private Color _emissionTint = new(1f, 0.45f, 0.02f, 1f);

    [SerializeField, Min(0f)]
    private float _emissionPerRicochet = 0.45f;

    [SerializeField, Min(0f)]
    private float _maxEmissionIntensity = 3f;

    private MaterialPropertyBlock _propertyBlock;

    private int _colorPropertyId;
    private int _emissionPropertyId;

    private Color _currentColor;
    private float _emissionIntensity;

    private void Awake()
    {
        if (_visualRenderer == null)
        {
            _visualRenderer = GetComponentInChildren<Renderer>(true);
        }

        _propertyBlock = new MaterialPropertyBlock();

        _colorPropertyId = Shader.PropertyToID(_colorPropertyName);

        _emissionPropertyId = Shader.PropertyToID(_emissionPropertyName);

        ResetVisual();
    }

    public void ResetVisual()
    {
        _currentColor = _defaultColor;
        _emissionIntensity = 0f;

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

        _emissionIntensity = Mathf.Min(
            _emissionIntensity + _emissionPerRicochet,
            _maxEmissionIntensity);

        ApplyVisual();
    }

    private void ApplyVisual()
    {
        if (_visualRenderer == null || _propertyBlock == null)
        {
            return;
        }

        Color emissionColor = _emissionTint * _emissionIntensity;

        emissionColor.a = 1f;

        _visualRenderer.GetPropertyBlock(_propertyBlock);

        _propertyBlock.SetColor(_colorPropertyId,_currentColor);

        _propertyBlock.SetColor(_emissionPropertyId, emissionColor);

        _visualRenderer.SetPropertyBlock(_propertyBlock);
    }
}
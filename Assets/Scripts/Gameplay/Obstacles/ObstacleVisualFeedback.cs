using DG.Tweening;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Obstacle))]
public sealed class ObstacleVisualFeedback : MonoBehaviour
{
    private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");

    [Header("References")]
    [SerializeField] private GameObject _visualRoot;
    [SerializeField] private Renderer _bodyRenderer;
    [SerializeField] private Transform _destructionEffectOrigin;

    [Header("Hit")]
    [SerializeField] private Color _hitColor = new Color(1f, 0.85f, 0.35f, 1f);
    [SerializeField, Min(0.02f)] private float _hitDuration = 0.1f;
    [SerializeField, Range(0f, 0.3f)] private float _hitScaleAmount = 0.07f;

    private Obstacle _obstacle;
    private EffectPool _destructionEffectPool;
    private MaterialPropertyBlock _propertyBlock;
    private Sequence _hitSequence;
    private Transform _scaleTarget;
    private Vector3 _initialScale;
    private Color _defaultColor = Color.white;
    private bool _hasColorProperty;

    private void Awake()
    {
        _obstacle = GetComponent<Obstacle>();
        _propertyBlock = new MaterialPropertyBlock();
        _scaleTarget = _visualRoot != null ? _visualRoot.transform : transform;
        _initialScale = _scaleTarget.localScale;

        _hasColorProperty =
            _bodyRenderer != null &&
            _bodyRenderer.sharedMaterial != null &&
            _bodyRenderer.sharedMaterial.HasProperty(ColorPropertyId);

        if (_hasColorProperty)
        {
            _defaultColor = _bodyRenderer.sharedMaterial.GetColor(ColorPropertyId);
        }
    }

    private void OnEnable()
    {
        if (_obstacle == null)
        {
            return;
        }

        _obstacle.HealthChanged += OnHealthChanged;
        _obstacle.Destroyed += OnDestroyed;
        _obstacle.ReturnedToPool += OnReturnedToPool;
    }

    private void OnDisable()
    {
        if (_obstacle == null)
        {
            return;
        }

        _obstacle.HealthChanged -= OnHealthChanged;
        _obstacle.Destroyed -= OnDestroyed;
        _obstacle.ReturnedToPool -= OnReturnedToPool;
    }

    public void SetDestructionEffectPool(EffectPool effectPool)
    {
        _destructionEffectPool = effectPool;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        if (currentHealth >= maxHealth)
        {
            ResetVisual();
            return;
        }

        PlayHit();
    }

    private void OnDestroyed(Obstacle obstacle)
    {
        StopHitAnimation();
        RestoreScaleAndColor();

        Vector3 effectPosition = _destructionEffectOrigin != null
            ? _destructionEffectOrigin.position
            : transform.position;

        if (_destructionEffectPool != null)
        {
            _destructionEffectPool.Get().PlayAt(effectPosition);
        }

        if (_visualRoot != null && _visualRoot != gameObject)
        {
            _visualRoot.SetActive(false);
        }
    }

    private void OnReturnedToPool(Obstacle obstacle)
    {
        ResetVisual();
    }

    private void PlayHit()
    {
        if (isActiveAndEnabled == false)
        {
            return;
        }

        StopHitAnimation();
        RestoreScaleAndColor();

        float halfDuration = _hitDuration * 0.5f;
        Vector3 hitScale = _initialScale * (1f + _hitScaleAmount);

        _hitSequence = DOTween.Sequence();

        _hitSequence.Append(
            _scaleTarget.DOScale(hitScale, halfDuration)
                .SetEase(Ease.OutQuad));

        _hitSequence.Join(
            DOVirtual.Color(
                    _defaultColor,
                    _hitColor,
                    halfDuration,
                    ApplyColor)
                .SetEase(Ease.OutQuad));

        _hitSequence.Append(
            _scaleTarget.DOScale(_initialScale, halfDuration)
                .SetEase(Ease.InQuad));

        _hitSequence.Join(
            DOVirtual.Color(
                    _hitColor,
                    _defaultColor,
                    halfDuration,
                    ApplyColor)
                .SetEase(Ease.InQuad));

        _hitSequence.OnComplete(() =>
        {
            _hitSequence = null;
            RestoreScaleAndColor();
        });
    }

    private void ResetVisual()
    {
        StopHitAnimation();

        if (_visualRoot != null && _visualRoot != gameObject)
        {
            _visualRoot.SetActive(true);
        }

        RestoreScaleAndColor();
    }

    private void StopHitAnimation()
    {
        if (_hitSequence == null)
        {
            return;
        }

        _hitSequence.Kill();
        _hitSequence = null;
    }

    private void RestoreScaleAndColor()
    {
        if (_scaleTarget != null)
        {
            _scaleTarget.localScale = _initialScale;
        }

        ApplyColor(_defaultColor);
    }

    private void ApplyColor(Color color)
    {
        if (_hasColorProperty == false)
        {
            return;
        }

        _bodyRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(ColorPropertyId, color);
        _bodyRenderer.SetPropertyBlock(_propertyBlock);
    }
}
using DG.Tweening;
using System;
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
    [SerializeField] private Color _hitColor = new(1f, 0.85f, 0.35f, 1f);
    [SerializeField, Min(0.02f)] private float _hitDuration = 0.1f;
    [SerializeField, Range(0f, 0.3f)] private float _hitScaleAmount = 0.07f;

    private Obstacle _obstacle;
    private EffectPool _destructionEffectPool;
    private MaterialPropertyBlock _propertyBlock;
    private Sequence _hitSequence;
    private Transform _scaleTarget;
    private Vector3 _initialScale;
    private Material[] _materials = Array.Empty<Material>();
    private Color[] _defaultColors = Array.Empty<Color>();

    private void Awake()
    {
        _obstacle = GetComponent<Obstacle>();
        _propertyBlock = new MaterialPropertyBlock();
        _scaleTarget = _visualRoot != null ? _visualRoot.transform : transform;
        _initialScale = _scaleTarget.localScale;

        CacheDefaultColors();
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

        _hitSequence.Append(_scaleTarget.DOScale(hitScale, halfDuration).SetEase(Ease.OutQuad));

        _hitSequence.Join(
            DOVirtual.Float(
                    0f,
                    1f,
                    halfDuration,
                    ApplyHitBlend)
                .SetEase(Ease.OutQuad));

        _hitSequence.Append(
            _scaleTarget.DOScale(_initialScale, halfDuration)
                .SetEase(Ease.InQuad));

        _hitSequence.Join(
            DOVirtual.Float(
                    1f,
                    0f,
                    halfDuration,
                    ApplyHitBlend)
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

        RestoreColors();
    }

    private void CacheDefaultColors()
    {
        if (_bodyRenderer == null)
        {
            _materials = Array.Empty<Material>();
            _defaultColors = Array.Empty<Color>();
            return;
        }

        _materials = _bodyRenderer.sharedMaterials;
        _defaultColors = new Color[_materials.Length];

        for (int i = 0; i < _materials.Length; i++)
        {
            Material material = _materials[i];

            _defaultColors[i] =
                material != null && material.HasProperty(ColorPropertyId)
                    ? material.GetColor(ColorPropertyId)
                    : Color.white;
        }
    }

    private void RestoreColors()
    {
        for (int i = 0; i < _defaultColors.Length; i++)
        {
            SetColor(i, _defaultColors[i]);
        }
    }

    private void SetColor(int materialIndex, Color color)
    {
        Material material = _materials[materialIndex];

        if (material == null ||
            material.HasProperty(ColorPropertyId) == false)
        {
            return;
        }

        _bodyRenderer.GetPropertyBlock(
            _propertyBlock,
            materialIndex);

        _propertyBlock.SetColor(ColorPropertyId, color);

        _bodyRenderer.SetPropertyBlock(
            _propertyBlock,
            materialIndex);
    }

    private void ApplyHitBlend(float blend)
    {
        for (int i = 0; i < _materials.Length; i++)
        {
            Color color = Color.Lerp(
                _defaultColors[i],
                _hitColor,
                blend);

            SetColor(i, color);
        }
    }
}
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CurrencyPickup : MonoBehaviour
{
    private enum MovementState
    {
        Inactive,
        Dropping,
        Waiting,
        Attracting
    }

    [Header("Drop")]
    [SerializeField, Min(0f)] private float _scatterRadius = 0.65f;
    [SerializeField, Min(0f)] private float _popHeight = 0.45f;
    [SerializeField, Min(0.01f)] private float _dropDuration = 0.3f;
    [SerializeField] private float _rotationSpeed = 180f;

    [Header("Attraction")]
    [SerializeField, Min(0.1f)] private float _magnetRadius = 3.5f;
    [SerializeField, Min(0f)] private float _targetHeight = 0.8f;
    [SerializeField, Min(0.1f)] private float _startSpeed = 4f;
    [SerializeField, Min(0.1f)] private float _maxSpeed = 18f;
    [SerializeField, Min(0f)] private float _acceleration = 35f;
    [SerializeField, Min(0.05f)] private float _collectDistance = 0.25f;
    [SerializeField, Range(0.05f, 1f)] private float _minimumScale = 0.15f;

    [Header("Cleanup")]
    [SerializeField, Min(0f)] private float _behindPlayerDistance = 5f;

    private MovementState _state;
    private CurrencyDropSpawner _owner;
    private Transform _target;
    private Tween _dropTween;
    private Vector3 _initialScale;
    private Vector3 _dropStart;
    private Vector3 _dropEnd;
    private float _dropElapsed;
    private float _currentSpeed;

    public int Value { get; private set; }

    private void Awake()
    {
        _initialScale = transform.localScale;
    }

    public void Launch(
        CurrencyDropSpawner owner,
        Transform target,
        int value,
        Vector3 position)
    {
        _owner = owner;
        _target = target;
        Value = Mathf.Max(1, value);

        transform.SetParent(owner.DropRoot, true);
        transform.position = position;
        transform.localScale = _initialScale;

        Vector3 dropStart = transform.localPosition;
        Vector2 scatter = Random.insideUnitCircle * _scatterRadius;
        Vector3 dropEnd = dropStart + new Vector3(scatter.x, 0f, scatter.y);

        _currentSpeed = _startSpeed;
        _state = MovementState.Dropping;

        _dropTween = transform.DOLocalJump(
                dropEnd,
                _popHeight,
                1,
                _dropDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(OnDropCompleted);
    }

    private void Update()
    {
        if (_state == MovementState.Inactive || _owner == null || _target == null)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        if (_rotationSpeed != 0f)
        {
            transform.Rotate(
                Vector3.up,
                _rotationSpeed * deltaTime,
                Space.Self);
        }

        if (_state != MovementState.Attracting &&
            Vector3.Distance(transform.position, GetTargetPosition()) <= _magnetRadius)
        {
            BeginAttraction();
        }

        if (_state == MovementState.Attracting)
        {
            UpdateAttraction(deltaTime);
            return;
        }

        if (transform.position.z < _target.position.z - _behindPlayerDistance)
        {
            Expire();
        }
    }

    private void BeginAttraction()
    {
        _state = MovementState.Attracting;
        StopDropAnimation();
    }

    private void UpdateAttraction(float deltaTime)
    {
        Vector3 targetPosition = GetTargetPosition();
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance <= _collectDistance)
        {
            Collect();
            return;
        }

        _currentSpeed = Mathf.Min(
            _maxSpeed,
            _currentSpeed + _acceleration * deltaTime);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            _currentSpeed * deltaTime);

        float distanceRatio = Mathf.Clamp01(distance / _magnetRadius);
        float scaleMultiplier = Mathf.Lerp(
            _minimumScale,
            1f,
            distanceRatio);

        transform.localScale = _initialScale * scaleMultiplier;
    }

    private Vector3 GetTargetPosition()
    {
        return _target.position + Vector3.up * _targetHeight;
    }

    private void OnDropCompleted()
    {
        _dropTween = null;

        if (_state == MovementState.Dropping)
        {
            _state = MovementState.Waiting;
        }
    }

    private void StopDropAnimation()
    {
        if (_dropTween == null)
        {
            return;
        }

        _dropTween.Kill();
        _dropTween = null;
    }

    private void Collect()
    {
        if (_state == MovementState.Inactive)
        {
            return;
        }

        _state = MovementState.Inactive;
        StopDropAnimation();
        _owner.ReleaseCollected(this);
    }

    private void Expire()
    {
        if (_state == MovementState.Inactive)
        {
            return;
        }

        _state = MovementState.Inactive;
        StopDropAnimation();
        _owner.ReleaseExpired(this);
    }
}

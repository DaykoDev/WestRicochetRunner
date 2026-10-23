using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private PlayerLateralCalculator _lateralCalculator;
    [SerializeField] private PlayerMover _mover;
    [SerializeField] private PlayerShooting _playerShooting;

    private bool _isControlEnabled;

    public float ForwardSpeed => _mover.ForwardSpeed;

    private void Awake()
    {
        float initialX = transform.position.x;

        _lateralCalculator.Initialize(initialX);
        _mover.Initialize(initialX);
    }

    private void OnEnable()
    {
        _inputReader.Moved += OnPointerMoved;
        _inputReader.HoldPointerChanged += OnHoldPointerChanged;
    }

    private void OnDisable()
    {
        _inputReader.Moved -= OnPointerMoved;
        _inputReader.HoldPointerChanged -= OnHoldPointerChanged;

        SetControlEnabled(false);
    }

    public void SetControlEnabled(bool isEnabled)
    {
        _isControlEnabled = isEnabled;

        _mover.SetMovementEnabled(isEnabled);
        _inputReader.SetInputEnabled(isEnabled);
        _playerShooting.SetControlEnabled(isEnabled);

        if (isEnabled == false)
        {
            _lateralCalculator.SetPointerHeld(false);
        }
    }

    public void ResetToStart()
    {
        SetControlEnabled(false);

        _mover.ResetToStart();
        _lateralCalculator.ResetToStart(transform.position.x);
    }

    public void ClearRun()
    {
        SetControlEnabled(false);
        _playerShooting.ResetRun();
    }

    public void SetRunModifiers(RunModifiers modifiers)
    {
        _playerShooting.SetRunModifiers(modifiers);
    }

    private void OnPointerMoved(Vector2 delta)
    {
        if (_isControlEnabled == false)
        {
            return;
        }

        float targetX = _lateralCalculator.CalculateTargetX(delta.x);
        _mover.SetTargetX(targetX);
    }

    private void OnHoldPointerChanged(bool isHeld)
    {
        if (_isControlEnabled == false)
        {
            return;
        }

        _lateralCalculator.SetPointerHeld(isHeld);
    }
}
using UnityEngine;

[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerLateralCalculator))]
[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerShooting))]
public class Player : MonoBehaviour
{
    private PlayerInputReader _inputReader;
    private PlayerLateralCalculator _lateralCalculator;
    private PlayerMover _mover;
    private PlayerShooting _playerShooting;

    private bool _isControlEnabled;

    public float ForwardSpeed => _mover.ForwardSpeed;

    private void Awake()
    {
        _inputReader = GetComponent<PlayerInputReader>();
        _lateralCalculator = GetComponent<PlayerLateralCalculator>();
        _mover = GetComponent<PlayerMover>();
        _playerShooting = GetComponent<PlayerShooting>();

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
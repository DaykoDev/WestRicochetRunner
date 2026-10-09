
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInputReader _inputReader;
    [SerializeField] private PlayerLateralController _lateralController;
    [SerializeField] private PlayerMover _mover;

    private void Awake()
    {
        float initialX = transform.position.x;

        _lateralController.Initialize(initialX);
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
        _mover.SetMovementEnabled(isEnabled);
        _inputReader.SetInputEnabled(isEnabled);

        if (isEnabled == false)
        {
            _lateralController.SetPointerHeld(false);
        }
    }

    private void OnPointerMoved(Vector2 delta)
    {
        float targetX = _lateralController.CalculateTargetX(delta.x);
        _mover.SetTargetX(targetX);
    }

    private void OnHoldPointerChanged(bool isHeld)
    {
        _lateralController.SetPointerHeld(isHeld);
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    public event Action<Vector2> Moved;
    public event Action<bool> HoldPointerChanged;

    private PlayerInputActions _controls;
    private bool _isHoldPointerPressed;

    private void Awake()
    {
        _controls = new PlayerInputActions();
        _controls.Player.SetCallbacks(this);
    }

    private void OnDisable()
    {
        SetInputEnabled(false);
    }

    private void OnDestroy()
    {
        _controls.Dispose();
    }

    public void SetInputEnabled(bool isEnabled)
    {
        if (isEnabled)
        {
            _controls.Player.Enable();
            return;
        }

        _controls.Player.Disable();
        SetHoldPointerState(false);
    }

    public void OnMovePointer(InputAction.CallbackContext context)
    {
        if (context.performed == false)
        {
            return;
        }

        Vector2 delta = context.ReadValue<Vector2>();

        Moved?.Invoke(delta);
    }

    public void OnHoldPointer(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SetHoldPointerState(true);
        }
        else if (context.canceled)
        {
            SetHoldPointerState(false);
        }
    }

    private void SetHoldPointerState(bool isPressed)
    {
        if (_isHoldPointerPressed == isPressed)
        {
            return;
        }

        _isHoldPointerPressed = isPressed;
        HoldPointerChanged?.Invoke(isPressed);
    }
}
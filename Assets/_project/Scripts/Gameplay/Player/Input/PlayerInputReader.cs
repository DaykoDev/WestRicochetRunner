using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WestRicochet.Gameplay.Player.Input
{
    public class PlayerInputReader : MonoBehaviour, PlayerInputActions.IPlayerActions
    {
        private PlayerInputActions _controls;
        private bool _isHoldPointerPressed;

        public event Action<Vector2> Moved;
        public event Action<bool> HoldPointerChanged;

        public float KeyboardDirection { get; private set; }

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
            KeyboardDirection = 0f;
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

        public void OnMoveKeyboard(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                KeyboardDirection = context.ReadValue<float>();
            }
            else if (context.canceled)
            {
                KeyboardDirection = 0f;
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
}
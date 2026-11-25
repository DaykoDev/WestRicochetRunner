using UnityEngine;

namespace WestRicochet.Gameplay.Player.Movement
{
    public class PlayerLateralCalculator : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _sensitivity = 6f;
        [SerializeField] private float _lateralLimit = 3f;
        [SerializeField, Min(0f)] private float _keyboardSpeed = 4f;

        private float _targetX;
        private bool _isPointerHeld;

        public float LateralLimit => _lateralLimit;

        public void Initialize(float initialX)
        {
            ResetToStart(initialX);
        }

        public void ResetToStart(float initialX)
        {
            _targetX = Mathf.Clamp(initialX, -_lateralLimit, _lateralLimit);
            _isPointerHeld = false;
        }

        public void SetPointerHeld(bool isHeld)
        {
            _isPointerHeld = isHeld;
        }

        public float CalculateTargetX(float horizontalDeltaPixels)
        {
            if (_isPointerHeld == false)
            {
                return _targetX;
            }

            float normalizedDelta = horizontalDeltaPixels / Screen.width;

            _targetX += normalizedDelta * _sensitivity;
            _targetX = Mathf.Clamp(_targetX, -_lateralLimit, _lateralLimit);

            return _targetX;
        }

        public float CalculateKeyboardTargetX(float direction, float deltaTime)
        {
            float clampedDirection = Mathf.Clamp(direction, -1f, 1f);

            _targetX += clampedDirection * _keyboardSpeed * deltaTime;
            _targetX = Mathf.Clamp(_targetX, -_lateralLimit, _lateralLimit);

            return _targetX;
        }

        public void SetLateralLimit(float limit)
        {
            _lateralLimit = Mathf.Max(0f, limit);
        }
    }
}
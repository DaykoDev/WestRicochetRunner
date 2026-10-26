
using UnityEngine;

public class PlayerLateralCalculator : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _sensitivity = 6f;
    [SerializeField] private float _minX = -3f;
    [SerializeField] private float _maxX = 3f;

    private float _targetX;
    private bool _isPointerHeld;

    public void Initialize(float initialX)
    {
        _targetX = Mathf.Clamp(initialX, _minX, _maxX);
    }

    public void ResetToStart(float initialX)
    {
        _targetX = Mathf.Clamp(initialX, _minX, _maxX);
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
        _targetX = Mathf.Clamp(_targetX, _minX, _maxX);

        return _targetX;
    }
}
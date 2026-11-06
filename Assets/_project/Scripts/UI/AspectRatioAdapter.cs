using Cinemachine;
using UnityEngine;

public sealed class AspectRatioAdapter : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineVirtualCamera _defaultCamera;
    [SerializeField] private CinemachineVirtualCamera _mobileCamera;

    [Header("Gameplay")]
    [SerializeField] private Transform _forwardBoundary;

    [SerializeField] private PlayerLateralCalculator _playerLateralCalculator;

    [Header("Portrait Settings")]
    [SerializeField] private float _portraitForwardBoundaryZ;

    [SerializeField, Min(0f)] private float _portraitLateralLimit;

    private float _defaultForwardBoundaryZ;
    private float _defaultLateralLimit;

    private bool? _isPortrait;

    private void Awake()
    {
        _defaultForwardBoundaryZ = _forwardBoundary.localPosition.z;
        _defaultLateralLimit = _playerLateralCalculator.LateralLimit;
    }

    private void Start()
    {
        bool isPortrait = Screen.height > Screen.width;
        ApplyOrientation(isPortrait);
    }

    private void LateUpdate()
    {
        bool isPortrait = Screen.height > Screen.width;

        if (_isPortrait == isPortrait)
        {
            return;
        }

        ApplyOrientation(isPortrait);
    }

    private void ApplyOrientation(bool isPortrait)
    {
        _isPortrait = isPortrait;

        ApplyCameraSettings(isPortrait);
        ApplyGameplaySettings(isPortrait);
    }

    private void ApplyCameraSettings(bool isPortrait)
    {
        _defaultCamera.Priority = isPortrait ? 10 : 20;
        _mobileCamera.Priority = isPortrait ? 20 : 10;
    }

    private void ApplyGameplaySettings(bool isPortrait)
    {
        ApplyForwardBoundary(isPortrait);
        ApplyLateralLimit(isPortrait);
    }

    private void ApplyForwardBoundary(bool isPortrait)
    {
        Vector3 position = _forwardBoundary.localPosition;

        position.z = isPortrait
            ? _portraitForwardBoundaryZ
            : _defaultForwardBoundaryZ;

        _forwardBoundary.localPosition = position;
    }

    private void ApplyLateralLimit(bool isPortrait)
    {
        float limit = isPortrait
            ? _portraitLateralLimit
            : _defaultLateralLimit;

        _playerLateralCalculator.SetLateralLimit(limit);
    }
}
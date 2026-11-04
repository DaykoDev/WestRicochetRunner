using Cinemachine;
using UnityEngine;

public sealed class CameraAspectSelector : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _defaultCamera;
    [SerializeField] private CinemachineVirtualCamera _mobileCamera;

    [SerializeField] private float _wideScreenAspect = 1.9f;

    private int _lastWidth;
    private int _lastHeight;

    private void OnEnable()
    {
        ApplyCamera();
    }

    private void LateUpdate()
    {
        if (Screen.width == _lastWidth && Screen.height == _lastHeight)
        {
            return;
        }

        ApplyCamera();
    }

    private void ApplyCamera()
    {
        if (_defaultCamera == null ||
            _mobileCamera == null ||
            Screen.width <= 0 ||
            Screen.height <= 0)
        {
            return;
        }

        float aspect = (float)Screen.width / Screen.height;
        bool useWideCamera = Screen.height > Screen.width;

        _defaultCamera.Priority = useWideCamera ? 10 : 20;
        _mobileCamera.Priority = useWideCamera ? 20 : 10;

        _lastWidth = Screen.width;
        _lastHeight = Screen.height;
    }
}
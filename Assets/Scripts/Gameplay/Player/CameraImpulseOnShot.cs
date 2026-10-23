using Cinemachine;
using UnityEngine;

public class CameraImpulseOnShot : MonoBehaviour
{
    [SerializeField] private BallLauncher _ballLauncher;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    private void Awake()
    {
        if (_ballLauncher == null)
        {
            Debug.LogError("BallLauncher is not assigned.", this);
        }

        if (_impulseSource == null)
        {
            Debug.LogError("Impulse source is not assigned.", this);
        }
    }

    private void OnEnable()
    {
        _ballLauncher.Fired += OnFired;
    }

    private void OnDisable()
    {
        _ballLauncher.Fired -= OnFired;
    }

    private void OnFired()
    {
        _impulseSource.GenerateImpulse();
    }
}
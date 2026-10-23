using DG.Tweening;
using UnityEngine;

public class RevolverRecoilVisual : MonoBehaviour
{
    [SerializeField] private BallLauncher _ballLauncher;
    [SerializeField] private Transform _recoilRoot;

    [Header("Recoil")]
    [SerializeField, Min(0f)] private float _recoilDistance = 0.08f;
    [SerializeField, Min(0f)] private float _recoilAngle = 6f;
    [SerializeField, Min(0.01f)] private float _recoilDuration = 0.12f;

    private void Awake()
    {
        if (_ballLauncher == null)
        {
            Debug.LogError("BallLauncher is not assigned.", this);
        }

        if (_recoilRoot == null)
        {
            Debug.LogError("Recoil root is not assigned.", this);
        }
    }

    private void OnEnable()
    {
        _ballLauncher.Fired += OnFired;
    }

    private void OnDisable()
    {
        _ballLauncher.Fired -= OnFired;
        _recoilRoot.DOKill();
    }

    private void OnFired()
    {
        if (_recoilRoot == null)
        {
            return;
        }

        _recoilRoot.DOKill(true);
        _recoilRoot.DOPunchPosition(Vector3.back * _recoilDistance, _recoilDuration, 1, 0f);
        _recoilRoot.DOPunchRotation(Vector3.left * _recoilAngle, _recoilDuration, 1, 0f);
    }
}
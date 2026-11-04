using UnityEngine;

public sealed class BonusLevelPresentation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private BonusLevelSettings _settings;

    [Header("Shooting")]
    [SerializeField] private BallLauncher _ballLauncher;
    [SerializeField] private AutoShooter _autoShooter;
    [SerializeField] private PlayerAimer _playerAimer;

    [Header("Revolvers")]
    [SerializeField] private GameObject _secondRevolverRoot;
    [SerializeField] private Transform _primaryAimPivot;
    [SerializeField] private Transform _secondaryAimPivot;

    [SerializeField, Min(0f)]
    private float _aimPivotOffsetX = 0.2f;

    private Vector3 _primaryDefaultPosition;
    private Vector3 _secondaryDefaultPosition;

    private void Awake()
    {
        if (_primaryAimPivot != null)
        {
            _primaryDefaultPosition = _primaryAimPivot.localPosition;
        }

        if (_secondaryAimPivot != null)
        {
            _secondaryDefaultPosition = _secondaryAimPivot.localPosition;
        }

        SetLevel(1);
    }

    public void SetLevel(int level)
    {
        bool isBonus = _settings != null && _settings.IsBonusLevel(level);

        bool useSecondRevolver = isBonus && _settings.SecondRevolverEnabled;

        bool useRainbowBalls = isBonus && _settings.RainbowBallsEnabled;

        if (_secondRevolverRoot != null)
        {
            _secondRevolverRoot.SetActive(useSecondRevolver);
        }

        UpdateRevolverPositions(useSecondRevolver);

        if (_playerAimer != null)
        {
            _playerAimer.SetTrajectoryCentered(useSecondRevolver);
        }

        if (_ballLauncher != null)
        {
            _ballLauncher.SetDualWieldEnabled(useSecondRevolver);

            _ballLauncher.SetRainbowVisualEnabled(useRainbowBalls);
        }

        if (_autoShooter != null)
        {
            float fireRateMultiplier = useSecondRevolver ? _settings.FireRateMultiplier : 1f;

            _autoShooter.SetBonusFireRateMultiplier(fireRateMultiplier);
        }
    }

    private void UpdateRevolverPositions(bool useSecondRevolver)
    {
        float primaryOffset = useSecondRevolver ? _aimPivotOffsetX : 0f;

        float secondaryOffset = useSecondRevolver ? -_aimPivotOffsetX : 0f;

        SetLocalX(_primaryAimPivot, _primaryDefaultPosition.x + primaryOffset);

        SetLocalX(_secondaryAimPivot, _secondaryDefaultPosition.x + secondaryOffset);
    }

    private static void SetLocalX(Transform target, float x)
    {
        if (target == null)
        {
            return;
        }

        Vector3 position = target.localPosition;
        position.x = x;
        target.localPosition = position;
    }
}
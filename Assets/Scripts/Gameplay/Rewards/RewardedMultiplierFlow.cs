using System;
using UnityEngine;

public class RewardedMultiplierFlow : IDisposable
{
    private readonly AdsService _adsService;
    private readonly Wallet _wallet;
    private readonly RunRewardCollector _rewardCollector;
    private readonly float _multiplier;

    private bool _isUsed;
    private bool _requestInFlight;

    private int _activeRequestId;
    private int _requestedCoins;

    public RewardedMultiplierFlow(AdsService adsService, Wallet wallet, RunRewardCollector rewardCollector, float multiplier)
    {
        _adsService = adsService;
        _wallet = wallet;
        _rewardCollector = rewardCollector;
        _multiplier = multiplier;
    }

    public bool CanApply => _isUsed == false && _requestInFlight == false;

    public event Action<int> BonusGranted;
    public event Action AvailabilityChanged;

    public void Reset()
    {
        Cancel();

        _isUsed = false;
        _requestedCoins = 0;

        AvailabilityChanged?.Invoke();
    }

    public void Request()
    {
        if (CanApply == false)
        {
            return;
        }

        _requestInFlight = true;
        _requestedCoins = _rewardCollector.CollectedThisRun;

        bool isStarted = _adsService.ShowRewarded( AdsPlacements.VictoryMultiplier, OnRewarded, OnAdError, out _activeRequestId);

        if (isStarted == false)
        {
            CompleteRequest();
        }

        AvailabilityChanged?.Invoke();
    }

    public void Cancel()
    {
        if (_requestInFlight == false)
        {
            return;
        }

        _adsService.CancelRewarded(_activeRequestId);
        CompleteRequest();

        AvailabilityChanged?.Invoke();
    }

    public void Dispose()
    {
        Cancel();
    }

    private void OnRewarded(int requestId)
    {
        if (IsActiveRequest(requestId) == false)
        {
            return;
        }

        CompleteRequest();
        _isUsed = true;

        int bonus = Mathf.RoundToInt(_requestedCoins * (_multiplier - 1f));

        if (bonus > 0)
        {
            _wallet.Add(bonus);
        }

        AvailabilityChanged?.Invoke();
        BonusGranted?.Invoke(bonus);
    }

    private void OnAdError(int requestId)
    {
        if (IsActiveRequest(requestId) == false)
        {
            return;
        }

        CompleteRequest();
        AvailabilityChanged?.Invoke();
    }

    private bool IsActiveRequest(int requestId)
    {
        return _requestInFlight && _activeRequestId == requestId;
    }

    private void CompleteRequest()
    {
        _requestInFlight = false;
        _activeRequestId = 0;
    }
}
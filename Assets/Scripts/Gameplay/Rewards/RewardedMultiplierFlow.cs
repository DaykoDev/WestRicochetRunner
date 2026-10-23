using System;
using UnityEngine;

public class RewardedMultiplierFlow
{
    private readonly AdsService _adsService;
    private readonly Wallet _wallet;
    private readonly RunRewardCollector _rewardCollector;
    private readonly float _multiplier;

    private bool _isUsed;
    private bool _requestInFlight;

    public event Action<int> BonusGranted;

    public RewardedMultiplierFlow(AdsService adsService, Wallet wallet, RunRewardCollector rewardCollector, float multiplier)
    {
        _adsService = adsService;
        _wallet = wallet;
        _rewardCollector = rewardCollector;
        _multiplier = multiplier;
    }

    public bool CanApply =>
        _isUsed == false && _requestInFlight == false;

    public void Reset()
    {
        _isUsed = false;
        _requestInFlight = false;
    }

    public void Request()
    {
        if (CanApply == false)
        {
            return;
        }

        _requestInFlight = true;
        _adsService.ShowRewarded(AdsPlacements.VictoryMultiplier, OnRewarded, OnAdError);
    }

    private void OnRewarded()
    {
        _requestInFlight = false;
        _isUsed = true;

        int collected = _rewardCollector.CollectedThisRun;
        int bonus = Mathf.RoundToInt(collected * (_multiplier - 1f));

        if (bonus > 0)
        {
            _wallet.Add(bonus);
        }

        BonusGranted?.Invoke(bonus);
    }

    private void OnAdError()
    {
        _requestInFlight = false;
    }
}
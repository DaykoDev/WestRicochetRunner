using System;
using UnityEngine;

public class AdsService
{
    private readonly IAdsService _provider;
    private readonly AdsConfig _config;

    private float _lastInterstitialTime = float.NegativeInfinity;

    public AdsService(IAdsService provider, AdsConfig config)
    {
        _provider = provider;
        _config = config;
    }

    public bool IsBusy => _provider.IsBusy;

    public bool CanShowInterstitial =>
        _provider.IsBusy == false &&
        Time.unscaledTime - _lastInterstitialTime >= _config.InterstitialCooldownSeconds;

    private Action _pendingRewarded;

    public void TryShowInterstitial()
    {
        if (CanShowInterstitial == false)
        {
            return;
        }

        _lastInterstitialTime = Time.unscaledTime;
        _provider.ShowInterstitial(null, null);
    }

    public void ShowRewarded(string placement, Action onRewarded, Action onError)
    {
        if (_provider.IsBusy)
        {
            onError?.Invoke();
            return;
        }

        _pendingRewarded = onRewarded;
        _provider.ShowRewarded(placement, OnProviderRewarded, onError);
    }

    private void OnProviderRewarded()
    {
        _lastInterstitialTime = Time.unscaledTime;

        Action callback = _pendingRewarded;
        _pendingRewarded = null;
        callback?.Invoke();
    }
}
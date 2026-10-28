using System;
using UnityEngine;

public class AdsService : IDisposable
{
    private readonly IAdsService _provider;
    private readonly AdsConfig _config;

    private float _lastInterstitialTime = float.NegativeInfinity;
    private int _lastRewardedRequestId;
    private int _activeRewardedRequestId;

    public AdsService(IAdsService provider, AdsConfig config)
    {
        _provider = provider;
        _config = config;
    }

    public bool IsBusy => _provider.IsBusy;

    public bool CanShowInterstitial => _provider.IsBusy == false && Time.unscaledTime - _lastInterstitialTime >= _config.InterstitialCooldownSeconds;

    private Action<int> _pendingRewarded;
    private Action<int> _pendingError;

    public void TryShowInterstitial()
    {
        if (CanShowInterstitial == false)
        {
            return;
        }

        _lastInterstitialTime = Time.unscaledTime;
        _provider.ShowInterstitial(null, null);
    }

    public bool ShowRewarded(string placement, Action<int> onRewarded, Action<int> onError, out int requestId)
    {
        requestId = 0;

        if(_activeRewardedRequestId != 0 || _provider.IsBusy)
        {
            return false;
        }

        requestId = ++_lastRewardedRequestId;

        _activeRewardedRequestId = requestId;
        _pendingRewarded = onRewarded;
        _pendingError = onError;

        _provider.ShowRewarded(placement, OnProviderRewarded, OnProviderError);

        return true;
    }

    public void CancelRewarded(int requestId)
    {
        if (requestId == 0 || requestId != _activeRewardedRequestId)
        {
            return;
        }

        ClearRewardedRequest();
    }

    public void Dispose()
    {
        ClearRewardedRequest();
    }

    private void OnProviderRewarded()
    {
        if (_activeRewardedRequestId == 0)
        {
            return;
        }

        int requestId = _activeRewardedRequestId;
        Action<int> callback = _pendingRewarded;

        ClearRewardedRequest();

        _lastInterstitialTime = Time.unscaledTime;
        callback?.Invoke(requestId);
    }

    private void OnProviderError()
    {
        if (_activeRewardedRequestId == 0)
        {
            return;
        }

        int requestId = _activeRewardedRequestId;
        Action<int> callback = _pendingError;

        ClearRewardedRequest();
        callback?.Invoke(requestId);
    }

    private void ClearRewardedRequest()
    {
        _activeRewardedRequestId = 0;
        _pendingRewarded = null;
        _pendingError = null;
    }
}
using System;
using UnityEngine;
using YG;

public class YandexAdsService : IAdsService, IDisposable
{
    private Action _onInterstitialClosed;
    private Action _onInterstitialError;
    private Action _onRewarded;
    private Action _onRewardedError;

    public bool IsBusy => YG2.nowAdsShow;
    private bool _rewardGranted;

    public YandexAdsService()
    {
        YG2.onCloseInterAdv += OnCloseInterAdv;
        YG2.onCloseRewardedAdv += OnRewardedClosed;
        YG2.onErrorInterAdv += OnErrorInterAdv;
        YG2.onErrorRewardedAdv += OnErrorRewardedAdv;
    }

    public void Dispose()
    {
        YG2.onCloseInterAdv -= OnCloseInterAdv;
        YG2.onCloseRewardedAdv -= OnRewardedClosed;
        YG2.onErrorInterAdv -= OnErrorInterAdv;
        YG2.onErrorRewardedAdv -= OnErrorRewardedAdv;
    }

    public void ShowInterstitial(Action onClosed, Action onError)
    {
        if (IsBusy)
        {
            onError?.Invoke();
            return;
        }

        _onInterstitialClosed = onClosed;
        _onInterstitialError = onError;
        YG2.InterstitialAdvShow();
    }

    public void ShowRewarded(string placement, Action onRewarded, Action onError)
    {
        if (IsBusy)
        {
            onError?.Invoke();
            return;
        }

        _rewardGranted = false;
        _onRewarded = onRewarded;
        _onRewardedError = onError;
        YG2.RewardedAdvShow(placement, OnRewardGranted);
    }

    private void OnRewardGranted()
    {
        _rewardGranted = true;

        Action callback = _onRewarded;
        ClearRewarded();
        callback?.Invoke();
    }

    private void OnRewardedClosed()
    {
        if (_rewardGranted)
        {
            return;
        }

        Action callback = _onRewardedError;
        ClearRewarded();
        callback?.Invoke();
    }

    private void OnCloseInterAdv()
    {
        Action callback = _onInterstitialClosed;
        ClearInterstitial();
        callback?.Invoke();
    }

    private void OnErrorInterAdv()
    {
        Action callback = _onInterstitialError;
        ClearInterstitial();
        callback?.Invoke();
    }

    private void OnErrorRewardedAdv()
    {
        Action callback = _onRewardedError;
        ClearRewarded();
        callback?.Invoke();
    }

    private void ClearInterstitial()
    {
        _onInterstitialClosed = null;
        _onInterstitialError = null;
    }

    private void ClearRewarded()
    {
        _onRewarded = null;
        _onRewardedError = null;
    }
}
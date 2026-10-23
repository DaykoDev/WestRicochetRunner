using System;
using UnityEngine;

public class EditorAdsService : IAdsService
{
    public bool IsBusy => false;

    public void ShowInterstitial(Action onClosed, Action onError)
    {
        Debug.Log("[EditorAdsService] Interstitial shown and closed (mock)");
        onClosed?.Invoke();
    }

    public void ShowRewarded(string placement, Action onRewarded, Action onError)
    {
        Debug.Log($"[EditorAdsService] Rewarded granted (mock): {placement}");
        onRewarded?.Invoke();
    }
}
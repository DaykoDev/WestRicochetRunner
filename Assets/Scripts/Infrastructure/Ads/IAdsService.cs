using System;

public interface IAdsService
{
    bool IsBusy { get; }

    void ShowInterstitial(Action onClosed, Action onError);
    void ShowRewarded(string placement, Action onRewarded, Action onError);
}
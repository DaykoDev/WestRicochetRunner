using System;

namespace WestRicochet.Infrastructure.Ads
{
    public interface IAdsService
    {
        bool IsBusy { get; }

        void ShowInterstitial(Action onClosed, Action onError);
        void ShowRewarded(string placement, Action onRewarded, Action onError);
    }
}
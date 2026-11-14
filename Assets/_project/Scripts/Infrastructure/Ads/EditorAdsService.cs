using System;

namespace WestRicochet.Infrastructure.Ads
{
    public class EditorAdsService : IAdsService
    {
        public bool IsBusy => false;

        public void ShowInterstitial(Action onClosed, Action onError)
        {
            onClosed?.Invoke();
        }

        public void ShowRewarded(string placement, Action onRewarded, Action onError)
        {
            onRewarded?.Invoke();
        }
    }
}
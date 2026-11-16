using System;

namespace WestRicochet.Infrastructure.Ads
{
    public interface IRewardedAdsService
    {
        bool ShowRewarded(
            string placement,
            Action<int> onRewarded,
            Action<int> onError,
            out int requestId);

        void CancelRewarded(int requestId);
    }
}
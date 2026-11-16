using System;
using UnityEngine;
using WestRicochet.Infrastructure.Ads;

namespace WestRicochet.Gameplay.Rewards.Flow
{
    public class RewardedMultiplierFlow : RewardedFlowBase
    {
        private readonly Wallet _wallet;
        private readonly RunRewardCollector _rewardCollector;
        private readonly float _multiplier;

        private bool _isUsed;
        private int _requestedCoins;

        public RewardedMultiplierFlow(
            IRewardedAdsService adsService,
            Wallet wallet,
            RunRewardCollector rewardCollector,
            float multiplier)
            : base(adsService)
        {
            _wallet = wallet;
            _rewardCollector = rewardCollector;
            _multiplier = multiplier;
        }

        public event Action<int> BonusGranted;

        public bool CanApply =>
            _isUsed == false && RequestInFlight == false;

        public int CurrentBonus =>
            Mathf.Max(
                0,
                Mathf.RoundToInt(
                    _rewardCollector.CollectedThisRun *
                    (_multiplier - 1f)));

        public void Reset()
        {
            Cancel();

            _isUsed = false;
            _requestedCoins = 0;

            NotifyAvailabilityChanged();
        }

        public void Request()
        {
            if (CanApply == false)
            {
                return;
            }

            _requestedCoins =
                _rewardCollector.CollectedThisRun;

            StartRequest(AdsPlacements.VictoryMultiplier);
        }

        protected override void HandleRewarded()
        {
            _isUsed = true;

            int bonus = Mathf.RoundToInt(
                _requestedCoins * (_multiplier - 1f));

            if (bonus > 0)
            {
                _wallet.Add(bonus);
            }

            NotifyAvailabilityChanged();
            BonusGranted?.Invoke(bonus);
        }
    }
}
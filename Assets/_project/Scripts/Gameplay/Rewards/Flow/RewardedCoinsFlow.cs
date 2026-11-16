using System;
using WestRicochet.Infrastructure.Ads;
using WestRicochet.Meta;

namespace WestRicochet.Gameplay.Rewards.Flow
{
    public class RewardedCoinsFlow : RewardedFlowBase
    {
        private readonly Wallet _wallet;
        private readonly PlayerProgress _playerProgress;
        private readonly LevelEconomySettings _levelEconomySettings;

        private int _requestedReward;

        public RewardedCoinsFlow(
            IRewardedAdsService adsService,
            Wallet wallet,
            PlayerProgress playerProgress,
            LevelEconomySettings levelEconomySettings)
            : base(adsService)
        {
            _wallet = wallet;
            _playerProgress = playerProgress;
            _levelEconomySettings = levelEconomySettings;
        }

        public event Action RewardGranted;

        public bool CanRequest =>
            RequestInFlight == false &&
            _playerProgress.CanClaimMenuCoinsReward;

        public int CurrentReward =>
            _levelEconomySettings.GetMenuReward(
                _playerProgress.CurrentLevel);

        public void Request()
        {
            if (CanRequest == false)
            {
                return;
            }

            _requestedReward =
                _levelEconomySettings.GetMenuReward(
                    _playerProgress.CurrentLevel);

            StartRequest(AdsPlacements.MenuCoins);
        }

        protected override void HandleRewarded()
        {
            int reward = _requestedReward;

            _playerProgress.MarkMenuCoinsRewardClaimed();
            _wallet.Add(reward);

            NotifyAvailabilityChanged();
            RewardGranted?.Invoke();
        }
    }
}
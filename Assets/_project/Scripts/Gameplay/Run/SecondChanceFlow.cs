using System;
using WestRicochet.Gameplay.Obstacles;
using WestRicochet.Gameplay.Player;
using WestRicochet.Gameplay.Player.Damage;
using WestRicochet.Gameplay.Rewards.Flow;
using WestRicochet.Infrastructure.Ads;

namespace WestRicochet.Gameplay.Run
{
    public class SecondChanceFlow : RewardedFlowBase
    {
        private readonly ObstacleAreaReleaser _obstacleAreaReleaser;
        private readonly PlayerController _player;
        private readonly PlayerHealth _playerHealth;
        private readonly float _clearRadius;

        private RunContext _context;
        private RunContext _requestContext;

        public SecondChanceFlow(
            IRewardedAdsService adsService,
            ObstacleAreaReleaser obstacleAreaReleaser,
            PlayerController player,
            PlayerHealth playerHealth,
            float clearRadius)
            : base(adsService)
        {
            _obstacleAreaReleaser = obstacleAreaReleaser;
            _player = player;
            _playerHealth = playerHealth;
            _clearRadius = clearRadius;
        }

        public event Action SecondChanceGranted;

        public bool CanOffer =>
            _context != null &&
            _context.SecondChanceUsed == false &&
            RequestInFlight == false;

        public void SetContext(RunContext context)
        {
            Cancel();

            _requestContext = null;
            _context = context;

            NotifyAvailabilityChanged();
        }

        public void Request()
        {
            if (CanOffer == false)
            {
                return;
            }

            _requestContext = _context;

            StartRequest(AdsPlacements.SecondChance);
        }

        public override void Dispose()
        {
            base.Dispose();

            _context = null;
            _requestContext = null;
        }

        protected override void HandleRewarded()
        {
            RunContext requestContext = _requestContext;

            bool isContextValid =
                requestContext != null &&
                ReferenceEquals(requestContext, _context) &&
                requestContext.SecondChanceUsed == false;

            if (isContextValid == false)
            {
                NotifyAvailabilityChanged();
                return;
            }

            requestContext.UseSecondChance();

            _obstacleAreaReleaser.Release(
                _player.transform.position,
                _clearRadius);

            _playerHealth.ResetHealth();

            NotifyAvailabilityChanged();
            SecondChanceGranted?.Invoke();
        }

        protected override void OnRequestCompleted()
        {
            _requestContext = null;
        }
    }
}
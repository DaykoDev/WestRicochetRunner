using System;
using WestRicochet.Gameplay.Obstacles;
using WestRicochet.Gameplay.Obstacles.Spawning;
using WestRicochet.Gameplay.Player;
using WestRicochet.Gameplay.Player.Damage;
using WestRicochet.Infrastructure.Ads;

namespace WestRicochet.Gameplay.Run
{
    public class SecondChanceFlow : IDisposable
    {
        private readonly AdsService _adsService;
        private readonly ObstacleAreaReleaser _obstacleAreaReleaser;
        private readonly PlayerController _player;
        private readonly PlayerHealth _playerHealth;
        private readonly float _clearRadius;

        private RunContext _context;
        private RunContext _requestContext;

        private bool _requestInFlight;
        private int _activeRequestId;

        public SecondChanceFlow(
            AdsService adsService,
            ObstacleAreaReleaser obstacleAreaReleaser,
            PlayerController player,
            PlayerHealth playerHealth,
            float clearRadius)
        {
            _adsService = adsService;
            _obstacleAreaReleaser = obstacleAreaReleaser;
            _player = player;
            _playerHealth = playerHealth;
            _clearRadius = clearRadius;
        }
        public event Action SecondChanceGranted;
        public event Action AvailabilityChanged;

        public bool CanOffer =>
            _context != null &&
            _context.SecondChanceUsed == false &&
            _requestInFlight == false;

        public void SetContext(RunContext context)
        {
            Cancel();

            _context = context;
            AvailabilityChanged?.Invoke();
        }

        public void Request()
        {
            if (CanOffer == false)
            {
                return;
            }

            _requestInFlight = true;
            _requestContext = _context;

            bool isStarted = _adsService.ShowRewarded(AdsPlacements.SecondChance, OnRewarded, OnAdError, out _activeRequestId);

            if (isStarted == false)
            {
                CompleteRequest();
            }

            AvailabilityChanged?.Invoke();
        }

        public void Cancel()
        {
            if (_requestInFlight == false)
            {
                return;
            }

            _adsService.CancelRewarded(_activeRequestId);
            CompleteRequest();

            AvailabilityChanged?.Invoke();
        }

        public void Dispose()
        {
            Cancel();
            _context = null;
        }

        private void OnRewarded(int requestId)
        {
            if (IsActiveRequest(requestId) == false)
            {
                return;
            }

            RunContext requestContext = _requestContext;
            CompleteRequest();

            bool isContextValid =
                requestContext != null &&
                ReferenceEquals(requestContext, _context) &&
                requestContext.SecondChanceUsed == false;

            if (isContextValid == false)
            {
                AvailabilityChanged?.Invoke();
                return;
            }

            requestContext.UseSecondChance();

            _obstacleAreaReleaser.Release(_player.transform.position, _clearRadius);

            _playerHealth.ResetHealth();

            AvailabilityChanged?.Invoke();
            SecondChanceGranted?.Invoke();
        }

        private void OnAdError(int requestId)
        {
            if (IsActiveRequest(requestId) == false)
            {
                return;
            }

            CompleteRequest();
            AvailabilityChanged?.Invoke();
        }

        private bool IsActiveRequest(int requestId)
        {
            return _requestInFlight && _activeRequestId == requestId;
        }

        private void CompleteRequest()
        {
            _requestInFlight = false;
            _activeRequestId = 0;
            _requestContext = null;
        }
    }
}
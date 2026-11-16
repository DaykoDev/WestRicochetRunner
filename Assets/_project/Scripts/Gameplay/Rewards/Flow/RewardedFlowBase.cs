using System;
using WestRicochet.Infrastructure.Ads;

namespace WestRicochet.Gameplay.Rewards.Flow
{
    public abstract class RewardedFlowBase : IDisposable
    {
        private readonly IRewardedAdsService _adsService;

        private bool _requestInFlight;
        private int _activeRequestId;

        protected RewardedFlowBase(IRewardedAdsService adsService)
        {
            _adsService = adsService;
        }

        public event Action AvailabilityChanged;

        protected bool RequestInFlight => _requestInFlight;

        public virtual void Cancel()
        {
            if (_requestInFlight == false)
            {
                return;
            }

            _adsService.CancelRewarded(_activeRequestId);

            CompleteRequest();
            NotifyAvailabilityChanged();
        }

        public virtual void Dispose()
        {
            Cancel();
        }

        protected virtual void OnRequestCompleted()
        {
        }

        protected void StartRequest(string placement)
        {
            if (_requestInFlight)
            {
                return;
            }

            _requestInFlight = true;

            bool isStarted = _adsService.ShowRewarded(
                placement,
                OnRewarded,
                OnAdError,
                out _activeRequestId);

            if (isStarted == false)
            {
                CompleteRequest();
            }

            NotifyAvailabilityChanged();
        }

        protected void NotifyAvailabilityChanged()
        {
            AvailabilityChanged?.Invoke();
        }

        protected abstract void HandleRewarded();

        private void OnRewarded(int requestId)
        {
            if (IsActiveRequest(requestId) == false)
            {
                return;
            }

            CompleteRequest();
            HandleRewarded();
            OnRequestCompleted();
        }

        private void OnAdError(int requestId)
        {
            if (IsActiveRequest(requestId) == false)
            {
                return;
            }

            CompleteRequest();
            OnRequestCompleted();
            NotifyAvailabilityChanged();
        }

        private bool IsActiveRequest(int requestId)
        {
            return _requestInFlight &&
                _activeRequestId == requestId;
        }

        private void CompleteRequest()
        {
            _requestInFlight = false;
            _activeRequestId = 0;
        }
    }
}
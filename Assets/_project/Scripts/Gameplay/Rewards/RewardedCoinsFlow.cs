using System;

public class RewardedCoinsFlow : IDisposable
{
    private readonly AdsService _adsService;
    private readonly Wallet _wallet;
    private readonly PlayerProgress _playerProgress;
    private readonly MenuCoinsRewardCalculator _rewardCalculator;

    private bool _requestInFlight;
    private int _activeRequestId;
    private int _requestedReward;

    public RewardedCoinsFlow(AdsService adsService, Wallet wallet, PlayerProgress playerProgress, MenuCoinsRewardCalculator rewardCalculator)
    {
        _adsService = adsService;
        _wallet = wallet;
        _playerProgress = playerProgress;
        _rewardCalculator = rewardCalculator;
    }

    public bool CanRequest => _requestInFlight == false && _adsService.IsBusy == false;
    public int CurrentReward => _rewardCalculator.Calculate(_playerProgress.CurrentLevel);

    public event Action<int> RewardGranted;
    public event Action AvailabilityChanged;

    public void Request()
    {
        if (CanRequest == false)
        {
            return;
        }

        _requestInFlight = true;
        _requestedReward = _rewardCalculator.Calculate(_playerProgress.CurrentLevel);

        bool isStarted = _adsService.ShowRewarded(AdsPlacements.MenuCoins, OnRewarded, OnAdError, out _activeRequestId);

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
    }

    private void OnRewarded(int requestId)
    {
        if (IsActiveRequest(requestId) == false)
        {
            return;
        }

        int reward = _requestedReward;
        CompleteRequest();

        _wallet.Add(reward);

        AvailabilityChanged?.Invoke();
        RewardGranted?.Invoke(reward);
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
        _requestedReward = 0;
    }
}
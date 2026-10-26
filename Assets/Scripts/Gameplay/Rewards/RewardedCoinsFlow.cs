using System;

public class RewardedCoinsFlow
{
    private readonly AdsService _adsService;
    private readonly Wallet _wallet;
    private readonly int _rewardCoins;

    private bool _requestInFlight;

    public RewardedCoinsFlow(AdsService adsService, Wallet wallet, int rewardCoins)
    {
        _adsService = adsService;
        _wallet = wallet;
        _rewardCoins = rewardCoins;
    }

    public event Action<int> RewardGranted;

    public void Request()
    {
        if (_requestInFlight || _adsService.IsBusy)
        {
            return;
        }

        _requestInFlight = true;
        _adsService.ShowRewarded(AdsPlacements.MenuCoins, OnRewarded, OnAdError);
    }

    private void OnRewarded()
    {
        _requestInFlight = false;
        _wallet.Add(_rewardCoins);

        RewardGranted?.Invoke(_rewardCoins);
    }

    private void OnAdError()
    {
        _requestInFlight = false;
    }
}
using System;

public class SecondChanceFlow
{
    private readonly AdsService _adsService;
    private readonly ObstacleFactory _obstacleFactory;
    private readonly Player _player;
    private readonly PlayerHealth _playerHealth;
    private readonly float _clearRadius;

    private RunContext _context;
    private bool _requestInFlight;

    public event Action SecondChanceGranted;

    public SecondChanceFlow(AdsService adsService, ObstacleFactory obstacleFactory, Player player, PlayerHealth playerHealth, float clearRadius)
    {
        _adsService = adsService;
        _obstacleFactory = obstacleFactory;
        _player = player;
        _playerHealth = playerHealth;
        _clearRadius = clearRadius;
    }

    public bool CanOffer =>
        _context != null &&
        _context.SecondChanceUsed == false &&
        _requestInFlight == false;

    public void SetContext(RunContext context)
    {
        _context = context;
        _requestInFlight = false;
    }

    public void Request()
    {
        if (CanOffer == false)
        {
            return;
        }

        _requestInFlight = true;
        _adsService.ShowRewarded(AdsPlacements.SecondChance, OnRewarded, OnAdError);
    }

    private void OnRewarded()
    {
        _requestInFlight = false;
        _context.UseSecondChance();
        _obstacleFactory.ReleaseObstaclesNear(_player.transform.position, _clearRadius);
        _playerHealth.ResetHealth();
        SecondChanceGranted?.Invoke();
    }

    private void OnAdError()
    {
        _requestInFlight = false;
    }
}
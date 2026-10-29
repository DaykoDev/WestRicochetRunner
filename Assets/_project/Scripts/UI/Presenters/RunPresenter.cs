using System;

public class RunPresenter : IDisposable
{
    private readonly RunScreen _screen;
    private readonly PlayerHealth _playerHealth;
    private readonly PlayerProgress _playerProgress;
    private readonly RunRewardCollector _rewardCollector;
    private readonly TrackScroller _trackScroller;
    private readonly LevelGenerator _levelGenerator;
    private readonly BallQueue _ballQueue;

    public RunPresenter(RunScreen screen, PlayerHealth playerHealth, PlayerProgress playerProgress, RunRewardCollector rewardCollector, TrackScroller trackScroller, LevelGenerator levelGenerator, BallQueue ballQueue)
    {
        _screen = screen;
        _playerHealth = playerHealth;
        _playerProgress = playerProgress;
        _rewardCollector = rewardCollector;
        _trackScroller = trackScroller;
        _levelGenerator = levelGenerator;
        _ballQueue = ballQueue;

        _screen.TutorialDismissed += OnTutorialDismissed;
        _screen.SettingsClicked += OnSettingsClicked;
        _playerHealth.HealthChanged += OnHealthChanged;
        _playerProgress.LevelChanged += OnLevelChanged;
        _rewardCollector.CollectedThisRunChanged += OnCollectedCoinsChanged;
        _trackScroller.DistanceChanged += OnDistanceChanged;
        _ballQueue.AvailableCountChanged += OnAvailableCountChanged;
    }

    public event Action SettingsRequested;

    public void HandleStateEntered()
    {
        _screen.SetTutorialVisible(_playerProgress.IsTutorialCompleted == false);
        _screen.Show();
        _screen.SetHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        _screen.SetLevel(_playerProgress.CurrentLevel);
        _screen.SetCoins(_rewardCollector.CollectedThisRun);
        UpdateProgress();
        _screen.SetBallCount(_ballQueue.AvailableCount, _ballQueue.Capacity);
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _screen.TutorialDismissed -= OnTutorialDismissed;
        _screen.SettingsClicked -= OnSettingsClicked;
        _playerHealth.HealthChanged -= OnHealthChanged;
        _playerProgress.LevelChanged -= OnLevelChanged;
        _rewardCollector.CollectedThisRunChanged -= OnCollectedCoinsChanged;
        _trackScroller.DistanceChanged -= OnDistanceChanged;
        _ballQueue.AvailableCountChanged -= OnAvailableCountChanged;
    }

    private void OnTutorialDismissed()
    {
        _playerProgress.CompleteTutorial();
    }

    private void OnSettingsClicked()
    {
        SettingsRequested?.Invoke();
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        _screen.SetHealth(currentHealth, maxHealth);
    }

    private void OnLevelChanged(int level)
    {
        _screen.SetLevel(level);
    }

    private void OnCollectedCoinsChanged(int coins)
    {
        _screen.SetCoins(coins);
    }

    private void OnDistanceChanged(float distance)
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        float length = _levelGenerator.TrackLength;

        float progress = length > 0f ? _trackScroller.DistanceScrolled / length : 0f;

        _screen.SetProgress(progress);
    }

    private void OnAvailableCountChanged(int available, int capacity)
    {
        _screen.SetBallCount(available, capacity);
    }
}
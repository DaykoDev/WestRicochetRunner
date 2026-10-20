using System;

public class RunPresenter : IDisposable
{
    private readonly RunScreen _screen;
    private readonly PlayerHealth _playerHealth;
    private readonly PlayerProgress _playerProgress;
    private readonly Wallet _wallet;

    public RunPresenter(RunScreen screen, PlayerHealth playerHealth, PlayerProgress playerProgress, Wallet wallet)
    {
        _screen = screen;
        _playerHealth = playerHealth;
        _playerProgress = playerProgress;
        _wallet = wallet;

        _playerHealth.HealthChanged += OnHealthChanged;
        _playerProgress.LevelChanged += OnLevelChanged;
        _wallet.BalanceChanged += OnBalanceChanged;
    }

    public void HandleStateEntered()
    {
        _screen.Show();
        _screen.SetHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        _screen.SetLevel(_playerProgress.CurrentLevel);
        _screen.SetCoins(_wallet.Balance);
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _playerHealth.HealthChanged -= OnHealthChanged;
        _playerProgress.LevelChanged -= OnLevelChanged;
        _wallet.BalanceChanged -= OnBalanceChanged;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        _screen.SetHealth(currentHealth, maxHealth);
    }

    private void OnLevelChanged(int level)
    {
        _screen.SetLevel(level);
    }

    private void OnBalanceChanged(int balance)
    {
        _screen.SetCoins(balance);
    }
}
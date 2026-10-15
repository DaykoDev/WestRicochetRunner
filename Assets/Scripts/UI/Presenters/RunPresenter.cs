using System;

public class RunPresenter : IDisposable
{
    private readonly RunScreen _screen;
    private readonly PlayerHealth _playerHealth;

    public RunPresenter(RunScreen screen, PlayerHealth playerHealth)
    {
        _screen = screen;
        _playerHealth = playerHealth;

        _playerHealth.HealthChanged += OnHealthChanged;
    }

    public event Action TestWinRequested;

    public void HandleStateEntered()
    {
        _screen.Show();
        _screen.SetHealth(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
    }

    public void HandleStateExited()
    {
        _screen.Hide();
    }

    public void Dispose()
    {
        _playerHealth.HealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int currentHealth, int maxHealth) =>
        _screen.SetHealth(currentHealth, maxHealth);

    private void OnTestWinClicked()
    {
        TestWinRequested?.Invoke();
    }
}
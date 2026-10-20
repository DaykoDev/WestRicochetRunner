using System;

public class UpgradeService : IDisposable
{
    private readonly PlayerProgress _playerProgress;
    private readonly Wallet _wallet;
    private readonly PermanentUpgradeSettings _settings;
    private readonly UpgradeCostCalculator _costCalculator;
    private bool _isDisposed;

    public UpgradeService(PlayerProgress playerProgress, Wallet wallet, PermanentUpgradeSettings settings, UpgradeCostCalculator costCalculator)
    {
        _playerProgress = playerProgress;
        _wallet = wallet;
        _settings = settings;
        _costCalculator = costCalculator;

        _playerProgress.LevelChanged += OnProgressLevelChanged;
        _playerProgress.UpgradeLevelChanged += OnProgressUpgradeLevelChanged;
        _wallet.BalanceChanged += OnWalletBalanceChanged;
    }

    public int CurrentLevel => _playerProgress.CurrentLevel;
    public int Balance => _wallet.Balance;

    public event Action<int> LevelChanged;
    public event Action<int> BalanceChanged;
    public event Action<PermanentUpgradeType, int> UpgradeLevelChanged;

    public int GetLevel(PermanentUpgradeType type)
    {
        return _playerProgress.GetUpgradeLevel(type);
    }

    public int GetMaxLevel(PermanentUpgradeType type)
    {
        return TryGetDefinition(type, out PermanentUpgradeSettings.Definition definition) ? definition.MaxLevel : -1;
    }

    public int GetNextCost(PermanentUpgradeType type)
    {
        if (TryGetDefinition(type, out PermanentUpgradeSettings.Definition definition) == false)
        {
            return 0;
        }

        return _costCalculator.CalculateNextCost(definition, _playerProgress.GetUpgradeLevel(type));
    }

    public bool CanPurchase(PermanentUpgradeType type)
    {
        if (TryGetDefinition(type, out PermanentUpgradeSettings.Definition definition) == false)
        {
            return false;
        }

        int level = _playerProgress.GetUpgradeLevel(type);

        if (IsMaxLevelReached(definition, level))
        {
            return false;
        }

        int cost = _costCalculator.CalculateNextCost(definition, level);
        return _wallet.Balance >= cost;
    }

    public bool TryPurchase(PermanentUpgradeType type)
    {
        if (TryGetDefinition(type, out PermanentUpgradeSettings.Definition definition) == false)
        {
            return false;
        }

        int currentLevel = _playerProgress.GetUpgradeLevel(type);

        if (IsMaxLevelReached(definition, currentLevel))
        {
            return false;
        }

        int cost = _costCalculator.CalculateNextCost(definition, currentLevel);

        if (_wallet.TrySpend(cost) == false)
        {
            return false;
        }

        if (_playerProgress.TryIncreaseUpgradeLevel(type))
        {
            return true;
        }

        _wallet.Add(cost);

        return false;
    }

    public void ApplyPermanentUpgrades(RunModifiers modifiers)
    {
        if (modifiers == null)
        {
            return;
        }

        ApplyStartBallCount(modifiers);
        ApplyBallDamage(modifiers);
        ApplyIncomeMultiplier(modifiers);
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        _playerProgress.LevelChanged -= OnProgressLevelChanged;
        _playerProgress.UpgradeLevelChanged -= OnProgressUpgradeLevelChanged;
        _wallet.BalanceChanged -= OnWalletBalanceChanged;
    }

    private void ApplyStartBallCount(RunModifiers modifiers)
    {
        if (TryGetDefinition(PermanentUpgradeType.StartBallCount, out PermanentUpgradeSettings.Definition definition) == false)
        {
            return;
        }

        int level = _playerProgress.GetUpgradeLevel(PermanentUpgradeType.StartBallCount);

        if (level > 0)
        {
            modifiers.Apply(UpgradeType.BallCount, definition.BonusPerLevel * level);
        }
    }

    private void ApplyBallDamage(RunModifiers modifiers)
    {
        if (TryGetDefinition(PermanentUpgradeType.BallDamage, out PermanentUpgradeSettings.Definition definition) == false)
        {
            return;
        }

        int level = _playerProgress.GetUpgradeLevel(PermanentUpgradeType.BallDamage);

        if (level > 0)
        {
            modifiers.AddBallDamageBonus(definition.BonusPerLevel * level);
        }
    }

    private void ApplyIncomeMultiplier(RunModifiers modifiers)
    {
        if (TryGetDefinition(PermanentUpgradeType.IncomeMultiplier, out PermanentUpgradeSettings.Definition definition) == false)
        {
            return;
        }

        int level = _playerProgress.GetUpgradeLevel(PermanentUpgradeType.IncomeMultiplier);

        if (level > 0)
        {
            modifiers.AddIncomeMultiplierBonus(definition.BonusPerLevel * level);
        }
    }

    private bool TryGetDefinition(PermanentUpgradeType type, out PermanentUpgradeSettings.Definition definition)
    {
        if (_playerProgress != null &&
            _wallet != null &&
            _settings != null &&
            _costCalculator != null &&
            _settings.TryGetDefinition(type, out definition))
        {
            return true;
        }

        definition = null;
        return false;
    }

    private static bool IsMaxLevelReached(PermanentUpgradeSettings.Definition definition, int level)
    {
        return definition.MaxLevel > 0 && level >= definition.MaxLevel;
    }

    private void OnProgressLevelChanged(int level)
    {
        LevelChanged?.Invoke(level);
    }

    private void OnProgressUpgradeLevelChanged(PermanentUpgradeType type, int level)
    {
        UpgradeLevelChanged?.Invoke(type, level);
    }

    private void OnWalletBalanceChanged(int balance)
    {
        BalanceChanged?.Invoke(balance);
    }
}
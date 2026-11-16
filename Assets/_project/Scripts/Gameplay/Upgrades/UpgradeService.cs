using System;
using UnityEngine;
using WestRicochet.Gameplay.Rewards;
using WestRicochet.Gameplay.Run;
using WestRicochet.Meta;

namespace WestRicochet.Gameplay.Upgrades
{
    public class UpgradeService : IDisposable
    {
        private readonly PlayerProgress _playerProgress;
        private readonly Wallet _wallet;
        private readonly PermanentUpgradeSettings _settings;
        private readonly UpgradeCostCalculator _costCalculator;
        private bool _isDisposed;

        public UpgradeService(
            PlayerProgress playerProgress,
            Wallet wallet,
            PermanentUpgradeSettings settings,
            UpgradeCostCalculator costCalculator)
        {
            _playerProgress = playerProgress;
            _wallet = wallet;
            _settings = settings;
            _costCalculator = costCalculator;

            _playerProgress.LevelChanged += OnProgressLevelChanged;
            _playerProgress.UpgradeLevelChanged += OnProgressUpgradeLevelChanged;
            _wallet.BalanceChanged += OnWalletBalanceChanged;
        }

        public event Action<int> LevelChanged;
        public event Action<int> BalanceChanged;
        public event Action<PermanentUpgradeType, int> UpgradeLevelChanged;

        public int CurrentLevel => _playerProgress.CurrentLevel;
        public int Balance => _wallet.Balance;

        public int GetLevel(PermanentUpgradeType type)
        {
            return _playerProgress.GetUpgradeLevel(type);
        }

        public int GetMaxLevel(PermanentUpgradeType type)
        {
            return TryGetDefinition(type, out PermanentUpgradeDefinition definition) ? definition.MaxLevel : -1;
        }

        public int GetNextCost(PermanentUpgradeType type)
        {
            if (TryGetDefinition(type, out PermanentUpgradeDefinition definition) == false)
            {
                return 0;
            }

            return _costCalculator.CalculateNextCost(definition, _playerProgress.GetUpgradeLevel(type));
        }

        public bool CanPurchase(PermanentUpgradeType type)
        {
            if (TryGetDefinition(type, out PermanentUpgradeDefinition definition) == false ||
                CurrentLevel < definition.UnlockLevel)
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
            if (TryGetDefinition(type, out PermanentUpgradeDefinition definition) == false ||
                CurrentLevel < definition.UnlockLevel)
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

            ApplyUpgrade(
                modifiers,
                PermanentUpgradeType.StartBallCount,
                (target, bonus) =>
                    target.Apply(UpgradeType.BallCount, bonus));

            ApplyUpgrade(
                modifiers,
                PermanentUpgradeType.BallDamage,
                (target, bonus) =>
                    target.AddBallDamageBonus(Mathf.RoundToInt(bonus)));

            ApplyUpgrade(
                modifiers,
                PermanentUpgradeType.IncomeMultiplier,
                (target, bonus) =>
                    target.AddIncomeMultiplierBonus(bonus));

            ApplyUpgrade(
                modifiers,
                PermanentUpgradeType.BounceCount,
                (target, bonus) =>
                    target.Apply(UpgradeType.BounceCount, bonus));
        }

        public bool IsUnlocked(PermanentUpgradeType type)
        {
            return TryGetDefinition(type, out PermanentUpgradeDefinition definition) &&
                CurrentLevel >= definition.UnlockLevel;
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

        private void ApplyUpgrade(
            RunModifiers modifiers,
            PermanentUpgradeType type,
            Action<RunModifiers, float> apply)
        {
            if (TryGetDefinition(
                    type,
                    out PermanentUpgradeDefinition definition) == false)
            {
                return;
            }

            int level = _playerProgress.GetUpgradeLevel(type);

            if (level <= 0)
            {
                return;
            }

            float bonus = definition.BonusPerLevel * level;

            apply(modifiers, bonus);
        }

        private bool TryGetDefinition(PermanentUpgradeType type, out PermanentUpgradeDefinition definition)
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

        private static bool IsMaxLevelReached(PermanentUpgradeDefinition definition, int level)
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
}
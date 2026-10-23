using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField, Min(0)] private int _initialBalance;

    private bool _isInitialized;

    public int Balance { get; private set; }

    public event Action<int> BalanceChanged;

    public void Initialize(int? savedBalance = null)
    {
        if (_isInitialized)
        {
            return;
        }

        Balance = Math.Max(0, savedBalance ?? _initialBalance);
        _isInitialized = true;
        BalanceChanged?.Invoke(Balance);
    }

    public void Add(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Balance = (int)Math.Min((long)Balance + amount, int.MaxValue);
        BalanceChanged?.Invoke(Balance);
    }

    public void Remove(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Balance = Math.Max(0, Balance - amount);
        BalanceChanged?.Invoke(Balance);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || Balance < amount)
        {
            return false;
        }

        Balance -= amount;
        BalanceChanged?.Invoke(Balance);

        return true;
    }
}
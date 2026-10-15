using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    [SerializeField, Min(0)] private int _initialBalance;

    public int Balance { get; private set; }

    public event Action<int> BalanceChanged;

    private void Awake()
    {
        Balance = _initialBalance;
    }

    public void Add(int amount)
    {
        if (amount <= 0)
            return;

        Balance = (int)Math.Min((long)Balance + amount, int.MaxValue);
        BalanceChanged?.Invoke(Balance);
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0 || Balance < amount)
            return false;

        Balance -= amount;
        BalanceChanged?.Invoke(Balance);
        return true;
    }
}
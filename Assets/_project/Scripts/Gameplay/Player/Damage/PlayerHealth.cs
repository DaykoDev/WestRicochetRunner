using System;
using UnityEngine;

namespace WestRicochet.Gameplay.Player.Damage
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _maxHealth = 3;

        private bool _isDead;
        private bool _isOneHitDeathEnabled;
        
        public event Action<int, int> HealthChanged;
        public event Action Died;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => _maxHealth;
        public bool IsDead => _isDead;

        private void Awake()
        {
            ResetHealth();
        }

        public void TakeDamage(int damage)
        {
            if (_isDead || damage <= 0)
            {
                return;
            }

            CurrentHealth = _isOneHitDeathEnabled ? 0 : Mathf.Max(0, CurrentHealth - damage);

            HealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth == 0)
            {
                Die();
            }
        }

        public void Kill()
        {
            if (_isDead)
            {
                return;
            }

            CurrentHealth = 0;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
            Die();
        }

        public void SetOneHitDeathEnabled(bool isEnabled)
        {
            _isOneHitDeathEnabled = isEnabled;
        }

        public void ResetHealth()
        {
            _isOneHitDeathEnabled = false;
            _isDead = false;
            CurrentHealth = _maxHealth;
            HealthChanged?.Invoke(CurrentHealth, MaxHealth);
        }

        private void Die()
        {
            if (_isDead)
            {
                return;
            }

            _isDead = true;
            Died?.Invoke();
        }
    }
}
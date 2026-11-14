using UnityEngine;
using WestRicochet.Gameplay.Balls.Shooting;
using WestRicochet.Gameplay.Run;

namespace WestRicochet.Gameplay.Player.Shooting
{
    public class AutoShooter : MonoBehaviour
    {
        private const float RetryInterval = 0.05f;

        [Header("References")]
        [SerializeField] private BallLauncher _ballLauncher;

        [Header("Shooting Settings")]
        [SerializeField, Min(0.01f)] private float _fireInterval = 0.3f;

        private float _shootTimer;
        private bool _isShooting;
        private RunModifiers _runModifiers;
        private float _bonusFireRateMultiplier = 1f;

        private void Awake()
        {
            if (_ballLauncher == null)
            {
                Debug.LogError("AutoShooter: BallLauncher is not assigned.", this);

                enabled = false;
            }
        }

        private void Update()
        {
            if (_ballLauncher == null)
            {
                return;
            }

            if (_isShooting == false)
            {
                return;
            }

            if (_shootTimer > 0f)
            {
                _shootTimer -= Time.deltaTime;
                return;
            }

            float permanentMultiplier = _runModifiers != null ? _runModifiers.FireRateMultiplier : 1f;

            float totalMultiplier = permanentMultiplier * _bonusFireRateMultiplier;

            float interval = _fireInterval / totalMultiplier;

            _shootTimer = _ballLauncher.TryLaunch() ? interval : RetryInterval;
        }

        public void SetRunModifiers(RunModifiers modifiers)
        {
            _runModifiers = modifiers;
        }

        public void SetBonusFireRateMultiplier(float multiplier)
        {
            _bonusFireRateMultiplier = Mathf.Max(1f, multiplier);
            _shootTimer = 0f;
        }

        public void StartShooting()
        {
            _isShooting = true;
            _shootTimer = 0f;
        }

        public void StopShooting()
        {
            _isShooting = false;
            _shootTimer = 0f;
        }
    }
}
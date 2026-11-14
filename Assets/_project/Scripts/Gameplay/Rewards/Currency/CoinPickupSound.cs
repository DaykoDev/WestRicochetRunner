using UnityEngine;
using WestRicochet.Gameplay.Rewards.Flow;

namespace WestRicochet.Gameplay.Rewards.Currency
{
    public sealed class CoinPickupSound : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RunRewardCollector _rewardCollector;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _coinClip;

        [Header("Sound")]
        [SerializeField, Range(0f, 1f)] private float _volume = 0.7f;

        [Header("Pitch Combo")]
        [SerializeField, Min(0f)] private float _pitchStep = 0.04f;
        [SerializeField, Min(0.5f)] private float _defaultPitch = 0.8f;
        [SerializeField, Min(1f)] private float _maxPitch = 1.5f;
        [SerializeField, Min(0.05f)] private float _comboResetDelay = 0.3f;

        [Header("Spam Protection")]
        [SerializeField, Min(0f)] private float _minInterval = 0.035f;

        private int _lastCollectedAmount;

        private float _nextPlayTime;
        private float _lastPlayTime;
        private float _currentPitch;

        private void Awake()
        {
            if (_audioSource == null)
            {
                _audioSource = GetComponent<AudioSource>();
            }

            if (_rewardCollector == null ||
                _audioSource == null ||
                _coinClip == null)
            {
                Debug.LogError("CoinPickupSound references are missing.", this);

                enabled = false;
                return;
            }

            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.dopplerLevel = 0f;
        }

        private void OnEnable()
        {
            if (_rewardCollector == null)
            {
                return;
            }

            _lastCollectedAmount = _rewardCollector.CollectedThisRun;

            _currentPitch = _defaultPitch;
            _nextPlayTime = 0f;
            _lastPlayTime = float.NegativeInfinity;

            _rewardCollector.CollectedThisRunChanged += OnCollectedAmountChanged;
        }

        private void OnDisable()
        {
            if (_rewardCollector != null)
            {
                _rewardCollector.CollectedThisRunChanged -= OnCollectedAmountChanged;
            }
        }

        private void OnCollectedAmountChanged(int collectedAmount)
        {
            bool amountIncreased = collectedAmount > _lastCollectedAmount;

            _lastCollectedAmount = collectedAmount;

            if (amountIncreased == false)
            {
                return;
            }

            float currentTime = Time.unscaledTime;

            if (currentTime < _nextPlayTime)
            {
                return;
            }

            UpdatePitch(currentTime);

            _audioSource.pitch = _currentPitch;
            _audioSource.PlayOneShot(_coinClip, _volume);

            _lastPlayTime = currentTime;
            _nextPlayTime = currentTime + _minInterval;
        }

        private void UpdatePitch(float currentTime)
        {
            bool comboContinues = currentTime - _lastPlayTime <= _comboResetDelay;

            if (comboContinues == false)
            {
                _currentPitch = _defaultPitch;
                return;
            }

            _currentPitch = Mathf.Min(_currentPitch + _pitchStep, _maxPitch);
        }
    }
}
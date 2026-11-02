using UnityEngine;

public sealed class CoinPickupSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RunRewardCollector _rewardCollector;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _coinClip;

    [Header("Sound")]
    [SerializeField, Range(0f, 1f)] private float _volume = 0.7f;
    [SerializeField] private float _minPitch = 0.95f;
    [SerializeField] private float _maxPitch = 1.05f;

    [Header("Spam Protection")]
    [SerializeField, Min(0f)] private float _minInterval = 0.035f;

    private int _lastCollectedAmount;
    private float _nextPlayTime;

    private void Awake()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        if (_rewardCollector == null || _audioSource == null || _coinClip == null)
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

        if (Time.unscaledTime < _nextPlayTime)
        {
            return;
        }

        _nextPlayTime = Time.unscaledTime + _minInterval;

        _audioSource.pitch = Random.Range(_minPitch, _maxPitch);

        _audioSource.PlayOneShot(_coinClip, _volume);
    }
}
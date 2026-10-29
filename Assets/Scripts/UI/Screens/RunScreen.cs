using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunScreen : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private TMP_Text _levelText;

    [Header("Coins")]
    [SerializeField] private TMP_Text _coinsText;
    [SerializeField, Range(0f, 1f)] private float _coinsPunchStrength = 0.2f;
    [SerializeField, Min(0.02f)] private float _coinsPunchDuration = 0.18f;

    [Header("Health")]
    [SerializeField] private Image[] _healthIcons;
    [SerializeField] private Sprite _fullHeartSprite;
    [SerializeField] private Sprite _emptyHeartSprite;

    [Header("HUD")]
    [SerializeField] private Image _progressFill;
    [SerializeField] private TMP_Text _ballsText;
    [SerializeField] private float _punchKoef = 0.2f;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;

    [Header("Tutorial")]
    [SerializeField] private GameObject _tutorialHint;
    [SerializeField] private Button _tutorialDismissButton;
    [SerializeField, Min(0f)] private float _tutorialDuration = 4f;

    private float _tutorialTimer;
    private bool _isTutorialCounting;

    private int _previousCoins;
    private bool _isCoinsInitialized;

    public event Action TutorialDismissed;
    public event Action SettingsClicked;

    private void OnEnable()
    {
        _tutorialDismissButton.onClick.AddListener(OnTutorialDismissed);
        _settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    private void OnDisable()
    {
        _tutorialDismissButton.onClick.RemoveListener(OnTutorialDismissed);
        _settingsButton.onClick.RemoveListener(OnSettingsClicked);
    }

    private void Update()
    {
        if (_isTutorialCounting == false)
        {
            return;
        }

        _tutorialTimer -= Time.deltaTime;

        if (_tutorialTimer <= 0f)
        {
            _isTutorialCounting = false;
            OnTutorialDismissed();
        }
    }

    public void SetTutorialVisible(bool isVisible)
    {
        _tutorialHint.SetActive(isVisible);

        if (isVisible)
        {
            _tutorialTimer = _tutorialDuration;
            _isTutorialCounting = true;
        }
        else
        {
            _isTutorialCounting = false;
        }
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < _healthIcons.Length; i++)
        {
            Image icon = _healthIcons[i];

            if (icon == null)
            {
                continue;
            }

            icon.gameObject.SetActive(i < maxHealth);

            if (i < maxHealth)
            {
                icon.sprite = i < currentHealth ? _fullHeartSprite : _emptyHeartSprite;
            }
        }
    }

    public void SetLevel(int level)
    {
        if (_levelText != null)
        {
            _levelText.text = $"Level: {level}";
        }
    }

    public void SetCoins(int coins)
    {
        if (_coinsText == null)
        {
            return;
        }

        bool shouldAnimate = _isCoinsInitialized && coins != _previousCoins;

        _coinsText.text = coins.ToString();

        _previousCoins = coins;
        _isCoinsInitialized = true;

        if (shouldAnimate == false)
        {
            return;
        }

        Transform target = _coinsText.transform;

        target.DOKill();
        target.localScale = Vector3.one;

        target.DOPunchScale(Vector3.one * _coinsPunchStrength, _coinsPunchDuration);
    }

    public void SetProgress(float progress)
    {
        if (_progressFill != null)
        {
            _progressFill.fillAmount = Mathf.Clamp01(progress);
        }
    }

    public void SetBallCount(int available, int capacity)
    {
        if (_ballsText != null)
        {
            _ballsText.text = $"{available}/{capacity}";
            _ballsText.transform.DOKill();
            _ballsText.transform.localScale = Vector3.one;
            _ballsText.transform.DOPunchScale(Vector3.one * _punchKoef, _punchKoef);
        }
    }

    private void OnTutorialDismissed()
    {
        SetTutorialVisible(false);
        TutorialDismissed?.Invoke();
    }

    private void OnSettingsClicked()
    {
        SettingsClicked?.Invoke();
    }
}
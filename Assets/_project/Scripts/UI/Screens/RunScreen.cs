using System;
using UnityEngine;
using UnityEngine.UI;
using WestRicochet.Gameplay.Run;
using WestRicochet.Infrastructure.Localization;
using WestRicochet.UI.Components;

namespace WestRicochet.UI.Screens
{
    public class RunScreen : MonoBehaviour
    {
        [Header("Progress")]
        [SerializeField] private LocalizedText _levelText;

        [Header("Health")]
        [SerializeField] private Image[] _healthIcons;
        [SerializeField] private Sprite _fullHeartSprite;
        [SerializeField] private Sprite _emptyHeartSprite;

        [Header("HUD")]
        [SerializeField] private Image _progressFill;
        [SerializeField] private PunchTextView _ballsView;
        [SerializeField] private PunchTextView _coinsView;

        [Header("Buttons")]
        [SerializeField] private Button _settingsButton;

        [Header("Tutorial")]
        [SerializeField] private GameObject _tutorialHint;
        [SerializeField] private Button _tutorialDismissButton;
        [SerializeField, Min(0f)] private float _tutorialDuration = 4f;

        [Header("Bonus")]
        [SerializeField] private BonusLevelSettings _bonusLevelSettings;
        [SerializeField] private RainbowTextGroup _rainbowTextGroup;

        private float _tutorialTimer;
        private bool _isTutorialCounting;

        public event Action TutorialDismissed;
        public event Action SettingsClicked;

        private void Awake()
        {
            if (_settingsButton == null ||
                _tutorialDismissButton == null ||
                _tutorialHint == null ||
                _healthIcons == null)
            {
                Debug.LogError(
                    "RunScreen: Buttons, tutorial hint and health icons " +
                    "must be assigned.",
                    this);

                enabled = false;
            }
        }

        private void OnEnable()
        {
            _tutorialDismissButton.onClick.AddListener(OnTutorialDismissed);
            _settingsButton.onClick.AddListener(OnSettingsClicked);
        }

        private void OnDisable()
        {
            _tutorialDismissButton?.onClick.RemoveListener(OnTutorialDismissed);
            _settingsButton?.onClick.RemoveListener(OnSettingsClicked);
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
                _levelText.SetKey(LocalizationKeys.Level, level);
            }

            bool isBonus = _bonusLevelSettings != null && _bonusLevelSettings.IsBonusLevel(level);

            if (_rainbowTextGroup != null)
            {
                _rainbowTextGroup.SetEnabled(isBonus);
            }
        }

        public void SetCoins(int coins)
        {
            if (_coinsView != null)
            {
                _coinsView.SetText(coins.ToString());
            }
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
            if (_ballsView != null)
            {
                _ballsView.SetText($"{available}/{capacity}");
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
}
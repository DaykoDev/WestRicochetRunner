using UnityEngine;

[CreateAssetMenu(fileName = nameof(AdsConfig), menuName = "Game/Ads Config")]
public class AdsConfig : ScriptableObject
{
    [SerializeField, Min(0f)] private float _interstitialCooldownSeconds = 90f;
    [SerializeField, Min(1f)] private float _victoryCoinsMultiplier = 2f;
    [SerializeField] private MenuCoinsRewardCalculator _menuCoinsRewardCalculator;
    [SerializeField, Min(0f)] private float _secondChanceClearRadius = 12f;

    public float InterstitialCooldownSeconds => _interstitialCooldownSeconds;
    public float VictoryCoinsMultiplier => _victoryCoinsMultiplier;
    public MenuCoinsRewardCalculator MenuCoinsRewardCalculator => _menuCoinsRewardCalculator;
    public float SecondChanceClearRadius => _secondChanceClearRadius;
}
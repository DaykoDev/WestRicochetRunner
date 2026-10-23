using UnityEngine;

[CreateAssetMenu(fileName = nameof(AdsConfig), menuName = "Game/Ads Config")]
public class AdsConfig : ScriptableObject
{
    [SerializeField, Min(0f)] private float _interstitialCooldownSeconds = 90f;
    [SerializeField, Min(1f)] private float _victoryCoinsMultiplier = 2f;
    [SerializeField, Min(0)] private int _menuRewardCoins = 100;
    [SerializeField, Min(0f)] private float _secondChanceClearRadius = 12f;

    public float InterstitialCooldownSeconds => _interstitialCooldownSeconds;
    public float VictoryCoinsMultiplier => _victoryCoinsMultiplier;
    public int MenuRewardCoins => _menuRewardCoins;
    public float SecondChanceClearRadius => _secondChanceClearRadius;
}
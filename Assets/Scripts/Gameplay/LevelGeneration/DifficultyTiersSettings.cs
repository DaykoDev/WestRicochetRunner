using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyTiersSettings", menuName = "Game/Level Generation/Difficulty Tiers Settings")]
public class DifficultyTiersSettings : ScriptableObject
{
    [Header("Tier Boundaries")]
    [SerializeField, Min(1)] private int _earlyMaxLevel = 3;
    [SerializeField, Min(1)] private int _midMaxLevel = 15;

    [Header("Challenge Blocks per Tier")]
    [SerializeField, Min(1)] private int _earlyChallengeBlocks = 1;
    [SerializeField, Min(1)] private int _midChallengeBlocks = 2;
    [SerializeField, Min(1)] private int _highChallengeBlocks = 3;

    [Header("Safe Density")]
    [SerializeField, Range(0f, 1f)] private float _safeEarly = 0.15f;
    [SerializeField, Range(0f, 1f)] private float _safeMid = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _safeHigh = 0.35f;

    [Header("Challenge Density")]
    [SerializeField, Range(0f, 1f)] private float _challengeEarly = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _challengeMid = 0.65f;
    [SerializeField, Range(0f, 1f)] private float _challengeHigh = 0.9f;

    public int GetChallengeBlockCount(int level)
    {
        switch (GetTier(level))
        {
            case DifficultyTier.Early:
                return _earlyChallengeBlocks;
            case DifficultyTier.Mid:
                return _midChallengeBlocks;
            default:
                return _highChallengeBlocks;
        }
    }

    public float GetSpawnDensity(TrackSectionType type, int level)
    {
        switch (GetTier(level))
        {
            case DifficultyTier.Early:
                return type == TrackSectionType.Safe ? _safeEarly : type == TrackSectionType.Challenge ? _challengeEarly : 0f;
            case DifficultyTier.Mid:
                return type == TrackSectionType.Safe ? _safeMid : type == TrackSectionType.Challenge ? _challengeMid : 0f;
            default:
                return type == TrackSectionType.Safe ? _safeHigh : type == TrackSectionType.Challenge ? _challengeHigh : 0f;
        }
    }

    private DifficultyTier GetTier(int level)
    {
        if (level <= _earlyMaxLevel)
        {
            return DifficultyTier.Early;
        }

        return level <= _midMaxLevel ? DifficultyTier.Mid : DifficultyTier.High;
    }

    private enum DifficultyTier
    {
        Early,
        Mid,
        High
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "ObstaclePlacementSettings", menuName = "Game/Level Generation/Obstacle Placement Settings")]
public class ObstaclePlacementSettings : ScriptableObject
{
    [Header("Safe Density")]
    [SerializeField, Range(0f, 1f)] private float _safeEarly = 0.15f;
    [SerializeField, Range(0f, 1f)] private float _safeMid = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _safeHigh = 0.35f;

    [Header("Challenge Density")]
    [SerializeField, Range(0f, 1f)] private float _challengeEarly = 0.25f;
    [SerializeField, Range(0f, 1f)] private float _challengeMid = 0.65f;
    [SerializeField, Range(0f, 1f)] private float _challengeHigh = 0.9f;

    public float GetSpawnDensity(TrackSectionType type, int level)
    {
        if (level <= 3)
        {
            return type == TrackSectionType.Safe ? _safeEarly : type == TrackSectionType.Challenge ? _challengeEarly : 0f;
        }

        if (level <= 15)
        {
            return type == TrackSectionType.Safe ? _safeMid : type == TrackSectionType.Challenge ? _challengeMid : 0f;
        }

        return type == TrackSectionType.Safe ? _safeHigh : type == TrackSectionType.Challenge ? _challengeHigh : 0f;
    }
}
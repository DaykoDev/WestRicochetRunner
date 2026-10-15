using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObstacleFactory _obstacleFactory;
    [SerializeField] private ObstaclePlacementSettings _placementSettings;

    public int PlaceObstacles(Transform sectionRoot, int level, TrackSectionType sectionType, float incomeMultiplier, int levelSeed, int sectionIndex)
    {
        if (sectionRoot == null || _obstacleFactory == null || _placementSettings == null)
        {
            Debug.LogError("ObstaclePlacer references are missing.", this);
            return 0;
        }

        if (TryMapSection(sectionType, out ObstacleSection obstacleSection) == false)
        {
            return 0;
        }

        ObstacleSpawnPoints spawnPoints = sectionRoot.GetComponent<ObstacleSpawnPoints>();

        if (spawnPoints == null || spawnPoints.Points == null)
        {
            Debug.LogWarning($"No spawn points found on section {sectionRoot.name}.", sectionRoot);
            return 0;
        }

        var points = new List<Transform>();

        foreach (Transform point in spawnPoints.Points)
        {
            if (point != null)
            {
                points.Add(point);
            }
        }

        if (points.Count == 0)
        {
            return 0;
        }

        var random = new System.Random(CreateSeed(levelSeed, sectionIndex));
        float density = Mathf.Clamp01(_placementSettings.GetSpawnDensity(sectionType, level));
        int targetCount = Mathf.Clamp(Mathf.CeilToInt(points.Count * density), 0, points.Count);

        for (int i = points.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (points[i], points[j]) = (points[j], points[i]);
        }

        int placedCount = 0;

        for (int i = 0; i < targetCount; i++)
        {
            Transform point = points[i];

            Obstacle obstacle = _obstacleFactory.Create(
                level,
                obstacleSection,
                point.position,
                point.rotation,
                sectionRoot,
                incomeMultiplier,
                random
            );

            if (obstacle != null)
            {
                placedCount++;
            }
        }

        return placedCount;
    }

    public void BeginRun()
    {
        _obstacleFactory.BeginRun();
    }

    private bool TryMapSection(TrackSectionType type, out ObstacleSection section)
    {
        switch (type)
        {
            case TrackSectionType.Safe:
                section = ObstacleSection.Safe;
                return true;

            case TrackSectionType.Challenge:
                section = ObstacleSection.Challenge;
                return true;

            default:
                section = default;
                return false;
        }
    }

    private static int CreateSeed(int seed, int sectionIndex)
    {
        return unchecked(seed * 397 ^ (sectionIndex + 1) * 7919);
    }
}
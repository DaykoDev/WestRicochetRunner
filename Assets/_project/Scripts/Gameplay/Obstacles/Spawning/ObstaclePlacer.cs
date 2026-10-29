using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObstacleFactory _obstacleFactory;
    [SerializeField] private DifficultyTiersSettings _difficultyTiersSettings;

    [Header("Placement")]
    [SerializeField, Min(1)] private int _maxPlacementAttempts = 200;

    public int PlaceObstacles(
        Transform sectionRoot,
        int level,
        TrackSectionType sectionType,
        float incomeMultiplier,
        int levelSeed,
        int sectionIndex)
    {
        if (sectionRoot == null || _obstacleFactory == null || _difficultyTiersSettings == null)
        {
            Debug.LogError("ObstaclePlacer references are missing.", this);

            return 0;
        }

        if (TryMapSection(sectionType, out ObstacleSection obstacleSection) == false)
        {
            return 0;
        }

        ObstacleGrid grid = sectionRoot.GetComponent<ObstacleGrid>();

        if (grid == null)
        {
            Debug.LogWarning( $"No ObstacleGrid found on section {sectionRoot.name}.", sectionRoot);

            return 0;
        }

        grid.EnsureInitialized();

        int allowedCellCount = grid.AllowedCellCount;

        if (allowedCellCount == 0)
        {
            return 0;
        }

        List<ObstacleFactory.ObstacleEntry> availableEntries = GetAvailableEntries(level, obstacleSection, grid.Size);

        if (availableEntries.Count == 0)
        {
            return 0;
        }

        System.Random random = new System.Random(CreateSeed(levelSeed, sectionIndex));

        float density = Mathf.Clamp01(_difficultyTiersSettings.GetSpawnDensity(sectionType, level));

        int targetCells = Mathf.CeilToInt(allowedCellCount * density);

        ObstaclePlacementGrid placementGrid = new ObstaclePlacementGrid(grid);

        int occupiedCells = 0;
        int placedCount = 0;

        for (int attempt = 0; attempt < _maxPlacementAttempts && occupiedCells < targetCells; attempt++)
        {
            ObstacleFactory.ObstacleEntry entry = GetRandomEntry(availableEntries, random);

            Obstacle prefab = entry.Prefab;
            Vector2Int footprintSize = prefab.GridSize;

            Vector2Int anchor = new Vector2Int(
                random.Next(0, grid.Size.x - footprintSize.x + 1),
                random.Next(0, grid.Size.y - footprintSize.y + 1));

            if (placementGrid.CanPlace(anchor, footprintSize) == false)
            {
                continue;
            }

            Vector3 position = grid.GetWorldCenter(anchor, footprintSize);

            Quaternion rotation = grid.transform.rotation;

            Obstacle obstacle = _obstacleFactory.Create(
                prefab,
                level,
                obstacleSection,
                position,
                rotation,
                sectionRoot,
                incomeMultiplier);

            if (obstacle == null)
            {
                continue;
            }

            placementGrid.Occupy(anchor, footprintSize);
            occupiedCells += footprintSize.x * footprintSize.y;
            placedCount++;
        }

        if (occupiedCells < targetCells)
        {
            Debug.LogWarning(
                $"Obstacle placement incomplete in '{sectionRoot.name}'. " +
                $"Target: {targetCells} cells, placed: {occupiedCells} cells, " +
                $"obstacles: {placedCount}, attempts: {_maxPlacementAttempts}.",
                sectionRoot);
        }

        return placedCount;
    }

    public void BeginRun()
    {
        _obstacleFactory.BeginRun();
    }

    public void ReleaseObstacles()
    {
        _obstacleFactory.ReleaseAll();
    }

    private List<ObstacleFactory.ObstacleEntry> GetAvailableEntries(int level, ObstacleSection section, Vector2Int gridSize)
    {
        List<ObstacleFactory.ObstacleEntry> result = new();

        foreach (ObstacleFactory.ObstacleEntry entry in _obstacleFactory.Entries)
        {
            if (entry == null || entry.Prefab == null || entry.Weight <= 0 || entry.IsAvailable(level, section) == false)
            {
                continue;
            }

            Vector2Int size = entry.Prefab.GridSize;

            if (size.x < 1 || size.y < 1 || size.x > gridSize.x || size.y > gridSize.y)
            {
                continue;
            }

            result.Add(entry);
        }

        return result;
    }

    private static ObstacleFactory.ObstacleEntry GetRandomEntry(List<ObstacleFactory.ObstacleEntry> entries, System.Random random)
    {
        int totalWeight = 0;

        foreach (ObstacleFactory.ObstacleEntry entry in entries)
        {
            totalWeight += entry.Weight;
        }

        int roll = random.Next(totalWeight);

        foreach (ObstacleFactory.ObstacleEntry entry in entries)
        {
            roll -= entry.Weight;

            if (roll < 0)
            {
                return entry;
            }
        }

        return entries[entries.Count - 1];
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
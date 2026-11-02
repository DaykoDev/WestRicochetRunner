using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ObstacleFactory _obstacleFactory;
    [SerializeField] private DifficultyTiersSettings _difficultyTiersSettings;

    [Header("Placement")]
    [SerializeField, Min(1)] private int _maxPlacementAttempts = 200;

    [Header("Final Bonus")]
    [SerializeField] private Obstacle _finalCratePrefab;
    [SerializeField] private Obstacle _finalChestPrefab;
    [SerializeField, Min(1)] private int _finalRowCount = 5;
    [SerializeField, Min(0)] private int _finalRowGap = 1;

    [SerializeField]
    private float[] _finalRowHealthMultipliers =
    {
        1f,
        1.5f,
        2.2f,
        3.2f,
        4.5f,
        6f
    };

    [SerializeField, Min(0.01f)] private float _finalChestHealthMultiplier = 10f;

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

        System.Random random = new(CreateSeed(levelSeed, sectionIndex));

        //bool fillAllCells = level == 1 || level % 5 == 0;
        bool fillAllCells = level == 1;

        float density = Mathf.Clamp01(_difficultyTiersSettings.GetSpawnDensity(sectionType, level));

        int targetCells = fillAllCells ? allowedCellCount : Mathf.CeilToInt(allowedCellCount * density);

        ObstaclePlacementGrid placementGrid = new(grid);

        int occupiedCells = 0;
        int placedCount = 0;

        for (int attempt = 0; attempt < _maxPlacementAttempts && occupiedCells < targetCells; attempt++)
        {
            ObstacleFactory.ObstacleEntry entry = GetRandomEntry(availableEntries, random);

            Obstacle prefab = entry.Prefab;
            Vector2Int footprintSize = prefab.GridSize;

            Vector2Int anchor;

            if (fillAllCells)
            {
                if (TryFindAvailableAnchor(
                        grid,
                        placementGrid,
                        footprintSize,
                        out anchor) == false)
                {
                    continue;
                }
            }
            else
            {
                anchor = new Vector2Int(
                    random.Next(0, grid.Size.x - footprintSize.x + 1),
                    random.Next(0, grid.Size.y - footprintSize.y + 1));
            }

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

    public Obstacle PlaceFinalBonus(
        Transform sectionRoot,
        int level,
        float incomeMultiplier)
    {
        if (sectionRoot == null || _obstacleFactory == null)
        {
            Debug.LogError("Final bonus references are missing.", this);
            return null;
        }

        ObstacleGrid grid = sectionRoot.GetComponent<ObstacleGrid>();

        if (grid == null || _finalCratePrefab == null || _finalChestPrefab == null)
        {
            Debug.LogError($"Final bonus section '{sectionRoot.name}' is not configured.", sectionRoot);

            return null;
        }

        grid.EnsureInitialized();

        Obstacle cratePrefab = _finalCratePrefab;
        Obstacle chestPrefab = _finalChestPrefab;

        Vector2Int crateSize = cratePrefab.GridSize;
        Vector2Int chestSize = chestPrefab.GridSize;

        if (grid.Size.x % crateSize.x != 0)
        {
            Debug.LogError("Crate width must divide the final bonus grid width.", sectionRoot);

            return null;
        }

        int rowStride = crateSize.y + _finalRowGap;
        int chestRow = _finalRowCount * rowStride;

        if (chestRow + chestSize.y > grid.Size.y)
        {
            Debug.LogError("Final bonus grid does not have enough rows.", sectionRoot);

            return null;
        }

        ObstaclePlacementGrid placementGrid = new(grid);

        Quaternion rotation = grid.transform.rotation;

        for (int row = 0; row < _finalRowCount; row++)
        {
            float healthMultiplier = GetFinalRowHealthMultiplier(row);

            int z = row * rowStride;

            for (int x = 0; x < grid.Size.x; x += crateSize.x)
            {
                Vector2Int anchor = new(x, z);

                if (placementGrid.CanPlace(anchor, crateSize) == false)
                {
                    Debug.LogError($"Cannot place final bonus crate at {anchor}.", sectionRoot);

                    return null;
                }

                Vector3 position = grid.GetWorldCenter(anchor, crateSize);

                Obstacle obstacle = _obstacleFactory.Create(
                    cratePrefab,
                    level,
                    ObstacleSection.Bonus,
                    position,
                    rotation,
                    sectionRoot,
                    incomeMultiplier,
                    healthMultiplier);

                if (obstacle == null)
                {
                    return null;
                }

                placementGrid.Occupy(anchor, crateSize);
            }
        }

        Vector2Int chestAnchor = new((grid.Size.x - chestSize.x) / 2, chestRow);

        if (placementGrid.CanPlace(chestAnchor, chestSize) == false)
        {
            Debug.LogError($"Cannot place final bonus chest at {chestAnchor}.",
                sectionRoot);

            return null;
        }

        Vector3 chestPosition =
            grid.GetWorldCenter(chestAnchor, chestSize);

        return _obstacleFactory.Create(
            chestPrefab,
            level,
            ObstacleSection.Bonus,
            chestPosition,
            rotation,
            sectionRoot,
            incomeMultiplier,
            _finalChestHealthMultiplier);
    }

    public void BeginRun()
    {
        _obstacleFactory.BeginRun();
    }

    public void ReleaseObstacles()
    {
        _obstacleFactory.ReleaseAll();
    }

    private float GetFinalRowHealthMultiplier(int row)
    {
        if (_finalRowHealthMultipliers == null || _finalRowHealthMultipliers.Length == 0)
        {
            return 1f + row;
        }

        int index = Mathf.Clamp(row, 0, _finalRowHealthMultipliers.Length - 1);

        return Mathf.Max(0.01f, _finalRowHealthMultipliers[index]);
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

    private static bool TryFindAvailableAnchor(
        ObstacleGrid grid,
        ObstaclePlacementGrid placementGrid,
        Vector2Int footprintSize,
        out Vector2Int anchor)
    {
        for (int z = 0; z < grid.Size.y; z++)
        {
            for (int x = 0; x < grid.Size.x; x++)
            {
                Vector2Int candidate = new Vector2Int(x, z);

                if (placementGrid.CanPlace(candidate, footprintSize))
                {
                    anchor = candidate;
                    return true;
                }
            }
        }

        anchor = default;
        return false;
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
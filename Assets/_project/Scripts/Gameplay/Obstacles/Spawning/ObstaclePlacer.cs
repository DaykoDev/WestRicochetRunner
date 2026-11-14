using System.Collections.Generic;
using UnityEngine;
using WestRicochet.Gameplay.Level.Generation;
using WestRicochet.Gameplay.Level.Sections;
using WestRicochet.Gameplay.Obstacles.Calculations;
using WestRicochet.Gameplay.Obstacles.Core;
using WestRicochet.Gameplay.Rewards;
using WestRicochet.Gameplay.Run;

namespace WestRicochet.Gameplay.Obstacles.Spawning
{
    public class ObstaclePlacer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ObstacleFactory _obstacleFactory;
        [SerializeField] private FinalBonusObstaclePlacer _finalBonusObstaclePlacer;
        [SerializeField] private DifficultyTiersSettings _difficultyTiersSettings;
        [SerializeField] private BonusLevelSettings _bonusLevelSettings;
        [SerializeField] private LevelEconomySettings _levelEconomySettings;
        [SerializeField] private IncomeCalculator _incomeCalculator;

        [Header("Placement")]
        [SerializeField, Min(1)] private int _maxPlacementAttempts = 200;

        private readonly List<Obstacle> _regularObstacles = new();

        public int PlaceObstacles(
            Transform sectionRoot,
            int level,
            TrackSectionType sectionType,
            int levelSeed,
            int sectionIndex)
        {
            if (sectionRoot == null ||
                _obstacleFactory == null ||
                _difficultyTiersSettings == null ||
                _bonusLevelSettings == null)
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
                Debug.LogWarning($"No ObstacleGrid found on section {sectionRoot.name}.", sectionRoot);

                return 0;
            }

            grid.EnsureInitialized();

            int allowedCellCount = grid.AllowedCellCount;

            if (allowedCellCount == 0)
            {
                return 0;
            }

            List<ObstacleEntry> availableEntries = GetAvailableEntries(level, obstacleSection, grid.Size);

            if (availableEntries.Count == 0)
            {
                return 0;
            }

            System.Random random = new(SectionSeedUtility.Create(levelSeed, sectionIndex));

            bool fillAllCells = _bonusLevelSettings.ShouldFillAllCells(level);

            float density = Mathf.Clamp01(_difficultyTiersSettings.GetSpawnDensity(sectionType, level));

            int targetCells = fillAllCells ? allowedCellCount : Mathf.CeilToInt(allowedCellCount * density);

            ObstaclePlacementGrid placementGrid = new(grid);

            int occupiedCells = 0;
            int placedCount = 0;

            for (int attempt = 0; attempt < _maxPlacementAttempts && occupiedCells < targetCells; attempt++)
            {
                ObstacleEntry entry = GetRandomEntry(availableEntries, random);

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
                    sectionRoot);

                if (obstacle == null)
                {
                    continue;
                }

                _regularObstacles.Add(obstacle);
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

        public bool DistributeRewards(int level, float incomeMultiplier)
        {
            if (_levelEconomySettings == null ||
                _incomeCalculator == null ||
                _finalBonusObstaclePlacer == null)
            {
                Debug.LogError(
                    "Obstacle reward settings are missing.",
                    this);

                return false;
            }

            _incomeCalculator.Distribute(
                _regularObstacles,
                _levelEconomySettings.GetRegularObstacleBudget(level),
                incomeMultiplier);

            _incomeCalculator.Distribute(
                _finalBonusObstaclePlacer.PlacedObstacles,
                _levelEconomySettings.GetFinalBonusBudget(level),
                incomeMultiplier);

            return true;
        }

        public void BeginRun()
        {
            _regularObstacles.Clear();

            if (_finalBonusObstaclePlacer != null)
            {
                _finalBonusObstaclePlacer.Clear();
            }
        }

        public void ReleaseObstacles()
        {
            _regularObstacles.Clear();

            if (_finalBonusObstaclePlacer != null)
            {
                _finalBonusObstaclePlacer.Clear();
            }

            if (_obstacleFactory != null)
            {
                _obstacleFactory.ReleaseAll();
            }
        }

        private List<ObstacleEntry> GetAvailableEntries(int level, ObstacleSection section, Vector2Int gridSize)
        {
            List<ObstacleEntry> result = new();

            foreach (ObstacleEntry entry in _obstacleFactory.Entries)
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

        private static ObstacleEntry GetRandomEntry(List<ObstacleEntry> entries, System.Random random)
        {
            int totalWeight = 0;

            foreach (ObstacleEntry entry in entries)
            {
                totalWeight += entry.Weight;
            }

            int roll = random.Next(totalWeight);

            foreach (ObstacleEntry entry in entries)
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
                    Vector2Int candidate = new(x, z);

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
    }
}
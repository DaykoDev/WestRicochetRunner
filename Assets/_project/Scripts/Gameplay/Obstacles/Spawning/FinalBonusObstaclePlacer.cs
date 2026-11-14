using System.Collections.Generic;
using UnityEngine;
using WestRicochet.Gameplay.Obstacles.Core;

namespace WestRicochet.Gameplay.Obstacles.Spawning
{
    public sealed class FinalBonusObstaclePlacer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ObstacleFactory _obstacleFactory;

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

        private readonly List<Obstacle> _placedObstacles = new();

        public IReadOnlyList<Obstacle> PlacedObstacles => _placedObstacles;

        public Obstacle Place(Transform sectionRoot, int level)
        {
            if (sectionRoot == null || _obstacleFactory == null)
            {
                Debug.LogError("Final bonus references are missing.", this);

                return null;
            }

            ObstacleGrid grid = sectionRoot.GetComponent<ObstacleGrid>();

            if (grid == null ||
                _finalCratePrefab == null ||
                _finalChestPrefab == null)
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
                float healthMultiplier =
                    GetFinalRowHealthMultiplier(row);

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
                        healthMultiplier);

                    if (obstacle == null)
                    {
                        return null;
                    }

                    _placedObstacles.Add(obstacle);
                    placementGrid.Occupy(anchor, crateSize);
                }
            }

            Vector2Int chestAnchor = new((grid.Size.x - chestSize.x) / 2, chestRow);

            if (placementGrid.CanPlace(chestAnchor, chestSize) == false)
            {
                Debug.LogError($"Cannot place final bonus chest at {chestAnchor}.", sectionRoot);

                return null;
            }

            Vector3 chestPosition = grid.GetWorldCenter(chestAnchor, chestSize);

            return _obstacleFactory.Create(
                chestPrefab,
                level,
                ObstacleSection.Bonus,
                chestPosition,
                rotation,
                sectionRoot,
                _finalChestHealthMultiplier);
        }

        public void Clear()
        {
            _placedObstacles.Clear();
        }

        private float GetFinalRowHealthMultiplier(int row)
        {
            if (_finalRowHealthMultipliers == null ||
                _finalRowHealthMultipliers.Length == 0)
            {
                return 1f + row;
            }

            int index = Mathf.Clamp(row, 0, _finalRowHealthMultipliers.Length - 1);

            return Mathf.Max(0.01f, _finalRowHealthMultipliers[index]);
        }
    }
}
using System;
using UnityEngine;

namespace WestRicochet.Gameplay.Obstacles.Spawning
{
    [DisallowMultipleComponent]
    public sealed class ObstacleGrid : MonoBehaviour
    {
        private const float MinimumCellSize = 0.5f;
        private const float CellVisualScale = 0.96f;
        private const float CellVisualHeight = 0.02f;
        private const float CellVisualCenterOffset = 0.02f;
        private const float LineVisualOffset = 0.03f;

        [Header("Floor")]
        [SerializeField] private MeshFilter _floor;
        [SerializeField] private Vector3 _localOffset;

        [Header("Grid")]
        [SerializeField, Min(MinimumCellSize)] private float _cellSize = 1f;
        [SerializeField, HideInInspector] private bool[] _allowedCells;

        [Header("Scene View")]
        [SerializeField] private bool _showGrid = true;
        [SerializeField] private Color _allowedColor = new(0.2f, 0.9f, 0.2f, 0.35f);
        [SerializeField] private Color _blockedColor = new(0.9f, 0.2f, 0.2f, 0.12f);
        [SerializeField] private Color _lineColor = new(1f, 1f, 1f, 0.5f);

        [SerializeField, HideInInspector]
        private Vector2Int _size;

        private Vector3 _gridStart;

        public Vector2Int Size => _size;

        public int AllowedCellCount
        {
            get
            {
                if (_size.x <= 0 || _size.y <= 0 ||
                    _allowedCells == null ||
                    _allowedCells.Length != _size.x * _size.y)
                {
                    return 0;
                }

                int count = 0;

                for (int i = 0; i < _allowedCells.Length; i++)
                {
                    if (_allowedCells[i])
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void OnValidate()
        {
            _cellSize = Mathf.Max(MinimumCellSize, _cellSize);
            EnsureInitialized();
        }

        private void OnDrawGizmosSelected()
        {
            if (_showGrid == false || IsFloorAvailable() == false)
            {
                return;
            }

            EnsureInitialized();

            if (_size.x <= 0 || _size.y <= 0)
            {
                return;
            }

            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            DrawCells();
            DrawLines();

            Gizmos.matrix = previousMatrix;
        }

        public void EnsureInitialized()
        {
            _cellSize = Mathf.Max(MinimumCellSize, _cellSize);

            if (TryGetFloorBounds(out Bounds bounds) == false)
            {
                _size = Vector2Int.zero;
                _gridStart = Vector3.zero;
                return;
            }

            Vector2Int newSize = new(
                Mathf.Max(0, Mathf.FloorToInt(bounds.size.x / _cellSize)),
                Mathf.Max(0, Mathf.FloorToInt(bounds.size.z / _cellSize)));

            _gridStart = new Vector3(
                bounds.center.x - newSize.x * _cellSize * 0.5f,
                bounds.max.y,
                bounds.center.z - newSize.y * _cellSize * 0.5f) + _localOffset;

            int requiredLength = newSize.x * newSize.y;


            if (_size == newSize &&
                _allowedCells != null &&
                _allowedCells.Length == requiredLength)
            {
                return;
            }

            bool[] previousCells = _allowedCells;
            Vector2Int previousSize = _size;

            bool hasPreviousGrid =
                previousCells != null &&
                previousSize.x > 0 &&
                previousSize.y > 0 &&
                previousCells.Length == previousSize.x * previousSize.y;

            if (hasPreviousGrid == false &&
                previousCells != null &&
                previousCells.Length == requiredLength)
            {
                _size = newSize;
                return;
            }

            _size = newSize;
            _allowedCells = new bool[requiredLength];

            for (int i = 0; i < _allowedCells.Length; i++)
            {
                _allowedCells[i] = true;
            }

            if (hasPreviousGrid)
            {
                CopyPreviousCells(previousCells, previousSize);
            }
        }

        public bool IsAllowed(Vector2Int cell)
        {
            if (IsInside(cell) == false ||
                _allowedCells == null ||
                _allowedCells.Length != _size.x * _size.y)
            {
                return false;
            }


            return _allowedCells[GetCellIndex(cell)];
        }

        public Vector3 GetWorldCenter(Vector2Int anchor, Vector2Int footprintSize)
        {
            if (IsFloorAvailable() == false)
            {
                throw new InvalidOperationException($"{name}: не назначен пол с MeshFilter и Mesh.");
            }

            Vector3 localCenter = GetLocalCenter(anchor, footprintSize);

            return transform.TransformPoint(localCenter);
        }

        private bool IsInside(Vector2Int cell)
        {
            return cell.x >= 0 &&
                cell.y >= 0 &&
                cell.x < _size.x &&
                cell.y < _size.y;
        }

        private bool IsFloorAvailable()
        {
            return _floor != null && _floor.sharedMesh != null;
        }

        private bool TryGetFloorBounds(out Bounds bounds)
        {
            bounds = default;

            if (IsFloorAvailable() == false)
            {
                return false;
            }

            Bounds meshBounds = _floor.sharedMesh.bounds;

            Matrix4x4 floorToGrid =
                transform.worldToLocalMatrix *
                _floor.transform.localToWorldMatrix;

            Vector3 meshMin = meshBounds.min;
            Vector3 meshMax = meshBounds.max;

            Vector3 gridMin = Vector3.positiveInfinity;
            Vector3 gridMax = Vector3.negativeInfinity;

            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        Vector3 corner = new Vector3(
                        x == 0 ? meshMin.x : meshMax.x,
                        y == 0 ? meshMin.y : meshMax.y,
                        z == 0 ? meshMin.z : meshMax.z);

                        Vector3 gridCorner =
                        floorToGrid.MultiplyPoint3x4(corner);

                        gridMin = Vector3.Min(gridMin, gridCorner);
                        gridMax = Vector3.Max(gridMax, gridCorner);
                    }
                }
            }

            bounds = new Bounds();
            bounds.SetMinMax(gridMin, gridMax);
            return true;
        }

        private Vector3 GetLocalCenter(Vector2Int anchor, Vector2Int footprintSize)
        {
            return _gridStart + new Vector3(
                (anchor.x + footprintSize.x * 0.5f) * _cellSize,
                0f,
                (anchor.y + footprintSize.y * 0.5f) * _cellSize);
        }

        private int GetCellIndex(Vector2Int cell)
        {
            return cell.y * _size.x + cell.x;
        }

        private void CopyPreviousCells(
            bool[] previousCells,
            Vector2Int previousSize)
        {
            if (previousCells == null || previousCells.Length == 0)
            {
                return;
            }

            int copyWidth = Mathf.Min(previousSize.x, _size.x);
            int copyLength = Mathf.Min(previousSize.y, _size.y);

            for (int z = 0; z < copyLength; z++)
            {
                for (int x = 0; x < copyWidth; x++)
                {
                    int previousIndex = z * previousSize.x + x;

                    if (previousIndex >= previousCells.Length)
                    {
                        continue;
                    }

                    _allowedCells[z * _size.x + x] = previousCells[previousIndex];
                }
            }
        }

        private void DrawCells()
        {
            Vector3 cellScale = new(
                _cellSize * CellVisualScale,
                CellVisualHeight,
                _cellSize * CellVisualScale);

            for (int z = 0; z < _size.y; z++)
            {
                for (int x = 0; x < _size.x; x++)
                {
                    Vector2Int cell = new(x, z);

                    Gizmos.color = IsAllowed(cell)
                        ? _allowedColor
                        : _blockedColor;

                    Vector3 center = GetLocalCenter(cell, Vector2Int.one);
                    center.y += CellVisualCenterOffset;

                    Gizmos.DrawCube(center, cellScale);
                }
            }
        }

        private void DrawLines()
        {
            Gizmos.color = _lineColor;

            Vector3 start = _gridStart + Vector3.up * LineVisualOffset;
            float width = _size.x * _cellSize;
            float length = _size.y * _cellSize;

            for (int x = 0; x <= _size.x; x++)
            {
                Vector3 lineStart = start + Vector3.right * (x * _cellSize);
                Vector3 lineEnd = lineStart + Vector3.forward * length;
                Gizmos.DrawLine(lineStart, lineEnd);
            }

            for (int z = 0; z <= _size.y; z++)
            {
                Vector3 lineStart = start + Vector3.forward * (z * _cellSize);
                Vector3 lineEnd = lineStart + Vector3.right * width;
                Gizmos.DrawLine(lineStart, lineEnd);
            }
        }
    }
}
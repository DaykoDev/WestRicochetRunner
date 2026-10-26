using UnityEngine;

public sealed class ObstaclePlacementGrid
{
    private readonly ObstacleGrid _grid;
    private readonly Vector2Int _size;
    private readonly bool[] _occupiedCells;

    public ObstaclePlacementGrid(ObstacleGrid grid)
    {
        if (grid == null)
        {
            throw new System.ArgumentNullException(nameof(grid));
        }

        _grid = grid;
        _size = grid.Size;

        if (_size.x <= 0 || _size.y <= 0)
        {
            throw new System.ArgumentException("ObstacleGrid is not initialized.", nameof(grid));
        }

        _occupiedCells = new bool[_size.x * _size.y];
    }


    public bool CanPlace(Vector2Int anchor, Vector2Int footprintSize)
    {
        if (footprintSize.x < 1 || footprintSize.y < 1)
        {
            return false;
        }

        if (anchor.x < 0 || anchor.y < 0 ||
            anchor.x + footprintSize.x > _size.x ||
            anchor.y + footprintSize.y > _size.y)
        {
            return false;
        }

        for (int z = 0; z < footprintSize.y; z++)
        {
            for (int x = 0; x < footprintSize.x; x++)
            {
                Vector2Int cell = new Vector2Int(anchor.x + x, anchor.y + z);

                if (_grid.IsAllowed(cell) == false)
                {
                    return false;
                }

                if (_occupiedCells[GetIndex(cell)])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Occupy(Vector2Int anchor, Vector2Int footprintSize)
    {
        for (int z = 0; z < footprintSize.y; z++)
        {
            for (int x = 0; x < footprintSize.x; x++)
            {
                Vector2Int cell = new Vector2Int(anchor.x + x, anchor.y + z);

                _occupiedCells[GetIndex(cell)] = true;
            }
        }
    }

    private int GetIndex(Vector2Int cell)
    {
        return cell.y * _size.x + cell.x;
    }
}
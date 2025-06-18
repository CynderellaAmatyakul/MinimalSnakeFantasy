using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    public float cellSize = 1f;
    public GameObject cellPrefab; // Optional: for visualization

    private GridCell[,] grid;

    void Awake()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        grid = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 worldPos = new Vector3(x * cellSize, 0f, z * cellSize);
                grid[x, z] = new GridCell(x, z, worldPos);

                // Optional: Visual cell prefab
                if (cellPrefab != null)
                {
                    Instantiate(cellPrefab, worldPos, Quaternion.identity, transform);
                }
            }
        }
    }

    public Vector3 GridToWorld(int x, int z)
    {
        return new Vector3(x * cellSize, 0f, z * cellSize);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int z = Mathf.FloorToInt(worldPos.z / cellSize);
        return new Vector2Int(x, z);
    }

    public bool IsWithinBounds(int x, int z)
    {
        return x >= 0 && x < width && z >= 0 && z < height;
    }

    public bool IsCellOccupied(int x, int z)
    {
        if (!IsWithinBounds(x, z)) return true;
        return grid[x, z].contentType != CellContentType.None;
    }

    public GridCell GetCell(int x, int z)
    {
        if (IsWithinBounds(x, z))
            return grid[x, z];
        return null;
    }

    public List<GridCell> GetAllCells()
    {
        List<GridCell> list = new List<GridCell>();
        foreach (var cell in grid)
            list.Add(cell);
        return list;
    }

    public List<GridCell> GetNeighbors(GridCell cell)
    {
        List<GridCell> neighbors = new List<GridCell>();
        Vector2Int[] directions = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            int nx = cell.x + dir.x;
            int nz = cell.z + dir.y;
            if (IsWithinBounds(nx, nz))
            {
                neighbors.Add(grid[nx, nz]);
            }
        }

        return neighbors;
    }

    public CellContentType GetCellContent(int x, int z)
    {
        if (!IsWithinBounds(x, z)) return CellContentType.Obstacle; // treat out of bounds as wall
        return grid[x, z].contentType;
    }

    public void SetCellContent(int x, int z, CellContentType type)
    {
        if (IsWithinBounds(x, z))
        {
            grid[x, z].contentType = type;
        }
    }
}

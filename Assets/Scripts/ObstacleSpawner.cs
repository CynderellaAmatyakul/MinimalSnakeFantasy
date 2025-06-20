using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ObstacleConfig
{
    public GameObject prefab;
    public Vector2Int size;
}

public class ObstacleSpawner : MonoBehaviour
{
    public GridManager gridManager;
    public List<ObstacleConfig> obstacleTypes;
    public float obstacleHeightOffset = 0.5f;
    public int obstacleCount = 10;

    void Start()
    {
        SpawnObstacles();
    }

    void SpawnObstacles()
    {
        int tries = 0;
        int placed = 0;

        while (placed < obstacleCount && tries < 1000)
        {
            tries++;

            ObstacleConfig config = obstacleTypes[Random.Range(0, obstacleTypes.Count)];
            Vector2Int size = config.size;

            int x = Random.Range(0, gridManager.width - size.x + 1);
            int z = Random.Range(0, gridManager.height - size.y + 1);

            bool canPlace = true;
            for (int dx = 0; dx < size.x; dx++)
            {
                for (int dz = 0; dz < size.y; dz++)
                {
                    if (gridManager.GetCellContent(x + dx, z + dz) != CellContentType.None)
                    {
                        canPlace = false;
                        break;
                    }
                }
                if (!canPlace) break;
            }

            if (!canPlace) continue;

            Vector3 centerPos = gridManager.GridToWorld(x, z) + new Vector3((size.x - 1) * 0.5f, 0f, (size.y - 1) * 0.5f);
            Instantiate(config.prefab, centerPos + Vector3.up * obstacleHeightOffset, Quaternion.identity);

            for (int dx = 0; dx < size.x; dx++)
            {
                for (int dz = 0; dz < size.y; dz++)
                {
                    gridManager.SetCellContent(x + dx, z + dz, CellContentType.Obstacle);
                }
            }

            placed++;
        }
    }
}
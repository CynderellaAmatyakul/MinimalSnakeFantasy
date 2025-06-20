using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GridManager gridManager;
    public List<GameObject> itemPrefabs;
    public float spawnInterval = 30f;

    void Start() => StartCoroutine(SpawnLoop());

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            List<GridCell> freeCells = gridManager.GetAllCells().FindAll(c => c.contentType == CellContentType.None);
            if (freeCells.Count == 0) continue;

            GridCell cell = freeCells[Random.Range(0, freeCells.Count)];
            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Count)];

            Instantiate(prefab, cell.worldPosition + Vector3.up * 0.5f, Quaternion.identity);
            gridManager.SetCellContent(cell.x, cell.z, CellContentType.None); // Optional
        }
    }
}
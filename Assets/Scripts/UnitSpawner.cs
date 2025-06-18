using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    public GridManager gridManager;

    [Header("Spawn Settings")]
    public GameObject collectableHeroPrefab;
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public int baseHP = 5;
    public int baseAttack = 2;
    public int baseDefense = 1;

    private float elapsedTime = 0f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            elapsedTime += spawnInterval;

            TrySpawnUnit(collectableHeroPrefab, isEnemy: false);
            TrySpawnUnit(enemyPrefab, isEnemy: true);
        }
    }

    void TrySpawnUnit(GameObject prefab, bool isEnemy)
    {
        //Find a null grid
        List<GridCell> freeCells = gridManager.GetAllCells().FindAll(c => c.contentType == CellContentType.None);

        if (freeCells.Count == 0) return;

        GridCell cell = freeCells[Random.Range(0, freeCells.Count)];
        Vector3 spawnPos = cell.worldPosition;

        GameObject unitGO = Instantiate(prefab, spawnPos, Quaternion.identity);
        UnitStats stats = unitGO.GetComponent<UnitStats>();

        if (stats != null)
        {
            //Scaling by time
            int scale = Mathf.FloorToInt(elapsedTime / 10f); //Every 10 sec.
            int hp = baseHP + Random.Range(0, scale + 3);
            int atk = baseAttack + Random.Range(0, scale + 2);
            int def = baseDefense + Random.Range(0, scale + 2);
            UnitClass randomClass = (UnitClass)Random.Range(0, System.Enum.GetValues(typeof(UnitClass)).Length);

            stats.Setup(randomClass, hp, atk, def);
            stats.unitName = isEnemy ? "Enemy_" + Time.frameCount : "Hero_" + Time.frameCount;
        }

        gridManager.SetCellContent(cell.x, cell.z, isEnemy ? CellContentType.Monster : CellContentType.CollectableHero);
    }
}

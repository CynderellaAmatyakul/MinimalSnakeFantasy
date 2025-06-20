using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    public GridManager gridManager;

    [Header("Spawn Settings")]
    public GameObject collectableHeroPrefab;
    public GameObject enemyPrefab;
    public float heroSpawnInterval = 20f;
    public float enemySpawnInterval = 10f;
    public int baseHP = 5;
    public int baseAttack = 2;
    public int baseDefense = 1;

    [Header("UI")]
    public GameObject hpUIPrefab;

    private float elapsedTime = 0f;

    void Start()
    {
        StartCoroutine(HeroSpawnLoop());
        StartCoroutine(EnemySpawnLoop());
    }

    IEnumerator HeroSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(heroSpawnInterval);
            elapsedTime += heroSpawnInterval;
            TrySpawnUnit(collectableHeroPrefab, isEnemy: false);
        }
    }

    IEnumerator EnemySpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpawnInterval);
            elapsedTime += enemySpawnInterval;
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
            int scale = Mathf.FloorToInt(elapsedTime / 30f); //Time scaling per sec.
            int hp = baseHP + Random.Range(0, scale + 3);
            int atk = baseAttack + Random.Range(0, scale + 2);
            int def = baseDefense + Random.Range(0, scale + 2);
            UnitClass randomClass = (UnitClass)Random.Range(0, System.Enum.GetValues(typeof(UnitClass)).Length);

            stats.Setup(randomClass, hp, atk, def);
            stats.unitName = isEnemy ? "Enemy_" + Time.frameCount : "Hero_" + Time.frameCount;

            UnitHealthUI ui = unitGO.GetComponentInChildren<UnitHealthUI>();

            if (ui != null)
            {
                ui.followTarget = stats.healthBarAnchor;
            }
            else if (hpUIPrefab != null)
            {
                GameObject hpUI = Instantiate(hpUIPrefab);
                ui = hpUI.GetComponent<UnitHealthUI>();
                ui.followTarget = stats.healthBarAnchor;
                hpUI.transform.SetParent(unitGO.transform);
            }
        }

        if (isEnemy)
        {
            EnemyAI ai = unitGO.GetComponent<EnemyAI>();
            if (ai != null)
            {
                ai.gridManager = gridManager;
                ai.heroController = FindObjectOfType<HeroController>();
            }
        }

        gridManager.SetCellContent(cell.x, cell.z, isEnemy ? CellContentType.Monster : CellContentType.CollectableHero);
    }
}

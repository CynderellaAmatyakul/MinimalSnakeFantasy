using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitSpawner : MonoBehaviour
{
    public GridManager gridManager;

    [Header("Spawn Settings")]
    public float heroSpawnInterval = 20f;
    public float enemySpawnInterval = 10f;
    public int baseHP = 5;
    public int baseAttack = 2;
    public int baseDefense = 1;

    [Header("Hero & Enemy Prefabs")]
    public GameObject[] collectableHeroPrefabs;
    public GameObject[] enemyPrefabs;

    [Header("UI")]
    public GameObject hpUIPrefab;

    private int currentLevel = 1;
    private float timeSinceLastLevelUp = 0f;

    void Start()
    {
        StartCoroutine(HeroSpawnLoop());
        StartCoroutine(EnemySpawnLoop());
    }

    void Update()
    {
        timeSinceLastLevelUp += Time.deltaTime;
        if (timeSinceLastLevelUp >= 30f)
        {
            timeSinceLastLevelUp = 0f;
            currentLevel += 1;
            Debug.Log("Level Up! Current Level: " + currentLevel);
        }
    }

    IEnumerator HeroSpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(heroSpawnInterval);
            TrySpawnUnit(false);
        }
    }

    IEnumerator EnemySpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(enemySpawnInterval);
            TrySpawnUnit(true);
        }
    }

    void TrySpawnUnit(bool isEnemy)
    {
        //Find a null grid
        List<GridCell> freeCells = gridManager.GetAllCells().FindAll(c => c.contentType == CellContentType.None);

        if (freeCells.Count == 0) return;

        GridCell cell = freeCells[Random.Range(0, freeCells.Count)];
        Vector3 spawnPos = cell.worldPosition;

        GameObject[] prefabArray = isEnemy ? enemyPrefabs : collectableHeroPrefabs;
        GameObject selectedPrefab = prefabArray[Random.Range(0, prefabArray.Length)];

        GameObject unitGO = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

        UnitStats stats = unitGO.GetComponent<UnitStats>();

        if (stats != null)
        {
            if (stats != null)
            {
                int hp = baseHP + currentLevel + Random.Range(0, 3);
                int atk = baseAttack + Mathf.CeilToInt(currentLevel * 0.75f) + Random.Range(0, 2);
                int def = baseDefense + Mathf.FloorToInt(currentLevel * 0.5f) + Random.Range(0, 2);

                stats.Setup(stats.unitClass, currentLevel, hp, atk, def);

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
}

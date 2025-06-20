using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class HeroController : MonoBehaviour
{
    [Header("References")]
    public GridManager gridManager;
    public GameObject heroPrefab;
    public CinemachineVirtualCamera cam;
    public List<EnemyAI> enemies = new List<EnemyAI>();

    [Header("Settings")]
    public float moveDelay = 0.1f;

    public List<GameObject> heroChain = new List<GameObject>();
    private Vector2Int currentDirection = Vector2Int.right;
    private bool canMove = true;

    private void Start()
    {
        List<GridCell> freeCells = gridManager.GetAllCells().FindAll(c => c.contentType == CellContentType.None);

        if (freeCells.Count == 0)
        {
            Debug.LogWarning("No free cell found to spawn hero!");
            return;
        }

        GridCell spawnCell = freeCells[Random.Range(0, freeCells.Count)];
        Vector2Int startPos = new Vector2Int(spawnCell.x, spawnCell.z);

        GameObject hero = Instantiate(heroPrefab, gridManager.GridToWorld(startPos.x, startPos.y), Quaternion.identity);

        hero.GetComponent<HeroCollisionHandler>().ownerController = this;

        heroChain.Add(hero);

        gridManager.SetCellContent(startPos.x, startPos.y, CellContentType.HeroBody);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed || !canMove) return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input == Vector2.up && currentDirection != Vector2Int.down)
            SetDirection(Vector2Int.up);
        else if (input == Vector2.down && currentDirection != Vector2Int.up)
            SetDirection(Vector2Int.down);
        else if (input == Vector2.left && currentDirection != Vector2Int.right)
            SetDirection(Vector2Int.left);
        else if (input == Vector2.right && currentDirection != Vector2Int.left)
            SetDirection(Vector2Int.right);

        UpdateHeroTags();
    }

    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
            RotateLeft();
    }

    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.performed)
            RotateRight();
    }

    void SetDirection(Vector2Int direction)
    {
        if (!canMove) return;
        currentDirection = direction;
        StartCoroutine(Move());
    }

    public IEnumerator Move()
    {
        if (!canMove) yield break;
        canMove = false;

        //Normalize list
        heroChain.RemoveAll(h => h == null);

        if (heroChain.Count == 0)
        {
            Debug.LogWarning("No hero remaining.");
            yield break;
        }

        GameObject head = heroChain[0];
        if (head == null)
        {
            Debug.LogWarning("Head is null.");
            yield break;
        }

        Vector3 headPos = head.transform.position;
        Vector2Int headGrid = gridManager.WorldToGrid(headPos);
        Vector2Int nextGrid = headGrid + currentDirection;

        if (!gridManager.IsWithinBounds(nextGrid.x, nextGrid.y))
        {
            Debug.Log("Hit wall!");
            canMove = true;
            yield break;
        }

        // Move body
        for (int i = heroChain.Count - 1; i > 0; i--)
        {
            if (heroChain[i] != null && heroChain[i - 1] != null)
            {
                heroChain[i].transform.position = heroChain[i - 1].transform.position;
            }
        }

        // Move head
        Vector3 nextWorldPos = gridManager.GridToWorld(nextGrid.x, nextGrid.y);
        head.transform.position = nextWorldPos;

        // Face direction
        Vector3 facingDir = new Vector3(currentDirection.x, 0f, currentDirection.y);
        if (facingDir != Vector3.zero)
            head.transform.rotation = Quaternion.LookRotation(facingDir);

        yield return new WaitForSeconds(moveDelay);
        canMove = true;

        StartCoroutine(MoveEnemiesAfterDelay(0.05f));

        //foreach (EnemyAI enemy in enemies)
        //{
        //    if (enemy != null)
        //    {
        //        Vector2Int heroHeadGrid = gridManager.WorldToGrid(heroChain[0].transform.position);
        //        enemy.MoveOneStepToward(heroHeadGrid);
        //    }
        //}
    }


    void RotateLeft()
    {
        if (heroChain.Count < 2) return;

        GameObject head = heroChain[0];
        GameObject second = heroChain[1];

        Vector3 tempPos = head.transform.position;
        Quaternion tempRot = head.transform.rotation;

        head.transform.position = second.transform.position;
        head.transform.rotation = second.transform.rotation;

        second.transform.position = tempPos;
        second.transform.rotation = tempRot;

        heroChain[0] = second;
        heroChain[1] = head;

        UpdateHeroTags();
        UpdateCameraFollow();
    }

    void RotateRight()
    {
        if (heroChain.Count < 2) return;

        GameObject head = heroChain[0];
        GameObject last = heroChain[heroChain.Count - 1];

        Vector3 tempPos = head.transform.position;
        Quaternion tempRot = head.transform.rotation;

        head.transform.position = last.transform.position;
        head.transform.rotation = last.transform.rotation;

        last.transform.position = tempPos;
        last.transform.rotation = tempRot;

        heroChain[0] = last;
        heroChain[heroChain.Count - 1] = head;

        UpdateHeroTags();
        UpdateCameraFollow();
    }

    public void AddHeroToChain(UnitStats unitData)
    {
        GameObject tail = heroChain[heroChain.Count - 1];
        Vector3 tailPos = tail.transform.position;

        GameObject newHero = Instantiate(heroPrefab, tailPos, Quaternion.identity);

        // Apply data from the unit you picked up
        UnitStats newUnit = newHero.GetComponent<UnitStats>();
        newUnit.CopyFrom(unitData);

        newHero.tag = "HeroBody";

        newHero.GetComponent<HeroCollisionHandler>().ownerController = this;
        heroChain.Add(newHero);

        UpdateCameraFollow();

        Debug.Log("Added unit to chain. Chain length: " + heroChain.Count);
    }

    void UpdateHeroTags()
    {
        for (int i = 0; i < heroChain.Count; i++)
        {
            heroChain[i].tag = (i == 0) ? "Hero" : "HeroBody";
        }
    }

    public void RemoveFrontHero()
    {
        if (heroChain.Count == 0) return;

        GameObject dead = heroChain[0];
        heroChain.RemoveAt(0);
        if (dead != null) Destroy(dead);

        //Set new tag for head
        if (heroChain.Count > 0)
        {
            heroChain[0].tag = "Hero";
            for (int i = 1; i < heroChain.Count; i++)
                heroChain[i].tag = "HeroBody";
        }
        else
        {
            Debug.Log("Game Over!");
            // TODO: trigger UI/game reset
        }

        UpdateCameraFollow();
    }

    void UpdateCameraFollow()
    {
        if (heroChain.Count > 0 && cam != null)
            cam.Follow = heroChain[0].transform;
    }

    public void RegisterEnemy(EnemyAI enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyAI enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    IEnumerator MoveEnemiesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (heroChain.Count == 0) yield break;

        Vector2Int heroHeadGrid = gridManager.WorldToGrid(heroChain[0].transform.position);

        foreach (EnemyAI enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.MoveOneStepToward(heroHeadGrid);
            }
        }
    }
}

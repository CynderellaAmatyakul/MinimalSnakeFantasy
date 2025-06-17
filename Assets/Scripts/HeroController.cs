using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class HeroController : MonoBehaviour
{
    public GridManager gridManager;
    public float moveDelay = 0.1f;
    public GameObject heroPrefab;

    private HeroControls controls;
    private List<GameObject> heroChain = new List<GameObject>();
    private Vector2Int currentDirection = Vector2Int.right;
    private bool canMove = true;

    void Awake()
    {
        controls = new HeroControls();

        controls.Gameplay.Move.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            if (input == Vector2.up && currentDirection != Vector2Int.down)
                SetDirection(Vector2Int.up);
            else if (input == Vector2.down && currentDirection != Vector2Int.up)
                SetDirection(Vector2Int.down);
            else if (input == Vector2.left && currentDirection != Vector2Int.right)
                SetDirection(Vector2Int.left);
            else if (input == Vector2.right && currentDirection != Vector2Int.left)
                SetDirection(Vector2Int.right);
        };

        controls.Gameplay.RotateLeft.performed += _ => RotateLeft();
        controls.Gameplay.RotateRight.performed += _ => RotateRight();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
        Vector2Int startPos = new Vector2Int(5, 5);
        GameObject hero = Instantiate(heroPrefab, gridManager.GridToWorld(startPos.x, startPos.y), Quaternion.identity);
        heroChain.Add(hero);
    }

    void SetDirection(Vector2Int direction)
    {
        if (!canMove) return;
        currentDirection = direction;
        StartCoroutine(Move());
    }

    System.Collections.IEnumerator Move()
    {
        if (!canMove) yield break;
        canMove = false;

        Vector3 headPos = heroChain[0].transform.position;
        Vector2Int headGrid = gridManager.WorldToGrid(headPos);
        Vector2Int nextGrid = headGrid + currentDirection;

        if (!gridManager.IsWithinBounds(nextGrid.x, nextGrid.y))
        {
            Debug.Log("Hit wall!");
            canMove = true;
            yield break;
        }

        for (int i = heroChain.Count - 1; i > 0; i--)
        {
            heroChain[i].transform.position = heroChain[i - 1].transform.position;
        }

        Vector3 nextWorldPos = gridManager.GridToWorld(nextGrid.x, nextGrid.y);
        heroChain[0].transform.position = nextWorldPos;

        Vector3 facingDir = new Vector3(currentDirection.x, 0f, currentDirection.y);
        if (facingDir != Vector3.zero)
            heroChain[0].transform.rotation = Quaternion.LookRotation(facingDir);

        yield return new WaitForSeconds(moveDelay);
        canMove = true;
    }


    void RotateLeft()
    {
        if (heroChain.Count < 2) return;
        GameObject first = heroChain[0];
        GameObject second = heroChain[1];

        heroChain.RemoveAt(1);
        heroChain.Insert(0, second);
        heroChain.Add(first);
    }

    void RotateRight()
    {
        if (heroChain.Count < 2) return;
        GameObject first = heroChain[0];
        GameObject last = heroChain[heroChain.Count - 1];

        heroChain.RemoveAt(heroChain.Count - 1);
        heroChain.Insert(0, last);
        heroChain.Insert(1, first);
    }
}

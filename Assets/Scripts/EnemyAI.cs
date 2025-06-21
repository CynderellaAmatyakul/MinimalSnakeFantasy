using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public GridManager gridManager;
    public HeroController heroController;

    private void Start()
    {
        if (heroController != null)
            heroController.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        if (heroController != null)
            heroController.UnregisterEnemy(this);
    }

    public void MoveOneStepToward(Vector2Int targetGrid)
    {
        Vector2Int start = gridManager.WorldToGrid(transform.position);
        List<Vector2Int> path = FindPath(start, targetGrid);

        if (path != null && path.Count > 1)
        {
            Vector2Int nextStep = path[1];
            CellContentType nextContent = gridManager.GetCellContent(nextStep.x, nextStep.y);

            if (nextContent != CellContentType.None && nextContent != CellContentType.CollectableHero)
            {
                Debug.Log("Blocked! Enemy won't move");
                return;
            }

            gridManager.SetCellContent(nextStep.x, nextStep.y, CellContentType.Monster);

            Vector3 nextWorld = gridManager.GridToWorld(nextStep.x, nextStep.y);
            StartCoroutine(MoveSmoothly(nextWorld, start, nextStep));

            Vector3 dir = (nextWorld - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    List<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == goal) break;

            foreach (Vector2Int dir in directions)
            {
                Vector2Int next = current + dir;
                if (!gridManager.IsWithinBounds(next.x, next.y)) continue;

                var content = gridManager.GetCellContent(next.x, next.y);
                if (content == CellContentType.HeroBody || content == CellContentType.Obstacle || content == CellContentType.Monster)
                    continue;

                if (cameFrom.ContainsKey(next)) continue;

                queue.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        if (!cameFrom.ContainsKey(goal)) return null;

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = goal;

        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }

        path.Add(start);
        path.Reverse();

        return path;
    }

    private static readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    IEnumerator MoveSmoothly(Vector3 targetPos, Vector2Int from, Vector2Int to)
    {
        float speed = 5f;
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null) anim.SetBool("isMoving", true);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        if (anim != null) anim.SetBool("isMoving", false);

        gridManager.SetCellContent(from.x, from.y, CellContentType.None);
    }
}
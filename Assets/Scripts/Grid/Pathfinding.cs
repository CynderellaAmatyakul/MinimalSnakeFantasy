using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public GridManager gridManager;

    public List<GridCell> FindPath(Vector2Int start, Vector2Int target)
    {
        GridCell startNode = gridManager.GetCell(start.x, start.y);
        GridCell targetNode = gridManager.GetCell(target.x, target.y);

        if (startNode == null || targetNode == null) return null;

        List<GridCell> openList = new List<GridCell>();
        HashSet<GridCell> closedSet = new HashSet<GridCell>();

        foreach (var cell in gridManager.GetAllCells())
        {
            cell.gCost = int.MaxValue;
            cell.hCost = 0;
            cell.parent = null;
        }

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridCell current = GetLowestFCostNode(openList);
            if (current == targetNode)
                return RetracePath(startNode, targetNode);

            openList.Remove(current);
            closedSet.Add(current);

            foreach (GridCell neighbor in gridManager.GetNeighbors(current))
            {
                if (neighbor.contentType != CellContentType.None && neighbor != targetNode) continue;
                if (closedSet.Contains(neighbor)) continue;

                int tentativeGCost = current.gCost + GetDistance(current, neighbor);
                if (tentativeGCost < neighbor.gCost)
                {
                    neighbor.parent = current;
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = GetDistance(neighbor, targetNode);

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }

    List<GridCell> RetracePath(GridCell start, GridCell end)
    {
        List<GridCell> path = new List<GridCell>();
        GridCell current = end;

        while (current != start)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    GridCell GetLowestFCostNode(List<GridCell> nodes)
    {
        GridCell lowest = nodes[0];
        foreach (GridCell node in nodes)
        {
            if (node.FCost < lowest.FCost ||
                (node.FCost == lowest.FCost && node.hCost < lowest.hCost))
            {
                lowest = node;
            }
        }
        return lowest;
    }

    int GetDistance(GridCell a, GridCell b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dz = Mathf.Abs(a.z - b.z);
        return dx + dz;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellContentType
{
    None,
    HeroBody,
    CollectableHero,
    Monster,
    Obstacle
}

public class GridCell
{
    public int x;
    public int z;
    public Vector3 worldPosition;
    public CellContentType contentType = CellContentType.None;

    public int gCost;
    public int hCost;
    public GridCell parent;

    public int FCost => gCost + hCost;

    public GridCell(int x, int z, Vector3 pos)
    {
        this.x = x;
        this.z = z;
        this.worldPosition = pos;
        this.contentType = CellContentType.None;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadEnd
{
    public Tile de;
    public int mazeID;

    public DeadEnd(Tile de, int mazeID)
    {
        this.de = de;
        this.mazeID = mazeID;
    }

    public Tile GetTile()
    {
        return de;
    }

    public int GetID()
    {
        return de.GetTileID();
    }
    public int GetMazeID()
    {
        return mazeID;
    }
    public int[] GetWalls()
    {
        return de.GetWallArray();
    }
    public int GetRow()
    {
        return de.GetRow();
    }
    public int GetCol()
    {
        return de.GetCol();
    }
}

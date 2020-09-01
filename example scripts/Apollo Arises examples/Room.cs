using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public int mazeID; // the maze segment that the room is in
    public List<Tile> tiles = new List<Tile>();// should only contain empty tiles which content can be place before used to generating content
    public Tile exitTile;
    public Tile entranceTile;


    public Room(RoomFinder rf)
    {
        mazeID = rf.mazeID;
        tiles = rf.GetRoom();
        FindEntryTiles();
    }

    public void AddTile(Tile t)
    {
        tiles.Add(t);
    }

    public void AddTiles(List<Tile> t)
    {
        tiles.AddRange(t);
    }

    public void SetMazeID(int mazeID)
    {
        this.mazeID = mazeID;
    }

    public void FindEntryTiles()
    {
        Tile closeToEntrance = tiles[0];
        Tile closeToExit = tiles[0]; // set them to something and skip index 0

        for (int i = 1; i < tiles.Count; i++)
        {
            if (tiles[i].isAStarTile) // has to be an Astar tile
            {
                if (tiles[i].prevDistance < closeToEntrance.prevDistance)
                {
                    closeToEntrance = tiles[i];
                    //Debug.Log("new entrance in maze" +mazeID);
                }
                if (tiles[i].nextDistance < closeToExit.nextDistance)
                {
                    closeToExit = tiles[i];
                    //Debug.Log("new exit in maze" + mazeID);
                }
            }

        }

        exitTile = closeToExit;
        entranceTile = closeToEntrance;
    }

    public void CreateRoom()
    {
        foreach (Tile t in tiles)
        {
            t.isRoomTile = true;
        }
        MarkAsEntryTile();

        Tile.ConnectTiles(tiles[0], tiles[3]);
    }

    public void MarkAsEntryTile()
    {
        entranceTile.isRoomEntryTile = true;
        exitTile.isRoomEntryTile = true;
    }
    public void SetRoomPosition()
    {

    }

    public void DebugRoom()
    {
        Debug.Log("Room at DeadEnd (" + tiles[0].GetRow()+ ","+ tiles[0].GetCol() + ") in maze " +mazeID);
    }
}
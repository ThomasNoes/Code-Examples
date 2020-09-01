using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFinder
{
    public Tile de;
    public int mazeID;
    public Tile[,] tileArray;

    List<Tile> clockTiles = new List<Tile>();
    List<Tile> counterTiles = new List<Tile>();
    List<Tile> returnRoom = new List<Tile>();
    bool clockwiseIncludeAStar=false;
    bool counterwiseIncludeAStar=false;

    bool roomisClockwise;
    bool roomExists = false;
    public List<Tile> debugTiles = new List<Tile>();



    public RoomFinder(DeadEnd de, Tile[,] tileArray)
    {
        this.de = de.GetTile();
        mazeID = de.GetMazeID();
        this.tileArray = tileArray;
    }

    public bool SearchForRoom()
    {
        if (de.isPortalTile)
        {
            Debug.Log("searching from a portal tile dead end");
            return false;
        }

        //turn on again //Debug.Log("DeadEnd at (" +de.GetRow() + "," + de.GetCol() +") in Maze "+ mazeID);
        AddToClock(de);
        AddToCounter(de);

        bool roomClockWise = false;
        bool roomCounterWise = false;
        Tile leadIn;

        for (int i = 0; i < de.wallArray.Length; i++)
        {
            if (de.wallArray[i] == 1) // if the tile is traversable in that direction
            {
                switch (i)
                {
                    case 0:
                        leadIn = tileArray[de.GetRow() - 1,de.GetCol()];
                        SetClockwiseIncludeAStar(leadIn);
                        SetCounterwiseIncludeAStar(leadIn);
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    case 1:
                        leadIn = tileArray[de.GetRow(), de.GetCol() + 1];
                        SetClockwiseIncludeAStar(leadIn);
                        SetCounterwiseIncludeAStar(leadIn);
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    case 2:
                        leadIn = tileArray[de.GetRow() + 1, de.GetCol()];
                        SetClockwiseIncludeAStar(leadIn);
                        SetCounterwiseIncludeAStar(leadIn);
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    case 3:
                        leadIn = tileArray[de.GetRow(), de.GetCol() - 1];
                        SetClockwiseIncludeAStar(leadIn);
                        SetCounterwiseIncludeAStar(leadIn);
                        roomClockWise = CheckPerpendicularDirection(leadIn, i, true);
                        roomCounterWise = CheckPerpendicularDirection(leadIn, i, false);
                        break;
                    default:
                        break;
                }
            }
        }


        //check if a room exist and decide which room it is
        if (roomClockWise || roomCounterWise)
        {
            SetRoom(roomClockWise, roomCounterWise);

            roomExists = NoOverlappingRoomCheck();
        }

        if (roomExists)
        {
            SetAsPotentialRoom();
        }

        return roomExists;
    }

    public void SetClockwiseIncludeAStar(Tile t)
    {
        if (clockwiseIncludeAStar == false)
        {
            clockwiseIncludeAStar = t.isAStarTile;
            //Debug.Log("clockwiseAStar: " + clockwiseIncludeAStar);
        }
    }
    public void SetCounterwiseIncludeAStar(Tile t)
    {
        if (counterwiseIncludeAStar == false)
        {
            counterwiseIncludeAStar = t.isAStarTile;
            //Debug.Log("counterwiseAStar: "+ counterwiseIncludeAStar);
        }
    }

    private bool CheckPerpendicularDirection(Tile t, int i, bool clockWise)
    {
        bool roomThisWay = false;
        Tile leadIn;

        if (clockWise)
        {
            i = Modulus(i + 1);
            AddToClock(t);
        }
        else
        {
            i = Modulus(i - 1);
            AddToCounter(t);
        }

        int row = 0;
        int col = 0;

        switch (i)
        {
            case 0:
                row = t.GetRow() - 1;
                col = t.GetCol();
                break;
            case 1:
                row = t.GetRow();
                col = t.GetCol() + 1;

                break;
            case 2:
                row = t.GetRow() + 1;
                col = t.GetCol();

                break;
            case 3:
                row = t.GetRow();
                col = t.GetCol() - 1;
                break;
            default:
                break;
        }
        if (CheckBounds(row, col))
        {
            leadIn = tileArray[row, col];

            if (t.wallArray[i] == 1) // if the path is open
            {
                if (//leadIn.isAStarTile && // if it is a a star tile
                    !leadIn.isPortalTile) // and it is not the portal tile
                {
                    if (clockWise)
                    {
                        SetClockwiseIncludeAStar(leadIn);
                    }
                    else
                    {
                        SetCounterwiseIncludeAStar(leadIn);
                    }
                    roomThisWay = CheckPerpendicularDirection(leadIn, i, clockWise);
                }
            }
            else if (t.wallArray[i] == 0) // if the path is closed 
            {
                if (leadIn == de) // we found our way back to the deadend de
                {
                    //turn on again //Debug.Log("i found the deadend");
                    if (clockWise)
                    {
                        SetClockwiseIncludeAStar(leadIn);
                        //turn on again //Debug.Log("clockwise include AStar:" + clockwiseIncludeAStar);
                        if (clockwiseIncludeAStar)
                        {
                            roomThisWay = true;
                            AddToClock(leadIn);
                        }
                    }
                    else
                    {
                        SetCounterwiseIncludeAStar(leadIn);
                        //turn on again //Debug.Log("counterwise include AStar:" + counterwiseIncludeAStar);
                        if (counterwiseIncludeAStar)
                        {
                            roomThisWay = true;
                            AddToCounter(leadIn);
                        }
                    }
                }
            }
        }
        return roomThisWay;
    }

    private bool CheckBounds(int row, int col)
    {
        bool inBounds = true;
        if (row < 0 ||
            col < 0 ||
            row >= tileArray.GetLength(0) ||
            col >= tileArray.GetLength(1))
        {
            inBounds = false;
        }

        return inBounds;
    }

    private void AddToClock(Tile t)
    {
        if (!clockTiles.Contains(t))
        {
            clockTiles.Add(t);
        }

        if (!debugTiles.Contains(t))
        {
            debugTiles.Add(t);
        }
    }

    private void AddToCounter(Tile t)
    {
        if (!counterTiles.Contains(t))
        {
            counterTiles.Add(t);
        }

        if (!debugTiles.Contains(t))
        {
            debugTiles.Add(t);
        }
    }

    private int Modulus(int input)
    {
        if (input == -1)
        {
            return 3;
        }
        else if (input == 4)
        {
            return 0;
        }
        return input;

    }

    public bool GetRoomExists()
    {
        return roomExists;
    }

    public List<Tile> GetRoom()
    {
        if (roomExists)
        {
            return returnRoom;
        }
        return null;

    }

    private void SetRoom(bool roomClockWise, bool roomCounterWise)
    {
        if (roomClockWise && roomCounterWise) // if both directions are possible
        {
            if (ShortestDistanceToPortal(clockTiles) >= ShortestDistanceToPortal(counterTiles)) // pick the room that is closest to the mid-point on the optimal route
            {
                returnRoom = clockTiles;
            }
            else
            {
                returnRoom = counterTiles;
            }
        }
        else if(roomClockWise)
        {
            returnRoom = clockTiles;
        }
        else if (roomCounterWise)
        {
            returnRoom = counterTiles;
        }
    }

    private bool NoOverlappingRoomCheck()
    {
        foreach (Tile t in returnRoom)
        {
            if (t.GetIsPartOfPotentialRoom())
            {
                return false;
            }
        }
        return true;
    }
    private void SetAsPotentialRoom()
    {
        foreach ( Tile t in returnRoom)
        {
            t.SetIsPartOfPotentialRoom(true);
        }
    }

    private int ShortestDistanceToPortal(List<Tile> tiles)
    {
        int shortest = int.MaxValue;

        foreach (var t in tiles)
        {

            if(t.nextDistance < shortest)
            {
                shortest = t.nextDistance;
            }
            if (t.prevDistance < shortest)
            {
                shortest = t.prevDistance;
            }
        }
        return shortest;
    }
    
}
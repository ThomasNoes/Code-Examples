using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Camera;
using UnityEngine;


//This is for

public class Tile : MonoBehaviour {

    [Header("A Star information")]
    //A star information
    public bool isMarked;
    public int nextDistance;
    public int prevDistance;

    [Header("Tile information")]
    //tile information
    public int tileID;
    /// <summary>
    /// [0] = north, [1] = east, [2]= south, [3] = west. 0 = only wall, 1 = wall/window, 2 = wall/window/no wall
    /// </summary>
    public int[] outerWalls;
    /// <summary>
    /// [0] = north, [1] = east, [2]= south, [3] = west. 1 = traversable / no wall, 0 = not traversable / wall
    /// </summary>
    public int[] wallArray;
    public float tileWidth;

    //for A Star algorithm
    public int aStarId;
    private int row;
    private int column;
    private int gCost;
    private int hCost;
    private Tile parent;

    //for room position
    public string RoomPosition = "Corridor";
    public bool isRoomEntryTile, isDoorTile;
    private bool isPartOfPotentialRoom;

    //connect a star tile for non a star tiles
    public Tile ConnectedAStar;
    public int connectAStarIndex;



    //tile type
    public bool isAStarTile, isPortalTile, isOuterTile, isRoomTile, isOpenRoof, isDeadEnd, isShutOffTile;
    public int partOfMaze;
    public bool occupied, blocked;

    void Awake () {
        wallArray = new int[] { 0, 0, 0, 0 };
        outerWalls = new int[] { -1, -1, -1, -1 };
        tileID = 0;
    }

    public void SetRowAndColumn(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

    public void SetWidth (float width) {
        tileWidth = width;
        transform.localScale *= tileWidth;
    }

    private void SetIDFromArray () {
        tileID = 8 * wallArray[3] + 4 * wallArray[2] + 2 * wallArray[1] + wallArray[0];
        //SetMaterial(tileID, 'p');
    }

    // Direction specifies which side we want to traverse through,
    // the opposite side of that on the "to" tile is always dir-2,
    // and to prevent out of bounds we take the mod(4) of it.
    public static bool IsTraversable (Tile from, Tile to, int direction) {
        int oppositeDirection = (direction + 2) % 4;
        return (from.wallArray[direction] == to.wallArray[oppositeDirection]);
    }

    //Sets the value of the wallArray at position "direction" to val (0 to close 1 to open) then sets the ID
    public void SetWall (int direction, int val) {
        wallArray[direction] = val;
        SetIDFromArray ();
    }

    public void OpenWall (int direction) {
        SetWall (direction, 1);
    }

    public void CloseWall (int direction) {
        SetWall (direction, 0);
    }

    //Calls openWall on the from tile towards direction, and the to tile towards oppositeDirection (0-2 1-3)
    public static void ConnectTiles (Tile from, Tile to, int direction) {
        int oppositeDirection = (direction + 2) % 4;
        from.OpenWall (direction);
        to.OpenWall (oppositeDirection);
    }

    public static void ConnectTiles(Tile from, Tile to)
    {
        int direction = -1;
        bool connect = false;
        //find the direction
        if (from.GetRow() == to.GetRow() + 1 && from.GetCol() == to.GetCol()) //connect north
        {
            direction = 0;
            connect = true;
            //turn on again //Debug.Log("open north");
        }
        else if (from.GetRow() == to.GetRow() && from.GetCol() == to.GetCol() - 1) //connect east
        {
            direction = 1;
            connect = true;
            //turn on again //Debug.Log("open east");
        }
        else if (from.GetRow() == to.GetRow() - 1 && from.GetCol() == to.GetCol()) //connect south
        {
            direction = 2;
            connect = true;
            //turn on again //Debug.Log("open south");
        }
        else if (from.GetRow() == to.GetRow() && from.GetCol() == to.GetCol() + 1) //connect west
        {
            direction = 3;
            connect = true;
            //turn on again //Debug.Log("open west");
        }
        else
        {
            Debug.Log("trying to connect two tiles that are not adjacent");
        }
        if (connect)
        {
            int oppositeDirection = (direction + 2) % 4;
            from.OpenWall(direction);
            to.OpenWall(oppositeDirection);
        }

    }

    //Same as connectTiles but closes them down.
    public static void DisconnectTiles (Tile from, Tile to, int direction) {
        int oppositeDirection = (direction + 2) % 4;
        from.CloseWall (direction);
        to.CloseWall (oppositeDirection);
    }

    // Could be done with an event and a listener so the functions themselves
    // wouldn't have to call setmaterial but it could react to changes
    public void SetMaterial (int materialID, char materialType) {
        Renderer renderer = GetComponentInChildren<Renderer> ();
        renderer.material = Resources.Load<Material> (materialType + materialID.ToString ());
    }

    public void SetArrayFromID () {
        int temp = tileID;
        for (int i = 3; i >= 0; i--) {
            wallArray[i] = temp / (int) Mathf.Pow (2, i);
            temp %= (int) Mathf.Pow (2, i);
        }
        //SetMaterial(tileID, 'p');
    }

    public void SetWallArray (int[] a) {
        wallArray = a;
        SetIDFromArray ();
    }

    public int[] GetWallArray () {
        return wallArray;
    }

    public void SetTileID (int i) {
        tileID = i;
        SetArrayFromID ();
    }

    public int GetTileID () {
        return tileID;
    }

    public int GetG()
    {
        return gCost;
    }
    public void SetG( int g)
    {
        gCost = g;
    }
    public int GetH()
    {
        return hCost;
    }
    public void SetH(int h)
    {
        hCost = h;
    }
    public int GetF()
    {
        return gCost + hCost;
    }
    public Tile GetParent()
    {
        return parent;
    }
    public void SetParent(Tile parent)
    {
        this.parent = parent;
    }
    public int GetRow()
    {
        return row;
    }
    public void SetRow(int r)
    {
        row = r;
    }
    public int GetCol()
    {
        return column;
    }
    public void SetCol(int c)
    {
        column = c;
    }
    public void SetPortalDistance(int index, int length, bool CheckForPortalTile = true)
    {
        nextDistance = (length-1) - index;
        prevDistance = index;
        if (CheckForPortalTile)
        {
            isPortalTile = CheckIfPortalTile();
        }

    }
    public void SetPortalDistance(Tile t)
    {
        nextDistance = t.nextDistance+1;
        prevDistance = t.prevDistance+1;
        isPortalTile = CheckIfPortalTile();
    }

    public void SetIsPortalTile(bool isPortalTile)
    {
        this.isPortalTile = isPortalTile;
    }

    public void SetAsAstarTile()
    {
        isAStarTile = true;
    }
    public void SetAsMarked()
    {
        isMarked = true;
    }

    public bool CheckIfPortalTile()
    {
        if (nextDistance == 0 || prevDistance == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetIsPartOfPotentialRoom(bool b)
    {
        isPartOfPotentialRoom = b;
    }

    public bool GetIsPartOfPotentialRoom()
    {
        return isPartOfPotentialRoom;
    }


    //private void OnTriggerEnter(Collider col)
    //{
    //    if (cPosSwitch != null)
    //        if (col.CompareTag("TileChecker"))
    //        {
    //            cPosSwitch.SetDistanceVariables(prevdistance, nextDistance);
    //        }
    //}
}
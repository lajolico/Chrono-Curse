using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using System;
using UnityEngine.Tilemaps;


/// <summary>
/// Generates our rooms and corridors, linking those rooms. Using the MapGenerator to assist in this.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

    [SerializeField]
    protected RoomMaker roomParams;

    private int dungeonWidth, dungeonHeight;

    [SerializeField]
    private int minRoomWidth, minRoomHeight;

    public int minSize = 50;
    public int maxSize = 120;

    public int minRoomSize = 10;
    public int maxRoomSize = 30;

    //How large our dungeon should be when spliting it into other rooms.

    private int level;

    private TilemapUtil tilemapUtil;

    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;

    [SerializeField]
    [Range(1,3)]
    [Tooltip("The bigger the value, the more distance between rooms.")]
    private int roomDistance = 3;

    [SerializeField]
    [Range(2,3)]
    private int corridorWidth = 1;

    public UnityEvent FinishedGeneration;

    private HashSet<Vector2Int> dungeonFloorTiles = new HashSet<Vector2Int>();

    private HashSet<Vector2Int> dungeonWallTiles = new HashSet<Vector2Int>();

    private DungeonGenerator() { }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
       
    }


    private void Start()
    {
        tilemapUtil = GetComponent<TilemapUtil>();
        if (tilemapUtil == null)
        {
            tilemapUtil = gameObject.AddComponent<TilemapUtil>();
        }

        // Get the player's level
        level = PlayerManager.Instance.GetLevel();

        // Calculate the dungeon size based on the player's level
        dungeonWidth = Mathf.Clamp(level + 2, minSize, maxSize);
        dungeonHeight = Mathf.Clamp(level + 2, minSize, maxSize);

        // Calculate the room sizes based on the player's level
        minRoomWidth = Mathf.Clamp(Mathf.FloorToInt(level / 2) + 2, minRoomSize, maxRoomSize);
        minRoomHeight = Mathf.Clamp(Mathf.FloorToInt(level / 2) + 2, minRoomSize, maxRoomSize);


    }
    /// <summary>
    /// Reset our dungeon and clear it.
    /// </summary>
    internal void ResetDungeon()
    {
        tilemapUtil.Clear();
        RoomManager.Instance.Reset();
        PropManager.Instance.Reset();
        PlayerManager.Instance.DestroyPlayer();
        EnemyManager.Instance.Reset();
        ExitPoint.Instance.DestroyExit();

    }

    /// <summary>
    /// Entry point for our Dungeon Generator
    /// </summary>
    public void GenerateDungeon()
    {
        ResetDungeon();
        CreateRooms();
        RoomManager.Instance.GatherRoomData();
        Invoke("RunEvent", 1);
     }

    private void RunEvent()
    {
        FinishedGeneration?.Invoke();
    }

    /// <summary>
    /// As the name says, create our rooms.
    /// </summary>
    private void CreateRooms()
    {
        List<BoundsInt> rooms = ProcedureAlgorithm.BSP(new BoundsInt((Vector3Int)startPos, new Vector3Int(    
            dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);


        for(int i = 0; i < rooms.Count; i++)
        {
            RoomManager.Instance.Rooms.Add(GenerateRooms(rooms[i], (Vector2Int)Vector3Int.RoundToInt(rooms[i].center)));
        }

        //Get all the positions of our RoomFloorTiles
        HashSet<Vector2Int> floor = RoomManager.Instance.GetRoomFloorTiles();
        HashSet<Vector2Int> corridors = ConnectRooms(RoomManager.Instance.GetRoomCenters());
        
        //Floor will unionwith Corridors for painting
        floor.UnionWith(corridors);

        //Pass our corridor positions for use in our room Manager
        RoomManager.Instance.Corridors.UnionWith(corridors);

        //Set our room types and find  the locations of where our player and exit will spawn.
        RoomManager.Instance.SetRoomTypes(RoomManager.Instance.Rooms);

        //Paint all of our tiles
        tilemapUtil.PaintFloorTiles(floor);
        WallUtil.CreateWalls(floor, tilemapUtil);

        //This is for our save file, where we reload the dungeon tiles
        dungeonFloorTiles.UnionWith(floor);
        dungeonWallTiles.UnionWith(RoomManager.Instance.Walls);
    }

    /// <summary>
    /// Connects the rooms.
    /// Will find the closest point between the start to the end. This is similar to how rooms will be generated with prefabbed weapons, armour, etc.
    /// </summary>
    /// <param name="roomCenters">List of room centers in our world</param>
    /// <returns></returns>
    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter  = roomCenters[Random.Range(0, roomCenters.Count)]; //Grab a random room center to start
        roomCenters.Remove(currentRoomCenter); //remove it, since we have already found it.

        while(roomCenters.Count > 0)
        {
            Vector2Int closest = DistanceUtil.FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = ProcedureAlgorithm.GetCorridors(currentRoomCenter, closest,corridorWidth );
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor); //Avoid duplicatews

        }
        return corridors;
    }

    /// <summary>
    /// Generates our different rooms based on the WxH of the BSP algorithm
    /// 
    /// </summary>
    /// <param name="roomsList"></param>
    /// <param name="roomCenter"></param>
    /// <returns></returns>
    private Room GenerateRooms(BoundsInt room, Vector2Int roomCenter)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        
        var roomBounds = room;
        var roomBoundsXY = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
        var roomFloor = ProcedureAlgorithm.GetFloorPositions(roomParams, roomBoundsXY);
        foreach (var position in roomFloor)
        {
            if(position.x >= (roomBounds.xMin + roomDistance) && position.x <= (roomBounds.xMax - roomDistance)
                    && position.y >= (roomBounds.yMin - roomDistance) && position.y <= (roomBounds.yMax - roomDistance))
            {
                floor.Add(position);
            }
        }
       
        return new Room(roomCenter, floor);
    }

   public void SetDungeonData(DungeonData dungeonData, PropData propData)
   {
        ResetDungeon();

        foreach (TileSaveData tileSaveData in dungeonData.floorTiles)
        {
            tilemapUtil.FloorTilemap.SetTile(tileSaveData.position, tileSaveData.tileBase);
        }

        // Load wall tiles
        foreach (TileSaveData tileSaveData in dungeonData.wallTiles)
        {
            tilemapUtil.WallTilemap.SetTile(tileSaveData.position, tileSaveData.tileBase);

            // Add colliders to the wall tiles
            TilemapCollider2D tilemapCollider = tilemapUtil.WallTilemap.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                tilemapCollider.usedByComposite = true;
                tilemapCollider.isTrigger = false;
                tilemapCollider.enabled = true;
            }
        }

        PropManager.Instance.LoadPropData(propData);
    }

   public DungeonData GetDungeonData()
   {
        DungeonData dungeonData = new DungeonData();

        dungeonData.floorTiles = new List<TileSaveData>();
        dungeonData.wallTiles = new List<TileSaveData>();

        foreach(Vector3Int pos in dungeonFloorTiles)
        {
            TileBase tile = tilemapUtil.FloorTilemap.GetTile(pos);
            if(tile != null)
            {
                TileSaveData tileData = new TileSaveData(pos, tile);
                dungeonData.floorTiles.Add(tileData);
            }
        }

        foreach (Vector3Int pos in dungeonWallTiles)
        {
            TileBase tile = tilemapUtil.WallTilemap.GetTile(pos);
            if (tile != null)
            {
                TileSaveData tileData = new TileSaveData(pos, tile);
                dungeonData.wallTiles.Add(tileData);
            }
        }

        return dungeonData;
    }

    public int GetDungeonWidth()
    {
        return dungeonWidth;
    }

    public int GetDungeonHeight()
    {
        return dungeonHeight;
    }
}

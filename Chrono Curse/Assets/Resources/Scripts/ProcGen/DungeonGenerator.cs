using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.Events;

/// <summary>
/// Generates our rooms and corridors, linking those rooms. Using the MapGenerator to assist in this.
/// </summary>
public class DungeonGenerator : AbstractDungeons
{

    [SerializeField]
    protected RoomMaker roomParams;

    private RoomManager roomManager;

    [SerializeField]
    private int minRoomWidth = 10, minRoomHeight = 10; 
   
    //How large our dungeon should be when spliting it into other rooms.
    [SerializeField]
    private int dungeonWidth = 60, dungeonHeight = 60;
   

    [SerializeField]
    [Range(1,3)]
    [Tooltip("The bigger the value, the more distance between rooms.")]
    private int roomDistance = 3;

    [SerializeField]
    [Range(2,3)]
    private int corridorWidth = 1;

    public UnityEvent FinishedGeneration;

    /// <summary>
    /// Check if our required gameobjects are attacked to our DungeonGenerator Object
    /// Important ones such as DungeonManager and the TilempaUtil
    /// </summary>
    private void InitDungeon()
    {
        roomManager = FindObjectOfType<RoomManager>();
        if (roomManager == null)
        {
            roomManager = gameObject.AddComponent<RoomManager>();
        }

        tilemapUtil = FindObjectOfType<TilemapUtil>();
        if (tilemapUtil == null)
        {
            tilemapUtil = gameObject.AddComponent<TilemapUtil>();
        }
    }

    /// <summary>
    /// Entry point for our Dungeon Generator
    /// </summary>
    protected override void RunProceduralGeneration()
    {
        InitDungeon();
        tilemapUtil.Clear();
        roomManager.Reset();
        
        CreateRooms();
        roomManager.GatherRoomData();
        Invoke("RunEvent", 1);
    }

    public void RunEvent()
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

       
        foreach (var room in rooms)
        {
            roomManager.Rooms.Add(GenerateRooms(rooms,(Vector2Int)Vector3Int.RoundToInt(room.center)));
        }

        HashSet<Vector2Int> floor = roomManager.GetRoomFloorTiles();
        HashSet<Vector2Int> corridors = ConnectRooms(roomManager.GetRoomCenters());

        //Get our room floor tiles so we can substract from the Set corridors
        //HashSet<Vector2Int> tempRoomPositions = new HashSet<Vector2Int>(floor);

        //Floor will unionwith Corridors for painting
        floor.UnionWith(corridors);

        //For every position in our corridor positions subtract those that are in
        //the rooms
        //corridors.ExceptWith(tempRoomPositions);

        //Pass our corridor positions for use in our room Manager
        roomManager.Corridors.UnionWith(corridors);

        //Paint all of our tiles
        tilemapUtil.PaintFloorTiles(floor);
        WallUtil.CreateWalls(floor, tilemapUtil);
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
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = ProcedureAlgorithm.GetCorridors(currentRoomCenter, closest,corridorWidth );
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor); //Avoid duplicatews

        }
        return corridors;
    }

    /*
        Find the closest dungeon room point to that specific room. Loop through the roomCenters
        Return the point back to ConnectRooms, where it will continue the algorithm
    */
    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    /// <summary>
    /// Generates our different rooms based on the WxH of the BSP algorithm
    /// 
    /// </summary>
    /// <param name="roomsList"></param>
    /// <param name="roomCenter"></param>
    /// <returns></returns>
    private Room GenerateRooms(List<BoundsInt> roomsList, Vector2Int roomCenter)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
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
        }
        return new Room(roomCenter, floor);
    }

}

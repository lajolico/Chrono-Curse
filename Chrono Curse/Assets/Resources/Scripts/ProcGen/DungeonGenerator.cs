using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Generates our rooms and corridors, linking those rooms. Using the MapGenerator to assist in this.
/// </summary>
public class DungeonGenerator : AbstractDungeons
{

    [SerializeField]
    protected RoomMaker roomParams;

    [SerializeField]
    private int minRoomWidth = 10, minRoomHeight = 10; 
   
    //How large our dungeon should be when spliting it into other rooms.
    [SerializeField]
    private int dungeonWidth = 60, dungeonHeight = 60;
   

    [SerializeField]
    [Range(1,3)]
    [Tooltip("The bigger the value, the more distance between rooms.")]
    private int roomDistance = 1;

    [SerializeField]
    [Range(2,3)]
    private int corridorWidth = 1;

    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }

    /// <summary>
    /// Get our floor positions that will help with placing our walls and other objects
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    protected override HashSet<Vector2Int> GetFloorPositions(RoomMaker parameters, Vector2Int position)
    {
        var currentPos = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProcedureAlgorithm.GetPath(currentPos, parameters.walkLength);
            floorPositions.UnionWith(path); //remove dupes and ensure O(n)

            if (parameters.changePosPerIteration)
            {
                currentPos = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }

    /// <summary>
    /// As the name says, create our rooms.
    /// </summary>
    private void CreateRooms()
    {
        var rooms = ProcedureAlgorithm.BSP(new BoundsInt((Vector3Int)startPos, new Vector3Int(    
            dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        floor = SetRooms(rooms);
        
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in rooms)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);

        tilemapUtil.PaintFloorTiles(floor);
        WallLogic.CreateWalls(floor, tilemapUtil);
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
            HashSet<Vector2Int> newCorridor = SetCorridors(currentRoomCenter, closest,corridorWidth );
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
    /// Creates our corridors, connecting the different rooms.
    /// </summary>
    /// <param name="currentRoomCenter">Recieves roomCenter from </param>
    /// <param name="destination">Where are we going!?</param>
    /// <param name="corridorWidth">How big is this corridor going to be?</param>
    /// <returns></returns>
    private HashSet<Vector2Int> SetCorridors(Vector2Int currentRoomCenter, Vector2Int destination, int corridorWidth)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if(destination.y > position.y){
                position += Vector2Int.up;
            }else if(destination.y < position.y){
                position += Vector2Int.down;
            }   
            for(int k = 0; k < corridorWidth; k++) 
            {
                for(int j = 0; j < corridorWidth; j++) 
                {
                    var offset = new Vector2Int(k,j); 

                    corridor.Add(position+offset);
                }
            }
        }
        while(position.x != destination.x)
        {
            if(destination.x > position.x){
                position += Vector2Int.right;
            }else if(destination.x < position.x){
                position += Vector2Int.left;
            }
            for(int k = 0; k < corridorWidth; k++) 
            {
                for(int j = 0; j < corridorWidth; j++) 
                {
                    var offset = new Vector2Int(k,j); 

                    corridor.Add(position+offset);
                }
            }
        }
        
        return corridor;
    }

    /*
        Purpose: Generate imperfect rooms, giving to the feeling of a dynamic dungeon and enviroment
    */
    private HashSet<Vector2Int> SetRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = GetFloorPositions(roomParams, roomCenter);
            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + roomDistance) && position.x <= (roomBounds.xMax - roomDistance)
                        && position.y >= (roomBounds.yMin - roomDistance) && position.y <= (roomBounds.yMax - roomDistance))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }

}

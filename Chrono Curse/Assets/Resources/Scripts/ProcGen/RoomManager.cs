using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

    public List<Room> Rooms { get; set; } = new List<Room>();

    [SerializeField]
    private bool showGizmo = false;

    public HashSet<Vector2Int> Corridors { get; private set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> Walls = new HashSet<Vector2Int>();
    private RoomManager() { }

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

    //Reset our dungeon and how we interact with it.
    public void Reset()
    {
        foreach (Room room in Rooms)
        {
            foreach (var item in room.EnemiesInRoom)
            {
                Destroy(item);
            }

            room.PropPositions.Clear();
        }

        Rooms = new();
        Corridors = new();
        Walls = new();
        PlayerManager.Instance.DestroyPlayer();
    }

    /// <summary>
    /// Helper function that gets all of the RoomCenters from our Rooms List
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetRoomCenters()
    {
        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach(Room room in Rooms) 
        {
            roomCenters.Add(room.RoomCenter);
        }
        return roomCenters;
    }

    /// <summary>
    /// Helper function the loops through all rooms and get alls tiles into one HashSet for use 
    /// </summary>
    /// <returns>Vector2Int HashSet of all tiles</returns>
    public HashSet<Vector2Int> GetRoomFloorTiles()
    {
        HashSet<Vector2Int> tiles = new HashSet<Vector2Int>();
        foreach(Room room in Rooms)
        {
            foreach(Vector2Int position in room.FloorTiles)
            {
                tiles.Add(position);
            }
        }

        return tiles;
    }

    /// <summary>
    /// Void method that gathers all available data from our rooms and puts them into a specific rooms lists.
    /// This assists in item/prop placing later on in the dungeon levels
    /// </summary>
    /// 
    public void GatherRoomData()
    {
        foreach (Room room in Rooms)
        {
            
            foreach(var position in room.FloorTiles)
            {
                int neighborCount = GetNeighbors(position, room, DirectionUtil.eightDirectionsList);

                if(neighborCount >= 6)
                {
                    room.TilesInsideRoom.Add(position);
                } 
                else
                {
                    if(!Corridors.Contains(position))
                    {
                        if (Walls.Contains(position + Vector2Int.up) && !Corridors.Contains(position + Vector2Int.up))
                        {
                            room.WallsTop.Add(position + Vector2Int.up);
                            room.TilesNearUpperSide.Add(position);
                        }
                        if(Walls.Contains(position + Vector2Int.left) && !Corridors.Contains(position + Vector2Int.left))
                        {
                            room.TilesNearLeftSide.Add(position);
                        }

                        if (Walls.Contains(position + Vector2Int.right) && !Corridors.Contains(position + Vector2Int.right))
                        {
                            room.TilesNearRightSide.Add(position);
                        }

                        if (Walls.Contains(position + Vector2Int.down) && !Corridors.Contains(position + Vector2Int.down))
                        {
                            room.TilesNearBottomSide.Add(position);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get the neighbors of every single tile inside the room.
    /// Sets us up for getting the rest of the neighbors later on
    /// </summary>
    /// <param name="origin">tile position we are looking for</param>
    /// <param name="room">What room</param>
    /// <param name="offSets">Where to look from the original position</param>
    /// <returns></returns>
    private int GetNeighbors(Vector2Int origin, Room room, List<Vector2Int> offSets)
    {
        int neighbors = new int();

        foreach (Vector2Int neighborPosition in offSets)
        {
            Vector2Int possibleNeighbor = origin + neighborPosition;

            if (room.FloorTiles.Contains(possibleNeighbor))
            {
                neighbors++;
            }
            
        }
        return neighbors;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmo == false)
            return;

        Color color = Color.green;
        color.a = 0.3f;
        Gizmos.color = color;

       
        foreach (Vector2Int pos in Walls)
        {
            Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
        }
        
        /*
        //Get our corridors and paint them for development
        Gizmos.color = Color.black;
        foreach (Vector2Int position in Corridors)
        {
            Gizmos.DrawCube(position + Vector2.one * 0.5f, Vector2.one);
        }
        */
    }


    public void SetRoomTypes(List<Room> rooms)
    {
         for(int i = 0; i < rooms.Count; i++) 
         {

            Room room = rooms[i];

            if(i==0)
            {
                room.roomType = Room.RoomType.Entrance;
            }else if(i == rooms.Count - 1)
            {
                room.roomType = Room.RoomType.Exit;
            }else
            {
                room.roomType = (Room.RoomType)Random.Range(0, 3);
            }
         }
    }

    

}

 
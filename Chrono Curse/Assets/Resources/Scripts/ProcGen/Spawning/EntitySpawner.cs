using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using UnityEditor;

public class EntitySpawner : MonoBehaviour
{


    [SerializeField]
    private GameObject enemyPrefab, exitPrefab, bossEnemyPrefab;
     
    [SerializeField]
    private CinemachineVirtualCamera vCamera;

    RoomManager roomManager;

    [SerializeField]
    private bool showGizmo = false;
     
    private void Awake()
    {
        roomManager = FindObjectOfType<RoomManager>();
    }

    public void InitEntites()
    {
        if (roomManager == null)
        {
            Debug.Log("roomManager is null in Entities Script");
            return;
        } 
        PlaceEntities();
    }

    private void PlaceEntities()
    {
        //Get the count of our rooms to loop through start spawning our player and enemies
        foreach (Room room in roomManager.Rooms)
        {
            //Start generating the locations for our dictionary to be used in our pathfinder
            SpawnValidatorAlgorithm spawnGraph = new SpawnValidatorAlgorithm(room.FloorTiles);

            //Grab only the rooms tiles WITHOUT intersecting the corridor paths. 

            HashSet<Vector2Int> floors = new HashSet<Vector2Int>(room.FloorTiles);
            floors.IntersectWith(roomManager.Corridors);

            //Run the BFS to find all the tiles in the room accessible from the path
            Dictionary<Vector2Int, Vector2Int> floorMap = spawnGraph.FindSpawnLocations(floors.First(), room.PropPositions);

            //Positions that we can reach + path == positions where we can place enemies
            room.PossibleSpawnPostions = floorMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            room.PossibleSpawnPostions.Remove(room.RoomCenter);

            SpawnEnemies(room);

            if (room.roomType == Room.RoomType.Entrance)
            {
                PlayerManager.Instance.SpawnPlayer();
                PlayerManager.Instance.SetPlayerPosition(room.RoomCenter + Vector2.one * 0.5f);
                vCamera.Follow = PlayerManager.Instance.GetPlayerTransform();
                vCamera.LookAt = PlayerManager.Instance.GetPlayerTransform();
            }

            if(room.roomType == Room.RoomType.Exit)
            {
                GameObject exit = Instantiate(exitPrefab);
                exit.transform.localPosition = room.RoomCenter + Vector2.one * 0.5f;
                roomManager.ExitReference = exit;
            }
        }
    }
    /// <summary>
    /// Spawns enemies in a room based on its RoomType.
    /// </summary>
    /// <param name="room">The Room object to spawn enemies in.</param>
    private void SpawnEnemies(Room room)
    {
        //TODO, remove this and update it with the GM's level 
        int playerLevel = 5;

        switch (room.roomType)
        {
            case Room.RoomType.Normal:
                // Spawn a random number of enemies based on the player's level
                int numEnemies = Random.Range(playerLevel, playerLevel + 3);
                for (int i = 0; i < numEnemies; i++)
                {
                    if(room.PossibleSpawnPostions.Count <= i)
                    {
                        return;
                    }
                    // Spawn enemy prefab
                    GameObject enemy = Instantiate(enemyPrefab, room.PossibleSpawnPostions[i] + Vector2.one * 0.5f, Quaternion.identity);
                    room.EnemiesInRoom.Add(enemy);
                    //Save our enemy spawn positions, so we can spawn other items, such as portals or other things.
                    room.EnemySpawnPositions.Add(room.PossibleSpawnPostions[i]);
                }
                break;
            case Room.RoomType.Important:
                // Spawn more enemies than normal rooms
                int numImportantEnemies = Random.Range(playerLevel + 2, playerLevel + 5);
                for (int i = 0; i < numImportantEnemies; i++)
                {

                    if (room.PossibleSpawnPostions.Count <= i)
                    {
                        return;
                    }
                    // Spawn enemy prefab
                    GameObject enemy = Instantiate(enemyPrefab);
                    enemy.transform.localPosition = room.PossibleSpawnPostions[i] + Vector2.one * 0.5f;
                    room.EnemiesInRoom.Add(enemy);
                    //Save our enemy spawn positions, so we can spawn other items, such as portals or other things.
                    room.EnemySpawnPositions.Add(room.PossibleSpawnPostions[i]);
                }
                break;
            case Room.RoomType.Exit:
                GameObject boss = Instantiate(bossEnemyPrefab, room.RoomCenter + Vector2.one * 0.8f, Quaternion.identity);
                roomManager.BossReference = boss;

                int numExitEnemies = Random.Range(playerLevel + 2, playerLevel + 8);

                for (int i = 0; i < numExitEnemies; i++) 
                {
                    if (room.PossibleSpawnPostions.Count <= i)
                    {
                        return;
                    }
                    // Spawn enemy prefab
                    GameObject enemy = Instantiate(enemyPrefab, room.PossibleSpawnPostions[i] + Vector2.one * 0.5f, Quaternion.identity);
                    room.EnemiesInRoom.Add(enemy);
                    //Save our enemy spawn positions, so we can spawn other items, such as portals or other things.
                    room.EnemySpawnPositions.Add(room.PossibleSpawnPostions[i]);
                }
                break;
            case Room.RoomType.Empty:
                //No enemies
                break;
            case Room.RoomType.Entrance:
                //No enemies
                break;
            default:
                Debug.LogError("Invalid room type.");
                break;
        }
    }
    /// <summary>
    /// Visually help see where everything spawns, very helpful.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (roomManager == null || showGizmo == false)
            return;
       
        Color color = Color.green;
        color.a = 0.3f;
        Gizmos.color= color;
 
        foreach(Room room in roomManager.Rooms)
        {
            foreach (Vector2Int pos in  room.PossibleSpawnPostions)
            {
                Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}

/// <summary>
/// The purpose of this class is to find spawn locations for our entities (enemies) and other objects
/// </summary>
internal class SpawnValidatorAlgorithm
{
    Dictionary<Vector2Int, List<Vector2Int>> graph = new Dictionary<Vector2Int, List<Vector2Int>>();

    public SpawnValidatorAlgorithm(HashSet<Vector2Int> roomFloor)
    {
        foreach (Vector2Int pos in roomFloor)
        {
            List<Vector2Int> neighbours = new List<Vector2Int>();
            foreach (Vector2Int direction in DirectionUtil.cardinalDirections)
            {
                Vector2Int newPos = pos + direction;
                if (roomFloor.Contains(newPos))
                {
                    neighbours.Add(newPos);
                }
            }
            graph.Add(pos, neighbours);
        }
    }

    /// <summary>
    /// Creates a map of reachable tiles in our dungeon. Will also assist in pathfinding for our enemies in their rooms.
    /// </summary>
    /// <param name="startPos">Door position or tile position on the path between rooms inside this room</param>
    /// <param name="obstacles">Things, we do not want to spawn on</param>
    /// <returns></returns>
    public Dictionary<Vector2Int, Vector2Int> FindSpawnLocations(Vector2Int startPos, HashSet<Vector2Int> obstacles)
    {
        //Nodes that have yet to be explored in our dungeon
        Queue<Vector2Int> unexploredNodes = new Queue<Vector2Int>();
        unexploredNodes.Enqueue(startPos);

        HashSet<Vector2Int> visitedNodes = new HashSet<Vector2Int>
        {
            startPos
        };
 
        Dictionary<Vector2Int, Vector2Int> map = new Dictionary<Vector2Int, Vector2Int>
        {
            { startPos, startPos }
        };

        while (unexploredNodes.Count > 0)
        {
            //Dequeue the node then put into our neighbors list
            Vector2Int node = unexploredNodes.Dequeue();
            List<Vector2Int> neighbours = graph[node];

            //This algorithm tries to find a path between prop positions or obstacles
            foreach (Vector2Int neighbourPosition in neighbours)
            {
             
                if (visitedNodes.Contains(neighbourPosition) == false &&
                    obstacles.Contains(neighbourPosition) == false)
                {
                    unexploredNodes.Enqueue(neighbourPosition);
                    visitedNodes.Add(neighbourPosition);
                    map[neighbourPosition] = node;
                }
            }
        }

        return map;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> allEnemies = new List<GameObject>();

    [SerializeField]
    internal List<EnemyObject> enemyDataList = new List<EnemyObject>();

    [SerializeField]
    private GameObject enemyPrefab, bossPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartEnemySpawn()
    {
        //Get the count of our rooms to loop through start spawning our player and enemies
        foreach (Room room in RoomManager.Instance.Rooms)
        {
            //Start generating the locations for our dictionary to be used in our pathfinder
            SpawnValidatorAlgorithm spawnGraph = new SpawnValidatorAlgorithm(room.FloorTiles);

            //Grab only the rooms tiles WITHOUT intersecting the corridor paths. 

            HashSet<Vector2Int> floors = new HashSet<Vector2Int>(room.FloorTiles);
            floors.IntersectWith(RoomManager.Instance.Corridors);

            //Run the BFS to find all the tiles in the room accessible from the path
            Dictionary<Vector2Int, Vector2Int> floorMap = spawnGraph.FindSpawnLocations(floors.First(), room.PropPositions);

            //Positions that we can reach + path == positions where we can place enemies
            room.PossibleSpawnPostions = floorMap.Keys.OrderBy(x => Guid.NewGuid()).ToList();

            room.PossibleSpawnPostions.Remove(room.RoomCenter);

            InitEnemySpawn(room);

            if (room.roomType == Room.RoomType.Entrance)
            {
                PlayerManager.Instance.SpawnPlayer();
                PlayerManager.Instance.SetPlayerPosition(room.RoomCenter + Vector2.one * 0.5f);
                PlayerManager.Instance.SetPlayerCamera();

                PlayerManager.Instance.SetPlayerInDungeon(true);
            }

            if (room.roomType == Room.RoomType.Exit)
            {
                ExitPoint.Instance.SpawnExitPoint();
                ExitPoint.Instance.SetExitPosition(room.RoomCenter + Vector2.one * 0.5f);
            }
        }

        AStarEditor.Instance.ResizeGraph(DungeonGenerator.Instance.GetDungeonWidth(),
           DungeonGenerator.Instance.GetDungeonWidth(), RoomManager.Instance.GetRoomCenters());
        GameManager.Instance.SaveDungeon();

    }

    private void InitEnemySpawn(Room room)
    {
        int playerLevel = PlayerManager.Instance.Level;

        switch (room.roomType)
        {
            case Room.RoomType.Normal:
                // Spawn a random number of enemies based on the player's level
                int numEnemies = Random.Range(playerLevel, playerLevel + 3);
                SpawnEnemiesInRoom(enemyPrefab, room.PossibleSpawnPostions, room, numEnemies);
                break;
            case Room.RoomType.Important:
                // Spawn more enemies than normal rooms
                int numImportantEnemies = Random.Range(playerLevel + 2, playerLevel + 5);
                SpawnEnemiesInRoom(enemyPrefab, room.PossibleSpawnPostions, room, numImportantEnemies);
                break;
            case Room.RoomType.Exit:
                // GameObject boss = Instantiate(bossEnemyPrefab, room.RoomCenter + Vector2.one * 0.8f, Quaternion.identity);
                int numExitEnemies = Random.Range(playerLevel + 2, playerLevel + 8);
                SpawnEnemiesInRoom(enemyPrefab, room.PossibleSpawnPostions, room, numExitEnemies);
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

    void SpawnEnemiesInRoom(GameObject enemyPrefab, List<Vector2Int> possibleSpawnPositions, Room room, int numEnemies)
    {
        List<EnemyObject> possibleEnemies = EnemyManager.Instance.enemyDataList;

        for (int i = 0; i < numEnemies; i++)
        {
            if (possibleSpawnPositions.Count <= i)
            {
                return;
            }

            // Spawn enemy prefab
            GameObject enemy = Instantiate(enemyPrefab, possibleSpawnPositions[i] + Vector2.one * 0.5f, Quaternion.identity);
            Enemy_Logan enemyScript = enemy.GetComponent<Enemy_Logan>();
            enemyScript.SetPropertiesFromObjectData(possibleEnemies[Random.Range(0, possibleEnemies.Count)], 
                enemy.GetComponent<Animator>(), enemy.GetComponent<SpriteRenderer>(), enemy.GetComponentInChildren<AttackChecker>().gameObject);
            AddEnemy(enemy);

            // Save our enemy spawn positions, so we can spawn other items, such as portals or other things.
            room.EnemySpawnPositions.Add(possibleSpawnPositions[i]);
        }
    }

    internal void AddEnemy(GameObject enemy)
    {
        allEnemies.Add(enemy);
    }

    internal void RemoveEnemy(GameObject enemy)
    {
        allEnemies.Remove(enemy);
    }

    public void LoadEnemies(EnemyData enemyData)
    {
        // Spawn enemies from the enemy data
        foreach (EnemyState enemyState in enemyData.enemies)
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Enemies/EnemyPrefab");
            GameObject enemyObject = Instantiate(enemyPrefab, enemyState.position, Quaternion.identity);
            
            Enemy_Logan enemy = enemyObject.GetComponent<Enemy_Logan>();
            enemy.SetPropertiesFromState(enemyState, enemyObject.GetComponent<Animator>(), enemyObject.GetComponent<SpriteRenderer>(),
                            enemyObject.GetComponentInChildren<AttackChecker>().gameObject);
            AddEnemy(enemyObject);
        }
        AStarEditor.Instance.ResizeGraph(DungeonGenerator.Instance.GetDungeonWidth(),
          DungeonGenerator.Instance.GetDungeonWidth(), RoomManager.Instance.GetRoomCenters());
    }

    public void Reset()
    {
        foreach (var enemy in allEnemies)
        {
            Destroy(enemy);
        }
    }

    public EnemyData GetEnemyData()
    {
        EnemyData enemyData = new EnemyData();

        // Populate the enemy data fields from the enemies list
        enemyData.enemies = new List<EnemyState>();
        foreach (var enemy in allEnemies)
        {
            EnemyState enemyState = new EnemyState();
            EnemyObject enemyObjectData = enemy.GetComponent<Enemy_Logan>().GetEnemyObjectData();
            enemyState.sprite = enemyObjectData.sprite;
            enemyState.animatorController = enemyObjectData.animatorController;
            enemyState.health = enemyObjectData.health;
            enemyState.position = enemy.gameObject.transform.position;
            enemyState.prefabName = enemy.gameObject.name;
            enemyState.speed = enemyObjectData.speed;
            enemyState.attackRate = enemyObjectData.attackRate;
            enemyState.attackRange = enemyObjectData.attackRange;
            enemyState.nextAttackTime = enemyObjectData.nextAttackTime;
            enemyData.enemies.Add(enemyState);
        }

        return enemyData;
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

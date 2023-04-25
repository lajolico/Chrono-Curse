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
                PlayerManager.Instance.SetAttackDamage(PlayerManager.Instance.GetLevel());
                PlayerManager.Instance.SetPlayerHealthPerLevel(PlayerManager.Instance.GetLevel());
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
        //AStarEditor.Instance.SaveGraphData();
        //GameManager.Instance.SaveDungeon();

    }

    private void InitEnemySpawn(Room room)
    {
        int playerLevel = PlayerManager.Instance.currentLevel;

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
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.SetAttackDamage(PlayerManager.Instance.currentLevel);
            enemyScript.SetPropertiesFromObjectData(possibleEnemies[Random.Range(0, possibleEnemies.Count)], 
                enemy.GetComponent<Animator>(), enemy.GetComponentInChildren<AttackChecker>().gameObject);
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
            
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            enemy.SetPropertiesFromState(enemyState, enemyObject.GetComponent<Animator>(),
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
            EnemyObject enemyObjectData = enemy.GetComponent<Enemy>().GetEnemyObjectData();
            enemyState.sprite = enemyObjectData.sprite;
            enemyState.animatorController = enemyObjectData.animatorController;
            enemyState.health = enemyObjectData.health;
            enemyState.position = enemy.gameObject.transform.position;
            enemyState.prefabName = enemy.gameObject.name;
            enemyState.speed = enemyObjectData.speed;
            enemyState.attackDamage = enemy.GetComponent<Enemy>().GetAttackDamage();
            enemyState.attackRate = enemyObjectData.attackRate;
            enemyState.attackRange = enemyObjectData.attackRange;
            enemyState.nextAttackTime = enemyObjectData.nextAttackTime;
            enemyData.enemies.Add(enemyState);
        }

        return enemyData;
    }
}


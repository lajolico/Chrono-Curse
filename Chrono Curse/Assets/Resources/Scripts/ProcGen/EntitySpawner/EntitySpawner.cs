using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;
using System;
using UnityEngine.UIElements;

public class EntitySpawner : MonoBehaviour
{


    [SerializeField]
    private GameObject enemyPrefab, playerPrefeb;

    [SerializeField]
    private int playerRoomIndex = 0;
    [SerializeField]
    private CinemachineVirtualCamera vCamera;

    RoomManager roomManager;

    [SerializeField]
    private bool showGizmo = false;

    private GameObject player;
    
     
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
        for (int i = 0; i < roomManager.Rooms.Count; i++)
        {

            Room room = roomManager.Rooms[i];

            HashSet<Vector2Int> positions = GetSpawnPostions(room);

            room.SpawnPositions = positions.OrderBy(x => Guid.NewGuid()).ToList();

            if (i != playerRoomIndex)
            {
                PlaceEnemies(room, 5);
            }

            if (i == playerRoomIndex){
                Debug.Log(i + " " + room.EnemiesInRoom.Count);

                player = Instantiate(playerPrefeb);
                player.transform.localPosition = room.RoomCenter + Vector2.one * 0.5f;
                vCamera.Follow = player.transform;
                vCamera.LookAt = player.transform;
                roomManager.PlayerReference = player;
             }
        }
    }


    private void PlaceEnemies(Room room, int amount)
    {
        for (int i = 0; i < room.SpawnPositions.Count; i++)
        {
            if (room.EnemiesInRoom.Count >= amount)
            { 
               return;
            } 

            if (!IsValidSpawnPosition(room.SpawnPositions[i], room))
            {
                return;
            }

            GameObject enemy = Instantiate(enemyPrefab);
            enemy.transform.localPosition = room.RoomCenter + Vector2.one * 0.5f;
            room.EnemiesInRoom.Add(enemy);
        }
    }

    private bool IsValidSpawnPosition(Vector2Int spawnPosition, Room room)
    {
        // Check if the spawn position is on top of another enemy
        foreach (GameObject enemy in room.EnemiesInRoom)
        {
            Vector2Int enemyPosition = new Vector2Int(Mathf.RoundToInt(enemy.transform.position.x), Mathf.RoundToInt(enemy.transform.position.y));

            Vector2Int playerPosition = new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));

            if (Vector2Int.Distance(spawnPosition, enemyPosition) < 2)
            {
                return false;
            }

            if (Vector2Int.Distance(spawnPosition, playerPosition) < 1)
            {
                return false;
            }

        }

        // Check if the spawn position is on top of or next to a prop
        foreach (GameObject prop in room.PropListReference)
        {
            Vector2Int propPosition = new Vector2Int(Mathf.RoundToInt(prop.transform.position.x), Mathf.RoundToInt(prop.transform.position.y));

            if (Vector2Int.Distance(spawnPosition, propPosition) < 2)
            {
                return false;
            }

            if (Mathf.Abs(spawnPosition.x - propPosition.x) <= 1 && Mathf.Abs(spawnPosition.y - propPosition.y) <= 1)
            {
                return false;
            }
        }



        // If the spawn position is not on top of or next to any other objects, it is a valid spawn position
        return true;
    }

    private HashSet<Vector2Int> GetSpawnPostions(Room room)
    {
        HashSet<Vector2Int> spawnPositions = new HashSet<Vector2Int>(room.FloorTiles);
        spawnPositions.ExceptWith(room.PropPositions);
        
        return spawnPositions;
    }

    

    private void OnDrawGizmosSelected()
    {
        if (roomManager == null || showGizmo == false)
            return;
        foreach (Room room in roomManager.Rooms)
        {
            Color color = Color.green;
            color.a = 0.3f;
            Gizmos.color = color;

            foreach (Vector2Int pos in room.SpawnPositions)
            {
                Gizmos.DrawCube((Vector2)pos + Vector2.one * 0.5f, Vector2.one);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



/// <summary>
/// Used to hold data for every room generated in our world.
/// </summary>
public class Room
{
    //Where are our rooms located at? Specifically used for finding different rooms in relation to eachother.
    public Vector2Int RoomCenter { get; private set; }
    
    //Get our floor tiles in a specific room
    public HashSet<Vector2Int> FloorTiles { get; private set; } = new HashSet<Vector2Int>();

    //Keep track of all of our tiles in a specific room and where they are located.

    //Tiles next to the walls on the right
    public HashSet<Vector2Int> TilesNearRightSide { get; private set; } = new HashSet<Vector2Int>();

    //Tiles next to the walls on the left
    public HashSet<Vector2Int> TilesNearLeftSide { get; private set; } = new HashSet<Vector2Int>();

    //Tiles next to the walls on the top of the room
    public HashSet<Vector2Int> TilesNearUpperSide { get; private set; } = new HashSet<Vector2Int>();

    //Tiles next to the walls on bottom of the room
    public HashSet<Vector2Int> TilesNearBottomSide { get; private set; } = new HashSet<Vector2Int>();

    //Tiles that have more than 4 neighbors, they are free, and do not touch empty tiles or walls
    public HashSet<Vector2Int> TilesInsideRoom { get; private set; } = new HashSet<Vector2Int>();

    public HashSet<Vector2Int> WallsTop { get; private set; } = new HashSet<Vector2Int>();


    //Track our prop locations and data;
    public HashSet<Vector2Int> PropPositions { get; private set; } = new HashSet<Vector2Int>();
    public List<GameObject> PropListReference { get; private set; } = new List<GameObject>();

    //Used for entity spawn locations, i.e. exit portal, enemies, and the player
    public List<Vector2Int> PossibleSpawnPostions { get; set; } = new List <Vector2Int>();

    //Holds the about of enemies in each room and what type
    public List<GameObject> EnemiesInRoom { get; private set; } = new List<GameObject>();
    public HashSet<Vector2Int> EnemySpawnPositions { get; private set; } = new HashSet<Vector2Int>();

    //Amount of enemies we want in rooms that don't have a set number of enemies
    public int NumberOfEnemiesPerRoom { get; set; } = Random.Range(1, 5);


    //Constructor to be used in other portions of our scripts, specifically Dungeon Generator
    public Room(Vector2Int roomCenter, HashSet<Vector2Int> floorTilePositions)
    {
        this.RoomCenter = roomCenter;
        this.FloorTiles = floorTilePositions;
    }
}

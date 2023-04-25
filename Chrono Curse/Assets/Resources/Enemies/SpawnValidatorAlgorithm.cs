using System;
using System.Collections;
using System.Collections.Generic;
 
using UnityEngine;


 
/// <summary>
/// The purpose of this class is to find spawn locations for our entities (enemies) and other objects
/// </summary>
public class SpawnValidatorAlgorithm
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

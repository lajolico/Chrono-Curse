using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//Author: Logan Jolicoeur
//Date: 1/21/2023
//Purpose: Houses the procedural generation and walking methods, the main generation happens here.

public class MapGenerator : AbstractDungeons
{
    [SerializeField]
    protected DungeonMaker dungeonParams;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPositions = GetFloorPositions(dungeonParams, startPos);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallLogic.CreateWalls(floorPositions, tilemapVisualizer);
    }

    protected HashSet<Vector2Int> GetFloorPositions(DungeonMaker parameters, Vector2Int position)
    {
        var currentPos = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for(int i = 0; i < parameters.iterations; i++)
        {
            var path = ProcedureAlgorithm.GetPath(currentPos, parameters.walkLength);
            floorPositions.UnionWith(path); //remove dupes and ensure O(n)

            if(parameters.changePosPerIteration)
            {
                currentPos = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }



}

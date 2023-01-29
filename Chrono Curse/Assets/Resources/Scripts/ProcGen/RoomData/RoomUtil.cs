using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//Author: Logan Jolicoeur
//Date: 1/21/2023
//Purpose: Assists with Room Generation, then saves it as a scriptable object to be used in development of ProcGen

public class RoomUtil : AbstractDungeons
{
    [SerializeField]
    protected RoomMaker roomParams;

    protected override void RunProceduralGeneration()
    { 
        HashSet<Vector2Int> floorPositions = GetFloorPositions(roomParams, startPos);
        tilemapUtil.PaintFloorTiles(floorPositions);
        
    }

    /// <summary>
    /// Get our floor positions that will help with placing our walls and other objects
    /// </summary>
    /// <param name="parameters">From our roomMaker Script Object</param>
    /// <param name="position">Position in Vector2 Space</param>
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

}

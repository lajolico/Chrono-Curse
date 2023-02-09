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
        tilemapUtil = FindObjectOfType<TilemapUtil>();
        tilemapUtil.Clear();

        HashSet<Vector2Int> floorPositions = ProcedureAlgorithm.GetFloorPositions(roomParams, startPos);
        tilemapUtil.PaintFloorTiles(floorPositions);

    }
}

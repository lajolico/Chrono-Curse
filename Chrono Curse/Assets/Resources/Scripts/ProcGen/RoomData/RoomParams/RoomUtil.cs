using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//Author: Logan Jolicoeur
//Date: 1/21/2023
//Purpose: Assists with Room Generation, then saves it as a scriptable object to be used in development of ProcGen

public class RoomUtil : MonoBehaviour
{

    [SerializeField]
    private TilemapUtil tilemapUtil; 

    [SerializeField]
    private RoomMaker roomParams;

    private void Awake()
    {
        tilemapUtil = FindObjectOfType<TilemapUtil>();
        if (tilemapUtil == null)
        {
            tilemapUtil = gameObject.AddComponent<TilemapUtil>();
        }
    }

    public void RunProceduralGeneration()
    {
        tilemapUtil.Clear();

        HashSet<Vector2Int> floorPositions = ProcedureAlgorithm.GetFloorPositions(roomParams, Vector2Int.zero);
        tilemapUtil.PaintFloorTiles(floorPositions);

    }
}

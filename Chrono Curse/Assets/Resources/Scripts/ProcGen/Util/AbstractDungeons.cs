using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeons : MonoBehaviour
{
    [SerializeField]
    protected TilemapUtil tilemapUtil = null;

    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;

    public void GenerateDungeon()
    {
        tilemapUtil.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();

    protected abstract HashSet<Vector2Int> GetFloorPositions(RoomMaker parameters, Vector2Int position);
}   

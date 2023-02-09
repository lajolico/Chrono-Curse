using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeons : MonoBehaviour
{

    protected TilemapUtil tilemapUtil = null;

    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;

    public void GenerateDungeon()
    {
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}   

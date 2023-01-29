using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

//Author: Logan Jolicoeur
//Date: 1/21/2023


/// <summary>
/// Deals with painting our tilemap
/// </summary>
public class TilemapUtil : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTileMap, wallTileMap;

    [SerializeField]
    private TileBase roomTile, floorTile, wallRight, wallLeft, wallBottom, wallFull,
         wallInnerCornerDownLeft, wallInnerCornerDownRight,
         wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    [SerializeField]
    private List<TileBase> wallFront = new List<TileBase>();

    //Helper function that allows other classes to pass their floorPositions to get painted
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTileMap, floorTile);
    }

    // Method in charge of painting out layered tiles to the TileMap
    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tileMap, TileBase tile)
    {
        foreach(var position in positions)
        {
            PaintSingleTile(tileMap, tile, position);
        }
    }

    internal void PaintSingleWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallUp.Contains(typeAsInt))
        {
            tile = wallFront.ElementAt(Random.Range(0, wallFront.Count-1));
        }
        else if (WallTypesHelper.wallRight.Contains(typeAsInt))
        {
            tile = wallRight;
        }
        else if (WallTypesHelper.wallLeft.Contains(typeAsInt))
        {
            tile = wallLeft;
        }
        else if (WallTypesHelper.wallDown.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }

        if (tile != null)
            PaintSingleTile(wallTileMap, tile, position);

    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeASInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottomEightDirections.Contains(typeASInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingleTile(wallTileMap, tile, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
    }
}

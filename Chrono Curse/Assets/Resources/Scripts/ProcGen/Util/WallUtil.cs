using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUtil : MonoBehaviour
{

    private static RoomManager roomManager = FindObjectOfType<RoomManager>();

    /// <summary>
    /// Other scripts will pass their floorPostions and the tilemaps we want to use to locate and find postions for our walls.
    /// </summary>
    /// <param name="floorPositions"></param>
    /// <param name="tilemap">Tilemap we are passing</param>
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapUtil tilemap)
    {
        var wallPositions = GetWallPositions(floorPositions, DirectionUtil.eightDirectionsList);
        var cornerWallPositions = GetWallPositions(floorPositions, DirectionUtil.diagonalDirectionsList);
        SetWall(tilemap, wallPositions, floorPositions);
        SetCornerWalls(tilemap, cornerWallPositions, floorPositions);
        roomManager.Walls.UnionWith(wallPositions);
        roomManager.Walls.UnionWith(cornerWallPositions);
    }

    /// <summary>
    /// This creates a wall, that is not a corner. Using a 1 to 0 method, it will generate a binary output, which is used to match against a datastructure of binary ints
    /// Holding which tile is doing what.
    /// </summary>
    /// <param name="tilemapVisualizer">tileMaps we want to paint to. </param>
    /// <param name="wallPositions">Where are the walls located on our tileMap. </param>
    /// <param name="floorPositions">Where are the floors?!</param>
    private static void SetWall(TilemapUtil tilemap, HashSet<Vector2Int> wallPositions, HashSet<Vector2Int> floorPositions)
     {
         foreach (var position in wallPositions)
         {
             string neighboursBinaryType = "";
             foreach (var direction in DirectionUtil.cardinalDirections)
             {
                 var neighbourPosition = position + direction;
                 if (floorPositions.Contains(neighbourPosition))
                 {
                     neighboursBinaryType += "1";
                 }
                 else
                 {
                     neighboursBinaryType += "0";
                 }
             }
             tilemap.PaintSingleWall(position, neighboursBinaryType);
         }
     }

    /// <summary>
    /// Sets the wall corners on our generated map, will get the position and direction of said tile and check it against 
    /// A Wall Corner List, if it matches, it will be painted that tile.
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="cornerWallPositions"></param>
    /// <param name="floorPositions"></param>
    private static void SetCornerWalls(TilemapUtil tilemap, HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in DirectionUtil.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            tilemap.PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    /// <summary>
    /// Loop through all of our floorPostions to find possible wall locations, using postion and offset of it's direction.
    /// </summary>
    /// <param name="floorPositions">Where are the floors at ?!</param>
    /// <param name="directionList">Check in every direction from the said tile in the loop.</param>
    /// <returns>Returns a wallPosition HashSet of all viable wall positions to recieve a tiled Wall</returns>
    private static HashSet<Vector2Int> GetWallPositions(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach(var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if(!floorPositions.Contains(neighbourPosition))
                {
                    wallPositions.Add(neighbourPosition);
                }
            }
           
        }

        return wallPositions;
    }
}

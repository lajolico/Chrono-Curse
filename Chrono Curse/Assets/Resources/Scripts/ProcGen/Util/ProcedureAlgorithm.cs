using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//Author: Logan Jolicoeur
//Date: 1/21/2023
//Purpose: The main alogriths used to assist in ProcGen for our world

public static class ProcedureAlgorithm
{
    /// <summary>
    /// Random Walk algorithm, will walk at a start pos and a set walkLength, given certain iterations
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="walkLength"></param>
    /// <returns></returns>
    public static HashSet<Vector2Int> GetPath(Vector2Int startPos, int walkLength)
    {
        //generate the path to other rooms
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        //Add our starting position to the HashSet and begin there
        path.Add(startPos);
        var prevPos = startPos;
        
        //walk through 0 to the walklength
        for(int i = 0; i < walkLength; i++)
        {
            //as we walk get a new position and add it the previous position randomly
            var newPos = prevPos + DirectionUtil.GetRandomDirection();
            path.Add(newPos); //Add it to our HashSet
            prevPos = newPos; //update our position to the algorithm
        }

        return path; 
    }

    /// <summary>
    /// Creates our corridors, connecting the different rooms.
    /// </summary>
    /// <param name="currentRoomCenter">Recieves roomCenter from </param>
    /// <param name="destination">Where are we going!?</param>
    /// <param name="corridorWidth">How big is this corridor going to be?</param>
    /// <returns></returns>
    public static HashSet<Vector2Int> GetCorridors(Vector2Int currentRoomCenter, Vector2Int destination, int corridorWidth)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            for (int k = 0; k < corridorWidth; k++)
            {
                for (int j = 0; j < corridorWidth; j++)
                {
                    var offset = new Vector2Int(k, j);

                    corridor.Add(position + offset);
                }
            }
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            for (int k = 0; k < corridorWidth; k++)
            {
                for (int j = 0; j < corridorWidth; j++)
                {
                    var offset = new Vector2Int(k, j);

                    corridor.Add(position + offset);
                }
            }
        }

        return corridor;
    }
    /// <summary>
    /// Assist in the generation of rooms, by splitting them from a big room into smaller ones.
    /// FIFO Algorithm, that deals with first room, spits back out, rinse and repeat.
    /// </summary>
    /// <param name="spaceToSplit"></param>
    /// <param name="minWidth"></param>
    /// <param name="minHeight"></param>
    /// <returns></returns>
    public static List<BoundsInt> BSP(BoundsInt spaceToSplit, int minWidth, int minHeight)
    {
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
        List<BoundsInt> roomsList = new List<BoundsInt>();
        roomsQueue.Enqueue(spaceToSplit);
        while(roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();
            if(room.size.y >= minHeight && room.size.x >= minHeight) //Has room to split
            {
                var chance = Random.value;
               
                if(room.size.y >= minHeight * 2 && chance < 0.5f)
                {
                    SplitHorizontally(minHeight, roomsQueue, room);
                }
                else if(room.size.x >= minWidth * 2 && chance > 0.5f)
                {
                    SplitVertically(minWidth, roomsQueue, room);
                }
                else{
                    roomsList.Add(room);
                }
            }

        }
        return roomsList;

    }

    /// <summary>
    /// BSP Helper function, split a room Vertically
    /// </summary>
    /// <param name="minWidth"></param>
    /// <param name="roomsQueue"></param>
    /// <param name="room"></param>
    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var xSplit = Random.Range(1, room.size.x);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
            new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// BSP Helper function, split a room horizontally
    /// </summary>
    /// <param name="minHeight"></param>
    /// <param name="roomsQueue"></param>
    /// <param name="room"></param>
    private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room)
    {
        var ySplit = Random.Range(1, room.size.y);
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
            new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
        roomsQueue.Enqueue(room1);
        roomsQueue.Enqueue(room2);
    }

    /// <summary>
    /// Get our floor positions that will help with placing our walls and other objects
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public static HashSet<Vector2Int> GetFloorPositions(RoomMaker parameters, Vector2Int position)
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

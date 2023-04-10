using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DistanceUtil
{
    /*
        Find the closest dungeon room point to that specific room. Loop through the roomCenters
        Return the point back to ConnectRooms, where it will continue the algorithm
    */
    public static Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Author: Logan Jolicoeur
//Date: 1/22/2023

//Purpose of this script is to generate Scriptable Object dungeons which can be saved or loaded in-game
//and to help with development


[CreateAssetMenu(fileName="Parameters_",menuName = "ChronoCurse/RoomMaker")]
public class RoomMaker : ScriptableObject
{
    public int iterations = 10, walkLength = 10;
    public bool changePosPerIteration = false;
}

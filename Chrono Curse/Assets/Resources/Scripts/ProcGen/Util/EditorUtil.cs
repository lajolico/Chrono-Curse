using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Assists in the making of Scriptable Dungeons, that can be premade with iterations and different walk lengths.
[CustomEditor(typeof(DungeonGenerator), true)]

public class EditorUtil : Editor
{
    DungeonGenerator generator;
    RoomUtil roomGenerator;

    private void Awake()
    {
        if (target is DungeonGenerator)
        {
            generator = (DungeonGenerator)target;
        }
        else if (target is RoomUtil)
        {
            roomGenerator = (RoomUtil)target;
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            if (generator != null)
            {
                generator.GenerateDungeon();
            }
            else if (roomGenerator != null)
            {
                roomGenerator.RunProceduralGeneration();
            }
        }
    }
}

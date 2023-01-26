using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Assists in the making of Scriptable Dungeons, that can be premade with iterations and different walk lengths.
[CustomEditor(typeof(AbstractDungeons), true)]


public class DungeonUtil : Editor
{
    AbstractDungeons generator;

    private void Awake()
    {
        generator = (AbstractDungeons) target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate"))
        {
            generator.GenerateDungeon();
        }
    }
}

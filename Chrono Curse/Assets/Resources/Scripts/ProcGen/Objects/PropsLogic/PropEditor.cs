using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PropManager), true)]

public class PropEditor : Editor
{
    PropManager generator;

    private void Awake()
    {
        generator = (PropManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Place"))
        {
            generator.InitProps();
        }
    }
}

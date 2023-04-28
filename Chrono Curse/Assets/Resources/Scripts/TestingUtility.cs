// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEditor;
// using UnityEngine.Events;

// [CustomEditor(typeof(DevTileMapping), true)]
// public class TestingUtility : Editor
// {

//     // Script that will be referenced
//     DevTileMapping testUtil;

//     private void Awake()
//     {
//         if (target is DevTileMapping)
//         {
//             testUtil = (DevTileMapping)target;
//         }
//     }

//     public override void OnInspectorGUI()
//     {
//         // Overrides script to call method from inspector button
//         base.OnInspectorGUI();
//         if (GUILayout.Button("Please"))
//         {
//             if (testUtil != null) // Makes sure function exists
//             {
//                 testUtil.OnButtonClicked();
//             }
//         }
//     }
// }
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(RecycleGridLayout))]
public class RecycleGridLayout_Editor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var gridLayout = (RecycleGridLayout)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Init", GUILayout.Width(120), GUILayout.Height(40)))
        {
            Debug.Log("Init");
            gridLayout.Init();
        }
        if (GUILayout.Button("Clear", GUILayout.Width(120), GUILayout.Height(40)))
        {
            Debug.Log("Clear");
            gridLayout.Clear();
        }
        if (GUILayout.Button("Refresh", GUILayout.Width(120), GUILayout.Height(40)))
        {
            Debug.Log("refresh");
        }
        EditorGUILayout.EndHorizontal();
    }
}

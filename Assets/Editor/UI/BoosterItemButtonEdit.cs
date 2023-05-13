using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoosterItemButton))]
public class BoosterItemButtonEdit : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }    
    }
}

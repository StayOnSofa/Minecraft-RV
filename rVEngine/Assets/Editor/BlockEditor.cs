using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BlockUtils;

[CustomEditor(typeof(Block), true)]
public class BlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Register"))
        {
            BlockScriptGenerator.Register();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CombineMeshFracturedParts))]
public class CombineMeshFracturedPartsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CombineMeshFracturedParts myScript = (CombineMeshFracturedParts)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("CombineMeshes"))
        {
            Debug.Log("CombineMeshes");
            myScript.CombineMeshes();
        }

        if (GUILayout.Button("ClearObjMeshArray"))
        {
            Debug.Log("ClearObjMeshArray");
            myScript.ClearObjMeshArray();
        }

        if (GUILayout.Button("AddAllChildrenToObjMeshArray"))
        {
            Debug.Log("AddAllChildrenToObjMeshArray");
            myScript.AddAllChildrenToObjMeshArray();
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(MeshFilter))]
public class MFSaveMeshEditor : Editor
{
    string fileName = "MeshTemp";
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MeshFilter myScript = (MeshFilter)target;

        EditorGUILayout.Space();
        fileName = EditorGUILayout.TextField("File name", fileName);

        if (GUILayout.Button("Save mesh"))
        {
            var savePath = "Assets/" + fileName + ".asset";
            Debug.Log("Saved Mesh to:" + savePath);
            AssetDatabase.CreateAsset(myScript.mesh, savePath);
        }
    }
}
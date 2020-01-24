using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshFracturedParts : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objMeshArray = new List<GameObject>();

    [SerializeField]
    private Mesh CombinedMesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector4> uvs = new List<Vector4>();

    private List<Vector3> verticesTemp = new List<Vector3>();
    private List<int> trianglesTemp = new List<int>();

    private List<Vector4> uvsV4Temp = new List<Vector4>();

    private List<Vector3> normals = new List<Vector3>();
    private List<Vector3> normalsTemp = new List<Vector3>();

    public void AddAllChildrenToObjMeshArray()
    {
        if (objMeshArray == null)
        {
            objMeshArray = new List<GameObject>();
        }
        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<MeshFilter>() != null)
            {
                objMeshArray.Add(child.gameObject);
            }
        }
    }

    public void ClearObjMeshArray()
    {
        objMeshArray.Clear();
    }

    /// <summary>
    /// Combines shard meshes. And writes shard offset to UV with uvChannelNum.
    /// </summary>
    /// <param name="uvChannelNum"></param>
    public void CombineMeshes(int uvChannelNum = 3)
    {
        CombinedMesh = new Mesh();

        CombinedMesh.name = "Combined Mesh";

        vertices.Clear();
        uvs.Clear();
        normals.Clear();

        for (int i = 0; i < objMeshArray.Count; i++)
        {
            MeshFilter meshFilter = objMeshArray[i].GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                verticesTemp.Clear();
                normalsTemp.Clear();
                //uvsV4Temp.Clear();

                Vector3 translation = objMeshArray[i].transform.localPosition;
                Quaternion rotation = objMeshArray[i].transform.localRotation;
                Vector3 scale = objMeshArray[i].transform.localScale;

                // Set the translation, rotation and scale parameters.
                Matrix4x4 matrix = Matrix4x4.TRS(translation, rotation, scale);
                Matrix4x4 matrixRotation = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

                int subMeshCount = meshFilter.sharedMesh.subMeshCount;
                
                int currentVertexCount = vertices.Count;

                meshFilter.sharedMesh.GetVertices(verticesTemp);
                //Do not use textures in this example - so do not combine uvs. If you use textures - need to combine uvs also.
                //meshFilter.sharedMesh.GetUVs(uvChannelNum, uvsV4Temp);
                meshFilter.sharedMesh.GetNormals(normalsTemp);

                for (int j = 0; j < verticesTemp.Count; j++)
                {
                    Vector3 transformedPos = matrix.MultiplyPoint3x4(verticesTemp[j]);
                    vertices.Add(transformedPos);
                    normals.Add(matrixRotation.MultiplyPoint3x4(normalsTemp[j]));

                    uvs.Add(objMeshArray[i].transform.localPosition);

                    Vector4 uvVec = uvs[j + currentVertexCount];
                    uvs[j + currentVertexCount] = uvVec;                    
                }

                for (int subMeshIndex = 0; subMeshIndex < subMeshCount; subMeshIndex++)
                {
                    trianglesTemp.Clear();
                    meshFilter.sharedMesh.GetTriangles(trianglesTemp, subMeshIndex);

                    for (int triangleIndex = 0; triangleIndex < trianglesTemp.Count; triangleIndex++)
                    {
                        triangles.Add(trianglesTemp[triangleIndex] + currentVertexCount);
                    }
                }
            }
        }

        CombinedMesh.SetVertices(vertices);
        CombinedMesh.SetNormals(normals);
        CombinedMesh.SetTriangles(triangles, 0);

        CombinedMesh.SetUVs(uvChannelNum, uvs);

        CombinedMesh.RecalculateBounds();
        
        if (GetComponent<MeshFilter>() != null)
        {
            GetComponent<MeshFilter>().mesh = CombinedMesh;
        }
    }

}

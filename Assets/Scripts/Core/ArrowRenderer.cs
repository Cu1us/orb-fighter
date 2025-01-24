using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ArrowRenderer : MonoBehaviour
{
    [SerializeField][HideInInspector] MeshFilter meshRenderer;

    public float stemHalfWidth;
    public float stemLength;
    public float tipProtrusion;
    public float tipLength;

    // void Update()
    // {
    //     meshRenderer.mesh = GenerateMesh();
    // }

    public void DisplayVector(Vector2 vector)
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.right, vector.normalized);
        stemLength = Mathf.Max(vector.magnitude - tipLength, 0);
        meshRenderer.mesh = GenerateMesh();
    }

    Mesh GenerateMesh()
    {
        Mesh mesh = new();
        Vector3[] vertices = new Vector3[7];
        int[] triangles = new int[9];

        GenerateStem(ref vertices, ref triangles);
        GenerateTip(ref vertices, ref triangles);

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

    void GenerateStem(ref Vector3[] vertices, ref int[] triangles)
    {
        vertices[0] = Vector3.up * stemHalfWidth;
        vertices[1] = Vector3.down * stemHalfWidth;
        vertices[2] = vertices[0] + Vector3.right * stemLength;
        vertices[3] = vertices[1] + Vector3.right * stemLength;

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 1;
    }
    void GenerateTip(ref Vector3[] vertices, ref int[] triangles)
    {
        vertices[4] = vertices[2] + Vector3.up * tipProtrusion;
        vertices[5] = vertices[3] + Vector3.down * tipProtrusion;
        vertices[6] = Vector3.right * (stemLength + tipLength);

        triangles[6] = 4;
        triangles[7] = 6;
        triangles[8] = 5;
    }

    [ContextMenu("Reset private component references")]
    void Reset()
    {
        meshRenderer = GetComponent<MeshFilter>();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

/* It's a class that renders a chunk of the world */
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;
    public bool showGizmo = false;

    public ChunkData ChunkData;

    public bool ModifiedByThePlayer
    {
        get => ChunkData.modifiedByThePlayer;
        set => ChunkData.modifiedByThePlayer = value;
    }

/// <summary>
/// `Awake()` is a function that is called when the object is created. 
/// In this case, we are using it to get the mesh filter and mesh collider components of the object. 
/// We also get the mesh from the mesh filter. 
/// </summary>
    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

/// <summary>
/// This function is called by the ChunkManager when it creates a new chunk. It passes the chunk data to
/// the chunk, which the chunk then uses to create the chunk
/// </summary>
/// <param name="data">This is the data that will be used to generate the chunk.</param>
    public void InitializeChunk(ChunkData data)
    {
        ChunkData = data;
    }

/// <summary>
/// Mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();
/// 
/// The above line of code is the only line that needs to be changed to make the water mesh render.
/// 
/// The rest of the code is just to make the water mesh render correctly.
/// The water mesh is rendered as a second sub mesh.
/// The water mesh is rendered as a second sub mesh because the water mesh is rendered with a different
/// shader than the terrain mesh.
/// </summary>
/// <param name="meshData">This is the class that contains all the data for the mesh.</param>
    private void RenderMesh(MeshData meshData)
    {
        mesh.Clear();

        mesh.subMeshCount = 2;
        mesh.vertices = meshData.vertexList.Concat(meshData.waterSubMeshData.vertexList).ToArray();

        mesh.SetTriangles(meshData.triangleList.ToArray(), 0);
        mesh.SetTriangles(meshData.waterSubMeshData.triangleList.Select(val => val + meshData.vertexList.Count).ToArray(), 1);

        mesh.uv = meshData.uvList.Concat(meshData.waterSubMeshData.uvList).ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new Mesh();
        collisionMesh.vertices = meshData.colliderVertexList.ToArray();
        collisionMesh.triangles = meshData.colliderTriangleList.ToArray();
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh;
    }

/// <summary>
/// It takes the data from the chunk and renders it
/// </summary>
    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

/// <summary>
/// "This function updates the mesh of the chunk with the data provided."
/// </summary>
/// <param name="data">The mesh data that will be used to update the mesh.</param>
    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

/// <summary>
/// If the chunk is selected in the editor, draw a green cube, otherwise draw a pink cube
/// </summary>
#if UNITY_EDITOR
    private void OnDrawGizmos()
{
    if (!showGizmo) return;
    
    if (!Application.isPlaying || ChunkData == null) return;
    
    Gizmos.color = Selection.activeObject == gameObject ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 1, 0.4f);

    Gizmos.DrawCube(transform.position + new Vector3(ChunkData.chunkSize / 2f, ChunkData.chunkHeight / 2f, ChunkData.chunkSize / 2f), new Vector3(ChunkData.chunkSize, ChunkData.chunkHeight, ChunkData.chunkSize));
}
#endif
}

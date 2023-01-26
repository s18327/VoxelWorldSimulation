using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Serialization;

/* It's a class that renders a chunk of the Terrain */
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private UnityEngine.Mesh mesh;
    

    public Chunk chunk;
    public bool showGizmo;
    public bool IsPlayerModified
    {
        set => chunk.isPlayerModified = value;
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
    public void InitializeChunk(Chunk data)
    {
        chunk = data;
    }

/// <summary>
/// Mesh.vertices = mesh.vertices.Concat(mesh.waterMesh.vertices).ToArray();
/// 
/// The above line of code is the only line that needs to be changed to make the water mesh render.
/// 
/// The rest of the code is just to make the water mesh render correctly.
/// The water mesh is rendered as a second sub mesh.
/// The water mesh is rendered as a second sub mesh because the water mesh is rendered with a different
/// shader than the terrain mesh.
/// </summary>
/// <param name="mesh">This is the class that contains all the data for the mesh.</param>
    private void RenderMesh(Mesh mesh)
    {
        this.mesh.Clear();

        this.mesh.subMeshCount = 2;
        this.mesh.vertices = mesh.vertexList.Concat(mesh.waterSubMesh.vertexList).ToArray();

        this.mesh.SetTriangles(mesh.triangleList.ToArray(), 0);

        int Selector(int val) => val + mesh.vertexList.Count;
        this.mesh.SetTriangles(mesh.waterSubMesh.triangleList.Select(Selector).ToArray(), 1);

        this.mesh.uv = mesh.uvList.Concat(mesh.waterSubMesh.uvList).ToArray();
        this.mesh.RecalculateNormals();
        
        var collisionMesh = new UnityEngine.Mesh
        {
            vertices = mesh.colliderVertexList.ToArray(),
            triangles = mesh.colliderTriangleList.ToArray()
        };
        collisionMesh.RecalculateNormals();

        meshCollider.sharedMesh = collisionMesh;
    }

/// <summary>
/// It takes the data from the chunk and renders it
/// </summary>
    public void UpdateChunk()
    {
        RenderMesh(ChunkHelper.GetChunkMeshData(chunk));
    }

/// <summary>
/// "This function updates the mesh of the chunk with the data provided."
/// </summary>
/// <param name="data">The mesh data that will be used to update the mesh.</param>
    public void UpdateChunk(Mesh data)
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
    
    if (!Application.isPlaying || chunk == null) return;
    
    Gizmos.color = Selection.activeObject == gameObject ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 1, 0.4f);

    Gizmos.DrawCube(transform.position + new Vector3(chunk.chunkSize / 2f, chunk.chunkHeight / 2f, chunk.chunkSize / 2f), new Vector3(chunk.chunkSize, chunk.chunkHeight, chunk.chunkSize));
}
#endif
}

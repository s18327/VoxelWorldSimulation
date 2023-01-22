using System.Collections.Generic;
using UnityEngine;

/* It's a class that holds a list of vertices, triangles, and uv coordinates. It also has a list of
vertices and triangles for the collider mesh. It also has a reference to another Mesh object
that will be used for the water mesh */
public class Mesh
{
    public List<Vector3> vertexList = new();
    public List<int> triangleList = new();
    public List<Vector2> uvList = new();

    public List<Vector3> colliderVertexList = new();
    public List<int> colliderTriangleList = new();

    public Mesh waterSubMesh;

/* It's a constructor that takes a boolean as a parameter. If the boolean is true, it creates a new
Mesh object and assigns it to the waterMesh variable. */
    public Mesh(bool isMainMesh)
    {
        if (!isMainMesh) return;
        waterSubMesh = new Mesh(false);
    }

    /// <summary>
    /// Adds a vertex to the list of vertices. If the vertex generates a collider, it is added to the list
    /// of collider vertices
    /// </summary>
    /// <param name="vertex">The position of the vertex.</param>
    /// <param name="isVertexGeneratingCollider">If true, the vertex will be added to the collider vertices list.</param>
    public void AddVertex(Vector3 vertex, bool isVertexGeneratingCollider)
    {
        vertexList.Add(vertex);

        if (!isVertexGeneratingCollider) return;

        colliderVertexList.Add(vertex);
    }

    /// <summary>
    /// It adds the vertices of the quad to the triangle list in the correct order so that the quad is
    /// rendered correctly.
    /// 
    /// The first three vertices are the first triangle, and the next three vertices are the second
    /// triangle.
    /// </summary>
    /// <param name="isGeneratingCollider">If true, the quad will be added to the collider mesh.</param>
    public void AddTriangles(bool isGeneratingCollider)
    {
        triangleList.Add(vertexList.Count - 4);
        triangleList.Add(vertexList.Count - 3);
        triangleList.Add(vertexList.Count - 2);

        triangleList.Add(vertexList.Count - 4);
        triangleList.Add(vertexList.Count - 2);
        triangleList.Add(vertexList.Count - 1);

        if (isGeneratingCollider) AddColliderTriangles();
    }

    private void AddColliderTriangles()
    {
        colliderTriangleList.Add(colliderVertexList.Count - 4);
        colliderTriangleList.Add(colliderVertexList.Count - 3);
        colliderTriangleList.Add(colliderVertexList.Count - 2);
        colliderTriangleList.Add(colliderVertexList.Count - 4);
        colliderTriangleList.Add(colliderVertexList.Count - 2);
        colliderTriangleList.Add(colliderVertexList.Count - 1);
    }
}
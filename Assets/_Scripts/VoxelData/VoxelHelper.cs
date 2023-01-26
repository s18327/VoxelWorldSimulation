using System;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelHelper
{
/* Creating an array of directions. */
    private static readonly Direction[] Directions;

    static VoxelHelper()
    {
        Directions = new[]
        {
            Direction.Backwards,
            Direction.Down,
            Direction.Forward,
            Direction.Left,
            Direction.Right,
            Direction.Up
        };
    }

    /// <summary>
    /// If the voxel is not air or nothing, then for each direction, if the voxel in that direction is not
    /// solid, then add the face data for that direction to the mesh data
    /// </summary>
    /// <param name="chunk">The chunk that the voxel is in</param>
    /// <param name="x">The x coordinate of the voxel in the chunk</param>
    /// <param name="y">The y coordinate of the voxel</param>
    /// <param name="z">The z coordinate of the voxel</param>
    /// <param name="mesh">The mesh data that will be returned.</param>
    /// <param name="voxelType">The type of voxel that is being checked.</param>
    /// <returns>
    /// The mesh data of the voxel.
    /// </returns>
    public static Mesh GetMeshData
        (Chunk chunk, int x, int y, int z, Mesh mesh, VoxelType voxelType)
    {
        if (voxelType is VoxelType.Air or VoxelType.Nothing)
            return mesh;

        foreach (var direction in Directions)
        {
            var adjacentVoxelLocation = new Vector3Int(x, y, z) + direction.GetVector();
            var adjacentVoxelType = ChunkHelper.GetVoxelFromChunkCoordinates(chunk, adjacentVoxelLocation);

            if (adjacentVoxelType is VoxelType.Nothing ||
                VoxelDataManager.voxelDataDictionary[adjacentVoxelType].isSolid)
                continue; 
            if (voxelType is VoxelType.Water)
            {
                if (adjacentVoxelType is VoxelType.Air)
                    mesh.waterSubMesh = GetFaceDataIn(direction, x, y, z, mesh.waterSubMesh, voxelType);
            }
            else
            {
                mesh = GetFaceDataIn(direction, x, y, z, mesh, voxelType);
            }
        }

        return mesh;
    }

    /// <summary>
    /// It adds the vertices, triangles, and UVs of a face to the mesh data
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="x">The x position of the voxel in the chunk</param>
    /// <param name="y">the y position of the voxel</param>
    /// <param name="z">The z position of the voxel in the chunk</param>
    /// <param name="mesh">The mesh data that will be returned.</param>
    /// <param name="voxelType">The type of voxel that is being generated.</param>
    /// <returns>
    /// The mesh is being returned.
    /// </returns>
    private static Mesh GetFaceDataIn(Direction direction, int x, int y, int z, Mesh mesh,
        VoxelType voxelType)
    {
        AddFaceVerticesToMesh(direction, x, y, z, mesh, voxelType);
        mesh.AddTriangles(VoxelDataManager.voxelDataDictionary[voxelType].isGeneratingCollider);
        mesh.uvList.AddRange(GetFaceUvVectors(direction, voxelType));

        return mesh;
    }

    /// <summary>
    /// It adds the vertices of a face to the mesh data
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="x">The x position of the voxel</param>
    /// <param name="y">the y position of the voxel</param>
    /// <param name="z">The z position of the voxel</param>
    /// <param name="mesh">The mesh data that will be used to create the mesh.</param>
    /// <param name="voxelType">The type of voxel that is being generated.</param>
    private static void AddFaceVerticesToMesh(Direction direction, int x, int y, int z, Mesh mesh,
        VoxelType voxelType)
    {
        var generatingCollider = VoxelDataManager.voxelDataDictionary[voxelType].isGeneratingCollider;
        switch (direction)
        {
            case Direction.Backwards:
                mesh.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                break;
            case Direction.Forward:
                mesh.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                break;
            case Direction.Left:
                mesh.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                break;

            case Direction.Right:
                mesh.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                break;
            case Direction.Down:
                mesh.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                break;
            case Direction.Up:
                mesh.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                mesh.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    /// <summary>
    /// It takes a direction and a voxel type, and returns a Vector2 array of UV coordinates for the face of
    /// the voxel in that direction
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="voxelType">The type of voxel you want to get the UVs for.</param>
    /// <returns>
    /// The UVs of the voxel.
    /// </returns>
    private static IEnumerable<Vector2> GetFaceUvVectors(Direction direction, VoxelType voxelType)
    {
        var faceUVs = new Vector2[4];
        var texturePosition = TexturePosition(direction, voxelType);

        faceUVs[0] = new Vector2(
            VoxelDataManager.tileDimX * texturePosition.x + VoxelDataManager.tileDimX - VoxelDataManager.tileOffset,
            VoxelDataManager.tileDimY * texturePosition.y + VoxelDataManager.tileOffset);

        faceUVs[1] = new Vector2(
            VoxelDataManager.tileDimX * texturePosition.x + VoxelDataManager.tileDimX - VoxelDataManager.tileOffset,
            VoxelDataManager.tileDimY * texturePosition.y + VoxelDataManager.tileDimY - VoxelDataManager.tileOffset);

        faceUVs[2] = new Vector2(VoxelDataManager.tileDimX * texturePosition.x + VoxelDataManager.tileOffset,
            VoxelDataManager.tileDimY * texturePosition.y + VoxelDataManager.tileDimY - VoxelDataManager.tileOffset);

        faceUVs[3] = new Vector2(VoxelDataManager.tileDimX * texturePosition.x + VoxelDataManager.tileOffset,
            VoxelDataManager.tileDimY * texturePosition.y + VoxelDataManager.tileOffset);

        return faceUVs;
    }

    /// <summary>
    /// If the direction is up, return the up texture position, if the direction is down, return the down
    /// texture position, otherwise return the side texture position
    /// </summary>
    /// <param name="direction">The direction of the voxel.</param>
    /// <param name="voxelType">The type of voxel you want to get the texture position of.</param>
    /// <returns>
    /// A Vector2Int
    /// </returns>
    private static Vector2Int TexturePosition(Direction direction, VoxelType voxelType)
    {
        return direction switch
        {
            Direction.Up => VoxelDataManager.voxelDataDictionary[voxelType].up,
            Direction.Down => VoxelDataManager.voxelDataDictionary[voxelType].down,
            _ => VoxelDataManager.voxelDataDictionary[voxelType].side
        };
    }
}
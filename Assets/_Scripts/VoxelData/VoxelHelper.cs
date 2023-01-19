using System;
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
    /// If the block is not air or nothing, then for each direction, if the block in that direction is not
    /// solid, then add the face data for that direction to the mesh data
    /// </summary>
    /// <param name="chunk">The chunk that the block is in</param>
    /// <param name="x">The x coordinate of the block in the chunk</param>
    /// <param name="y">The y coordinate of the block</param>
    /// <param name="z">The z coordinate of the block</param>
    /// <param name="meshData">The mesh data that will be returned.</param>
    /// <param name="voxelType">The type of block that is being checked.</param>
    /// <returns>
    /// The mesh data of the block.
    /// </returns>
    public static MeshData GetMeshData
        (ChunkData chunk, int x, int y, int z, MeshData meshData, VoxelType voxelType)
    {
        if (voxelType is VoxelType.Air or VoxelType.Nothing)
            return meshData;

        foreach (Direction direction in Directions)
        {
            var adjacentVoxelLocation = new Vector3Int(x, y, z) + direction.GetVector();
            var adjacentVoxelType = Chunk.GetBlockFromChunkCoordinates(chunk, adjacentVoxelLocation);

            if (adjacentVoxelType is VoxelType.Nothing ||
                VoxelDataManager.voxelDataDictionary[adjacentVoxelType].isSolid)
                continue; //TODO: Check if this should be reversed
            if (voxelType is VoxelType.Water)
            {
                if (adjacentVoxelType is VoxelType.Air)
                    meshData.waterSubMeshData = GetFaceDataIn(direction, x, y, z, meshData.waterSubMeshData, voxelType);
            }
            else
            {
                meshData = GetFaceDataIn(direction, x, y, z, meshData, voxelType);
            }
        }

        return meshData;
    }

    /// <summary>
    /// It adds the vertices, triangles, and UVs of a face to the mesh data
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="x">The x position of the block in the chunk</param>
    /// <param name="y">the y position of the block</param>
    /// <param name="z">The z position of the block in the chunk</param>
    /// <param name="meshData">The mesh data that will be returned.</param>
    /// <param name="voxelType">The type of block that is being generated.</param>
    /// <returns>
    /// The meshData is being returned.
    /// </returns>
    private static MeshData GetFaceDataIn(Direction direction, int x, int y, int z, MeshData meshData,
        VoxelType voxelType)
    {
        GetFaceVertices(direction, x, y, z, meshData, voxelType);
        meshData.AddTriangles(VoxelDataManager.voxelDataDictionary[voxelType].generatesCollider);
        meshData.uvList.AddRange(FaceUVs(direction, voxelType));

        return meshData;
    }

    /// <summary>
    /// It adds the vertices of a face to the mesh data
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="x">The x position of the block</param>
    /// <param name="y">the y position of the block</param>
    /// <param name="z">The z position of the block</param>
    /// <param name="meshData">The mesh data that will be used to create the mesh.</param>
    /// <param name="voxelType">The type of block that is being generated.</param>
    private static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData,
        VoxelType voxelType)
    {
        var generatingCollider = VoxelDataManager.voxelDataDictionary[voxelType].generatesCollider;
        switch (direction)
        {
            case Direction.Backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                break;
            case Direction.Forward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                break;
            case Direction.Left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                break;

            case Direction.Right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                break;
            case Direction.Down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f), generatingCollider);
                break;
            case Direction.Up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f), generatingCollider);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    /// <summary>
    /// It takes a direction and a block type, and returns a Vector2 array of UV coordinates for the face of
    /// the block in that direction
    /// </summary>
    /// <param name="direction">The direction of the face.</param>
    /// <param name="voxelType">The type of block you want to get the UVs for.</param>
    /// <returns>
    /// The UVs of the block.
    /// </returns>
    private static Vector2[] FaceUVs(Direction direction, VoxelType voxelType)
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
    /// <param name="direction">The direction of the block.</param>
    /// <param name="voxelType">The type of block you want to get the texture position of.</param>
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
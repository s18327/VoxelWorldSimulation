using UnityEngine;

public abstract class LayerHandler : MonoBehaviour
{
    [SerializeField]
    private LayerHandler next;

    public bool Handle(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (TryHandling(chunkData, x, y, z, surfaceHeightNoise, mapSeedOffset))
            return true;
        return next != null && next.Handle(chunkData, x, y, z, surfaceHeightNoise, mapSeedOffset);
    }

    protected abstract bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset);
}

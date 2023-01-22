using UnityEngine;

public abstract class LayerHandler : MonoBehaviour
{
    [SerializeField]
    private LayerHandler next;

    public bool Handle(Chunk chunk, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
    {
        if (TryHandling(chunk, x, y, z, surfaceHeightNoise, mapSeedOffset))
            return true;
        return next != null && next.Handle(chunk, x, y, z, surfaceHeightNoise, mapSeedOffset);
    }

    protected abstract bool TryHandling(Chunk chunk, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset);
}

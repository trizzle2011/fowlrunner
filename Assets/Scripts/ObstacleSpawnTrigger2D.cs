// ObstacleSpawnTrigger2D.cs
using UnityEngine;

public enum SpawnSide
{
    Left,
    Center,
    Right
}

[RequireComponent(typeof(Collider2D))]
public class ObstacleSpawnTrigger2D : MonoBehaviour
{
    [Tooltip("Which side this trigger represents")]
    public SpawnSide side;

    private void Reset()
    {
        // Auto-set a 2D trigger collider
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Spawn_{side}] OnTriggerEnter2D with {other.name} (tag={other.tag}) at {transform.position}");

        // 1) Only care about the player
        if (!other.CompareTag("PlayerDuck"))
        {
            Debug.Log($"[Spawn_{side}] Ignoring—tag is not PlayerDuck");
            return;
        }

        // 2) Find the ChunkInstance to know what to spawn
        var inst = GetComponentInParent<ChunkInstance>();
        if (inst == null)
        {
            Debug.LogError($"[Spawn_{side}] No ChunkInstance found on parent!");
            return;
        }

        // 3) Pick the right prefab from the ChunkData
        GameObject prefabToSpawn = side switch
        {
            SpawnSide.Left => inst.Data.leftObstacle,
            SpawnSide.Center => inst.Data.centerObstacle,
            SpawnSide.Right => inst.Data.rightObstacle,
            _ => null
        };

        if (prefabToSpawn == null)
        {
            Debug.LogError($"[Spawn_{side}] No obstacle prefab assigned for side {side} on chunk {inst.Data.name}");
            return;
        }

        // 4) Spawn via PoolManager
        var instance = PoolManager.Instance.Spawn(
            prefabToSpawn,
            transform.position,
            Quaternion.identity
        );
        Debug.Log($"[Spawn_{side}] Spawned instance '{instance.name}' from prefab '{prefabToSpawn.name}'");

        // 5) Sanity-check the spawned object's renderer
        var sr = instance.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogWarning($"[Spawn_{side}] Spawned object has no SpriteRenderer!");
        }
        else
        {
            // avoid null-propagation on UnityEngine.Object
            string spriteName = sr.sprite != null ? sr.sprite.name : "null";
            Debug.Log($"[Spawn_{side}] SpriteRenderer.sprite={spriteName}, layer={sr.sortingLayerName}, order={sr.sortingOrder}");
        }

        // 6) Ensure it gets reclaimed by the pool
        if (!instance.TryGetComponent<PooledObject>(out var po))
            po = instance.AddComponent<PooledObject>();
        po.OriginalPrefab = prefabToSpawn;

        // 7) Only fire this trigger once
        enabled = false;
    }
}

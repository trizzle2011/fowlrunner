// ObstacleSpawnTrigger2D.cs
using System.Collections;
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

    [Tooltip("Manager used for pooled obstacle instances")]
    public PoolManager poolManager;

    [Tooltip("Prefab to spawn when triggered")]
    public GameObject obstaclePrefab;

    [Tooltip("Parent transform for spawned obstacles")]
    public Transform spawnParent;

    private void Awake()
    {
        if (poolManager == null)
            poolManager = UnityEngine.Object.FindAnyObjectByType<PoolManager>();
        if (poolManager == null)
            Debug.LogError("ObstacleSpawnTrigger2D: poolManager not assigned", this);

        if (spawnParent == null)
            spawnParent = transform.parent;
        if (spawnParent == null)
            Debug.LogError("ObstacleSpawnTrigger2D: spawnParent not assigned", this);

        if (obstaclePrefab == null)
            obstaclePrefab = Resources.Load<GameObject>("ObstaclePrefab");

        if (obstaclePrefab == null)
        {
            Debug.LogError($"ObstacleSpawnTrigger2D: obstaclePrefab not assigned for side {side}", this);
            enabled = false;
            return;
        }
    }

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || !other.CompareTag("PlayerDuck"))
            return;

        if (poolManager == null || obstaclePrefab == null || spawnParent == null)
            return;

        var go = poolManager.GetPooledObject(obstaclePrefab);
        if (go == null)
            return;

        go.transform.SetParent(spawnParent, false);
        go.transform.position = transform.position;
        go.SetActive(true);

        if (go.TryGetComponent<SpriteRenderer>(out var sr))
            StartCoroutine(FadeInCoroutine(sr, 0.3f));
    }

    private IEnumerator FadeInCoroutine(SpriteRenderer sr, float duration)
    {
        if (sr == null)
            yield break;

        var c = sr.color;
        c.a = 0f;
        sr.color = c;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Clamp01(t / duration);
            sr.color = c;
            yield return null;
        }

        c.a = 1f;
        sr.color = c;
    }
}

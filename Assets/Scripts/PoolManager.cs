// PoolManager.cs
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    // Map from prefab → queue of pooled instances
    private readonly Dictionary<GameObject, Queue<GameObject>> pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // (optional) DontDestroyOnLoad(gameObject);
    }

    /// <summary>Preload a bunch of inactive instances.</summary>
    public void Preload(GameObject prefab, int count)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            var po = go.AddComponent<PooledObject>();
            po.OriginalPrefab = prefab;
            pools[prefab].Enqueue(go);
        }
    }

    /// <summary>
    /// Return an inactive instance for the given prefab, creating one if needed.
    /// </summary>
    public GameObject GetPooledObject(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject instance;
        if (pools[prefab].Count > 0)
        {
            instance = pools[prefab].Dequeue();
        }
        else
        {
            instance = Instantiate(prefab);
            instance.SetActive(false);
            var po = instance.AddComponent<PooledObject>();
            po.OriginalPrefab = prefab;
        }
        return instance;
    }

    /// <summary>Get an instance (reuse or new) at pos/rot.</summary>
    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        GameObject instance;
        if (pools[prefab].Count > 0)
        {
            instance = pools[prefab].Dequeue();
            instance.transform.SetPositionAndRotation(pos, rot);
            instance.SetActive(true);
        }
        else
        {
            instance = Instantiate(prefab, pos, rot);
            var po = instance.AddComponent<PooledObject>();
            po.OriginalPrefab = prefab;
        }
        return instance;
    }

    /// <summary>Disable and return an instance to its pool.</summary>
    public void Reclaim(GameObject instance)
    {
        var po = instance.GetComponent<PooledObject>();
        if (po == null || po.OriginalPrefab == null)
        {
            Debug.LogWarning("PoolManager: cannot reclaim—missing PooledObject.");
            Destroy(instance);
            return;
        }

        instance.SetActive(false);
        pools[po.OriginalPrefab].Enqueue(instance);
    }
}

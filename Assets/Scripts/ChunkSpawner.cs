using System.Collections.Generic;
using UnityEngine;

public class ChunkSpawner : MonoBehaviour
{
    [SerializeField] private List<ChunkData> chunkLibrary;
    [SerializeField] private Transform player;
    [SerializeField] private int initialChunks = 5;
    [SerializeField] private float recycleOffset = 5f;

    private readonly List<GameObject> activeChunks = new List<GameObject>();

    private void Start()
    {
        Vector3 spawnPos = transform.position;
        for (int i = 0; i < initialChunks; i++)
            SpawnNextChunk(ref spawnPos);
    }

    private void Update()
    {
        if (activeChunks.Count < 2) return;
        var secondEnd = activeChunks[1].transform.Find("End");
        if (secondEnd == null) return;

        // Compare Y, not Z
        if (player.position.y > secondEnd.position.y - recycleOffset)
            RecycleChunk();
    }

    private void RecycleChunk()
    {
        Destroy(activeChunks[0]);
        activeChunks.RemoveAt(0);

        var last = activeChunks[activeChunks.Count - 1];
        var end = last.transform.Find("End");
        if (end == null)
        {
            Debug.LogError($"ChunkSpawner: '{last.name}' missing 'End'.");
            return;
        }

        Vector3 spawnPos = end.position;
        SpawnNextChunk(ref spawnPos);
    }

    private void SpawnNextChunk(ref Vector3 spawnPos)
    {
        var data = PickChunk();
        Debug.Log($"Spawning '{data.name}' at {spawnPos}");

        var chunkGO = Instantiate(data.prefab, spawnPos, Quaternion.identity, transform);
        var ci = chunkGO.AddComponent<ChunkInstance>();
        ci.Data = data;
        activeChunks.Add(chunkGO);

        var end = chunkGO.transform.Find("End");
        if (end != null)
            spawnPos = end.position;
        else
            Debug.LogError($"ChunkSpawner: '{chunkGO.name}' missing an 'End' child.");
    }

    private ChunkData PickChunk()
    {
        int total = 0;
        foreach (var c in chunkLibrary) total += c.weight;

        int r = Random.Range(0, total);
        foreach (var c in chunkLibrary)
        {
            if (r < c.weight) return c;
            r -= c.weight;
        }
        return chunkLibrary[0];
    }
}

using System.Collections.Generic;
using UnityEngine;

public class ChunkPool : MonoBehaviour
{
    [Tooltip("Chunk prefab with SpawnAnchor + 3 lane markers")]
    public GameObject chunkPrefab;
    [Tooltip("All obstacle prefabs (cattail, log, bank, hunter…)")]
    public GameObject[] obstaclePrefabs;
    [Tooltip("Number of chunks to keep in play (4 for you)")]
    public int poolSize = 4;
    [Tooltip("Length of one chunk in world units")]
    public float chunkLength = 10f;

    // 1) Made readonly (IDE0044)  
    // 2) Used target-typed new() (IDE0090)
    private readonly List<GameObject> chunks = new();

    void Start()
    {
        // Instantiate poolSize chunks in a row
        for (int i = 0; i < poolSize; i++)
        {
            var c = Instantiate(
                chunkPrefab,
                new Vector3(0, 0, i * chunkLength),
                Quaternion.identity
            );
            chunks.Add(c);
        }

        // Fill chunks 1..end with obstacles (skip index 0)
        for (int i = 1; i < chunks.Count; i++)
            RefillObstacles(chunks[i]);
    }

    void Update()
    {
        // Recycle the front chunk when the player passes it
        var first = chunks[0];
        if (transform.position.z > first.transform.position.z + chunkLength)
        {
            chunks.RemoveAt(0);
            float maxZ = chunks[^1].transform.position.z;
            first.transform.position = new Vector3(0, 0, maxZ + chunkLength);

            RefillObstacles(first);
            chunks.Add(first);
        }
    }

    void RefillObstacles(GameObject chunk)
    {
        // Clear old obstacles
        foreach (Transform child in chunk.transform)
            if (child.CompareTag("Obstacle"))
                Destroy(child.gameObject);

        // Anchor at the top edge of the tile
        Vector3 spawnBase = chunk.transform.Find("SpawnAnchor").position;

        // Pick one safe lane
        int safeLane = Random.Range(0, 3);

        // 2) Simplified new[] for lanes (IDE0090)
        var lanes = new[]
        {
            chunk.transform.Find("Lane_Left").localPosition.x,
            chunk.transform.Find("Lane_Center").localPosition.x,
            chunk.transform.Find("Lane_Right").localPosition.x
        };

        // Spawn an obstacle in every other lane
        for (int lane = 0; lane < lanes.Length; lane++)
        {
            if (lane == safeLane) continue;

            Vector3 pos = new Vector3(lanes[lane], 0, spawnBase.z);
            var prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            var inst = Instantiate(prefab, pos, Quaternion.identity, chunk.transform);
            inst.tag = "Obstacle";
        }
    }
}

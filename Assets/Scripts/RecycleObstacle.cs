using System.Collections;
using UnityEngine;

public class RecycleObstacle : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Fall speed (units/sec)")]
    public float diveSpeed = 5f;

    [Header("Spawn Settings")]
    [Tooltip("Exact name of the RiverTile GameObject to spawn from")]
    public string spawnTileName = "RiverTile 1-4";
    [Tooltip("How long it takes to fade in on respawn")]
    public float fadeDuration = 0.5f;

    [Header("Lane (optional)")]
    [Tooltip("If you want to lock to a lane, assign 1 here and enable useLane; else leave empty")]
    public Transform laneToUse;
    public bool useLane = false;

    Camera cam;
    float bottomY, spawnY;
    SpriteRenderer sr;

    void Start()
    {
        // cache components & camera
        cam = Camera.main;
        if (cam == null) Debug.LogError("RecycleObstacle: No Camera.main found");
        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogError("RecycleObstacle: No SpriteRenderer on this GameObject");

        // off-screen bottom
        var bl = cam.ViewportToWorldPoint(Vector3.zero);
        bottomY = bl.y - 1f;

        // calculate top spawn Y from your river‐tile
        var topTile = GameObject.Find(spawnTileName);
        if (topTile != null)
        {
            var tileSR = topTile.GetComponent<SpriteRenderer>();
            float halfH = tileSR != null ? tileSR.bounds.extents.y : 0f;
            spawnY = topTile.transform.position.y + halfH;
        }
        else
        {
            Debug.LogWarning($"RecycleObstacle: “{spawnTileName}” not found—using current Y");
            spawnY = transform.position.y;
        }

        // prime the fade (in case it starts already off-screen)
        SetAlpha(0f);
        StartCoroutine(FadeIn());
    }

    void Update()
    {
        // 1) drop
        transform.position += Vector3.down * diveSpeed * Time.deltaTime;

        // 2) recycle
        if (transform.position.y < bottomY)
            Recycle();
    }

    void Recycle()
    {
        // reposition
        float x = useLane && laneToUse != null
                  ? laneToUse.position.x
                  : transform.position.x;
        transform.position = new Vector3(x, spawnY, transform.position.z);

        // restart fade
        StopAllCoroutines();
        SetAlpha(0f);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(Mathf.Clamp01(t / fadeDuration));
            yield return null;
        }
        SetAlpha(1f);
    }

    void SetAlpha(float a)
    {
        if (sr != null)
        {
            var c = sr.color;
            c.a = a;
            sr.color = c;
        }
    }
}

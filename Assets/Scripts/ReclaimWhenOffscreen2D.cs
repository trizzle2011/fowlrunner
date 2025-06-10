using UnityEngine;

public class ReclaimWhenOffscreen2D : MonoBehaviour
{
    [Tooltip("How far below the bottom of the camera before we reclaim")]
    public float despawnY = -10f;

    void Update()
    {
        if (transform.position.y < despawnY)
        {
            var po = GetComponent<PooledObject>();
            if (po != null && po.OriginalPrefab != null)
            {
                PoolManager.Instance.Reclaim(gameObject);
            }
            else
            {
                // Not from the pool (e.g. a designer-placed prefab), so just destroy it
                Destroy(gameObject);
            }
        }
    }
}

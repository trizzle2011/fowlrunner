// PooledObject.cs
using UnityEngine;

// MUST inherit MonoBehaviour so it’s a valid Component
public class PooledObject : MonoBehaviour
{
    // store which prefab this came from
    [HideInInspector]
    public GameObject OriginalPrefab;
}

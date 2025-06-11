using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DuckController : MonoBehaviour
{
    [Header("Lane Transforms (0 = Left, 1 = Center, 2 = Right)")]
    [Tooltip("Assign Lane_Left, Lane_Center and Lane_Right from the scene root")]
    public Transform[] lanes;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float diveDepth = 0.5f;
    public float diveDuration = 0.3f;

    private int currentLane = 1;
    private Vector3 targetPosition;
    private bool isDiving = false;

    void Awake()
    {
        // Ensure lane array always has 3 entries
        if (lanes == null || lanes.Length != 3)
            lanes = new Transform[3];

        // Warn if any lanes are missing. Assign them via the Inspector or a
        // setup script instead of relying on GameObject.Find.
        for (int i = 0; i < lanes.Length; i++)
        {
            if (lanes[i] == null)
                Debug.LogWarning($"DuckController: lane index {i} is not assigned.");
        }
    }

    void Start()
    {
        // Snap to center lane on start
        if (lanes.Length > 1 && lanes[1] != null)
            targetPosition = lanes[1].position;
        else
            targetPosition = transform.position;
    }

    void Update()
    {
        // Smoothly slide toward target X
        var p = transform.position;
        p.x = Mathf.MoveTowards(p.x, targetPosition.x, moveSpeed * Time.deltaTime);
        transform.position = p;
    }

    void OnMoveLeft(InputValue value)
    {
        if (!value.isPressed || isDiving) return;
        if (currentLane > 0) currentLane--;
        UpdateTarget();
    }

    void OnMoveRight(InputValue value)
    {
        if (!value.isPressed || isDiving) return;
        if (currentLane < lanes.Length - 1) currentLane++;
        UpdateTarget();
    }

    void OnDive(InputValue value)
    {
        if (value.isPressed && !isDiving)
            StartCoroutine(DiveCoroutine());
    }

    private void UpdateTarget()
    {
        if (lanes != null && lanes.Length > currentLane && lanes[currentLane] != null)
        {
            targetPosition = new Vector3(
                lanes[currentLane].position.x,
                transform.position.y,
                transform.position.z
            );
        }
        else
        {
            Debug.LogWarning("DuckController: unable to update lane—check your lane transforms.");
        }
    }

    private IEnumerator DiveCoroutine()
    {
        isDiving = true;
        var start = transform.position;
        var down = start + Vector3.down * diveDepth;

        float t = 0f;
        while (t < diveDuration)
        {
            transform.position = Vector3.Lerp(start, down, t / diveDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = down;

        t = 0f;
        while (t < diveDuration)
        {
            transform.position = Vector3.Lerp(down, start, t / diveDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.position = start;

        isDiving = false;
    }
}

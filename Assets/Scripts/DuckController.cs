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
        // Ensure lane array always has exactly 3 entries
        if (lanes == null || lanes.Length != 3)
            lanes = new Transform[3];

        string[] laneNames = { "Lane_Left", "Lane_Center", "Lane_Right" };
        for (int i = 0; i < lanes.Length && i < laneNames.Length; i++)
        {
            if (lanes[i] == null)
            {
                var found = GameObject.Find(laneNames[i]);
                if (found != null)
                    lanes[i] = found.transform;
                if (lanes[i] == null)
                    Debug.LogWarning($"DuckController: lane index {i} is not assigned.");
            }
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
        currentLane = Mathf.Clamp(currentLane - 1, 0, 2);
        UpdateTarget();
    }

    void OnMoveRight(InputValue value)
    {
        if (!value.isPressed || isDiving) return;
        currentLane = Mathf.Clamp(currentLane + 1, 0, 2);
        UpdateTarget();
    }

    void OnDive(InputValue value)
    {
        if (value.isPressed && !isDiving)
            StartCoroutine(DiveCoroutine());
    }

    private void UpdateTarget()
    {
        if (lanes == null || currentLane < 0 || currentLane >= lanes.Length || lanes[currentLane] == null)
        {
            Debug.LogWarning("DuckController: unable to update lane—check your lane transforms.");
            return;
        }

        targetPosition = new Vector3(
            lanes[currentLane].position.x,
            transform.position.y,
            transform.position.z
        );
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

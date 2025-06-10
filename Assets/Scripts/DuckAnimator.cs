// DuckAnimator.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DuckAnimator : MonoBehaviour
{
    [Tooltip("Drag your 4 walk frames here, in order.")]
    public Sprite[] walkFrames;

    [Tooltip("Frames per second for the walk cycle.")]
    public float fps = 10f;

    private SpriteRenderer _spriteRenderer;
    private int _currentFrame;
    private float _timer;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (walkFrames == null || walkFrames.Length == 0)
            Debug.LogWarning("DuckAnimator: No walk frames assigned!", this);
    }

    void Update()
    {
        if (walkFrames == null || walkFrames.Length == 0)
            return;

        _timer += Time.deltaTime;
        if (_timer >= 1f / fps)
        {
            _timer -= 1f / fps;
            _currentFrame = (_currentFrame + 1) % walkFrames.Length;
            _spriteRenderer.sprite = walkFrames[_currentFrame];
        }
    }
}

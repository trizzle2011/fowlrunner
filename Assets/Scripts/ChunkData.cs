using UnityEngine;

public enum Difficulty
{
    Easy, Medium, Hard
}

[CreateAssetMenu(menuName = "Runner/Chunk", fileName = "ChunkData_")]
public class ChunkData : ScriptableObject
{
    [Tooltip("The terrain chunk prefab (must have an End child)")]
    public GameObject prefab;

    [Tooltip("Use to ramp difficulty over time")]
    public Difficulty difficulty;

    [Tooltip("Relative weight for random selection")]
    public int weight = 1;

    [Header("Obstacle Prefabs")]
    [Tooltip("What to spawn when the player hits the Left trigger")]
    public GameObject leftObstacle;
    [Tooltip("…for the Center trigger")]
    public GameObject centerObstacle;
    [Tooltip("…for the Right trigger")]
    public GameObject rightObstacle;
}

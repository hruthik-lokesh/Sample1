using UnityEngine;

/// <summary>
/// Detects collisions with the first (go0) and last (go251) sine wave points.
/// Attach this to the grabbable sphere.
/// </summary>
public class SineWaveCollisionDetector : MonoBehaviour
{
    [Header("Collision State")]
    [Tooltip("1 if sphere has collided with go0 (start point), else 0")]
    public int Hit = 0;

    [Tooltip("1 if sphere has collided with go251 (end point), else 0")]
    public int Finished = 0;

    [Header("Position & Time Data")]
    public Vector3 hitPosition = Vector3.zero;
    public float hitTime = 0f;
    public Vector3 finishedPosition = Vector3.zero;
    public float finishedTime = 0f;

    [Header("Debug")]
    public bool logCollisions = true;

    private bool hasHitGo0 = false;
    private bool hasHitGo251 = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check for collision with go0 (start point)
        if (!hasHitGo0 && other.gameObject.name == "go0")
        {
            Hit = 1;
            hitPosition = transform.position;
            hitTime = Time.time;
            hasHitGo0 = true;

            if (logCollisions)
            {
                Debug.Log($"✓ HIT DETECTED at go0! Time={hitTime:F3}s, Position={hitPosition}");
            }
        }

        // Check for collision with go251 (end point)
        if (!hasHitGo251 && other.gameObject.name == "go251")
        {
            Finished = 1;
            finishedPosition = transform.position;
            finishedTime = Time.time;
            hasHitGo251 = true;

            if (logCollisions)
            {
                Debug.Log($"✓ FINISHED DETECTED at go251! Time={finishedTime:F3}s, Position={finishedPosition}");

                // Calculate duration if we also hit go0
                if (hasHitGo0)
                {
                    float duration = finishedTime - hitTime;
                    Debug.Log($"✓ Total duration from go0 to go251: {duration:F3}s");
                }
            }
        }
    }

    /// <summary>
    /// Reset the collision state (useful for new trials)
    /// </summary>
    public void ResetState()
    {
        Hit = 0;
        Finished = 0;
        hasHitGo0 = false;
        hasHitGo251 = false;
        hitPosition = Vector3.zero;
        hitTime = 0f;
        finishedPosition = Vector3.zero;
        finishedTime = 0f;

        if (logCollisions)
        {
            Debug.Log("SineWaveCollisionDetector: State reset");
        }
    }
}

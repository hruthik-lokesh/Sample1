using System.Collections;
using UnityEngine;

/// <summary>
/// Controls marker sphere activation when [BuildingBlock] Cube reaches the end of the sine wave.
/// Marker sphere glows after 1 second delay and disappears when captured by the Cube.
/// Attach this script to an EMPTY parent GameObject, with the actual marker sphere as a child.
/// OR keep this script on a separate always-active GameObject.
/// </summary>
public class MarkerSphereController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The [BuildingBlock] Cube that triggers the marker")]
    public GameObject buildingBlockCube;

    [Tooltip("Reference to SineWaveCollisionDetector on the Cube")]
    public SineWaveCollisionDetector collisionDetector;

    [Tooltip("The actual marker sphere visual (child object or separate sphere)")]
    public GameObject markerSphereVisual;

    [Header("Glow Settings")]
    [Tooltip("Material to use when marker is glowing")]
    public Material glowMaterial;

    [Tooltip("Original material (assigned automatically if not set)")]
    public Material originalMaterial;

    [Tooltip("Delay before marker appears after Cube reaches end (seconds)")]
    public float activationDelay = 1.0f;
                                
    [Tooltip("Scale when glowing (for visual emphasis)")]
    public Vector3 glowScale = new Vector3(0.02f, 0.02f, 0.02f);

    [Header("Collision Settings")]
    [Tooltip("Distance threshold for Cube to capture the marker (meters)")]
    public float captureDistance = 0.05f;

    // Internal state
    private MeshRenderer meshRenderer;
    private Vector3 originalScale;
    private bool isGlowing = false;
    private bool hasBeenCaptured = false;
    private bool waitingForActivation = false;
    private Coroutine pulseCoroutine;

    void Start()
    {
        // If no separate visual assigned, use this GameObject
        if (markerSphereVisual == null)
        {
            markerSphereVisual = gameObject;
        }

        meshRenderer = markerSphereVisual.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MarkerSphereController: No MeshRenderer found on marker sphere!");
            return;
        }

        // Store original values
        originalScale = markerSphereVisual.transform.localScale;
        if (originalMaterial == null)
        {
            originalMaterial = meshRenderer.material;
        }

        // Auto-create glow material if not assigned
        if (glowMaterial == null)
        {
            glowMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (glowMaterial != null)
            {
                glowMaterial.SetColor("_BaseColor", Color.yellow);
                glowMaterial.EnableKeyword("_EMISSION");
                glowMaterial.SetColor("_EmissionColor", Color.yellow * 3f);
                Debug.Log("MarkerSphereController: Auto-created yellow glow material");
            }
        }

        // Hide marker visual initially (but keep THIS script active!)
        meshRenderer.enabled = false;

        // Auto-find collision detector if not assigned
        if (collisionDetector == null && buildingBlockCube != null)
        {
            collisionDetector = buildingBlockCube.GetComponent<SineWaveCollisionDetector>();
        }

        if (collisionDetector == null)
        {
            Debug.LogError("MarkerSphereController: SineWaveCollisionDetector not assigned! Cannot detect sine wave end.");
        }
        else
        {
            Debug.Log($"MarkerSphereController: Initialized. Watching collisionDetector on '{collisionDetector.gameObject.name}'");
        }

        Debug.Log("MarkerSphereController: Ready and waiting for Cube to reach sine wave end (go251).");
    }

    void Update()
    {
        // Debug: Log current state every 2 seconds
        if (Time.frameCount % 120 == 0 && collisionDetector != null)
        {
            Debug.Log($"MarkerSphereController: Finished={collisionDetector.Finished}, isGlowing={isGlowing}, waiting={waitingForActivation}, captured={hasBeenCaptured}");
        }

        if (hasBeenCaptured) return;

        // Check if Cube has reached the end of sine wave
        if (!isGlowing && !waitingForActivation && collisionDetector != null && collisionDetector.Finished == 1)
        {
            Debug.Log("MarkerSphereController: *** DETECTED Finished=1! Starting activation coroutine...");
            StartCoroutine(ActivateMarkerAfterDelay());
        }

        // Check if Cube captures the marker while glowing
        if (isGlowing && buildingBlockCube != null)
        {
            float distance = Vector3.Distance(markerSphereVisual.transform.position, buildingBlockCube.transform.position);
            if (distance < captureDistance)
            {
                CaptureMarker();
            }
        }
    }

    /// <summary>
    /// Activates the marker sphere after the specified delay
    /// </summary>
    IEnumerator ActivateMarkerAfterDelay()
    {
        waitingForActivation = true;
        Debug.Log($"MarkerSphereController: Cube reached end of sine wave! Activating marker in {activationDelay}s...");

        yield return new WaitForSeconds(activationDelay);

        if (!hasBeenCaptured)
        {
            ActivateGlow();
        }

        waitingForActivation = false;
    }

    /// <summary>
    /// Activates the glowing state of the marker
    /// </summary>
    void ActivateGlow()
    {
        // Show the marker
        meshRenderer.enabled = true;
        isGlowing = true;

        // Apply glow material
        if (glowMaterial != null && meshRenderer != null)
        {
            meshRenderer.material = glowMaterial;
        }

        // Apply glow scale
        markerSphereVisual.transform.localScale = glowScale;

        // Start pulsing effect for visual feedback
        pulseCoroutine = StartCoroutine(PulseEffect());

        Debug.Log("MarkerSphereController: ✨ Marker is now GLOWING! Cube should return to capture it.");
    }

    /// <summary>
    /// Visual pulsing effect to indicate the marker is active
    /// </summary>
    IEnumerator PulseEffect()
    {
        float pulseSpeed = 2f;
        float pulseAmount = 0.3f;

        while (isGlowing)
        {
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            markerSphereVisual.transform.localScale = glowScale * scale;
            yield return null;
        }
    }

    /// <summary>
    /// Called when the Cube captures the marker
    /// </summary>
    void CaptureMarker()
    {
        hasBeenCaptured = true;
        isGlowing = false;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }

        Debug.Log("MarkerSphereController: ✓ Marker CAPTURED by Cube! Hiding marker.");

        // Hide the marker
        meshRenderer.enabled = false;

        // Reset to original state for potential reuse
        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
        markerSphereVisual.transform.localScale = originalScale;
    }

    /// <summary>
    /// Alternative collision detection using trigger colliders
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (isGlowing && !hasBeenCaptured)
        {
            // Check if it's the BuildingBlock Cube
            if (other.gameObject == buildingBlockCube ||
                other.gameObject.name.Contains("BuildingBlock") ||
                other.gameObject.name.Contains("Cube"))
            {
                CaptureMarker();
            }
        }
    }

    /// <summary>
    /// Reset the marker state for a new trial
    /// </summary>
    public void ResetMarker()
    {
        hasBeenCaptured = false;
        isGlowing = false;
        waitingForActivation = false;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }

        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }
        markerSphereVisual.transform.localScale = originalScale;

        meshRenderer.enabled = false;

        Debug.Log("MarkerSphereController: Reset for new trial.");
    }

    /// <summary>
    /// Set the marker position (call this to place the marker at a specific location)
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        markerSphereVisual.transform.position = position;
        Debug.Log($"MarkerSphereController: Position set to {position}");
    }

    /// <summary>
    /// Check if marker has been captured
    /// </summary>
    public bool IsCaptured() => hasBeenCaptured;

    /// <summary>
    /// Check if marker is currently glowing
    /// </summary>
    public bool IsGlowing() => isGlowing;
}
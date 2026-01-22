//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using System.Text;
//using UnityEngine.XR;
//using System;

//public class SaveDataTrainingXR : MonoBehaviour
//{
//    [Header("Main References")]
//    public XRNode trackedHand = XRNode.RightHand;
//    public GameObject pacer;
//    public GameObject background;

//    [Header("Grabbable Sphere")]
//    [Tooltip("Sphere that has IsHoldingSphere + SineWaveCollisionDetector")]
//    public GameObject sphere;
//    private IsHoldingSphere holdingSphere;
//    private SineWaveCollisionDetector collisionDetector;

//    [Header("Coins")]
//    public GameObject coin1;
//    public GameObject coin2;
//    public GameObject coin3;
//    public GameObject sinusoid;

//    [Header("Coin Scripts")]
//    [Tooltip("CoinBombScript1 - handles coin placement")]
//    public CoinBombScript1 coinBombScript;
//    [Tooltip("CoinCollector - handles coin collection")]
//    public CoinCollector coinCollectorScript;

//    [Header("File Settings")]
//    [Tooltip("Leave empty to use default location (My Documents/MetaXR_Logs). Or specify custom path.")]
//    public string filepathpre = "";
//    private string delimiter = ",";
//    private string extension = ".csv";
//    private string filepath;

//    // Runtime state
//    private InputDevice handDevice;
//    private Vector3 handPos;
//    private bool isHolding;
//    private int hitt;
//    private float pacerx;
//    private float distance;
//    private int finished;

//    // Coin data
//    private int CoinGet1, CoinGet2, CoinGet3;
//    private int Coin1go, Coin2go, Coin3go;

//    // CSV buffer - write every N frames instead of all at end
//    private readonly List<string> frameData = new List<string>();
//    private const int WRITE_BUFFER_SIZE = 300; // Write every 300 frames (~5 seconds at 60fps)

//    private void Awake()
//    {
//        Debug.Log("=== SaveDataTrainingXR: Awake() called ===");

//        hitt = 0;
//        finished = 0;

//        // Determine save location
//        if (string.IsNullOrEmpty(filepathpre))
//        {
//            // Try USERPROFILE environment variable first (more reliable)
//            string userProfile = System.Environment.GetEnvironmentVariable("USERPROFILE");
//            if (!string.IsNullOrEmpty(userProfile))
//            {
//                filepathpre = Path.Combine(userProfile, "Documents", "MetaXR_Logs");
//            }
//            else
//            {
//                // Fallback to SpecialFolder
//                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
//                filepathpre = Path.Combine(documentsPath, "MetaXR_Logs");
//            }
//            Debug.Log($"Using default documents path: {filepathpre}");
//        }
//        else
//        {
//            Debug.Log($"Using custom path from inspector: {filepathpre}");
//        }

//        // Create directory and verify write access
//        try
//        {
//            Directory.CreateDirectory(filepathpre);

//            // Test write permissions
//            string testFile = Path.Combine(filepathpre, ".test");
//            File.WriteAllText(testFile, "test");
//            File.Delete(testFile);

//            Debug.Log($"✓ Directory ready with write access: {filepathpre}");
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError($"✗ Cannot use directory '{filepathpre}': {e.Message}");

//            // Ultimate fallback
//            filepathpre = Application.persistentDataPath;
//            Debug.LogWarning($"⚠️ Using Unity persistent data path: {filepathpre}");
//        }

//        // Create unique filename with timestamp
//        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
//        filepath = Path.Combine(filepathpre, $"Trial_{timestamp}{extension}");

//        Debug.Log($"📁 CSV file will be: {filepath}");

//        // Write CSV header
//        StringBuilder header = new StringBuilder();
//        header.Append("Frame,Time,Distance,PosX,PosY,PosZ,PacerX,Holding,Hit,Finished,");
//        header.Append("HitTime,HitPosX,HitPosY,HitPosZ,");
//        header.Append("FinishedTime,FinishedPosX,FinishedPosY,FinishedPosZ,");
//        header.Append("Coin1x,Coin1y,Coin1go,Collected1,");
//        header.Append("Coin2x,Coin2y,Coin2go,Collected2,");
//        header.AppendLine("Coin3x,Coin3y,Coin3go,Collected3");

//        try
//        {
//            File.WriteAllText(filepath, header.ToString());
//            Debug.Log($"✅ CSV file created successfully!");
//            Debug.Log($"✅ Header written to: {filepath}");
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError($"❌ Failed to create CSV file: {e.Message}");
//            Debug.LogError($"❌ Attempted path: {filepath}");
//        }
//    }

//    private void Start()
//    {
//        Debug.Log("=== SaveDataTrainingXR: Start() called ===");

//        handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);
//        Debug.Log($"Hand device initialized for {trackedHand}");

//        // Cache components from sphere
//        if (sphere != null)
//        {
//            holdingSphere = sphere.GetComponent<IsHoldingSphere>();
//            if (holdingSphere == null)
//                Debug.LogWarning("⚠️ Sphere missing IsHoldingSphere component!");
//            else
//                Debug.Log("✓ IsHoldingSphere found");

//            collisionDetector = sphere.GetComponent<SineWaveCollisionDetector>();
//            if (collisionDetector == null)
//                Debug.LogWarning("⚠️ Sphere missing SineWaveCollisionDetector component!");
//            else
//                Debug.Log("✓ SineWaveCollisionDetector found");
//        }
//        else
//        {
//            Debug.LogError("✗ Sphere not assigned in inspector!");
//        }

//        Debug.Log($"Coin references - coin1:{coin1 != null}, coin2:{coin2 != null}, coin3:{coin3 != null}");
//        Debug.Log($"coinBombScript assigned: {coinBombScript != null}");
//        Debug.Log($"coinCollectorScript assigned: {coinCollectorScript != null}");
//    }

//    private void OnEnable()
//    {
//        if (holdingSphere != null)
//            holdingSphere.HoldingChanged += OnHoldingChanged;
//    }

//    private void OnDisable()
//    {
//        if (holdingSphere != null)
//            holdingSphere.HoldingChanged -= OnHoldingChanged;

//        // Write any remaining buffered data
//        WriteBufferedData();

//        Debug.Log($"✅ Final data saved to: {filepath}");
//    }

//    private void Update()
//    {
//        // Reacquire hand device if lost
//        if (!handDevice.isValid)
//        {
//            handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);
//        }

//        // Get hand position
//        if (handDevice.isValid && handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
//        {
//            handPos = position;
//        }

//        // Distance to background
//        distance = background != null ? Vector3.Distance(handPos, background.transform.position) : 0f;

//        // Pacer X position
//        if (pacer != null && handPos.x > 0 && handPos.x < 0.55f && hitt == 1)
//            pacerx = pacer.transform.position.x;
//        else if (handPos.x < 0)
//            pacerx = 0f;
//        else if (handPos.x > 0.55f)
//            pacerx = 0.55f;
//        else
//            pacerx = pacer != null ? pacer.transform.position.x : 0f;

//        // Coin positions
//        float coin1x = 0f, coin1y = 0f;
//        float coin2x = 0f, coin2y = 0f;
//        float coin3x = 0f, coin3y = 0f;

//        if (coin1 != null && coin1.activeInHierarchy)
//        {
//            coin1x = coin1.transform.position.x;
//            coin1y = coin1.transform.position.y;
//        }

//        if (coin2 != null && coin2.activeInHierarchy)
//        {
//            coin2x = coin2.transform.position.x;
//            coin2y = coin2.transform.position.y;
//        }

//        if (coin3 != null && coin3.activeInHierarchy)
//        {
//            coin3x = coin3.transform.position.x;
//            coin3y = coin3.transform.position.y;
//        }

//        // Get coin location indices from CoinBombScript1
//        if (coinBombScript != null && coinBombScript.IsInitialized())
//        {
//            Coin1go = coinBombScript.GetCoin1Location();
//            Coin2go = coinBombScript.GetCoin2Location();
//            Coin3go = coinBombScript.GetCoin3Location();
//        }
//        else
//        {
//            Coin1go = Coin2go = Coin3go = 0;
//        }

//        // Get coin collection status from CoinCollector
//        if (coinCollectorScript != null)
//        {
//            CoinGet1 = coinCollectorScript.GetCoin1Status();
//            CoinGet2 = coinCollectorScript.GetCoin2Status();
//            CoinGet3 = coinCollectorScript.GetCoin3Status();
//        }
//        else
//        {
//            CoinGet1 = CoinGet2 = CoinGet3 = 0;
//        }

//        // Get Hit and Finished from collision detector
//        if (collisionDetector != null)
//        {
//            hitt = collisionDetector.Hit;
//            finished = collisionDetector.Finished;
//        }
//        else
//        {
//            hitt = 0;
//            finished = 0;
//        }

//        // Get hit/finished times and positions
//        float hitTime = 0f;
//        Vector3 hitPos = Vector3.zero;
//        float finishedTime = 0f;
//        Vector3 finishedPos = Vector3.zero;

//        if (collisionDetector != null)
//        {
//            hitTime = collisionDetector.hitTime;
//            hitPos = collisionDetector.hitPosition;
//            finishedTime = collisionDetector.finishedTime;
//            finishedPos = collisionDetector.finishedPosition;
//        }

//        // Build CSV line
//        StringBuilder line = new StringBuilder();
//        line.Append($"{Time.frameCount}{delimiter}{Time.time:F4}{delimiter}{distance:F4}{delimiter}");
//        line.Append($"{handPos.x:F4}{delimiter}{handPos.y:F4}{delimiter}{handPos.z:F4}{delimiter}");
//        line.Append($"{pacerx:F4}{delimiter}{(isHolding ? 1 : 0)}{delimiter}{hitt}{delimiter}{finished}{delimiter}");
//        line.Append($"{hitTime:F4}{delimiter}{hitPos.x:F4}{delimiter}{hitPos.y:F4}{delimiter}{hitPos.z:F4}{delimiter}");
//        line.Append($"{finishedTime:F4}{delimiter}{finishedPos.x:F4}{delimiter}{finishedPos.y:F4}{delimiter}{finishedPos.z:F4}{delimiter}");
//        line.Append($"{coin1x:F4}{delimiter}{coin1y:F4}{delimiter}{Coin1go}{delimiter}{CoinGet1}{delimiter}");
//        line.Append($"{coin2x:F4}{delimiter}{coin2y:F4}{delimiter}{Coin2go}{delimiter}{CoinGet2}{delimiter}");
//        line.Append($"{coin3x:F4}{delimiter}{coin3y:F4}{delimiter}{Coin3go}{delimiter}{CoinGet3}");

//        frameData.Add(line.ToString());

//        // Write buffer periodically to avoid data loss
//        if (frameData.Count >= WRITE_BUFFER_SIZE)
//        {
//            WriteBufferedData();
//        }

//        // Debug log every 300 frames
//        if (Time.frameCount % 300 == 0)
//        {
//            Debug.Log($"Frame {Time.frameCount}: Hit={hitt}, Finished={finished}, " +
//                     $"Coins collected: [{CoinGet1},{CoinGet2},{CoinGet3}], " +
//                     $"Locations: [{Coin1go},{Coin2go},{Coin3go}], " +
//                     $"Buffered lines={frameData.Count}");
//        }
//    }

//    private void WriteBufferedData()
//    {
//        if (frameData.Count == 0) return;

//        try
//        {
//            File.AppendAllLines(filepath, frameData);
//            Debug.Log($"✓ Wrote {frameData.Count} lines to CSV");
//            frameData.Clear();
//        }
//        catch (System.Exception e)
//        {
//            Debug.LogError($"✗ Failed to write CSV data: {e.Message}");
//        }
//    }

//    private void OnHoldingChanged(bool holding)
//    {
//        isHolding = holding;
//        Debug.Log($"Holding changed: {holding}");
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.XR;
using System;

public class SaveDataTrainingXR : MonoBehaviour
{
    [Header("Main References")]
    public XRNode trackedHand = XRNode.RightHand;
    public GameObject pacer;
    public GameObject background;

    [Header("Grabbable Sphere")]
    [Tooltip("Sphere that has IsHoldingSphere + SineWaveCollisionDetector")]
    public GameObject sphere;
    private IsHoldingSphere holdingSphere;
    private SineWaveCollisionDetector collisionDetector;

    [Header("Coins")]
    public GameObject coin1;
    public GameObject coin2;
    public GameObject coin3;
    public GameObject sinusoid;

    [Header("Coin Scripts")]
    [Tooltip("CoinBombScript1 - handles coin placement")]
    public CoinBombScript1 coinBombScript;
    [Tooltip("CoinCollector - handles coin collection")]
    public CoinCollector coinCollectorScript;

    [Header("File Settings")]
    [Tooltip("Leave empty to use default location (My Documents/MetaXR_Logs). Or specify custom path.")]
    public string filepathpre = "";
    private string delimiter = ",";
    private string extension = ".csv";
    private string filepath;

    // Runtime state
    private InputDevice handDevice;
    private Vector3 handPos;
    private bool isHolding;
    private int hitt;
    private float pacerx;
    private float distance;
    private int finished;

    // Coin data
    private int CoinGet1, CoinGet2, CoinGet3;
    private int Coin1go, Coin2go, Coin3go;

    // Sine wave start/end positions
    private Vector3 go0Position = Vector3.zero;
    private Vector3 go251Position = Vector3.zero;
    private bool sineWavePositionsCached = false;

    // CSV buffer - write every N frames instead of all at end
    private readonly List<string> frameData = new List<string>();
    private const int WRITE_BUFFER_SIZE = 300; // Write every 300 frames (~5 seconds at 60fps)

    private void Awake()
    {
        Debug.Log("=== SaveDataTrainingXR: Awake() called ===");

        hitt = 0;
        finished = 0;

        // Determine save location
        if (string.IsNullOrEmpty(filepathpre))
        {
            // Try USERPROFILE environment variable first (more reliable)
            string userProfile = System.Environment.GetEnvironmentVariable("USERPROFILE");
            if (!string.IsNullOrEmpty(userProfile))
            {
                filepathpre = Path.Combine(userProfile, "Documents", "MetaXR_Logs");
            }
            else
            {
                // Fallback to SpecialFolder
                string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                filepathpre = Path.Combine(documentsPath, "MetaXR_Logs");
            }
            Debug.Log($"Using default documents path: {filepathpre}");
        }
        else
        {
            Debug.Log($"Using custom path from inspector: {filepathpre}");
        }

        // Create directory and verify write access
        try
        {
            Directory.CreateDirectory(filepathpre);

            // Test write permissions
            string testFile = Path.Combine(filepathpre, ".test");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);

            Debug.Log($"✓ Directory ready with write access: {filepathpre}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Cannot use directory '{filepathpre}': {e.Message}");

            // Ultimate fallback
            filepathpre = Application.persistentDataPath;
            Debug.LogWarning($"⚠️ Using Unity persistent data path: {filepathpre}");
        }

        // Create unique filename with timestamp
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filepath = Path.Combine(filepathpre, $"Trial_{timestamp}{extension}");

        Debug.Log($"📁 CSV file will be: {filepath}");

        // Write CSV header with new go0 and go251 position columns
        StringBuilder header = new StringBuilder();
        header.Append("Frame,Time,Distance,PosX,PosY,PosZ,PacerX,Holding,Hit,Finished,");
        header.Append("HitTime,HitPosX,HitPosY,HitPosZ,");
        header.Append("FinishedTime,FinishedPosX,FinishedPosY,FinishedPosZ,");
        header.Append("Go0_X,Go0_Y,Go0_Z,Go251_X,Go251_Y,Go251_Z,"); // NEW: Added go0 and go251 positions
        header.Append("Coin1x,Coin1y,Coin1go,Collected1,");
        header.Append("Coin2x,Coin2y,Coin2go,Collected2,");
        header.AppendLine("Coin3x,Coin3y,Coin3go,Collected3");

        try
        {
            File.WriteAllText(filepath, header.ToString());
            Debug.Log($"✅ CSV file created successfully!");
            Debug.Log($"✅ Header written to: {filepath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to create CSV file: {e.Message}");
            Debug.LogError($"❌ Attempted path: {filepath}");
        }
    }

    private void Start()
    {
        Debug.Log("=== SaveDataTrainingXR: Start() called ===");

        handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);
        Debug.Log($"Hand device initialized for {trackedHand}");

        // Cache components from sphere
        if (sphere != null)
        {
            holdingSphere = sphere.GetComponent<IsHoldingSphere>();
            if (holdingSphere == null)
                Debug.LogWarning("⚠️ Sphere missing IsHoldingSphere component!");
            else
                Debug.Log("✓ IsHoldingSphere found");

            collisionDetector = sphere.GetComponent<SineWaveCollisionDetector>();
            if (collisionDetector == null)
                Debug.LogWarning("⚠️ Sphere missing SineWaveCollisionDetector component!");
            else
                Debug.Log("✓ SineWaveCollisionDetector found");
        }
        else
        {
            Debug.LogError("✗ Sphere not assigned in inspector!");
        }

        Debug.Log($"Coin references - coin1:{coin1 != null}, coin2:{coin2 != null}, coin3:{coin3 != null}");
        Debug.Log($"coinBombScript assigned: {coinBombScript != null}");
        Debug.Log($"coinCollectorScript assigned: {coinCollectorScript != null}");

        // Cache sine wave positions after a short delay to ensure they're created
        Invoke(nameof(CacheSineWavePositions), 0.5f);
    }

    /// <summary>
    /// Find and cache the positions of go0 and go251 sine wave points
    /// </summary>
    private void CacheSineWavePositions()
    {
        GameObject go0 = GameObject.Find("go0");
        GameObject go251 = GameObject.Find("go251");

        if (go0 != null)
        {
            go0Position = go0.transform.position;
            Debug.Log($"✓ Cached go0 position: {go0Position}");
        }
        else
        {
            Debug.LogWarning("⚠️ Could not find go0 GameObject. Make sure SinusoidScript7 has created the sine wave.");
        }

        if (go251 != null)
        {
            go251Position = go251.transform.position;
            Debug.Log($"✓ Cached go251 position: {go251Position}");
        }
        else
        {
            Debug.LogWarning("⚠️ Could not find go251 GameObject. Make sure SinusoidScript7 has created the sine wave.");
        }

        sineWavePositionsCached = true;
    }

    private void OnEnable()
    {
        if (holdingSphere != null)
            holdingSphere.HoldingChanged += OnHoldingChanged;
    }

    private void OnDisable()
    {
        if (holdingSphere != null)
            holdingSphere.HoldingChanged -= OnHoldingChanged;

        // Write any remaining buffered data
        WriteBufferedData();

        Debug.Log($"✅ Final data saved to: {filepath}");
    }

    private void Update()
    {
        // Reacquire hand device if lost
        if (!handDevice.isValid)
        {
            handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);
        }

        // Get hand position
        if (handDevice.isValid && handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
        {
            handPos = position;
        }

        // Distance to background
        distance = background != null ? Vector3.Distance(handPos, background.transform.position) : 0f;

        // Pacer X position
        if (pacer != null && handPos.x > 0 && handPos.x < 0.55f && hitt == 1)
            pacerx = pacer.transform.position.x;
        else if (handPos.x < 0)
            pacerx = 0f;
        else if (handPos.x > 0.55f)
            pacerx = 0.55f;
        else
            pacerx = pacer != null ? pacer.transform.position.x : 0f;

        // Coin positions
        float coin1x = 0f, coin1y = 0f;
        float coin2x = 0f, coin2y = 0f;
        float coin3x = 0f, coin3y = 0f;

        if (coin1 != null && coin1.activeInHierarchy)
        {
            coin1x = coin1.transform.position.x;
            coin1y = coin1.transform.position.y;
        }

        if (coin2 != null && coin2.activeInHierarchy)
        {
            coin2x = coin2.transform.position.x;
            coin2y = coin2.transform.position.y;
        }

        if (coin3 != null && coin3.activeInHierarchy)
        {
            coin3x = coin3.transform.position.x;
            coin3y = coin3.transform.position.y;
        }

        // Get coin location indices from CoinBombScript1
        if (coinBombScript != null && coinBombScript.IsInitialized())
        {
            Coin1go = coinBombScript.GetCoin1Location();
            Coin2go = coinBombScript.GetCoin2Location();
            Coin3go = coinBombScript.GetCoin3Location();
        }
        else
        {
            Coin1go = Coin2go = Coin3go = 0;
        }

        // Get coin collection status from CoinCollector
        if (coinCollectorScript != null)
        {
            CoinGet1 = coinCollectorScript.GetCoin1Status();
            CoinGet2 = coinCollectorScript.GetCoin2Status();
            CoinGet3 = coinCollectorScript.GetCoin3Status();
        }
        else
        {
            CoinGet1 = CoinGet2 = CoinGet3 = 0;
        }

        // Get Hit and Finished from collision detector
        if (collisionDetector != null)
        {
            hitt = collisionDetector.Hit;
            finished = collisionDetector.Finished;
        }
        else
        {
            hitt = 0;
            finished = 0;
        }

        // Get hit/finished times and positions
        float hitTime = 0f;
        Vector3 hitPos = Vector3.zero;
        float finishedTime = 0f;
        Vector3 finishedPos = Vector3.zero;

        if (collisionDetector != null)
        {
            hitTime = collisionDetector.hitTime;
            hitPos = collisionDetector.hitPosition;
            finishedTime = collisionDetector.finishedTime;
            finishedPos = collisionDetector.finishedPosition;
        }

        // Build CSV line with go0 and go251 positions
        StringBuilder line = new StringBuilder();
        line.Append($"{Time.frameCount}{delimiter}{Time.time:F4}{delimiter}{distance:F4}{delimiter}");
        line.Append($"{handPos.x:F4}{delimiter}{handPos.y:F4}{delimiter}{handPos.z:F4}{delimiter}");
        line.Append($"{pacerx:F4}{delimiter}{(isHolding ? 1 : 0)}{delimiter}{hitt}{delimiter}{finished}{delimiter}");
        line.Append($"{hitTime:F4}{delimiter}{hitPos.x:F4}{delimiter}{hitPos.y:F4}{delimiter}{hitPos.z:F4}{delimiter}");
        line.Append($"{finishedTime:F4}{delimiter}{finishedPos.x:F4}{delimiter}{finishedPos.y:F4}{delimiter}{finishedPos.z:F4}{delimiter}");
        // NEW: Add go0 and go251 positions
        line.Append($"{go0Position.x:F4}{delimiter}{go0Position.y:F4}{delimiter}{go0Position.z:F4}{delimiter}");
        line.Append($"{go251Position.x:F4}{delimiter}{go251Position.y:F4}{delimiter}{go251Position.z:F4}{delimiter}");
        line.Append($"{coin1x:F4}{delimiter}{coin1y:F4}{delimiter}{Coin1go}{delimiter}{CoinGet1}{delimiter}");
        line.Append($"{coin2x:F4}{delimiter}{coin2y:F4}{delimiter}{Coin2go}{delimiter}{CoinGet2}{delimiter}");
        line.Append($"{coin3x:F4}{delimiter}{coin3y:F4}{delimiter}{Coin3go}{delimiter}{CoinGet3}");

        frameData.Add(line.ToString());

        // Write buffer periodically to avoid data loss
        if (frameData.Count >= WRITE_BUFFER_SIZE)
        {
            WriteBufferedData();
        }

        // Debug log every 300 frames
        if (Time.frameCount % 300 == 0)
        {
            Debug.Log($"Frame {Time.frameCount}: Hit={hitt}, Finished={finished}, " +
                     $"Coins collected: [{CoinGet1},{CoinGet2},{CoinGet3}], " +
                     $"Locations: [{Coin1go},{Coin2go},{Coin3go}], " +
                     $"go0={go0Position}, go251={go251Position}, " +
                     $"Buffered lines={frameData.Count}");
        }
    }

    private void WriteBufferedData()
    {
        if (frameData.Count == 0) return;

        try
        {
            File.AppendAllLines(filepath, frameData);
            Debug.Log($"✓ Wrote {frameData.Count} lines to CSV");
            frameData.Clear();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ Failed to write CSV data: {e.Message}");
        }
    }

    private void OnHoldingChanged(bool holding)
    {
        isHolding = holding;
        Debug.Log($"Holding changed: {holding}");
    }
}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System.IO;
//using System.Text;
//using UnityEngine.XR;
//using UnityEngine.XR.Interaction.Toolkit;
//using System; // <-- For DateTime

//public class SaveDataTrainingXR : MonoBehaviour
//{
//    [Header("Main References")]
//    public XRNode trackedHand = XRNode.RightHand;
//    public GameObject pacer;
//    public GameObject background;

//    [Header("Coins (assign in inspector)")]
//    public GameObject coin1;
//    public GameObject coin2;
//    public GameObject coin3;
//    public GameObject sinusoid;

//    [Header("File Settings")]
//    [Tooltip("Base folder where logs are stored")]
//    public string filepathpre = @"C:/Users/Hruthik/Documents/MetaXR_Logs/";
//    private string delimiter = ",";
//    private string extension = ".csv";
//    private string filepath;

//    private InputDevice handDevice;
//    private Vector3 handPos;
//    private bool isHolding;
//    private int hitt;
//    private float pacerx;
//    private float distance;
//    private int finished;

//    private int CoinGet1, CoinGet2, CoinGet3;
//    private int Coin1go, Coin2go, Coin3go;

//    private List<string> frameData = new List<string>(); // Store data to write once at end

//    private void Awake()
//    {
//        hitt = 0;
//        finished = 0;

//        // ✅ Ensure directory exists
//        if (!Directory.Exists(filepathpre))
//        {
//            Directory.CreateDirectory(filepathpre);
//        }

//        // ✅ Create a new file per run using timestamp
//        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
//        filepath = Path.Combine(filepathpre, $"Trial_{timestamp}{extension}");

//        // ✅ Write header
//        StringBuilder header = new StringBuilder();
//        header.AppendLine("Frame,Time,Distance,PosX,PosY,PosZ,PacerX,Hit,Finished," +
//                          "Coin1x,Coin1y,Coin1go,Collected1," +
//                          "Coin2x,Coin2y,Coin2go,Collected2," +
//                          "Coin3x,Coin3y,Coin3go,Collected3");
//        File.AppendAllText(filepath, header.ToString());
//    }

//    private void Start()
//    {
//        handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);
//    }

//    private void Update()
//    {
//        // Reacquire hand device if lost
//        if (!handDevice.isValid)
//            handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);

//        // Get hand position
//        if (handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
//            handPos = position;

//        // Detect gripping (holding)
//        if (handDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isGripping))
//        {
//            isHolding = isGripping;
//            hitt = isHolding ? 1 : 0;
//        }

//        // Distance between hand and background
//        distance = background != null ? Vector3.Distance(handPos, background.transform.position) : 0f;

//        // Pacer X position limit
//        if (handPos.x > 0 && handPos.x < 0.55f && hitt == 1)
//            pacerx = pacer != null ? pacer.transform.position.x : 0f;
//        else if (handPos.x < 0)
//            pacerx = 0f;
//        else if (handPos.x > 0.55f)
//            pacerx = 0.55f;

//        // Get coin positions
//        float coin1x = coin1 ? coin1.transform.position.x : 0f;
//        float coin1y = coin1 ? coin1.transform.position.y : 0f;
//        float coin2x = coin2 ? coin2.transform.position.x : 0f;
//        float coin2y = coin2 ? coin2.transform.position.y : 0f;
//        float coin3x = coin3 ? coin3.transform.position.x : 0f;
//        float coin3y = coin3 ? coin3.transform.position.y : 0f;

//        // Placeholder coin logic
//        CoinGet1 = CoinGet2 = CoinGet3 = 0;
//        Coin1go = Coin2go = Coin3go = 0;

//        // Collect line of data
//        StringBuilder line = new StringBuilder();
//        line.AppendLine(Time.frameCount + delimiter + Time.time + delimiter + distance + delimiter +
//                        handPos.x + delimiter + handPos.y + delimiter + handPos.z + delimiter +
//                        pacerx + delimiter + hitt + delimiter + finished + delimiter +
//                        coin1x + delimiter + coin1y + delimiter + Coin1go + delimiter + CoinGet1 + delimiter +
//                        coin2x + delimiter + coin2y + delimiter + Coin2go + delimiter + CoinGet2 + delimiter +
//                        coin3x + delimiter + coin3y + delimiter + Coin3go + delimiter + CoinGet3);

//        frameData.Add(line.ToString());
//    }

//    private void OnDisable()
//    {
//        // ✅ Write all collected data when scene stops or GameObject is disabled
//        if (frameData.Count > 0)
//        {
//            File.AppendAllLines(filepath, frameData);
//            frameData.Clear();
//        }

//        Debug.Log($"✅ Data saved successfully to: {filepath}");
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System; // For DateTime

public class SaveDataTrainingXR : MonoBehaviour
{
    [Header("Main References")]
    public XRNode trackedHand = XRNode.RightHand;
    public GameObject pacer;
    public GameObject background;

    [Header("Grabbable Sphere (assign in inspector)")]
    [Tooltip("Sphere that has IsHoldingSphere + XRGrabInteractable/Meta GrabInteractable")]
    public GameObject sphere;
    private IsHoldingSphere holdingSphere;

    [Header("Coins (assign in inspector)")]
    public GameObject coin1;
    public GameObject coin2;
    public GameObject coin3;
    public GameObject sinusoid;

    [Header("File Settings")]
    [Tooltip("Base folder where logs are stored")]
    public string filepathpre = @"C:/Users/Hruthik/Documents/MetaXR_Logs/";
    private string delimiter = ",";
    private string extension = ".csv";
    private string filepath;

    // Runtime state
    private InputDevice handDevice;
    private Vector3 handPos;
    private bool isHolding; // driven by IsHoldingSphere events
    private int hitt;       // 1 if holding, else 0
    private float pacerx;
    private float distance;
    private int finished;

    // Coin placeholders (keep your existing logic if you later compute these)
    private int CoinGet1, CoinGet2, CoinGet3;
    private int Coin1go, Coin2go, Coin3go;

    // Buffer lines and write on disable
    private readonly List<string> frameData = new List<string>();

    private void Awake()
    {
        hitt = 0;
        finished = 0;

        // Ensure directory exists
        if (!Directory.Exists(filepathpre))
            Directory.CreateDirectory(filepathpre);

        // Unique file per run
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filepath = Path.Combine(filepathpre, $"Trial_{timestamp}{extension}");

        // CSV header
        StringBuilder header = new StringBuilder();
        header.AppendLine("Frame,Time,Distance,PosX,PosY,PosZ,PacerX,Hit,Finished," +
                          "Coin1x,Coin1y,Coin1go,Collected1," +
                          "Coin2x,Coin2y,Coin2go,Collected2," +
                          "Coin3x,Coin3y,Coin3go,Collected3");
        File.AppendAllText(filepath, header.ToString());
    }

    private void Start()
    {
        // Get the hand device once; we’ll re-acquire if it drops
        handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);

        // Cache IsHoldingSphere from the assigned sphere
        if (sphere != null)
        {
            holdingSphere = sphere.GetComponent<IsHoldingSphere>();
            if (holdingSphere == null)
                Debug.LogWarning("⚠️ The assigned sphere is missing IsHoldingSphere. Add it so grab events can drive Hit/Holding.");
        }
        else
        {
            Debug.LogWarning("⚠️ Please assign the grabbable sphere (with IsHoldingSphere) in the inspector.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to event-driven holding updates
        if (holdingSphere != null)
            holdingSphere.HoldingChanged += OnHoldingChanged;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid leaks
        if (holdingSphere != null)
            holdingSphere.HoldingChanged -= OnHoldingChanged;

        // Write all collected data when scene stops or GameObject is disabled
        if (frameData.Count > 0)
        {
            File.AppendAllLines(filepath, frameData);
            frameData.Clear();
        }

        Debug.Log($"✅ Data saved successfully to: {filepath}");
    }

    private void Update()
    {
        // Reacquire hand device if lost
        if (!handDevice.isValid)
            handDevice = InputDevices.GetDeviceAtXRNode(trackedHand);

        // Hand/controller/world position (leave as-is for your setup)
        if (handDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position))
            handPos = position;

        // ❌ Removed: gripButton polling
        // The Meta/XRI interaction system is driving isHolding/hitt via events now.

        // Distance to background (kept as-is)
        distance = background != null ? Vector3.Distance(handPos, background.transform.position) : 0f;

        // Pacer X position limit, only while holding
        if (handPos.x > 0 && handPos.x < 0.55f && hitt == 1)
            pacerx = pacer != null ? pacer.transform.position.x : 0f;
        else if (handPos.x < 0)
            pacerx = 0f;
        else if (handPos.x > 0.55f)
            pacerx = 0.55f;

        // Coin positions
        float coin1x = coin1 ? coin1.transform.position.x : 0f;
        float coin1y = coin1 ? coin1.transform.position.y : 0f;
        float coin2x = coin2 ? coin2.transform.position.x : 0f;
        float coin2y = coin2 ? coin2.transform.position.y : 0f;
        float coin3x = coin3 ? coin3.transform.position.x : 0f;
        float coin3y = coin3 ? coin3.transform.position.y : 0f;

        // Placeholder coin logic (keep your original if you compute these elsewhere)
        CoinGet1 = CoinGet2 = CoinGet3 = 0;
        Coin1go = Coin2go = Coin3go = 0;

        // Collect line of data
        StringBuilder line = new StringBuilder();
        line.AppendLine(
            Time.frameCount + delimiter + Time.time + delimiter + distance + delimiter +
            handPos.x + delimiter + handPos.y + delimiter + handPos.z + delimiter +
            pacerx + delimiter + hitt + delimiter + finished + delimiter +
            coin1x + delimiter + coin1y + delimiter + Coin1go + delimiter + CoinGet1 + delimiter +
            coin2x + delimiter + coin2y + delimiter + Coin2go + delimiter + CoinGet2 + delimiter +
            coin3x + delimiter + coin3y + delimiter + Coin3go + delimiter + CoinGet3
        );

        frameData.Add(line.ToString());
    }

    // Event handler from IsHoldingSphere (push-style integration)
    private void OnHoldingChanged(bool holding)
    {
        isHolding = holding;
        hitt = holding ? 1 : 0;
    }
}

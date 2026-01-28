//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CoinBombScript1 : MonoBehaviour
//{
//    [Header("Coin GameObjects")]
//    public GameObject coin1;
//    public GameObject coin2;
//    public GameObject coin3;

//    [Header("Visual Settings")]
//    public Material orange;
//    public Material blue; // NEW: Blue material for proactive mode

//    [Header("Mode Settings")]
//    [Tooltip("0 = Normal mode (coins on sine wave), 1 = Proactive mode (coins offset 10cm above/below)")]
//    [Range(0, 1)]
//    public int mode = 0;

//    [Header("Proactive Mode Settings")]
//    [Tooltip("Distance offset in proactive mode (in meters, default 0.1 = 10cm)")]
//    public float proactiveOffset = 0.1f;

//    [Header("Sine Wave Settings")]
//    [Tooltip("Optional: Reference to SinusoidScript7. If not set, will search for sine wave objects by name.")]
//    public SinusoidScript7 sinusoidScript;

//    [Tooltip("Vertical offset from sine wave position (in meters). Set to 0 to place coins directly on the wave.")]
//    public float verticalOffset = 0.0f;

//    [Tooltip("If true, coins will be placed with vertical offset. If false, coins are placed directly on the sine wave.")]
//    public bool useVerticalOffset = false;

//    // Internal variables
//    private GameObject[] coinarray;
//    private GameObject Gobj1;
//    private GameObject Gobj2;
//    private GameObject Gobj3;
//    private int[] ydirs;
//    private Vector3 coin1Position;
//    private Vector3 coin2Position;
//    private Vector3 coin3Position;

//    // NEW: Proactive mode - actual target positions on sine wave
//    private GameObject ProactiveGobj1;
//    private GameObject ProactiveGobj2;
//    private GameObject ProactiveGobj3;
//    private Vector3 proactiveCoin1Position;
//    private Vector3 proactiveCoin2Position;
//    private Vector3 proactiveCoin3Position;

//    // Random coin locations from StaticValsReach7
//    private int coin1Location;
//    private int coin2Location;
//    private int coin3Location;

//    // NEW: Proactive mode target locations (10cm offset from coin locations)
//    private int proactiveLocation1;
//    private int proactiveLocation2;
//    private int proactiveLocation3;

//    private bool isInitialized = false;

//    void Start()
//    {
//        mode = StaticValsReach7.requestedMode;

//        Debug.Log("Mode==="+ mode);
//        Debug.Log("=== CoinBombScript1: Start() called ===");
//        Debug.Log("CoinBombScript1: MODE = " + mode + " (0=Normal, 1=Proactive)");
//        Debug.Log("CoinBombScript1: coin1 assigned = " + (coin1 != null));
//        Debug.Log("CoinBombScript1: coin2 assigned = " + (coin2 != null));
//        Debug.Log("CoinBombScript1: coin3 assigned = " + (coin3 != null));
//        Debug.Log("CoinBombScript1: orange material assigned = " + (orange != null));
//        Debug.Log("CoinBombScript1: blue material assigned = " + (blue != null));

//        // Get random coin locations from StaticValsReach7
//        GetRandomCoinLocations();

//        Debug.Log("CoinBombScript1: coin1Location = " + coin1Location);
//        Debug.Log("CoinBombScript1: coin2Location = " + coin2Location);
//        Debug.Log("CoinBombScript1: coin3Location = " + coin3Location);

//        if (mode == 1)
//        {
//            Debug.Log("CoinBombScript1: PROACTIVE MODE - Offset targets by " + (proactiveOffset * 100) + "cm");
//            Debug.Log("CoinBombScript1: proactiveLocation1 = " + proactiveLocation1);
//            Debug.Log("CoinBombScript1: proactiveLocation2 = " + proactiveLocation2);
//            Debug.Log("CoinBombScript1: proactiveLocation3 = " + proactiveLocation3);
//        }

//        // Wait a frame to ensure sine wave is created first
//        StartCoroutine(InitializeCoins());
//    }

//    /// <summary>
//    /// Get random coin locations using StaticValsReach7.Shuffle()
//    /// </summary>
//    void GetRandomCoinLocations()
//    {
//        // Get 3 random locations with minimum 15 units apart
//        int[] randomLocations = StaticValsReach7.Shuffle();

//        coin1Location = randomLocations[0];
//        coin2Location = randomLocations[1];
//        coin3Location = randomLocations[2];

//        Debug.Log($"Random coin locations selected: [{coin1Location}, {coin2Location}, {coin3Location}]");

//        // NEW: Calculate proactive mode target locations (offset by ~10cm worth of sine wave points)
//        // 10cm = 0.1m, sine wave spacing is 0.001992032m per point
//        // So ~50 points = 0.0996m ≈ 10cm
//        int offsetPoints = Mathf.RoundToInt(proactiveOffset / 0.001992032f);

//        // Randomly decide if offset is positive or negative for each coin
//        System.Random random = new System.Random();
//        int dir1 = random.Next(0, 2) == 0 ? -1 : 1;
//        int dir2 = random.Next(0, 2) == 0 ? -1 : 1;
//        int dir3 = random.Next(0, 2) == 0 ? -1 : 1;

//        proactiveLocation1 = Mathf.Clamp(coin1Location + (dir1 * offsetPoints), 0, 251);
//        proactiveLocation2 = Mathf.Clamp(coin2Location + (dir2 * offsetPoints), 0, 251);
//        proactiveLocation3 = Mathf.Clamp(coin3Location + (dir3 * offsetPoints), 0, 251);

//        Debug.Log($"Proactive target locations: [{proactiveLocation1}, {proactiveLocation2}, {proactiveLocation3}]");
//    }

//    /// <summary>
//    /// Public method to randomize coin positions again (for new trials)
//    /// </summary>
//    public void RandomizeCoins()
//    {
//        GetRandomCoinLocations();
//        StartCoroutine(InitializeCoins());
//    }

//    IEnumerator InitializeCoins()
//    {
//        // Wait for SinusoidScript7 to finish creating the sine wave
//        GameObject sinusoidParent = null;
//        int waitAttempts = 0;
//        const int maxWaitAttempts = 50;

//        Debug.Log("CoinBombScript1: Waiting for SinusoidScript7 to create sine wave...

//        while (sinusoidParent == null && waitAttempts < maxWaitAttempts)
//        {
//            sinusoidParent = GameObject.Find("Sinusoid_Fixed_World_Space");
//            if (sinusoidParent == null)
//            {
//                waitAttempts++;
//                yield return new WaitForSeconds(0.1f);
//            }
//        }

//        if (sinusoidParent == null)
//        {
//            Debug.LogError("CoinBombScript1: SinusoidScript7 did not create the sine wave parent object!");
//            yield break;
//        }

//        Debug.Log("CoinBombScript1: Found sine wave parent object.");

//        // Wait for sine wave objects to be created
//        GameObject testObject = null;
//        waitAttempts = 0;
//        while (testObject == null && waitAttempts < maxWaitAttempts)
//        {
//            Transform[] children = sinusoidParent.GetComponentsInChildren<Transform>();
//            foreach (Transform child in children)
//            {
//                if (child.name == "go0" || child.name == "go251")
//                {
//                    testObject = child.gameObject;
//                    break;
//                }
//            }

//            if (testObject == null)
//            {
//                waitAttempts++;
//                yield return new WaitForSeconds(0.1f);
//            }
//        }

//        if (testObject == null)
//        {
//            Debug.LogError("CoinBombScript1: Sine wave objects were not created!");
//            yield break;
//        }

//        Debug.Log("CoinBombScript1: Sine wave objects confirmed. Finding coin locations...");

//        // Get all children
//        Transform[] allChildren = sinusoidParent.GetComponentsInChildren<Transform>();

//        if (mode == 0)
//        {
//            // NORMAL MODE: Find coin locations
//            FindNormalModeLocations(allChildren);
//        }
//        else if (mode == 1)
//        {
//            // PROACTIVE MODE: Find both coin locations AND target locations
//            FindProactiveModeLocations(allChildren);
//        }

//        // Validate that objects were found
//        if (Gobj1 == null || Gobj2 == null || Gobj3 == null)
//        {
//            Debug.LogError("CoinBombScript1: Could not find sine wave GameObjects!");
//            yield break;
//        }

//        // Generate random Y directions
//        ydirs = new int[3];
//        System.Random random = new System.Random();
//        for (int i = 0; i < 3; i++)
//        {
//            ydirs[i] = random.Next(-1, 2) < 1 ? -1 : 1;
//        }

//        // Calculate coin positions
//        CalculateCoinPositions();

//        Debug.Log("CoinBombScript1: Calculated coin positions:");
//        Debug.Log("  coin1Position: " + coin1Position);
//        Debug.Log("  coin2Position: " + coin2Position);
//        Debug.Log("  coin3Position: " + coin3Position);

//        // Place coins and mark sine wave points
//        PlaceCoins();

//        isInitialized = true;
//        Debug.Log("=== CoinBombScript1: Initialization complete! ===");
//    }

//    void FindNormalModeLocations(Transform[] allChildren)
//    {
//        int loc1 = Mathf.Clamp(coin1Location, 0, 251);
//        int loc2 = Mathf.Clamp(coin2Location, 0, 251);
//        int loc3 = Mathf.Clamp(coin3Location, 0, 251);

//        Debug.Log("CoinBombScript1: NORMAL MODE - Finding locations: " + loc1 + ", " + loc2 + ", " + loc3);

//        foreach (Transform child in allChildren)
//        {
//            if (child.name == "go" + loc1) Gobj1 = child.gameObject;
//            if (child.name == "go" + loc2) Gobj2 = child.gameObject;
//            if (child.name == "go" + loc3) Gobj3 = child.gameObject;
//        }
//    }

//    void FindProactiveModeLocations(Transform[] allChildren)
//    {
//        int loc1 = Mathf.Clamp(coin1Location, 0, 251);
//        int loc2 = Mathf.Clamp(coin2Location, 0, 251);
//        int loc3 = Mathf.Clamp(coin3Location, 0, 251);

//        int proLoc1 = Mathf.Clamp(proactiveLocation1, 0, 251);
//        int proLoc2 = Mathf.Clamp(proactiveLocation2, 0, 251);
//        int proLoc3 = Mathf.Clamp(proactiveLocation3, 0, 251);

//        Debug.Log("CoinBombScript1: PROACTIVE MODE");
//        Debug.Log("  Coin locations (where coins are placed): " + loc1 + ", " + loc2 + ", " + loc3);
//        Debug.Log("  Target locations (actual sine wave targets in BLUE): " + proLoc1 + ", " + proLoc2 + ", " + proLoc3);

//        foreach (Transform child in allChildren)
//        {
//            // Coin placement locations (10cm offset from targets)
//            if (child.name == "go" + loc1) Gobj1 = child.gameObject;
//            if (child.name == "go" + loc2) Gobj2 = child.gameObject;
//            if (child.name == "go" + loc3) Gobj3 = child.gameObject;

//            // NEW: Target locations (actual sine wave points to hit - marked in BLUE)
//            if (child.name == "go" + proLoc1) ProactiveGobj1 = child.gameObject;
//            if (child.name == "go" + proLoc2) ProactiveGobj2 = child.gameObject;
//            if (child.name == "go" + proLoc3) ProactiveGobj3 = child.gameObject;
//        }

//        if (ProactiveGobj1 != null)
//        {
//            proactiveCoin1Position = ProactiveGobj1.transform.position;
//            Debug.Log("  ProactiveGobj1 (go" + proLoc1 + "): " + proactiveCoin1Position);
//        }
//        if (ProactiveGobj2 != null)
//        {
//            proactiveCoin2Position = ProactiveGobj2.transform.position;
//            Debug.Log("  ProactiveGobj2 (go" + proLoc2 + "): " + proactiveCoin2Position);
//        }
//        if (ProactiveGobj3 != null)
//        {
//            proactiveCoin3Position = ProactiveGobj3.transform.position;
//            Debug.Log("  ProactiveGobj3 (go" + proLoc3 + "): " + proactiveCoin3Position);
//        }
//    }

//    void CalculateCoinPositions()
//    {
//        if (mode == 1)
//        {
//            // Place coins above/below the blue sphere (proactive target)
//            coin1Position = proactiveCoin1Position + new Vector3(0, proactiveOffset * ydirs[0], 0);
//            coin2Position = proactiveCoin2Position + new Vector3(0, proactiveOffset * ydirs[1], 0);
//            coin3Position = proactiveCoin3Position + new Vector3(0, proactiveOffset * ydirs[2], 0);
//        }
//        else if (useVerticalOffset)
//        {
//            coin1Position = Gobj1.transform.position + new Vector3(0, verticalOffset * ydirs[0], 0);
//            coin2Position = Gobj2.transform.position + new Vector3(0, verticalOffset * ydirs[1], 0);
//            coin3Position = Gobj3.transform.position + new Vector3(0, verticalOffset * ydirs[2], 0);
//        }
//        else
//        {
//            coin1Position = Gobj1.transform.position;
//            coin2Position = Gobj2.transform.position;
//            coin3Position = Gobj3.transform.position;
//        }
//    }
//    void PlaceCoins()
//    {
//        // Place coins at calculated positions
//        if (coin1 != null)
//        {
//            coin1.transform.position = coin1Position;
//            coin1.SetActive(true);
//            MeshRenderer renderer = coin1.GetComponent<MeshRenderer>();
//            if (renderer != null) renderer.enabled = true;
//            Debug.Log("CoinBombScript1: ✓ Placed coin1 at position: " + coin1Position);
//        }

//        if (coin2 != null)
//        {
//            coin2.transform.position = coin2Position;
//            coin2.SetActive(true);
//            MeshRenderer renderer = coin2.GetComponent<MeshRenderer>();
//            if (renderer != null) renderer.enabled = true;
//            Debug.Log("CoinBombScript1: ✓ Placed coin2 at position: " + coin2Position);
//        }

//        if (coin3 != null)
//        {
//            coin3.transform.position = coin3Position;
//            coin3.SetActive(true);
//            MeshRenderer renderer = coin3.GetComponent<MeshRenderer>();
//            if (renderer != null) renderer.enabled = true;
//            Debug.Log("CoinBombScript1: ✓ Placed coin3 at position: " + coin3Position);
//        }

//        // Mark sine wave points based on mode
//        if (mode == 0)
//        {
//            // NORMAL MODE: Mark coin locations in ORANGE
//            MarkSineWavePoint(Gobj1, orange, "ORANGE (Normal Mode)");
//            MarkSineWavePoint(Gobj2, orange, "ORANGE (Normal Mode)");
//            MarkSineWavePoint(Gobj3, orange, "ORANGE (Normal Mode)");
//        }
//        else if (mode == 1)
//        {
//            // PROACTIVE MODE:
//            // Mark coin locations in ORANGE (where coins actually are)
//            MarkSineWavePoint(Gobj1, orange, "ORANGE (Coin Location)");
//            MarkSineWavePoint(Gobj2, orange, "ORANGE (Coin Location)");
//            MarkSineWavePoint(Gobj3, orange, "ORANGE (Coin Location)");

//            // Mark target locations in BLUE (actual targets on sine wave)
//            if (blue != null)
//            {
//                MarkSineWavePoint(ProactiveGobj1, blue, "BLUE (Target Location)");
//                MarkSineWavePoint(ProactiveGobj2, blue, "BLUE (Target Location)");
//                MarkSineWavePoint(ProactiveGobj3, blue, "BLUE (Target Location)");
//            }
//            else
//            {
//                Debug.LogError("CoinBombScript1: Blue material not assigned! Proactive mode targets won't be visible!");
//            }
//        }

//        // Create coin array
//        coinarray = new GameObject[3];
//        coinarray[0] = coin1;
//        coinarray[1] = coin2;
//        coinarray[2] = coin3;

//        Debug.Log("CoinBombScript1: All coins placed and sine wave points marked.");
//    }

//    void MarkSineWavePoint(GameObject gobj, Material material, string description)
//    {
//        if (gobj != null && material != null)
//        {
//            MeshRenderer renderer = gobj.GetComponent<MeshRenderer>();
//            if (renderer != null)
//            {
//                renderer.material = material;
//                gobj.transform.localScale = new Vector3(.01f, .01f, .01f);
//                gobj.tag = "step1";
//                Debug.Log("CoinBombScript1: Marked " + gobj.name + " as " + description);
//            }
//        }
//    }

//    // Public methods
//    public int GetCoin1Location() { return coin1Location; }
//    public int GetCoin2Location() { return coin2Location; }
//    public int GetCoin3Location() { return coin3Location; }

//    // NEW: Get proactive target locations
//    public int GetProactiveLocation1() { return proactiveLocation1; }
//    public int GetProactiveLocation2() { return proactiveLocation2; }
//    public int GetProactiveLocation3() { return proactiveLocation3; }

//    // NEW: Get proactive target positions
//    public Vector3 GetProactiveCoin1Position() { return proactiveCoin1Position; }
//    public Vector3 GetProactiveCoin2Position() { return proactiveCoin2Position; }
//    public Vector3 GetProactiveCoin3Position() { return proactiveCoin3Position; }

//    public bool IsInitialized()
//    {
//        return isInitialized && Gobj1 != null && Gobj2 != null && Gobj3 != null;
//    }

//    public Vector3 GetCoinPosition(int coinIndex)
//    {
//        if (coinIndex == 0) return coin1Position;
//        if (coinIndex == 1) return coin2Position;
//        if (coinIndex == 2) return coin3Position;
//        return Vector3.zero;
//    }

//    public GameObject GetSineWavePoint(int coinIndex)
//    {
//        if (coinIndex == 0) return Gobj1;
//        if (coinIndex == 1) return Gobj2;
//        if (coinIndex == 2) return Gobj3;
//        return null;
//    }

//    public GameObject GetCoin(int coinIndex)
//    {
//        if (coinIndex == 0) return coin1;
//        if (coinIndex == 1) return coin2;
//        if (coinIndex == 2) return coin3;
//        return null;
//    }

//    // NEW: Get current mode
//    public int GetMode()
//    {
//        return mode;
//    }
//}





/*
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinBombScript1 : MonoBehaviour
{

    [Header("Coin GameObjects")]
    public GameObject coin1;
    public GameObject coin2;
    public GameObject coin3;

    [Header("Visual Settings")]
    public Material orange;
    public Material blue; // Blue material for proactive/reactive mode

    [Header("Mode Settings")]
    [Tooltip("0 = Normal, 1 = Proactive (always visible), 2 = Reactive (only visible when user is near blue sphere)")]
    [Range(0, 2)]
    public int mode = 0;

    [Header("Proactive/Reactive Mode Settings")]
    [Tooltip("Distance offset in proactive/reactive mode (in meters, default 0.1 = 10cm)")]
    public float proactiveOffset = 0.1f;

    [Header("Reactive Mode Settings")]
    [Tooltip("User sphere (player/camera) for proximity check in reactive mode")]
    public Transform userSphere; // Assign in Inspector

    [Header("Sine Wave Settings")]
    [Tooltip("Optional: Reference to SinusoidScript7. If not set, will search for sine wave objects by name.")]
    public SinusoidScript7 sinusoidScript;

    [Tooltip("Vertical offset from sine wave position (in meters). Set to 0 to place coins directly on the wave.")]
    public float verticalOffset = 0.0f;

    [Tooltip("If true, coins will be placed with vertical offset. If false, coins are placed directly on the sine wave.")]
    public bool useVerticalOffset = false;

    // Internal variables
    private GameObject[] coinarray;
    private GameObject Gobj1;
    private GameObject Gobj2;
    private GameObject Gobj3;
    private int[] ydirs;
    private Vector3 coin1Position;
    private Vector3 coin2Position;
    private Vector3 coin3Position;

    // Proactive/reactive mode - actual target positions on sine wave
    private GameObject ProactiveGobj1;
    private GameObject ProactiveGobj2;
    private GameObject ProactiveGobj3;
    private Vector3 proactiveCoin1Position;
    private Vector3 proactiveCoin2Position;
    private Vector3 proactiveCoin3Position;

    // Random coin locations from StaticValsReach7
    private int coin1Location;
    private int coin2Location;
    private int coin3Location;

    // Proactive/reactive mode target locations (10cm offset from coin locations)
    private int proactiveLocation1;
    private int proactiveLocation2;
    private int proactiveLocation3;

    private bool isInitialized = false;

    void Start()
    {
        // Ensure StaticValsReach7 has valid values
        if (StaticValsReach7.ran1 == 0 && StaticValsReach7.ran2 == 0 && StaticValsReach7.ran3 == 0)
        {
            Debug.LogWarning("CoinBombScript1: StaticValsReach7 values not set yet! Calling Set(0)...");
            StaticValsReach7.Set(0);
        }

        mode = StaticValsReach7.requestedMode;

        Debug.Log("Mode===" + mode);
        Debug.Log("=== CoinBombScript1: Start() called ===");
        Debug.Log("CoinBombScript1: MODE = " + mode + " (0=Normal, 1=Proactive, 2=Reactive)");
        Debug.Log("CoinBombScript1: coin1 assigned = " + (coin1 != null));
        Debug.Log("CoinBombScript1: coin2 assigned = " + (coin2 != null));
        Debug.Log("CoinBombScript1: coin3 assigned = " + (coin3 != null));
        Debug.Log("CoinBombScript1: orange material assigned = " + (orange != null));
        Debug.Log("CoinBombScript1: blue material assigned = " + (blue != null));
        Debug.Log("CoinBombScript1: userSphere assigned = " + (userSphere != null));

        // Get random coin locations from StaticValsReach7
        GetRandomCoinLocations();

        Debug.Log("CoinBombScript1: coin1Location = " + coin1Location);
        Debug.Log("CoinBombScript1: coin2Location = " + coin2Location);
        Debug.Log("CoinBombScript1: coin3Location = " + coin3Location);

        if (mode == 1 || mode == 2)
        {
            Debug.Log("CoinBombScript1: PRO/REACTIVE MODE - Offset targets by " + (proactiveOffset * 100) + "cm");
            Debug.Log("CoinBombScript1: proactiveLocation1 = " + proactiveLocation1);
            Debug.Log("CoinBombScript1: proactiveLocation2 = " + proactiveLocation2);
            Debug.Log("CoinBombScript1: proactiveLocation3 = " + proactiveLocation3);
        }

        // Wait a frame to ensure sine wave is created first
        StartCoroutine(InitializeCoins());
    }

    ///
        void Update()
    {
        // Reactive mode: update coin visibility based on user proximity
        if (mode == 2 && userSphere != null && isInitialized)
        {
            SetCoinVisibility(coin1, proactiveCoin1Position, userSphere.position);
            SetCoinVisibility(coin2, proactiveCoin2Position, userSphere.position);
            SetCoinVisibility(coin3, proactiveCoin3Position, userSphere.position);
        }
    }
    ///

    // Add this field to your class:
    private bool[] coinActivated = new bool[3];

    void Update()
    {
        if (mode == 2 && userSphere != null && isInitialized)
        {
            CheckAndActivateCoin(coin1, proactiveCoin1Position, userSphere.position, 0);
            CheckAndActivateCoin(coin2, proactiveCoin2Position, userSphere.position, 1);
            CheckAndActivateCoin(coin3, proactiveCoin3Position, userSphere.position, 2);
        }
    }

    // New method to handle activation logic
    void CheckAndActivateCoin(GameObject coin, Vector3 target, Vector3 user, int index)
    {
        if (coin == null || coinActivated[index]) return;

        float dist = Vector3.Distance(target, user);
        if (dist < 0.05f) // 0.05m = 5cm
        {
            coin.SetActive(true);
            coinActivated[index] = true;
        }
    }
    void SetCoinVisibility(GameObject coin, Vector3 target, Vector3 user)
    {
        if (coin == null) return;
        float dist = Vector3.Distance(target, user);
        coin.SetActive(dist < 0.1f); // 0.1m = 10cm
    }

    /// <summary>
    /// Get coin locations from StaticValsReach7 (already set by Set())
    /// </summary>
    void GetRandomCoinLocations()
    {
        // Use the coin locations already calculated by StaticValsReach7.Set()
        // DO NOT call Shuffle() again - that would generate different values!
        coin1Location = StaticValsReach7.ran1;
        coin2Location = StaticValsReach7.ran2;
        coin3Location = StaticValsReach7.ran3;

        Debug.Log($"Coin locations from StaticValsReach7: [{coin1Location}, {coin2Location}, {coin3Location}]");

        // Calculate proactive/reactive mode target locations (offset by ~10cm)
        int offsetPoints = Mathf.RoundToInt(proactiveOffset / 0.001992032f);

        System.Random random = new System.Random();
        int dir1 = random.Next(0, 2) == 0 ? -1 : 1;
        int dir2 = random.Next(0, 2) == 0 ? -1 : 1;
        int dir3 = random.Next(0, 2) == 0 ? -1 : 1;

        int min = StaticValsReach7.possiblelocations.Min();
        int max = StaticValsReach7.possiblelocations.Max();


        proactiveLocation1 = Mathf.Clamp(coin1Location + (dir1 * offsetPoints), min, max);
        proactiveLocation2 = Mathf.Clamp(coin2Location + (dir2 * offsetPoints), min, max);
        proactiveLocation3 = Mathf.Clamp(coin3Location + (dir3 * offsetPoints), min, max);


        //proactiveLocation1 = Mathf.Clamp(coin1Location + (dir1 * offsetPoints), 0, 251);
        //proactiveLocation2 = Mathf.Clamp(coin2Location + (dir2 * offsetPoints), 0, 251);
        //proactiveLocation3 = Mathf.Clamp(coin3Location + (dir3 * offsetPoints), 0, 251);

        Debug.Log($"Proactive/Reactive target locations: [{proactiveLocation1}, {proactiveLocation2}, {proactiveLocation3}]");
    }

    public void RandomizeCoins()
    {
        // Re-read the CURRENT values from StaticValsReach7
        coin1Location = StaticValsReach7.ran1;
        coin2Location = StaticValsReach7.ran2;
        coin3Location = StaticValsReach7.ran3;

        Debug.Log($"RandomizeCoins: Updated locations to [{coin1Location}, {coin2Location}, {coin3Location}]");

        // Recalculate proactive locations
        int offsetPoints = Mathf.RoundToInt(proactiveOffset / 0.001992032f);

        System.Random random = new System.Random();
        int dir1 = random.Next(0, 2) == 0 ? -1 : 1;
        int dir2 = random.Next(0, 2) == 0 ? -1 : 1;
        int dir3 = random.Next(0, 2) == 0 ? -1 : 1;

        int min = StaticValsReach7.possiblelocations.Min();
        int max = StaticValsReach7.possiblelocations.Max();

        proactiveLocation1 = Mathf.Clamp(coin1Location + (dir1 * offsetPoints), min, max);
        proactiveLocation2 = Mathf.Clamp(coin2Location + (dir2 * offsetPoints), min, max);
        proactiveLocation3 = Mathf.Clamp(coin3Location + (dir3 * offsetPoints), min, max);

        // Reset state
        isInitialized = false;
        Gobj1 = null;
        Gobj2 = null;
        Gobj3 = null;
        ProactiveGobj1 = null;
        ProactiveGobj2 = null;
        ProactiveGobj3 = null;

        // Reset activation flags for reactive mode
        for (int i = 0; i < coinActivated.Length; i++) coinActivated[i] = false;

        // Re-initialize coins
        StartCoroutine(InitializeCoins());
    }

    IEnumerator InitializeCoins()
    {
        // Wait for SinusoidScript7 to finish creating the sine wave
        GameObject sinusoidParent = null;
        int waitAttempts = 0;
        const int maxWaitAttempts = 50;

        Debug.Log("CoinBombScript1: Waiting for SinusoidScript7 to create sine wave...");

        while (sinusoidParent == null && waitAttempts < maxWaitAttempts)
        {
            sinusoidParent = GameObject.Find("Sinusoid_Fixed_World_Space");
            if (sinusoidParent == null)
            {
                waitAttempts++;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (sinusoidParent == null)
        {
            Debug.LogError("CoinBombScript1: SinusoidScript7 did not create the sine wave parent object!");
            yield break;
        }

        Debug.Log("CoinBombScript1: Found sine wave parent object.");

        // Wait for sine wave objects to be created
        GameObject testObject = null;
        waitAttempts = 0;
        while (testObject == null && waitAttempts < maxWaitAttempts)
        {
            Transform[] children = sinusoidParent.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name == "go0" || child.name == "go251")
                {
                    testObject = child.gameObject;
                    break;
                }
            }

            if (testObject == null)
            {
                waitAttempts++;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (testObject == null)
        {
            Debug.LogError("CoinBombScript1: Sine wave objects were not created!");
            yield break;
        }

        Debug.Log("CoinBombScript1: Sine wave objects confirmed. Finding coin locations...");

        // Get all children
        Transform[] allChildren = sinusoidParent.GetComponentsInChildren<Transform>();

        if (mode == 0)
        {
            FindNormalModeLocations(allChildren);
        }
        else if (mode == 1 || mode == 2)
        {
            FindProactiveModeLocations(allChildren);
        }

        // Validate that objects were found
        if (Gobj1 == null || Gobj2 == null || Gobj3 == null)
        {
            Debug.LogError("CoinBombScript1: Could not find sine wave GameObjects!");
            yield break;
        }

        // Generate random Y directions
        ydirs = new int[3];
        System.Random random = new System.Random();
        for (int i = 0; i < 3; i++)
        {
            ydirs[i] = random.Next(-1, 2) < 1 ? -1 : 1;
        }

        // Calculate coin positions
        CalculateCoinPositions();

        Debug.Log("CoinBombScript1: Calculated coin positions:");
        Debug.Log("  coin1Position: " + coin1Position);
        Debug.Log("  coin2Position: " + coin2Position);
        Debug.Log("  coin3Position: " + coin3Position);

        // Place coins and mark sine wave points
        PlaceCoins();

        isInitialized = true;
        Debug.Log("=== CoinBombScript1: Initialization complete! ===");
    }

    void FindNormalModeLocations(Transform[] allChildren)
    {
        int loc1 = Mathf.Clamp(coin1Location, 0, 251);
        int loc2 = Mathf.Clamp(coin2Location, 0, 251);
        int loc3 = Mathf.Clamp(coin3Location, 0, 251);

        Debug.Log("CoinBombScript1: NORMAL MODE - Finding locations: " + loc1 + ", " + loc2 + ", " + loc3);

        foreach (Transform child in allChildren)
        {
            if (child.name == "go" + loc1) Gobj1 = child.gameObject;
            if (child.name == "go" + loc2) Gobj2 = child.gameObject;
            if (child.name == "go" + loc3) Gobj3 = child.gameObject;
        }
    }

    void FindProactiveModeLocations(Transform[] allChildren)
    {
        int loc1 = Mathf.Clamp(coin1Location, 0, 251);
        int loc2 = Mathf.Clamp(coin2Location, 0, 251);
        int loc3 = Mathf.Clamp(coin3Location, 0, 251);

        int proLoc1 = Mathf.Clamp(proactiveLocation1, 0, 251);
        int proLoc2 = Mathf.Clamp(proactiveLocation2, 0, 251);
        int proLoc3 = Mathf.Clamp(proactiveLocation3, 0, 251);

        Debug.Log("CoinBombScript1: PRO/REACTIVE MODE");
        Debug.Log("  Coin locations (where coins are placed): " + loc1 + ", " + loc2 + ", " + loc3);
        Debug.Log("  Target locations (actual sine wave targets in BLUE): " + proLoc1 + ", " + proLoc2 + ", " + proLoc3);

        foreach (Transform child in allChildren)
        {
            // Coin placement locations (10cm offset from targets)
            if (child.name == "go" + loc1) Gobj1 = child.gameObject;
            if (child.name == "go" + loc2) Gobj2 = child.gameObject;
            if (child.name == "go" + loc3) Gobj3 = child.gameObject;

            // NEW: Target locations (actual sine wave points to hit - marked in BLUE)
            if (child.name == "go" + proLoc1) ProactiveGobj1 = child.gameObject;
            if (child.name == "go" + proLoc2) ProactiveGobj2 = child.gameObject;
            if (child.name == "go" + proLoc3) ProactiveGobj3 = child.gameObject;
        }

        if (ProactiveGobj1 != null)
        {
            proactiveCoin1Position = ProactiveGobj1.transform.position;
            Debug.Log("  ProactiveGobj1 (go" + proLoc1 + "): " + proactiveCoin1Position);
        }
        if (ProactiveGobj2 != null)
        {
            proactiveCoin2Position = ProactiveGobj2.transform.position;
            Debug.Log("  ProactiveGobj2 (go" + proLoc2 + "): " + proactiveCoin2Position);
        }
        if (ProactiveGobj3 != null)
        {
            proactiveCoin3Position = ProactiveGobj3.transform.position;
            Debug.Log("  ProactiveGobj3 (go" + proLoc3 + "): " + proactiveCoin3Position);
        }
    }

    void CalculateCoinPositions()
    {
        if (mode == 1 || mode == 2)
        {
            coin1Position = proactiveCoin1Position + new Vector3(0, proactiveOffset * ydirs[0], 0);
            coin2Position = proactiveCoin2Position + new Vector3(0, proactiveOffset * ydirs[1], 0);
            coin3Position = proactiveCoin3Position + new Vector3(0, proactiveOffset * ydirs[2], 0);
        }
        else if (useVerticalOffset)
        {
            coin1Position = Gobj1.transform.position + new Vector3(0, verticalOffset * ydirs[0], 0);
            coin2Position = Gobj2.transform.position + new Vector3(0, verticalOffset * ydirs[1], 0);
            coin3Position = Gobj3.transform.position + new Vector3(0, verticalOffset * ydirs[2], 0);
        }
        else
        {
            coin1Position = Gobj1.transform.position;
            coin2Position = Gobj2.transform.position;
            coin3Position = Gobj3.transform.position;
        }
    }

    void PlaceCoins()
    {
        // Place coins at calculated positions
        if (coin1 != null)
        {
            coin1.transform.position = coin1Position;
            coin1.SetActive(true);
            MeshRenderer renderer = coin1.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = true;
            Debug.Log("CoinBombScript1: ✓ Placed coin1 at position: " + coin1Position);
        }

        if (coin2 != null)
        {
            coin2.transform.position = coin2Position;
            coin2.SetActive(true);
            MeshRenderer renderer = coin2.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = true;
            Debug.Log("CoinBombScript1: ✓ Placed coin2 at position: " + coin2Position);
        }

        if (coin3 != null)
        {
            coin3.transform.position = coin3Position;
            coin3.SetActive(true);
            MeshRenderer renderer = coin3.GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = true;
            Debug.Log("CoinBombScript1: ✓ Placed coin3 at position: " + coin3Position);
        }

        // Mark sine wave points based on mode
        
        ///
        if (mode == 0)
        {
            MarkSineWavePoint(Gobj1, orange, "ORANGE (Normal Mode)");
            MarkSineWavePoint(Gobj2, orange, "ORANGE (Normal Mode)");
            MarkSineWavePoint(Gobj3, orange, "ORANGE (Normal Mode)");
        }
        ///
        if (mode == 0)
        {
            if (coin1 != null) coin1.SetActive(false);
            if (coin2 != null) coin2.SetActive(false);
            if (coin3 != null) coin3.SetActive(false);
            Debug.Log("CoinBombScript1: mode==0, coins are not placed.");
            return;
        }

        else if (mode == 1 || mode == 2)
        {
            MarkSineWavePoint(Gobj1, orange, "ORANGE (Coin Location)");
            MarkSineWavePoint(Gobj2, orange, "ORANGE (Coin Location)");
            MarkSineWavePoint(Gobj3, orange, "ORANGE (Coin Location)");

            if (blue != null)
            {
                MarkSineWavePoint(ProactiveGobj1, blue, "BLUE (Target Location)");
                MarkSineWavePoint(ProactiveGobj2, blue, "BLUE (Target Location)");
                MarkSineWavePoint(ProactiveGobj3, blue, "BLUE (Target Location)");
            }
            else
            {
                Debug.LogError("CoinBombScript1: Blue material not assigned! Pro/Reactive mode targets won't be visible!");
            }
        }

        // Hide coins at start in reactive mode
        if (mode == 2)
        {
            if (coin1 != null) coin1.SetActive(false);
            if (coin2 != null) coin2.SetActive(false);
            if (coin3 != null) coin3.SetActive(false);

            // Reset activation flags
            for (int i = 0; i < coinActivated.Length; i++) coinActivated[i] = false;
        }

        // Create coin array
        coinarray = new GameObject[3];
        coinarray[0] = coin1;
        coinarray[1] = coin2;
        coinarray[2] = coin3;

        Debug.Log("CoinBombScript1: All coins placed and sine wave points marked.");
    }

    void MarkSineWavePoint(GameObject gobj, Material material, string description)
    {
        if (gobj != null && material != null)
        {
            MeshRenderer renderer = gobj.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = material;
                gobj.transform.localScale = new Vector3(.01f, .01f, .01f);
                gobj.tag = "step1";
                Debug.Log("CoinBombScript1: Marked " + gobj.name + " as " + description);
            }
        }
    }

    // Public methods
    public int GetCoin1Location() { return coin1Location; }
    public int GetCoin2Location() { return coin2Location; }
    public int GetCoin3Location() { return coin3Location; }

    public int GetProactiveLocation1() { return proactiveLocation1; }
    public int GetProactiveLocation2() { return proactiveLocation2; }
    public int GetProactiveLocation3() { return proactiveLocation3; }

    public Vector3 GetProactiveCoin1Position() { return proactiveCoin1Position; }
    public Vector3 GetProactiveCoin2Position() { return proactiveCoin2Position; }
    public Vector3 GetProactiveCoin3Position() { return proactiveCoin3Position; }

    public bool IsInitialized()
    {
        return isInitialized && Gobj1 != null && Gobj2 != null && Gobj3 != null;
    }

    public Vector3 GetCoinPosition(int coinIndex)
    {
        if (coinIndex == 0) return coin1Position;
        if (coinIndex == 1) return coin2Position;
        if (coinIndex == 2) return coin3Position;
        return Vector3.zero;
    }

    public GameObject GetSineWavePoint(int coinIndex)
    {
        if (coinIndex == 0) return Gobj1;
        if (coinIndex == 1) return Gobj2;
        if (coinIndex == 2) return Gobj3;
        return null;
    }

    public GameObject GetCoin(int coinIndex)
    {
        if (coinIndex == 0) return coin1;
        if (coinIndex == 1) return coin2;
        if (coinIndex == 2) return coin3;
        return null;
    }

    public int GetMode()
    {
        return mode;
    }
}

*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinBombScript1 : MonoBehaviour
{
    [Header("Coin GameObjects")]
    public GameObject coin1;
    public GameObject coin2;
    public GameObject coin3;

    [Header("Visual Settings")]
    public Material orange;
    public Material blue;

    [Header("Mode Settings")]
    [Tooltip("0 = Normal, 1 = Proactive, 2 = Reactive")]
    [Range(0, 2)]
    public int mode = 0;

    [Header("Proactive/Reactive Mode Settings")]
    [Tooltip("Distance offset (meters, default 0.1 = 10cm)")]
    public float proactiveOffset = 0.1f;

    [Header("Reactive Mode Settings")]
    public Transform userSphere;

    [Header("Sine Wave Settings")]
    public SinusoidScript7 sinusoidScript;
    public float verticalOffset = 0.0f;
    public bool useVerticalOffset = false;

    // Internal variables
    private GameObject[] coinarray;
    private GameObject Gobj1, Gobj2, Gobj3;
    private GameObject ProactiveGobj1, ProactiveGobj2, ProactiveGobj3;
    private int[] ydirs;
    private Vector3 coin1Position, coin2Position, coin3Position;
    private Vector3 proactiveCoin1Position, proactiveCoin2Position, proactiveCoin3Position;
    private int coin1Location, coin2Location, coin3Location;
    private int proactiveLocation1, proactiveLocation2, proactiveLocation3;
    private bool isInitialized = false;
    private bool[] coinActivated = new bool[3];

    void Start()
    {
        if (StaticValsReach7.ran1 == 0 && StaticValsReach7.ran2 == 0 && StaticValsReach7.ran3 == 0)
        {
            Debug.LogWarning("CoinBombScript1: StaticValsReach7 not set. Calling Set(0)...");
            StaticValsReach7.Set(0);
        }

        mode = StaticValsReach7.requestedMode;
        Debug.Log($"CoinBombScript1: MODE = {mode} (0=Normal, 1=Proactive, 2=Reactive)");

        GetRandomCoinLocations();

        Debug.Log($"CoinBombScript1: Coin locations = [{coin1Location}, {coin2Location}, {coin3Location}]");
        if (mode == 1 || mode == 2)
        {
            Debug.Log($"CoinBombScript1: Proactive locations = [{proactiveLocation1}, {proactiveLocation2}, {proactiveLocation3}]");
        }

        StartCoroutine(InitializeCoins());
    }

    void Update()
    {
        if (mode == 2 && userSphere != null && isInitialized)
        {
            CheckAndActivateCoin(coin1, proactiveCoin1Position, userSphere.position, 0);
            CheckAndActivateCoin(coin2, proactiveCoin2Position, userSphere.position, 1);
            CheckAndActivateCoin(coin3, proactiveCoin3Position, userSphere.position, 2);
        }
    }

    void CheckAndActivateCoin(GameObject coin, Vector3 target, Vector3 user, int index)
    {
        if (coin == null || coinActivated[index]) return;
        if (Vector3.Distance(target, user) < 0.05f)
        {
            coin.SetActive(true);
            coinActivated[index] = true;
        }
    }

    /// <summary>
    /// Get coin locations from StaticValsReach7 and calculate unique proactive locations
    /// </summary>
    void GetRandomCoinLocations()
    {
        coin1Location = StaticValsReach7.ran1;
        coin2Location = StaticValsReach7.ran2;
        coin3Location = StaticValsReach7.ran3;

        Debug.Log($"Coin locations from StaticValsReach7: [{coin1Location}, {coin2Location}, {coin3Location}]");

        int offsetPoints = Mathf.RoundToInt(proactiveOffset / 0.001992032f); // ~50 points = 10cm

        // Calculate unique proactive locations using HashSet
        HashSet<int> usedLocations = new HashSet<int>();

        proactiveLocation1 = GetUniqueProactiveLocation(coin1Location, offsetPoints, usedLocations);
        usedLocations.Add(proactiveLocation1);

        proactiveLocation2 = GetUniqueProactiveLocation(coin2Location, offsetPoints, usedLocations);
        usedLocations.Add(proactiveLocation2);

        proactiveLocation3 = GetUniqueProactiveLocation(coin3Location, offsetPoints, usedLocations);

        Debug.Log($"Proactive locations (unique): [{proactiveLocation1}, {proactiveLocation2}, {proactiveLocation3}]");
    }

    /// <summary>
    /// Calculate a unique proactive location that isn't already used
    /// Uses smart direction to avoid edge clamping when possible
    /// </summary>
    int GetUniqueProactiveLocation(int coinLocation, int offsetPoints, HashSet<int> usedLocations)
    {
        const int MIN_CLAMP = 30;
        const int MAX_CLAMP = 210;

        // Smart direction: choose direction that avoids clamping
        int dirPositive = coinLocation + offsetPoints;
        int dirNegative = coinLocation - offsetPoints;

        bool positiveWouldClamp = dirPositive > MAX_CLAMP;
        bool negativeWouldClamp = dirNegative < MIN_CLAMP;

        int dir;
        if (positiveWouldClamp && !negativeWouldClamp)
        {
            dir = -1;  // Go negative to avoid upper clamp
        }
        else if (negativeWouldClamp && !positiveWouldClamp)
        {
            dir = 1;   // Go positive to avoid lower clamp
        }
        else
        {
            // Both safe or both clamp - pick random
            System.Random random = new System.Random();
            dir = random.Next(0, 2) == 0 ? 1 : -1;
        }

        int result = Mathf.Clamp(coinLocation + (dir * offsetPoints), MIN_CLAMP, MAX_CLAMP);

        // If already used, try opposite direction
        if (usedLocations.Contains(result))
        {
            result = Mathf.Clamp(coinLocation + (-dir * offsetPoints), MIN_CLAMP, MAX_CLAMP);
        }

        // If still duplicate, add small offset until unique
        while (usedLocations.Contains(result))
        {
            result = Mathf.Clamp(result + 5, 30, 210);
        }

        return result;
    }

    public void RandomizeCoins()
    {
        GetRandomCoinLocations();

        // Reset state
        isInitialized = false;
        Gobj1 = Gobj2 = Gobj3 = null;
        ProactiveGobj1 = ProactiveGobj2 = ProactiveGobj3 = null;
        for (int i = 0; i < coinActivated.Length; i++) coinActivated[i] = false;

        StartCoroutine(InitializeCoins());
    }

    IEnumerator InitializeCoins()
    {
        GameObject sinusoidParent = null;
        int waitAttempts = 0;

        while (sinusoidParent == null && waitAttempts < 50)
        {
            sinusoidParent = GameObject.Find("Sinusoid_Fixed_World_Space");
            if (sinusoidParent == null)
            {
                waitAttempts++;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (sinusoidParent == null)
        {
            Debug.LogError("CoinBombScript1: Sine wave parent not found!");
            yield break;
        }

        GameObject testObject = null;
        waitAttempts = 0;
        Transform[] children = null;

        while (testObject == null && waitAttempts < 50)
        {
            children = sinusoidParent.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.name == "go0" || child.name == "go251")
                {
                    testObject = child.gameObject;
                    break;
                }
            }
            if (testObject == null)
            {
                waitAttempts++;
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (testObject == null)
        {
            Debug.LogError("CoinBombScript1: Sine wave points not found!");
            yield break;
        }

        Transform[] allChildren = sinusoidParent.GetComponentsInChildren<Transform>();

        if (mode == 0)
            FindNormalModeLocations(allChildren);
        else if (mode == 1 || mode == 2)
            FindProactiveModeLocations(allChildren);

        if (Gobj1 == null || Gobj2 == null || Gobj3 == null)
        {
            Debug.LogError("CoinBombScript1: Could not find sine wave GameObjects!");
            yield break;
        }

        ydirs = new int[3];
        System.Random random = new System.Random();
        for (int i = 0; i < 3; i++)
            ydirs[i] = random.Next(0, 2) == 0 ? -1 : 1;

        CalculateCoinPositions();
        PlaceCoins();

        isInitialized = true;
        Debug.Log("CoinBombScript1: Initialization complete!");
    }

    void FindNormalModeLocations(Transform[] allChildren)
    {
        foreach (Transform child in allChildren)
        {
            if (child.name == "go" + coin1Location) Gobj1 = child.gameObject;
            if (child.name == "go" + coin2Location) Gobj2 = child.gameObject;
            if (child.name == "go" + coin3Location) Gobj3 = child.gameObject;
        }
    }

    void FindProactiveModeLocations(Transform[] allChildren)
    {
        foreach (Transform child in allChildren)
        {
            if (child.name == "go" + coin1Location) Gobj1 = child.gameObject;
            if (child.name == "go" + coin2Location) Gobj2 = child.gameObject;
            if (child.name == "go" + coin3Location) Gobj3 = child.gameObject;

            if (child.name == "go" + proactiveLocation1) ProactiveGobj1 = child.gameObject;
            if (child.name == "go" + proactiveLocation2) ProactiveGobj2 = child.gameObject;
            if (child.name == "go" + proactiveLocation3) ProactiveGobj3 = child.gameObject;
        }

        if (ProactiveGobj1 != null) proactiveCoin1Position = ProactiveGobj1.transform.position;
        if (ProactiveGobj2 != null) proactiveCoin2Position = ProactiveGobj2.transform.position;
        if (ProactiveGobj3 != null) proactiveCoin3Position = ProactiveGobj3.transform.position;
    }

    void CalculateCoinPositions()
    {
        if (mode == 1 || mode == 2)
        {
            coin1Position = proactiveCoin1Position + new Vector3(0, proactiveOffset * ydirs[0], 0);
            coin2Position = proactiveCoin2Position + new Vector3(0, proactiveOffset * ydirs[1], 0);
            coin3Position = proactiveCoin3Position + new Vector3(0, proactiveOffset * ydirs[2], 0);
        }
        else if (useVerticalOffset)
        {
            coin1Position = Gobj1.transform.position + new Vector3(0, verticalOffset * ydirs[0], 0);
            coin2Position = Gobj2.transform.position + new Vector3(0, verticalOffset * ydirs[1], 0);
            coin3Position = Gobj3.transform.position + new Vector3(0, verticalOffset * ydirs[2], 0);
        }
        else
        {
            coin1Position = Gobj1.transform.position;
            coin2Position = Gobj2.transform.position;
            coin3Position = Gobj3.transform.position;
        }
    }

    void PlaceCoins()
    {
        PlaceCoin(coin1, coin1Position);
        PlaceCoin(coin2, coin2Position);
        PlaceCoin(coin3, coin3Position);

        if (mode == 0)
        {
            coin1?.SetActive(false);
            coin2?.SetActive(false);
            coin3?.SetActive(false);
            return;
        }

        if (mode == 1 || mode == 2)
        {
            MarkSineWavePoint(Gobj1, orange);
            MarkSineWavePoint(Gobj2, orange);
            MarkSineWavePoint(Gobj3, orange);

            if (blue != null)
            {
                MarkSineWavePoint(ProactiveGobj1, blue);
                MarkSineWavePoint(ProactiveGobj2, blue);
                MarkSineWavePoint(ProactiveGobj3, blue);
            }
        }

        if (mode == 2)
        {
            coin1?.SetActive(false);
            coin2?.SetActive(false);
            coin3?.SetActive(false);
            for (int i = 0; i < coinActivated.Length; i++) coinActivated[i] = false;
        }

        coinarray = new GameObject[] { coin1, coin2, coin3 };
    }

    void PlaceCoin(GameObject coin, Vector3 position)
    {
        if (coin == null) return;
        coin.transform.position = position;
        coin.SetActive(true);
        var renderer = coin.GetComponent<MeshRenderer>();
        if (renderer != null) renderer.enabled = true;
    }

    void MarkSineWavePoint(GameObject gobj, Material material)
    {
        if (gobj == null || material == null) return;
        var renderer = gobj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = material;
            gobj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            gobj.tag = "step1";
        }
    }

    // Public getters
    public int GetCoin1Location() => coin1Location;
    public int GetCoin2Location() => coin2Location;
    public int GetCoin3Location() => coin3Location;
    public int GetProactiveLocation1() => proactiveLocation1;
    public int GetProactiveLocation2() => proactiveLocation2;
    public int GetProactiveLocation3() => proactiveLocation3;
    public Vector3 GetCoinPosition(int i) => i == 0 ? coin1Position : i == 1 ? coin2Position : coin3Position;
    public GameObject GetSineWavePoint(int i) => i == 0 ? Gobj1 : i == 1 ? Gobj2 : Gobj3;
    public GameObject GetCoin(int i) => i == 0 ? coin1 : i == 1 ? coin2 : coin3;
    public bool IsInitialized() => isInitialized && Gobj1 != null && Gobj2 != null && Gobj3 != null;
    public int GetMode() => mode;
}
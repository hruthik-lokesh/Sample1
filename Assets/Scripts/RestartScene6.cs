//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System;

//public class RestartScene6 : MonoBehaviour
//{
//    //    private Rigidbody body;
//    //    public GameObject sphere;
//    //    private float collisionTime;
//    //    private Vector3 zeroVelocity;
//    //    private float delay = 2f;
//    //    private float timer;
//    //    private bool hasCollided = false;
//    //    private int tap = 1;
//    //    public GameObject cornertrig;


//    private void Start()
//    {
//        //body = GetComponent<Rigidbody>();
//        //zeroVelocity = Vector3.zero;
//        StaticValsReach7.restart = false;
//    }



//    private void Update()
//    {

//        if (Input.GetKey("r"))
//        {
//            Restart(1);
//            Debug.Log("restart");
//        }
//        else if (Input.GetKey("m"))
//        {
//            Restart(0);
//        }

//    }

//    public void Restart(int next)
//    {
//        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        StaticValsReach7.Set(next);
//    }
//}


//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.InputSystem;

//public class RestartScene6 : MonoBehaviour
//{
//    private void Update()
//    {
//        if (Keyboard.current.rKey.wasPressedThisFrame)
//        {
//            StaticValsReach7.requestedMode = 0; // Normal mode
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//            Debug.Log("Scene restarted (normal mode)");
//        }
//        else if (Keyboard.current.pKey.wasPressedThisFrame)
//        {
//            StaticValsReach7.requestedMode = 1; // Proactive mode
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//            Debug.Log("Scene restarted (proactive mode)");
//        }
//        else if (Keyboard.current.aKey.wasPressedThisFrame)
//        {
//            StaticValsReach7.requestedMode = 2; // Reactive mode
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//            Debug.Log("Scene restarted (reactive mode)");
//        }
//    }
//}


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Simple scene restart controller.
/// Press R to restart scene and advance to next trial.
/// Mode is set in SaveDataTrainingXR inspector - no runtime mode switching.
/// </summary>
public class RestartScene6 : MonoBehaviour
{
    [Header("Current Trial Info (Read-Only)")]
    [SerializeField] private string currentModeDisplay = "";
    [SerializeField] private int currentTrialNumber = 0;

    private void Start()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        currentTrialNumber = StaticValsReach7.curindex;
        currentModeDisplay = $"Trial {currentTrialNumber} - {ExperimentModeConditionHelper.GetDisplayName(StaticValsReach7.requestedExperimentMode)}";
        Debug.Log($"?? {currentModeDisplay}");
    }

    private void Update()
    {
        // R key: Restart scene and advance to next trial
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartAndAdvanceTrial();
        }
    }

    /// <summary>
    /// Restart scene, advance trial index, and create new CSV file
    /// </summary>
    private void RestartAndAdvanceTrial()
    {
        int previousTrial = StaticValsReach7.curindex;
        StaticValsReach7.Set(1); // Advance to next trial with new random coins
        int newTrial = StaticValsReach7.curindex;
        
        Debug.Log($"?? Restarting: Trial {previousTrial} ? Trial {newTrial}");
        Debug.Log($"?? Mode: {ExperimentModeConditionHelper.GetDisplayName(StaticValsReach7.requestedExperimentMode)}");
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
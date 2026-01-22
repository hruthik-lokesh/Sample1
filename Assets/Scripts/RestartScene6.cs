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


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RestartScene6 : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            StaticValsReach7.requestedMode = 0; // Normal mode
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Scene restarted (normal mode)");
        }
        else if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            StaticValsReach7.requestedMode = 1; // Proactive mode
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Scene restarted (proactive mode)");
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            StaticValsReach7.requestedMode = 2; // Reactive mode
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("Scene restarted (reactive mode)");
        }
    }
}
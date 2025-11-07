/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidScript7 : MonoBehaviour
{
    //[HideInInspector]
    public Vector3[] Sinusoid;
    //[HideInInspector]
    public float[] zTarget;
    //[HideInInspector]
    public float[] yTarget;
    //[HideInInspector]
    public Mesh Sphere;
    //[HideInInspector]
    public Material greyLine;
    //[HideInInspector]
    public Material transparent;
    private int trial;
    [HideInInspector]
    public int breakpoint;
    public int[] rval;
    //[TextArea]
    //[Tooltip("1 = up, 2 = down")]
    [HideInInspector]
    public int dir; // 1 = up, 2 = down
                    //[TextArea]
                    //[Tooltip("1 = control, 2 = experimental")]
    [HideInInspector]
    public int cnd; // 1 = control, 2 == experimental
    //[HideInInspector]
    public GameObject runwayend;
    //[HideInInspector]
    public GameObject parms;
    [HideInInspector]
    public int block;
    //public SphereCollider spcollider;
    // Start is called before the first frame update
    void Start()
    {
        Sinusoid = new Vector3[158];
        zTarget = new float[158];
        yTarget = new float[158];
        //cnd = 2;
        //dir = 2;
        Vector3 p = runwayend.transform.position;

        // read in parameters from InputParameters 
        //cnd = parms.GetComponent<InputParameters>().condition;
        //block = parms.GetComponent<InputParameters>().block;

        // sinusoid is made up of 252 small spheres, this method creates them and places them in the correct location
        MakeObjects();
    }



    void MakeObjects()
    {

        int i = 0;
        while (i < 252)
        {
            //calculate position of ith object based on sin function
            Vector3 pos = new Vector3(0, Mathf.Sin(i * 0.05f) * .1f, i * 0.001992032f);


            // create object
            GameObject go1 = new GameObject();
            // name object
            go1.name = "go" + i;
            // add shape to object
            go1.AddComponent<MeshFilter>().mesh = Sphere;
            // add material/color to object
            go1.AddComponent<MeshRenderer>().material = greyLine;
            // add collider (for object interactions) to object
            SphereCollider sc = go1.AddComponent(typeof(SphereCollider)) as SphereCollider;
            // specify layer of object (for future identification)
            go1.layer = 5;
            // arrange xyz coordinates accurately
            Vector3 rotatepos = new Vector3(pos.z, pos.y, pos.x);
            go1.transform.position = rotatepos;
            // set size of object
            go1.transform.localScale = new Vector3(.005f, .005f, .005f);

            i++;

        }
    }


}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinusoidScript7 : MonoBehaviour
{
    // Public variables - set in Inspector
    public Vector3[] Sinusoid;
    public float[] zTarget;
    public float[] yTarget;
    public Mesh Sphere;
    public Material greyLine;
    public Material transparent;
    public GameObject runwayend;
    public GameObject parms;

    // *** NEW: Reference to the Main Camera Transform (XR Origin's Main Camera) ***
    public Transform mainCamera;

    // Hidden or internal variables
    private int trial;
    [HideInInspector]
    public int breakpoint;
    public int[] rval;
    [HideInInspector]
    public int dir; // 1 = up, 2 = down
    [HideInInspector]
    public int cnd; // 1 = control, 2 == experimental
    [HideInInspector]
    public int block;

    // Start is called before the first frame update
    void Start()
    {
        Sinusoid = new Vector3[158];
        zTarget = new float[158];
        yTarget = new float[158];

        // ** Important: Auto-find the main camera if it hasn't been set in the Inspector **
        if (mainCamera == null)
        {
            // Find the active Main Camera in the scene
            mainCamera = Camera.main.transform;
        }

        // Vector3 p = runwayend.transform.position; // unused in MakeObjects()

        // Read in parameters (currently commented out)
        // cnd = parms.GetComponent<InputParameters>().condition;
        // block = parms.GetComponent<InputParameters>().block;

        MakeObjects();
    }


    void MakeObjects()
    {
        // --- Placement Parameters ---
        // How far in front of the user's head should the wave start (in meters)
        const float DISTANCE_IN_FRONT = 0.6f;
        // Vertical offset from the user's eye level (negative is lower)
        const float VERTICAL_OFFSET = 1.1f;
        const float HORIZONTAL_OFFSET = -0.2f;
        // --- Calculate World Spawn Point ---
        // The point in world space where the first sphere (i=0) will be placed
        Vector3 startPosition = mainCamera.position +
                                    mainCamera.forward * DISTANCE_IN_FRONT +
                                    mainCamera.up * VERTICAL_OFFSET +
                                    mainCamera.right * HORIZONTAL_OFFSET;

        // The rotation of the camera determines the orientation of the wave 
        // (so it faces the user)
        Quaternion cameraRotation = mainCamera.rotation;

        // *** OPTIONAL: Create a single parent object for cleanup and organization ***
        GameObject sinusoidParent = new GameObject("Sinusoid_Fixed_World_Space");

        int i = 0;
        while (i < 252)
        {
            // 1. Calculate base sine wave position (Local Space)
            // X=0, Y=Sin(i), Z=Length(i)
            Vector3 pos = new Vector3(0, Mathf.Sin(i * 0.05f) * .1f, i * 0.001992032f);

            // 2. Swapped coordinates (Sinusoid runs along X-axis in local space)
            // (Length, Oscillation, Zero) -> (X, Y, Z)
            Vector3 rotatepos = new Vector3(pos.z, pos.y, pos.x);

            // 3. Transform the local position to the camera's World Space
            // We rotate the local sine wave vector by the camera's rotation 
            // and then offset it by the desired start position.
            Vector3 finalWorldPosition = startPosition + (cameraRotation * rotatepos);

            // create object
            GameObject go1 = new GameObject();
            go1.name = "go" + i;

            // add shape, material, and collider
            go1.AddComponent<MeshFilter>().mesh = Sphere;
            go1.AddComponent<MeshRenderer>().material = greyLine;
            SphereCollider sc = go1.AddComponent(typeof(SphereCollider)) as SphereCollider;
            go1.layer = 5;

            // *** Apply the final, camera-relative World Position ***
            go1.transform.position = finalWorldPosition;

            // set size of object
            go1.transform.localScale = new Vector3(.005f, .005f, .005f);

            // Parent the sphere for organization
            go1.transform.SetParent(sinusoidParent.transform);

            i++;
        }
    }
}

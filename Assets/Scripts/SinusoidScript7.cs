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

    // *** Reference to the Main Camera Transform (XR Origin's Main Camera) ***
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

    void Start()
    {
        Sinusoid = new Vector3[158];
        zTarget = new float[158];
        yTarget = new float[158];

        // Auto-find the main camera if it hasn't been set in the Inspector
        if (mainCamera == null)
        {
            mainCamera = Camera.main.transform;
            if (mainCamera != null)
            {
                Debug.Log("SinusoidScript7: Auto-found Main Camera");
            }
            else
            {
                Debug.LogError("SinusoidScript7: Could not find Main Camera! Please assign it in the inspector.");
                return;
            }
        }

        MakeObjects();
    }

    void MakeObjects()
    {
        // --- Placement Parameters ---
        const float DISTANCE_IN_FRONT = 0.4f; // half of their max arm->30cm
        const float VERTICAL_OFFSET = 1.0f;
        const float HORIZONTAL_OFFSET = -0.25f;

        // Calculate World Spawn Point
        Vector3 startPosition = mainCamera.position +
                                mainCamera.forward * DISTANCE_IN_FRONT +
                                mainCamera.up * VERTICAL_OFFSET +
                                mainCamera.right * HORIZONTAL_OFFSET;

        Quaternion cameraRotation = mainCamera.rotation;

        // Create parent object for organization
        GameObject sinusoidParent = new GameObject("Sinusoid_Fixed_World_Space");

        int i = 0;
        while (i < 252)
        {
            // Calculate base sine wave position (Local Space)
            Vector3 pos = new Vector3(0, Mathf.Sin(i * 0.05f) * .1f, i * 0.001992032f);

            // Swapped coordinates (Sinusoid runs along X-axis in local space)
            Vector3 rotatepos = new Vector3(pos.z, pos.y, pos.x);

            // Transform to World Space
            Vector3 finalWorldPosition = startPosition + (cameraRotation * rotatepos);

            // Create object
            GameObject go1 = new GameObject();
            go1.name = "go" + i;

            // Add shape, material, and collider
            go1.AddComponent<MeshFilter>().mesh = Sphere;
            go1.AddComponent<MeshRenderer>().material = greyLine;

            // Add sphere collider as trigger for collision detection
            SphereCollider sc = go1.AddComponent<SphereCollider>();
            sc.isTrigger = true; // Make it a trigger
            sc.radius = 0.015f; // Slightly larger than visual size

            go1.layer = 5;
            go1.transform.position = finalWorldPosition;
            go1.transform.localScale = new Vector3(.005f, .005f, .005f);
            go1.transform.SetParent(sinusoidParent.transform);

            // Debug logs for important points (no tags needed)
            if (i == 0)
            {
                Debug.Log($"✓ Created go0 (start point) at {finalWorldPosition} with trigger collider");
            }
            else if (i == 251)
            {
                Debug.Log($"✓ Created go251 (end point) at {finalWorldPosition} with trigger collider");
            }

            i++;
        }

        Debug.Log("✓ SinusoidScript7: Created 252 sine wave points with colliders");
    }
}
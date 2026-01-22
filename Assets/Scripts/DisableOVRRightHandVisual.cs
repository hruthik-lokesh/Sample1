using UnityEngine;

public class DisableOVRRightHandVisual : MonoBehaviour
{
    void Start()
    {
        GameObject rightHandVisual = GameObject.Find("OVRRightHandVisual");
        if (rightHandVisual != null)
        {
            rightHandVisual.SetActive(false);
            Debug.Log("OVRRightHandVisual disabled at start.");
        }
        else
        {
            Debug.LogWarning("OVRRightHandVisual not found in scene.");
        }
    }
}
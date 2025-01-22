using UnityEngine;

public class SpherePositionDispatcher : MonoBehaviour
{
    [SerializeField] private GameObject hmd;
    [SerializeField] private OVRInput.Controller rightController;
    public Vector3 globalSpherePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Dispatch the position of the sphere to the other scripts
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(rightController);
        globalSpherePosition = hmd.transform.position + rightControllerPosition + transform.position;
        //Debug.Log("Sphere position: " + globalSpherePosition);
    }
}

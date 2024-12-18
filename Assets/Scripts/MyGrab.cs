using UnityEngine;

public class MyGrab : MonoBehaviour
{
    public OVRInput.Controller controller;
    private float triggerValue;
    private bool isInCollider;
    private bool isSelected;
    private GameObject selectedObj;
    public SelectionTaskMeasure selectionTaskMeasure;
    public LocomotionTechnique locomotionTechnique;


    void Update()
    {
        /*
        if (isInCollider)
        {
            if (!isSelected && triggerValue > 0.95f)
            {
                isSelected = true;
                selectedObj.transform.parent.transform.parent = transform;
            }
            else if (isSelected && triggerValue < 0.95f)
            {
                isSelected = false;
                selectedObj.transform.parent.transform.parent = null;
            }
        }
*/
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("selectionTaskStart"))
        {
            if (!selectionTaskMeasure.isCountdown)
            {
                selectionTaskMeasure.isTaskStart = true;
                Debug.Log("Start task");
                locomotionTechnique.holdLocomotion = true;
                selectionTaskMeasure.TransformBroomToStaff();
                selectionTaskMeasure.StartOneTask();
            }
        }
        else if (other.gameObject.CompareTag("done"))
        {
            selectionTaskMeasure.isTaskStart = false;
            selectionTaskMeasure.EndOneTask();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("objectWand"))
        {
            isInCollider = false;
            selectedObj = null;
        }
    }
}
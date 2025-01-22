using UnityEngine;

public class MyGrabLeft : MonoBehaviour
{
    public OVRInput.Controller controller;
    private float triggerValue;
    private float bumperValue;
    private bool isInCollider;
    private bool isSelected;
    private GameObject selectedObj;
    public SelectionTaskMeasure selectionTaskMeasure;
    public LocomotionTechnique locomotionTechnique;
    public MyGrabRight myGrabRight;

    void Update()
    {
        bumperValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
        triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);

        if (bumperValue >= 0.95f)
        {
            myGrabRight.attractionSpell.SetActive(true);
            
        }
        else 
        {
            myGrabRight.attractionSpell.SetActive(false);

            // Release book in case it's being attracted by the attraction spell
            GameObject book = myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook;
            if (book != null)
            {
                book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook = null;
            }
        }

        if (triggerValue >= 0.95f)
        {
            myGrabRight.holdingSphere.SetActive(true);
        }
        else 
        {
            myGrabRight.holdingSphere.SetActive(false);

            // Release book in case it's being grabbed by the grabbing spell
            GameObject book = myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook;
            if (book != null)
            {
                book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook = null;
            }
        }
        
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
    
    }
}
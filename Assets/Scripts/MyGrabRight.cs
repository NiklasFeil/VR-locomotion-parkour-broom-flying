using System;
using UnityEngine;

public class MyGrabRight : MonoBehaviour
{
    public OVRInput.Controller controller;
    private float triggerValue;
    private bool isInCollider;
    private bool isSelected;
    private GameObject selectedObj;
    public SelectionTaskMeasure selectionTaskMeasure;
    public LocomotionTechnique locomotionTechnique;


    public GameObject attractionSpell;
    public GameObject holdingSphere;

    // Reference to current flying book. Set by SelectionTaskMeasure on task start when book is spawned.
    public GameObject FlyingBook;

    // Reference to current flying book movement script. Set by SelectionTaskMeasure on task start when book is spawned.
    public BookMovement bookMovement;

    // Empty GameObject indicating the target position of the book, usually the center of the holding sphere
    public GameObject targetPoint;

    


    void Start()
    {
        attractionSpell.SetActive(false);
        holdingSphere.SetActive(false);
    }

    void Update()
    {
        
        
        if (bookMovement.movementMode == BookMovement.MovementMode.Attracted)
        {
            //Debug.Log("Updated book target point to TargetPoint GameObject in MyGrabRight according to Attracted mode");
            bookMovement.targetPosition = targetPoint.transform.position;
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Grabbed)
        {
            //Debug.Log("Updated book target point to GrabbingSphere Spell in MyGrabRight according to Grabbed mode");
            bookMovement.targetPosition = holdingSphere.transform.position;
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Rotation)
        {
            // todo!: Implement rotation of book around its own axis
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("selectionTaskStart"))
        {
            if (!selectionTaskMeasure.isCountdown)
            {
                selectionTaskMeasure.isTaskStart = true;
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
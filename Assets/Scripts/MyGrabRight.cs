using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
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

    static private int maxStoredControllerPositions = 120;
    
    // Queue of Vector3 to store the last n controller positions. Using a queue to remove the oldest position when adding a new one and it reached a certain limit.
    private Queue<Vector3> controllerPositions = new Queue<Vector3>(maxStoredControllerPositions);
    private Vector3 meanControllerPosition;
    private Vector3 currentControllerMeanDifference;

    void Start()
    {
        attractionSpell.SetActive(false);
        holdingSphere.SetActive(false);
    }

    void FixedUpdate()
    {     
        if (bookMovement.movementMode == BookMovement.MovementMode.Attracted)
        {
            //Debug.Log("Updated book target point to TargetPoint GameObject in MyGrabRight according to Attracted mode");
            bookMovement.targetPosition = targetPoint.transform.position;
            meanControllerPosition = OVRInput.GetLocalControllerPosition(controller);
            ResetControllerPositionQueue();
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Grabbed)
        {
            //Debug.Log("Updated book target point to GrabbingSphere Spell in MyGrabRight according to Grabbed mode");
            bookMovement.targetPosition = holdingSphere.transform.position;
            meanControllerPosition = OVRInput.GetLocalControllerPosition(controller);
            ResetControllerPositionQueue();
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Rotation)
        {
            // todo!: Implement rotation of book around its own axis
            // Use mean position of controller to rotate book around its own axis.
            //meanPositionDebugSphere.transform.position = meanControllerPosition;
            UpdateControllerPositionQueue();
            UpdateMeanControllerPosition();
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Idle)
        {
            meanControllerPosition = OVRInput.GetLocalControllerPosition(controller);
            ResetControllerPositionQueue();
        }

        //meanPositionDebugSphere.transform.localPosition = meanControllerPosition;
        currentControllerMeanDifference = -meanControllerPosition + OVRInput.GetLocalControllerPosition(controller);
        Debug.Log("ControllerMeanDifference: " + currentControllerMeanDifference);

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

    void UpdateControllerPositionQueue()
    {
        controllerPositions.Enqueue(OVRInput.GetLocalControllerPosition(controller));

        if (controllerPositions.Count > 120)
        {
            controllerPositions.Dequeue();
        }
    }

    void UpdateMeanControllerPosition()
    {
        // Recalculate mean position of controller every update by converting Queue to Array. This is very inefficient and should be optimized by using the current meanControllePosition and the new position to update it without the O(n) conversion.
        // This optimization is for later though.
        Array controllerPositionsArray = controllerPositions.ToArray();
        meanControllerPosition = controllerPositionsArray.OfType<Vector3>().Aggregate((acc, cur) => acc + cur) / controllerPositionsArray.Length;
    }

    void ResetControllerPositionQueue()
    {
        controllerPositions.Clear();
    }
}
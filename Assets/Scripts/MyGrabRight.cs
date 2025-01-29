using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using Oculus.VoiceSDK.UX;
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

    private bool aButtonValue;

    public bool bookInsideGoal;
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
    private Vector3 lastControllerMeanDifference;
    private Vector3 currentControllerMeanDifference;

    [SerializeField] private float rotationSpeed = 0.5f;
    [SerializeField] private float maxRotationSpeed = 5.0f;

    private Quaternion targetAngleVelocity = Quaternion.identity;
    private Quaternion currentAngleVelocity = Quaternion.identity;

    void Start()
    {
        attractionSpell.SetActive(false);
        holdingSphere.SetActive(false);
        bookInsideGoal = false;
    }

    void FixedUpdate()
    {     
        if (bookMovement == null)
        {
            return;
        }

        if (bookMovement.movementMode == BookMovement.MovementMode.Attracted)
        {
            //Debug.Log("Updated book target point to TargetPoint GameObject in MyGrabRight according to Attracted mode");
            bookMovement.targetPosition = targetPoint.transform.position;
            currentAngleVelocity = Quaternion.identity;
            meanControllerPosition = OVRInput.GetLocalControllerPosition(controller);
            ResetControllerPositionQueue();
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Grabbed)
        {
            //Debug.Log("Updated book target point to GrabbingSphere Spell in MyGrabRight according to Grabbed mode");
            bookMovement.targetPosition = holdingSphere.transform.position;
            currentAngleVelocity = Quaternion.identity;
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
            RotateBook();
        }
        else if (bookMovement.movementMode == BookMovement.MovementMode.Idle)
        {
            currentAngleVelocity = Quaternion.identity;
            meanControllerPosition = OVRInput.GetLocalControllerPosition(controller);
            ResetControllerPositionQueue();
        }

        //meanPositionDebugSphere.transform.localPosition = meanControllerPosition;
        lastControllerMeanDifference = currentControllerMeanDifference;
        currentControllerMeanDifference = -meanControllerPosition + OVRInput.GetLocalControllerPosition(controller);
        //Debug.Log("ControllerMeanDifference: " + currentControllerMeanDifference);

    }

    void Update()
    {
        aButtonValue = OVRInput.Get(OVRInput.Button.One, controller);

        if (aButtonValue && bookInsideGoal)
        {
            selectionTaskMeasure.isTaskStart = false;
            selectionTaskMeasure.EndOneTask();
            bookInsideGoal = false;
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
        else if (other.gameObject.CompareTag("submissionSphere"))
        {
            bookInsideGoal = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("submissionSphere"))
        {
            bookInsideGoal = false;
        }
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

    void RotateBook()
    {
        // Calculate rotation axis by looking at the unit circle around the mean controller position.
        // Use cross product of last update's position on unit circle and current position on unit circle.
        // Unity uses a left-handed coordinate system, so use previous position first in cross product.

        Vector3 previousPositionOnUnitCircle = lastControllerMeanDifference.normalized;
        Vector3 currentPositionOnUnitCircle = currentControllerMeanDifference.normalized;
        Vector3 rotationAxis = Vector3.Cross(previousPositionOnUnitCircle, currentPositionOnUnitCircle);

        // Calculate the angle of rotation
        float angle = Vector3.Angle(previousPositionOnUnitCircle, currentPositionOnUnitCircle);

        // Calculate quaternion from axis and angle
        targetAngleVelocity = Quaternion.AngleAxis(angle, rotationAxis).normalized;

        // Interrupt when player is not moving the controller for a longer period as the book would jitter and rotate uncontrollably.
        if (lastControllerMeanDifference.magnitude < 0.03 || currentControllerMeanDifference.magnitude < 0.03)
        {
            targetAngleVelocity = Quaternion.identity;
        }

        // Smoothly rotate the book by using a spring mechanism like with the book movement.
        currentAngleVelocity = Quaternion.Slerp(currentAngleVelocity, targetAngleVelocity, Time.deltaTime * rotationSpeed);

        float currentAngle = Quaternion.Angle(Quaternion.identity, currentAngleVelocity);
        if (currentAngle > maxRotationSpeed)
        {
            currentAngleVelocity = Quaternion.Slerp(Quaternion.identity, currentAngleVelocity, maxRotationSpeed / currentAngle);
        }

        // Rotate the book around the calculated axis by the calculated angle.
        //FlyingBook.transform.Rotate(rotationAxis, angle, Space.World);
        //FlyingBook.transform.rotation *= currentAngleVelocity;
        FlyingBook.transform.Rotate(currentAngleVelocity.eulerAngles, Space.World);
    }
}
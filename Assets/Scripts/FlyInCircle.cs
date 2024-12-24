using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.UI;
using UnityEngine;

public class FlyInCircle : MonoBehaviour
{

    public enum MovementMode
    {
        Idle,
        Attracted
    }

    [SerializeField] private Vector3 centerPosition;
    private double x = 0;
    private double y = 0;
    [SerializeField] private float flightSpeed;
    [SerializeField] private float attractedFlightSpeed;
    public float circleRadius;
    private Vector3 rotationAxis;
    private float rotationAxisOffsetMultiplier;

    public OVRInput.Controller rightController;

    public MovementMode movementMode;
    private Vector3 positionToMoveTo;
    [SerializeField] private float maxDistanceDelta;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        centerPosition = transform.position;
        rotationAxis = new Vector3(15, 30, 45);
    }

    // Update is called once per frame
    void Update()
    {
        // Update position
        if (movementMode == MovementMode.Idle)
        {
            // Flying around in circle
            x += flightSpeed;
            y += flightSpeed;
            float current_offset_x = (float) Math.Cos(x);
            float current_offset_z = (float) Math.Sin(y);
            Vector3 current_offset = new Vector3(current_offset_x, 0, current_offset_z);
            positionToMoveTo = centerPosition + current_offset * circleRadius;
        }
        else if (movementMode == MovementMode.Attracted)
        {
            // Be attracted by spell
            Vector3 globalControllerPosition = OVRInput.GetLocalControllerPosition(rightController) + transform.position;
            
            positionToMoveTo = (globalControllerPosition - transform.position).normalized * attractedFlightSpeed;
            
        }

        transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, maxDistanceDelta);

        // Update orientation
        Vector3 newOffset = rotationAxisOffsetMultiplier * UnityEngine.Random.onUnitSphere;
        rotationAxis = (rotationAxis + newOffset).normalized;
        transform.Rotate(rotationAxis, Space.Self);    
    }
}

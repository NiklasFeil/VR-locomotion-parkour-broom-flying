using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Oculus.Interaction.Input;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.UI;
using UnityEngine;

public class BookMovement : MonoBehaviour
{

    public enum MovementMode
    {
        Idle,
        Attracted,
        Grabbed,
        Rotation
    }

    //[SerializeField] private OVRInput.Controller rightController;
    //[SerializeField] private GameObject hmd;
    [SerializeField] private Vector3 centerPosition;
    private double x = 0;
    private double y = 0;
    [SerializeField] private float circleMovementSpeed;
    public float circleRadius;
    private Vector3 rotationAxis;
    private float rotationAxisOffsetMultiplier;

    public MovementMode movementMode;
    
    private Vector3 movementThisUpdate;
    [SerializeField] private float maxDistanceDelta;
    [SerializeField] private ParticleSystem star_particles;
    
    // Variables for Spring Follow Behaviour
    public Vector3 targetPosition; // Target position to follow
    [SerializeField] private float stiffness = 10f; // Spring stiffness
    [SerializeField] private float damping = 5f; // Spring damping coefficient
    [SerializeField] private Vector3 grabbedVelocity = Vector3.zero; // Velocity of the book when grabbed
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        centerPosition = transform.position;
        rotationAxis = new Vector3(15, 30, 45);
    }

    // Update is called once per frame
    void LateUpdate()
    {   
        // Update book transform according to target position set by myGrabRight
        if (movementMode == MovementMode.Idle)
        {
            // Sets next target position to be on the idle circle
            SetTargetPositionOnCircle();
            UpdatePositionUniformMovement();
            UpdateOrientation();
        }
        else  if (movementMode == MovementMode.Attracted)
        {
            // Be attracted by spell
            // Target Position was updated by myGrabRight
            UpdatePositionUniformMovement();
        }
        else if (movementMode == MovementMode.Grabbed)
        {
            // Be grabbed within sphere and drawn to sphere center using spring mechanism
            UpdatePositionSpringMovement();
        }
        else if (movementMode == MovementMode.Rotation)
        {
            // Be grabbed within sphere and drawn to sphere center using spring mechanism
            UpdatePositionSpringMovement();
        }


        // Update Particle Rotation
        // UpdateParticleRotation();
    }

    void UpdateOrientation()
    {   
        Vector3 newOffset = rotationAxisOffsetMultiplier * UnityEngine.Random.onUnitSphere;
        rotationAxis = (rotationAxis + newOffset).normalized;
        transform.Rotate(rotationAxis, Space.Self);  
    }

    void UpdateParticleRotation()
    {
        // Update Particle Rotation
        Vector3 oppositeDirection = -movementThisUpdate;
        var shape = star_particles.shape;
        shape.rotation = Quaternion.LookRotation(oppositeDirection).eulerAngles;

    }

    void SetTargetPositionOnCircle()
    {
        // Flying around in circle
        x += circleMovementSpeed;
        y += circleMovementSpeed;
        float current_offset_x = (float) Math.Cos(x);
        float current_offset_z = (float) Math.Sin(y);
        Vector3 current_offset = new Vector3(current_offset_x, 0, current_offset_z);
        targetPosition = centerPosition + current_offset * circleRadius;
    }

    void UpdatePositionUniformMovement()
    {
        Vector3 newPositionAfterUpdate = Vector3.MoveTowards(transform.position, targetPosition, maxDistanceDelta);
        movementThisUpdate = newPositionAfterUpdate - transform.position;
        transform.position = newPositionAfterUpdate;
    }

    void UpdatePositionSpringMovement()
    {

        Vector3 displacement = targetPosition - transform.position;
        Vector3 springForce = stiffness * displacement;
        Vector3 dampingForce = damping * grabbedVelocity;
        Vector3 force = springForce - dampingForce;
        Vector3 acceleration = force;
        grabbedVelocity += acceleration * Time.deltaTime;
        transform.position += grabbedVelocity * Time.deltaTime;
    }
}
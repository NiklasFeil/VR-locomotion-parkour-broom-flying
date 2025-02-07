using System;
using UnityEngine;

public class BookMovement : MonoBehaviour
{

    public enum MovementMode
    {
        Idle,
        Attracted,
        Grabbed,
        Rotation,
        Finished
    }

    //[SerializeField] private OVRInput.Controller rightController;
    //[SerializeField] private GameObject hmd;
    [SerializeField] private Vector3 centerPosition;
    [SerializeField] private float circleMovementSpeed;
    [SerializeField] private float circleRadius;
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
    private Vector3 grabbedVelocity = Vector3.zero; // Velocity of the book when grabbed

    // Used so targetPosition is only updated in idle mode when book is on circle. Otherwise, the book would wobble around when flying back up.
    private bool bookIsOnCircle;
    private bool targetPositionIsResetToIdleCircle;

    private Vector3 currentOffset;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        centerPosition = transform.position;
        rotationAxis = new Vector3(15, 30, 45);
        bookIsOnCircle = true;
        targetPositionIsResetToIdleCircle = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {   
        // Update book transform according to target position set by myGrabRight
        if (movementMode == MovementMode.Idle)
        {
            if (bookIsOnCircle)
            {
                // Sets next target position to be on the idle circle
                SetTargetPositionOnCircle();
            }
            else if (!bookIsOnCircle && !targetPositionIsResetToIdleCircle)
            {
                // Book just re-idled from other movementMode. Target position needs to be set on the circle, but not be updated along the circle until book reaches the circle again.
                targetPosition = centerPosition + currentOffset * circleRadius;
                targetPositionIsResetToIdleCircle = true;
            }
            else if ((targetPosition - transform.position).magnitude < 0.05)
            {
                // Book is near circle in idle so it can resume updating the circle position
                bookIsOnCircle = true;
            }
            
            UpdatePositionSpringMovement();
            UpdateOrientation();
        }
        else  if (movementMode == MovementMode.Attracted)
        {
            // Be attracted by spell
            // Target Position was updated by myGrabRight
            bookIsOnCircle = false;
            targetPositionIsResetToIdleCircle = false;
            UpdatePositionUniformMovement();
        }
        else if (movementMode == MovementMode.Grabbed)
        {
            bookIsOnCircle = false;
            targetPositionIsResetToIdleCircle = false;
            // Be grabbed within sphere and drawn to sphere center using spring mechanism
            UpdatePositionSpringMovement();
        }
        else if (movementMode == MovementMode.Rotation)
        {
            bookIsOnCircle = false;
            targetPositionIsResetToIdleCircle = false;
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
        float current_time = Time.time;
        float currentOffsetX = (float) Math.Cos(current_time * circleMovementSpeed);
        float currentOffsetZ = (float) Math.Sin(current_time * circleMovementSpeed);
        currentOffset = new Vector3(currentOffsetX, 0, currentOffsetZ);
        targetPosition = centerPosition + currentOffset * circleRadius;
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
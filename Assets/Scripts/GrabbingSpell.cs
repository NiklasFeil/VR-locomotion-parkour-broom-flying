using UnityEngine;

public class GrabbingSpell : MonoBehaviour
{

    public GameObject grabbedBook;

    [SerializeField] private Material grabbingMaterial;
    [SerializeField] private Material rotationMaterial;

    
    private GameObject woodenStaff;
    [SerializeField] private GameObject basePositionObject;

    // Variables for Spring Follow Behaviour
    [SerializeField] private float stiffness = 10f; // Spring stiffness
    [SerializeField] private float damping = 5f; // Spring damping coefficient
    private Vector3 relocationVelocity = Vector3.zero; // Velocity of the sphere when moving back to base position

    private bool springBack = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabbedBook = null;
        //positionCurrentFrame = transform.position;
        woodenStaff = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        positionPreviousFrame = positionCurrentFrame;
        positionCurrentFrame = transform.position;

        if(grabbedBook.GetComponent<BookMovement>().movementMode == BookMovement.MovementMode.Rotation)
        {
            // Sphere needs to stop moving with the staff. This could be done by staying with the book. However, the book might still be off-center, so the following line does not produce a good result.
            // holdingSphere.transform.position = bookMovement.transform.position;

            // Instead, add the reverse movement of the right controller in the current frame to the sphere to keep it in place.
            Debug.Log("Holding sphere is standing still now due to Rotation Spell being active");
            transform.position += positionPreviousFrame - positionCurrentFrame;
        }
        */

        if (springBack)
        {
            // Move the sphere to its base position using spring mechanism instead of instant blinking
            Vector3 displacement = basePositionObject.transform.position - transform.position;
            Vector3 springForce = stiffness * displacement;
            Vector3 dampingForce = damping * relocationVelocity;
            Vector3 force = springForce - dampingForce;
            Vector3 acceleration = force;
            relocationVelocity += acceleration * Time.deltaTime;
            transform.position += relocationVelocity * Time.deltaTime;
            
            if (displacement.magnitude < 0.01f)
            {
                springBack = false;
            }
        }
    }


    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Sphere collided with " + other.name);
        if (other.gameObject.CompareTag("flyingBook"))
        {
            //Debug.Log("Book collided with sphere");
            // Set book to be grabbed by sphere
            other.gameObject.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Grabbed;
            grabbedBook = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("Sphere exited collision with " + other.name);
        if (other.gameObject.CompareTag("flyingBook"))
        {
            other.gameObject.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
            grabbedBook = null;
        }
    }

    public void ChangeMaterialToGrabbing()
    {
        GetComponent<Renderer>().material = grabbingMaterial;
    }

    public void ChangeMaterialToRotation()
    {
        GetComponent<Renderer>().material = rotationMaterial;
    }

    public void Unmount()
    {
        transform.parent = null;
    }

    public void Remount()
    {
        // transform.position = basePositionObject.transform.position;

        springBack = true;

        transform.parent = woodenStaff.transform;
    }
}

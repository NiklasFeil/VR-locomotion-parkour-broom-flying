using UnityEngine;

public class GrabbingSpell : MonoBehaviour
{

    public GameObject grabbedBook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabbedBook = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sphere collided with " + other.name);
        if (other.gameObject.CompareTag("flyingBook"))
        {
            Debug.Log("Book collided with sphere");
            // Set book to be grabbed by sphere
            other.gameObject.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Grabbed;
            grabbedBook = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log("Sphere exited collision with " + other.name);
        if (other.gameObject.CompareTag("flyingBook"))
        {
            other.gameObject.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
            grabbedBook = null;
        }
    }
}

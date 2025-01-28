using UnityEngine;

// A script that adapts the book's movement mode based on the attraction spell
public class AttractionSpell : MonoBehaviour
{
    public GameObject attractedBook;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attractedBook = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("flyingBook"))
        {
            other.gameObject.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Attracted;
            attractedBook = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("flyingBook"))
        {
            other.gameObject.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
            attractedBook = null;
        }
    }
}

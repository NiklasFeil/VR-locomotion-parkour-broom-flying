using UnityEngine;

public class AttractionSpell : MonoBehaviour
{

    public bool activated = false;
    public FlyInCircle bookMovementScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (activated && other.gameObject.CompareTag("flyingBook"))
        {
            bookMovementScript.movementMode = FlyInCircle.MovementMode.Attracted;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (activated && other.gameObject.CompareTag("flyingBook"))
        {
            bookMovementScript.movementMode = FlyInCircle.MovementMode.Idle;
        }
    }
}

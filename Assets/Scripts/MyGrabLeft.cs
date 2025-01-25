using UnityEditor.Rendering;
using UnityEngine;

public class MyGrabLeft : MonoBehaviour
{
    public enum Spell
    {   
        None,
        Attraction,
        Grabbing,
        Rotation
    }

    public OVRInput.Controller controller;
    private float triggerValue;
    private float bumperValue;
    private bool xButtonValue;
    private bool isInCollider;
    private bool isSelected;
    private GameObject selectedObj;
    public SelectionTaskMeasure selectionTaskMeasure;
    public LocomotionTechnique locomotionTechnique;
    public MyGrabRight myGrabRight;
    public Spell activeSpell;

    void Start()
    {
        activeSpell = Spell.None;
    }

    void Update()
    {
        // Bumper triggers Attraction Spell
        bumperValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller);
        // Trigger triggers Grabbing Spell
        triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller);
        // X button triggers Rotation Spell when Grabbing Spell is active
        xButtonValue = OVRInput.Get(OVRInput.Button.One, controller);

        switch (activeSpell)
        {
            case Spell.None:
                // Cast Attraction Spell without active spell
                if (bumperValue >= 0.95f)
                {
                    myGrabRight.attractionSpell.SetActive(true);
                    activeSpell = Spell.Attraction;
                }
                // Cast Grabbing Spell without active spell
                else if (triggerValue >= 0.95f)
                {
                    myGrabRight.holdingSphere.SetActive(true);
                    activeSpell = Spell.Grabbing;
                }
                break;

            case Spell.Attraction:
                // Grabbing Spell is cast while Attraction Spell is active
                if (triggerValue >= 0.95f)
                {
                    myGrabRight.holdingSphere.SetActive(true);
                    myGrabRight.attractionSpell.SetActive(false);
                    activeSpell = Spell.Grabbing;

                    // Release book in case it's being attracted by the attraction spell
                    GameObject book = myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook;
                    if (book != null)
                    {
                        book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                        myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook = null;
                    }
                }
                // Attraction Spell is released without other spells being casted
                else if (bumperValue < 0.95f)
                {
                    myGrabRight.attractionSpell.SetActive(false);
                    activeSpell = Spell.None;

                    // Release book in case it's being attracted by the attraction spell
                    GameObject book = myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook;
                    if (book != null)
                    {
                        book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                        myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook = null;
                    }
                }
                break;
            
            case Spell.Grabbing:
                // Trigger Rotation Spell while Grabbing Spell is active
                if (xButtonValue)
                {   
                    activeSpell = Spell.Rotation;
                    //myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().rotationMode = true;
                    // Change movementMode of book to Rotation
                    myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Rotation; 
                    // Change material of sphere to indicate rotation mode
                    myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().ChangeMaterialToRotation();
                    // Unparent sphere from staff and right controller
                    myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().Unmount();

                }
                // Grabbing Spell is released without other spells being casted
                else if (triggerValue < 0.95f)
                {
                    myGrabRight.holdingSphere.SetActive(false);
                    activeSpell = Spell.None;

                    // Release book in case it's being attracted by the attraction spell
                    GameObject book = myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook;
                    if (book != null)
                    {
                        book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook = null;
                    }
                }
                break;
            
            case Spell.Rotation:
                // Release Rotation Spell for Grabbing Spell
                if (!xButtonValue)
                {
                    activeSpell = Spell.Grabbing;
                    //myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().rotationMode = false;
                    //todo: Change movementMode of book to Grabbed
                    myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Grabbed; 
                    //todo: Change material of sphere to indicate grabbed mode
                    myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().ChangeMaterialToGrabbing();
                    // Reparent sphere to staff and right controller
                    myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().Remount();
                }
                // Release Rotation Spell due to Grabbing Spell being released
                else if (triggerValue < 0.95f)
                {
                    myGrabRight.holdingSphere.SetActive(false);
                    activeSpell = Spell.None;
                }
                break;
        }       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("selectionTaskStart"))
        {
            if (!selectionTaskMeasure.isCountdown)
            {
                selectionTaskMeasure.isTaskStart = true;
                Debug.Log("Start task");
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
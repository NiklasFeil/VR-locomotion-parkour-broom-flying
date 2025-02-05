using System.Collections;
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
    
    public MyGrabRight myGrabRight;
    public Spell activeSpell;

    private GameObject book;

    [SerializeField] private PlayAudiosSkeleton playAudiosSkeleton;

    void Start()
    {
        activeSpell = Spell.None;
    }

    IEnumerator EndTask()
    {
        playAudiosSkeleton.PlayLeavingAudio();
        yield return new WaitForSeconds(3.0f);
        selectionTaskMeasure.EndOneTask();
    }

    public void HandleVoiceCommand(VoiceCommand command)
    {
        if (!selectionTaskMeasure.isTaskStart)
        {
            return;
        }

        if (command == VoiceCommand.Leave)
        {
            BookMovement bookMovement = book.GetComponent<BookMovement>();
            if (bookMovement.movementMode == BookMovement.MovementMode.Finished)
            {
                StartCoroutine(EndTask());
            }
            else {
                playAudiosSkeleton.PlayGimmeAudio();
            }

            return;
        }

        switch(activeSpell)
        {
            case Spell.None:
                switch (command)
                {
                    case VoiceCommand.Attract:
                        myGrabRight.attractionSpell.SetActive(true);
                        activeSpell = Spell.Attraction;
                        break;
                    case VoiceCommand.Grab:
                        myGrabRight.holdingSphere.SetActive(true);
                        activeSpell = Spell.Grabbing;
                        break;
                }
                break;

            case Spell.Attraction:
                switch(command)
                {
                    case VoiceCommand.Grab:
                        myGrabRight.holdingSphere.SetActive(true);
                        myGrabRight.attractionSpell.SetActive(false);
                        activeSpell = Spell.Grabbing;

                        // Release book in case it's being attracted by the attraction spell
                        book = myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook;
                        if (book != null)
                        {
                            book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                            myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook = null;
                        }
                        break;
                    case VoiceCommand.Release:
                        myGrabRight.attractionSpell.SetActive(false);
                        activeSpell = Spell.None;

                        // Release book in case it's being attracted by the attraction spell
                        book = myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook;
                        if (book != null)
                        {
                            book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                            myGrabRight.attractionSpell.GetComponent<AttractionSpell>().attractedBook = null;
                        }
                        break;
                }
                break;
                
            case Spell.Grabbing:
                switch(command)
                {
                    case VoiceCommand.Rotate:
                        activeSpell = Spell.Rotation;
                        //myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().rotationMode = true;
                        // Change movementMode of book to Rotation
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Rotation; 
                        // Change material of sphere to indicate rotation mode
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().ChangeMaterialToRotation();
                        // Unparent sphere from staff and right controller
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().Unmount();
                        break;
                    case VoiceCommand.Release:
                        myGrabRight.holdingSphere.SetActive(false);
                        activeSpell = Spell.None;

                        // Release book in case it's being attracted by the attraction spell
                        book = myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook;
                        if (book != null)
                        {
                            if (myGrabRight.bookInsideGoal)
                            {
                                book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Finished;
                            }
                            else
                            {
                                book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                            }
                            myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook = null;
                        }
                        break;

                    case VoiceCommand.Attract:
                        myGrabRight.holdingSphere.SetActive(false);
                        myGrabRight.attractionSpell.SetActive(true);
                        activeSpell = Spell.Attraction;

                        // Release book in case it's being grabbed by the grabbing spell
                        book = myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook;
                        if (book != null)
                        {
                            book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                            myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook = null;
                        }
                        break;

                }
                break;

            case Spell.Rotation:
                switch(command)
                {
                    case VoiceCommand.Move:
                        activeSpell = Spell.Grabbing;
                        //myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().rotationMode = false;
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Grabbed; 
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().ChangeMaterialToGrabbing();
                        // Reparent sphere to staff and right controller
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().Remount();
                        break;
                    
                    case VoiceCommand.Attract:
                        myGrabRight.holdingSphere.SetActive(false);
                        myGrabRight.attractionSpell.SetActive(true);
                        activeSpell = Spell.Attraction;

                        // Release book in case it's being grabbed by the grabbing spell
                        book = myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook;
                        if (book != null)
                        {
                            book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                            myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook = null;
                        }
                        break;

                    case VoiceCommand.Release:
                        myGrabRight.holdingSphere.SetActive(false);
                        activeSpell = Spell.None;

                        if (myGrabRight.bookInsideGoal)
                        {
                            myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Finished;
                        }
                        else
                        {
                            myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                        }
                        break;
                }
                break;
        }
    }


    void Update()
    {
        /*
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
                        if (myGrabRight.bookInsideGoal)
                        {
                            book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Finished;
                        }
                        else
                        {
                            book.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                        }
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

                    if (myGrabRight.bookInsideGoal)
                    {
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Finished;
                    }
                    else
                    {
                        myGrabRight.holdingSphere.GetComponent<GrabbingSpell>().grabbedBook.GetComponent<BookMovement>().movementMode = BookMovement.MovementMode.Idle;
                    }
                }
                break;
        }       
        */
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("selectionTaskStart"))
        {
            if (!selectionTaskMeasure.isCountdown)
            {
                selectionTaskMeasure.isTaskStart = true;
                Debug.Log("Start task");
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
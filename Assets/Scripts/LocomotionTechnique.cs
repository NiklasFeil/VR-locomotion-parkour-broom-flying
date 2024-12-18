using UnityEngine;

public class LocomotionTechnique : MonoBehaviour
{
    // Please implement your locomotion technique in this script. 
    public OVRInput.Controller leftController;
    public OVRInput.Controller rightController;
    [Range(0, 10)] public float translationGain = 0.5f;
    public GameObject hmd;
    public LineRenderer broomLine;
    public LineRenderer bodyLine;
    public GameObject seatingPositionObject;
    public float speedMultiplier;
    private Vector3 seatingPosition;
    private Vector3 broomControllerPosition;
    private Vector3 headPosition;
    private Vector3 seatToHead;
    private Vector3 seatToController;
    private Vector3 movementDirection;
    private float speedValue;
    public bool holdLocomotion;
    [SerializeField] private AnimationCurve speedAnimationCurve;
    


    //[SerializeField] private float leftTriggerValue;    
    //[SerializeField] private float rightTriggerValue;
    //[SerializeField] private Vector3 startPos;
    //[SerializeField] private Vector3 offset;
    //[SerializeField] private bool isIndexTriggerDown;


    /////////////////////////////////////////////////////////
    // These are for the game mechanism.
    public ParkourCounter parkourCounter;
    public string stage;
    public SelectionTaskMeasure selectionTaskMeasure;
    
    void Start()
    {
        // Configure line renderers
        broomLine.positionCount = 2;
        bodyLine.positionCount = 2;
        holdLocomotion = false;
    }

    void Update()
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        // Please implement your LOCOMOTION TECHNIQUE in this script :D.
        
        // Setting positions
        seatingPosition = seatingPositionObject.transform.position;
        broomControllerPosition = OVRInput.GetLocalControllerPosition(rightController) + transform.position; // May need some further adjustments
        headPosition = hmd.transform.position;

        // Setting up line renderers
        broomLine.SetPosition(0, seatingPosition);
        broomLine.SetPosition(1, broomControllerPosition);   
        bodyLine.SetPosition(0, seatingPosition);
        bodyLine.SetPosition(1, headPosition);

        // Setting up essential vectors from seat to head and tip.
        // Normalize so upper body's size does not matter for acceleration
        // and speed stays independent of controller placement on broom 
        seatToController = (broomControllerPosition - seatingPosition).normalized;
        seatToHead = (headPosition - seatingPosition).normalized;

        // speedValue is set to the orthogonal projection of seatToHead on seatToController
        movementDirection = seatToController;
        float dot_product = Vector3.Dot(seatToHead, seatToController);

        speedValue = speedAnimationCurve.Evaluate(dot_product) * speedMultiplier;

        if (!holdLocomotion)
            transform.position = transform.position + movementDirection * speedValue;

        ////////////////////////////////////////////////////////////////////////////////
        // These are for the game mechanism.
        if (OVRInput.Get(OVRInput.Button.Two) || OVRInput.Get(OVRInput.Button.Four))
        {
            if (parkourCounter.parkourStart)
            {
                transform.position = parkourCounter.currentRespawnPos;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        // These are for the game mechanism.
        if (other.CompareTag("banner"))
        {
            stage = other.gameObject.name;
            parkourCounter.isStageChange = true;
        }
        else if (other.CompareTag("objectInteractionTask"))
        {
            selectionTaskMeasure.isTaskStart = true;
            selectionTaskMeasure.scoreText.text = "";
            selectionTaskMeasure.partSumErr = 0f;
            selectionTaskMeasure.partSumTime = 0f;
            // rotation: facing the user's entering direction
            float tempValueY = other.transform.position.y > 0 ? 12 : 0;
            Vector3 tmpTarget = new(hmd.transform.position.x, tempValueY, hmd.transform.position.z);
            selectionTaskMeasure.taskUI.transform.LookAt(tmpTarget);
            selectionTaskMeasure.taskUI.transform.Rotate(new Vector3(0, 180f, 0));
            selectionTaskMeasure.taskStartPanel.SetActive(true);
        }
        else if (other.CompareTag("coin"))
        {
            parkourCounter.coinCount += 1;
            GetComponent<AudioSource>().Play();
            other.gameObject.SetActive(false);
        }
        // These are for the game mechanism.
    }
}

/*
        leftTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, leftController); 
        rightTriggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, rightController); 

        if (leftTriggerValue > 0.95f && rightTriggerValue > 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = (OVRInput.GetLocalControllerPosition(leftController) + OVRInput.GetLocalControllerPosition(rightController)) / 2;
            }
            offset = hmd.transform.forward.normalized *
                    (OVRInput.GetLocalControllerPosition(leftController) - startPos +
                    (OVRInput.GetLocalControllerPosition(rightController) - startPos)).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);
        }
        else if (leftTriggerValue > 0.95f && rightTriggerValue < 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = OVRInput.GetLocalControllerPosition(leftController);
            }
            offset = hmd.transform.forward.normalized *
                     (OVRInput.GetLocalControllerPosition(leftController) - startPos).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);
        }
        else if (leftTriggerValue < 0.95f && rightTriggerValue > 0.95f)
        {
            if (!isIndexTriggerDown)
            {
                isIndexTriggerDown = true;
                startPos = OVRInput.GetLocalControllerPosition(rightController);
            }
           offset = hmd.transform.forward.normalized *
                    (OVRInput.GetLocalControllerPosition(rightController) - startPos).magnitude;
            Debug.DrawRay(startPos, offset, Color.red, 0.2f);
        }
        else
        {
            if (isIndexTriggerDown)
            {
                isIndexTriggerDown = false;
                offset = Vector3.zero;
            }
        }
        transform.position = transform.position + offset * translationGain;
        */
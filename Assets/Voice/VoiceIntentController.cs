using System;
using System.Collections;
using Meta.WitAi;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Intents;
using Meta.WitAi.Events;
using Meta.WitAi.Json;
using Meta.XR.BuildingBlocks.Editor;
using Oculus.Voice;
using UnityEngine;

public enum VoiceCommand
{
    Attract,
    Grab, 
    Move,
    Rotate,
    Release,
    Leave,
}

public class VoiceIntentController : MonoBehaviour
{
    [SerializeField] public AppVoiceExperience appVoiceExperience;
    [SerializeField] private MyGrabLeft myGrabLeft;
    [SerializeField] private SelectionTaskMeasure selectionTaskMeasure;

    [SerializeField] private OVRInput.Controller leftController;
    private bool activateVoiceThisUpdate = false;
    private bool deactivateVoiceThisUpdate = false;
    private float previousBumperValue = 0.0f;
    private float currentBumperValue = 0.0f;
    //[SerializeField] private String fullTranscriptText;
    //[SerializeField] private String partialTranscriptText;

    // Should be true when player is in interaction
    private bool appVoiceActive = false;
    private bool voiceToBeActivated = false;

    public void TriggerAttractionSpell(String[] info)
    {
        Debug.Log("TriggerAttractionSpell called");
        Debug.Log("Info: " + info[0]);
        myGrabLeft.HandleVoiceCommand(VoiceCommand.Attract);
    }

    public void TriggerGrabbingSpell(String[] info)
    {
        Debug.Log("TriggerGrabbingSpell called");
        Debug.Log("Info: " + info[0]);
        myGrabLeft.HandleVoiceCommand(VoiceCommand.Grab);
    }

    public void ToggleMove(String[] info)
    {
        Debug.Log("ToggleMove called");
        Debug.Log("Info: " + info[0]);
        myGrabLeft.HandleVoiceCommand(VoiceCommand.Move);
    }

    public void ToggleRotation(String[] info)
    {
        Debug.Log("ToggleRotation called");
        Debug.Log("Info: " + info[0]);
        myGrabLeft.HandleVoiceCommand(VoiceCommand.Rotate);
    }

    public void TriggerRelease(String[] info)
    {
        Debug.Log("TriggerRelease called");
        Debug.Log("Info: " + info[0]);
        myGrabLeft.HandleVoiceCommand(VoiceCommand.Release);
    }

    public void TriggerServantLeave(String[] info)
    {
        Debug.Log("TriggerServantLeave called");
        Debug.Log("Info: " + info[0]);
        myGrabLeft.HandleVoiceCommand(VoiceCommand.Leave);
    }

    void Update()
    {
        previousBumperValue = currentBumperValue;
        currentBumperValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, leftController);

        if (!selectionTaskMeasure.isTaskStart)
        {
            return;
        }

        if (previousBumperValue < 0.95f && currentBumperValue >= 0.95f)
        {
            // Pressed button this update
            activateVoiceThisUpdate = true;
        }
        else if (previousBumperValue >= 0.95f && currentBumperValue < 0.95f)
        {
            // Released button this update
            deactivateVoiceThisUpdate = true;
        }

        if (activateVoiceThisUpdate)
        {
            Debug.Log("Listening...");
            appVoiceExperience.Activate();
            activateVoiceThisUpdate = false;
        }

        if (deactivateVoiceThisUpdate)
        {
            Debug.Log("Stop listening...");
            appVoiceExperience.Deactivate();
            deactivateVoiceThisUpdate = false;
        }
    }

/*
    public void NoMatchFound(String info)
    {
        Debug.Log("No match for voice command could be found");
    }

    public void OutOfDomain(String info)
    {
        Debug.Log("Voice command is out of domain");
    }

    private void Awake()
    {
        appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnStoppedListening);
        Debug.Log("OnStoppedListening listener added");

        appVoiceExperience.VoiceEvents.OnValidatePartialResponse.AddListener(OnValidatePartialResponse);
        Debug.Log("OnValidatePartialResponse listener added");
    }

    private void OnDestroy()
    {
        appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(OnStoppedListening);
        Debug.Log("OnStoppedListening listener removed");

        appVoiceExperience.VoiceEvents.OnValidatePartialResponse.RemoveListener(OnValidatePartialResponse);

    }

    void OnValidatePartialResponse(VoiceSession session)
    {
        Debug.Log("OnValidatePartialResponse called");
        if (session.response == null)
        {
            Debug.Log("No response");
            return;
        }
        Debug.Log("Partial response: " + session.response);
        
        //WitResponseNode intent = session.response.GetFirstIntent();

    }

    // Update is called once per frame
    public void InitiateVoiceActivation()
    {
        Debug.Log("Activate voice");
        appVoiceExperience.Activate();
        appVoiceActive = true;
    }

    public void FinishVoiceActivation()
    {
        Debug.Log("Deactivate voice");
        appVoiceExperience.Deactivate();
        appVoiceActive = false;
    }

    void OnStoppedListening() 
    {
        Debug.Log("OnStoppedListening called");
        if (appVoiceActive){
            voiceToBeActivated = true;
        }
    }

    void Start()
    {
        InitiateVoiceActivation();
    }
*/
    
}

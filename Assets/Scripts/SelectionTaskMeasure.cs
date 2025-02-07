using System.Collections;
using UnityEngine;
using TMPro;
public class SelectionTaskMeasure : MonoBehaviour
{
    public GameObject targetT;
    public GameObject targetTPrefab;
    Vector3 targetTStartingPos;
    public GameObject objectT;
    public GameObject objectTPrefab;
    Vector3 objectTStartingPos;

    public GameObject taskStartPanel;

    public LocomotionTechnique locomotionTechnique;

    public GameObject donePanel;
    public GameObject submissionSphere;
    public TMP_Text startPanelText;
    
    public TMP_Text scoreText;
    public int completeCount;
    public bool isTaskStart;
    public bool isTaskEnd;
    public bool isCountdown;
    public Vector3 manipulationError;
    public float taskTime;
    public GameObject taskUI;
    public ParkourCounter parkourCounter;
    public DataRecording dataRecording;
    private int part;
    public float partSumTime;
    public float partSumErr;


    public GameObject staff;
    public GameObject broom;
    public GameObject flyingBookPrefab;
    public GameObject flyingBook;
    public GameObject targetBookPrefab;
    public GameObject targetBook;
    private Vector3 flyingBookCenterPosition;
    private float bookHeight;
    private Vector3 bookStartPointOffset;

    [SerializeField] private PlayAudiosSkeleton playAudiosSkeleton;

    public MyGrabRight myGrabRight;


    // Start is called before the first frame update
    void Start()
    {
        parkourCounter = GetComponent<ParkourCounter>();
        dataRecording = GetComponent<DataRecording>();
        part = 1;
        donePanel.SetActive(false);
        submissionSphere.SetActive(false);
        scoreText.text = "Part" + part.ToString();
        taskStartPanel.SetActive(false);


        staff.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTaskStart)
        {
            // recording time
            taskTime += Time.deltaTime;
        }

        if (isCountdown)
        {
            taskTime += Time.deltaTime;
            startPanelText.text = (3.0 - taskTime).ToString("F1");
        }
    }

    public void TransformBroomToStaff()
    {
        broom.SetActive(false);
        staff.SetActive(true);
    }

    public void TransformStaffToBroom()
    {
        broom.SetActive(true);
        staff.SetActive(false);
    }

    public void StartOneTask()
    {
        Debug.Log("Task started");
        taskTime = 0f;
        isTaskStart = true;
        isTaskEnd = false;

        locomotionTechnique.holdLocomotion = true;

        //voiceIntentController.InitiateVoiceActivation();
        
        taskStartPanel.SetActive(false);
        //donePanel.SetActive(true);
        submissionSphere.SetActive(true);

        playAudiosSkeleton.PlayAppearingAudio();


        targetBook = Instantiate(targetBookPrefab, submissionSphere.transform.position, Random.rotation);
        targetBook.transform.SetParent(submissionSphere.transform);

        TransformBroomToStaff();

        bookHeight = Random.Range(5.0f, 8.0f);
        bookStartPointOffset = Random.onUnitSphere * 2.0f;

        // Spawn flying book and add it to the script on the right hand
        flyingBookCenterPosition = taskUI.transform.position + Vector3.up * bookHeight + bookStartPointOffset;
        flyingBook = Instantiate(flyingBookPrefab, flyingBookCenterPosition, Quaternion.identity);
        myGrabRight.FlyingBook = flyingBook;
        myGrabRight.bookMovement = flyingBook.GetComponent<BookMovement>();

        /*
        objectTStartingPos = taskUI.transform.position + taskUI.transform.forward * 0.5f + taskUI.transform.up * 0.75f;
        targetTStartingPos = taskUI.transform.position + taskUI.transform.forward * 0.75f + taskUI.transform.up * 1.2f;
        objectT = Instantiate(objectTPrefab, objectTStartingPos, new Quaternion(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        targetT = Instantiate(targetTPrefab, targetTStartingPos, new Quaternion(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        */
    }


    public void EndOneTask()
    {
        
        //donePanel.SetActive(false);
        submissionSphere.SetActive(false);
        Debug.Log("Task ended");
        
        // release
        isTaskEnd = true;
        isTaskStart = false;
        
        //voiceIntentController.FinishVoiceActivation();

        // distance error
        manipulationError = Vector3.zero;
        manipulationError += targetBook.transform.position - flyingBook.transform.position;
        manipulationError += targetBook.transform.rotation.eulerAngles - flyingBook.transform.rotation.eulerAngles;

        scoreText.text = scoreText.text + "Time: " + taskTime.ToString("F1") + ", offset: " + manipulationError.magnitude.ToString("F2") + "\n";
        partSumErr += manipulationError.magnitude;
        partSumTime += taskTime;
        dataRecording.AddOneData(parkourCounter.locomotionTech.stage.ToString(), completeCount, taskTime, manipulationError);

        // Debug.Log("Time: " + taskTime.ToString("F1") + "\nPrecision: " + manipulationError.magnitude.ToString("F1"));
        Destroy(flyingBook);
        Destroy(targetBook);

        TransformStaffToBroom();

        StartCoroutine(Countdown(3f));
    }

    IEnumerator Countdown(float t)
    {
        Debug.Log("Coroutine started");
        taskTime = 0f;
        taskStartPanel.SetActive(true);
        isCountdown = true;
        completeCount += 1;

        if (completeCount > 4)
        {
            taskStartPanel.SetActive(false);
            scoreText.text = "Done Part" + part.ToString();
            part += 1;
            completeCount = 0;
        }
        else
        {
            yield return new WaitForSeconds(t);
            isCountdown = false;
            startPanelText.text = "start";
            taskStartPanel.SetActive(false);
            // After countdown is done, player can move again
            Debug.Log("Player should be able to move again");
            locomotionTechnique.holdLocomotion = false;
        }
        isCountdown = false;
        yield return 0;
    }
}

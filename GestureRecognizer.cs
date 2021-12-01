using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; // Control Input
using PDollarGestureRecognizer; // Recognition Algorithm
using System.IO; // Store Gestures and Get from Files
using UnityEngine.Events;

[SerializeField]
class MovementRecognizer : MonoBehaviour
{
    [SerializeField] XRNode inputSource;
    [SerializeField] UnityEngine.XR.Interaction.Toolkit.InputHelpers.Button inputButton;
    [SerializeField] float inputThreshold = 0.1f;
    [SerializeField] Transform movementSource;

    [SerializeField] float newPositionThresholdDistance = 0.05f;
    [SerializeField] GameObject debugCubePrefab;
    [SerializeField] int timeBeforeDeleting;
    [SerializeField] bool creationMode = true;
    [SerializeField] string newGestureName;


    // Once Gestures are Recognized 
    [SerializeField] float recognitionThreshold = 0.79f;

    [SerializeField] float recognitionDelay = 1.5f;
    float timer = 0;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    List<Gesture> trainingSet = new List<Gesture>();
    bool isMoving = false;
    List<Vector3> positionsList = new List<Vector3>();
    int strokeID = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Gets previous gestures / find data that was saved
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");

        foreach (var item in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.XR.Interaction.Toolkit.InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);

        //Start The Movement
        if (!isMoving && isPressed)
        {
            strokeID = 0;
            StartMovement();
        }
        //Ending The Movement
        else if (isMoving && !isPressed)
        {
            timer += Time.deltaTime;
            if (timer > recognitionDelay)
                EndMovement();
        }
        //Updating The Movement
        else if (isMoving && isPressed)
        {
            if (timer > 0)
            {
                strokeID++;
            }

            timer = 0;
            UpdateMovement();
        }
    }

    void StartMovement()
    {
        Debug.Log("Start Movement");
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);

        if (debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), timeBeforeDeleting);
    }

    void EndMovement()
    {
        Debug.Log("End Movement");
        isMoving = false;

        // Creates Gesture from position list
        Point[] pointArray = new Point[positionsList.Count];


        // Handles the 2D to 3D conversion 
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0); // Just ONE stroke
        }

        Gesture newGesture = new Gesture(pointArray);

        //Add A new gesture to training set
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        // Recognition
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log(result.GestureClass + " : " + result.Score);
            if (result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    void UpdateMovement()
    {
        Debug.Log("Update Movement");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];

        if (Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            positionsList.Add(movementSource.position);
            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), timeBeforeDeleting);
        }
    }
}


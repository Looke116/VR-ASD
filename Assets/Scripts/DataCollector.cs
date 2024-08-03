using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public int interval = 10;

    private string _path;
    private StringBuilder _stringBuilder;
    private static StreamWriter _writer;
    private static List<DataPoint> _dataPoints;

    private void Awake()
    {
        // Initialize variables
        _stringBuilder = new StringBuilder();

        // Check if the static variables are already initialized
        if (_writer == null)
        {
            _path = Application.persistentDataPath + "/player " + DateTime.Now.ToString("yyyy-M-d HH-mm") + ".json";
            // _path = Application.persistentDataPath + "/player.json";
            _writer = new StreamWriter(_path);
        }

        if (_dataPoints == null)
            _dataPoints = new List<DataPoint>();
        

        // Call the function Record() repeatedly with a delay of 0 seconds and with the specified interval between calls
        InvokeRepeating(nameof(Record), 0, interval);
    }

    // Track if the application looses focus and record it
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            _dataPoints.Add(new DataPointMessage("Lost focus/quit"));
            CancelInvoke();
        }
        else
        {
            _dataPoints.Add(new DataPointMessage("Regained focus"));
            InvokeRepeating(nameof(Record), 0, interval);
        }
    }

    // Before the application quits format the data points into JSON format and save them to a file
    // If the app is run on Quest/Android use the function "OnApplicationPause" instead of "OnApplicationQuit"
#if PLATFORM_ANDROID
    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus) return;
#else
    private void OnApplicationQuit()
    {
#endif
        // Start the JSON with an array identifier
        _stringBuilder.Append("[");

        // Convert the data points to JSON and add a comma or end the array if it is the las object
        for (int i = 0; i < _dataPoints.Count; i++)
        {
            _stringBuilder.Append(JsonUtility.ToJson(_dataPoints[i], true));
            _stringBuilder.Append(i != _dataPoints.Count - 1 ? "," : "]");
        }

        // Concatenated all the data point strings into one and write it to a file
        var jsonString = _stringBuilder.ToString();
        _writer.Write(jsonString);
        _writer.Close();
    }

    private void Record()
    {
        _dataPoints.Add(new DataPointAvatar(head, leftHand, rightHand));
    }

    // Public record function so it can be called from outside this script
    public void Record(String message)
    {
        _dataPoints.Add(new DataPointMessage(message));
    }
}

// Structure of the DataPoint class hierarchy
internal class DataPoint
{
    private static int _id;
    public int id;
    public string dateTime;

    protected DataPoint()
    {
        id = _id;
        dateTime = DateTime.Now.ToString("u");

        _id++;
    }
}

internal class DataPointMessage : DataPoint
{
    public string Message;

    public DataPointMessage(string message)
    {
        Message = message;
    }
}

internal class DataPointTransform : DataPoint
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Forward;

    public DataPointTransform(Transform gameObject)
    {
        Position = gameObject.position;
        Rotation = gameObject.rotation.eulerAngles;
        Forward = gameObject.forward;
    }
}

internal class DataPointAvatar : DataPoint
{
    public Vector3 HeadPosition;
    public Vector3 HeadRotation;
    public Vector3 HeadForward;

    public Vector3 LeftHandPosition;
    public Vector3 LeftHandRotation;
    public Vector3 LeftHandForward;

    public Vector3 RightHandPosition;
    public Vector3 RightHandRotation;
    public Vector3 RightHandForward;

    public DataPointAvatar(Transform head, Transform leftHand, Transform rightHand)
    {
        HeadPosition = head.position;
        HeadRotation = head.rotation.eulerAngles;
        HeadForward = head.forward;

        LeftHandPosition = leftHand.position;
        LeftHandRotation = leftHand.rotation.eulerAngles;
        LeftHandForward = leftHand.forward;

        RightHandPosition = rightHand.position;
        RightHandRotation = rightHand.rotation.eulerAngles;
        RightHandForward = rightHand.forward;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DataCaptureSystem : MonoBehaviour
{
    public static DataCaptureSystem Instance { get; private set; }

    public GUIStyle style;
    private string labelCaption = "hello!";


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("Warning: Repeated DataCaptureSystem initialization");
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10,10,400,40), labelCaption, style);
    }

    public void ReportEvent(string name, string obj)
    {
        labelCaption = name + " " + obj;
    }

    public void ReportEvent(string name, object obj)
    {
        ReportEvent(name, obj.ToString());
    }
}

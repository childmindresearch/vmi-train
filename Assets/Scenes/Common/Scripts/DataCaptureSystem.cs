using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// A class that captures and reports events in the game.
/// </summary>
/// <description>
/// Note: This class should be early in the script execution order due to the
/// frame number update.
/// </description>
public class DataCaptureSystem : MonoBehaviour
{
    public static DataCaptureSystem Instance { get; private set; }
    public GUIStyle style;
    private string labelCaption = "hello!";
    private List<string> Events = new List<string>();
    private DateTime StartTime = DateTime.Now;
    private int FrameNumber = 0;

    /// <summary>
    /// Called every frame, updates the frame number by one.
    /// </summary>
    void Update()
    {
        FrameNumber += 1;
    }

    /// <summary>
    /// Represents the time interval since the DataCaptureSystem was
    /// initialized.
    /// </summary>
    /// <returns>Time since the DataCaptureSystem was initialized</returns>
    private TimeSpan ExperimentTime()
    {
        return DateTime.Now - StartTime;
    }

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
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

    /// <summary>
    /// Called for rendering the latest logged event.
    /// </summary>
    void OnGUI()
    {
        GUI.Label(new Rect(10,10,400,40), labelCaption, style);
    }

    /// <summary>
    /// Reports an event to the data capture system.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="obj">The object associated with the event.</param>
    /// <param name="separator">The separator to use between the name and object.</param>
    public void ReportEvent(string name, string obj, string separator = "\t")
    {
        labelCaption = FrameNumber.ToString() + separator + ExperimentTime().ToString() + separator + name + separator + obj;
        Events.Add(labelCaption);
    }

    /// <summary>
    /// Reports an event to the data capture system.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="obj">The object associated with the event.</param>
    public void ReportEvent(string name, object obj)
    {
        ReportEvent(name, obj.ToString());
    }

    /// <summary>
    /// Returns a list of all events captured by the data capture system.
    /// </summary>
    /// <returns>A list of all events captured by the data capture system.</returns>
    public List<string> GetEvents()
    {
        return Events;
    }

    /// <summary>
    /// Exports the events captured by the data capture system to a CSV file.
    /// </summary>
    /// <param name="filepath">The name of the file to export to.</param>
    public void ExportEvents(string filepath)
    {
        System.IO.File.WriteAllLines(filepath, Events.ToArray());
    }
}

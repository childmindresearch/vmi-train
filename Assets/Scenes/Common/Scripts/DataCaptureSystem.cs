using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

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
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Called for rendering the latest logged event.
    /// </summary>
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 400, 40), labelCaption, style);
    }

    /// <summary>
    /// Reports an event to the data capture system.
    /// </summary>
    /// <param name="name">The name of the event.</param>
    /// <param name="obj">The object associated with the event.</param>
    /// <param name="separator">The separator to use between the name and object.</param>
    public void ReportEvent(string name, string obj, string separator = "\t")
    {
        labelCaption =
            Time.frameCount.ToString()
            + separator
            + ExperimentTime().ToString()
            + separator
            + name
            + separator
            + obj;
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
    /// Exports the events captured by the data capture system to a TSV file.
    /// This export is intended for use by developers.
    /// </summary>
    /// <param name="filepath">The name of the file to export to.</param>
    public void ExportDevEvents(string filepath)
    {
        System.IO.File.WriteAllLines(filepath, Events.ToArray());
    }

    /// <summary>
    /// Exports the events captured by the data capture system to a TSV file.
    /// This export is intended for use by researchers.
    /// </summary>
    /// <param name="filepath">The name of the file to export to.</param>
    /// <param name="separator">The separator to use between columns.</param>
    public void ExportAnalysisEvents(string filepath, string separator = "\t")
    {
        List<string> tsv = new List<string>();
        List<string> eventsOfInterest = new List<string>
        {
            "frame",
            "acceleration",
            "deceleration",
            "jump",
            "occlusion",
            "Click.position",
            "Train.transform.position",
            "Train.transform.eulerAngles",
            "Train.IsHeld"
        };
        tsv.Add(string.Join(separator, eventsOfInterest));

        // Group lines by frame number
        List<List<string>> eventsByFrame = this.Events
            .GroupBy(line => int.Parse(line.Split('\t')[0]))
            .Select(group => group.ToList())
            .ToList();

        // Convert frame number groups to tsv lines
        foreach (List<string> group in eventsByFrame)
        {
            List<string> csvLine = new List<string>();

            int groupFrameCount = int.Parse(group[0].Split('\t')[0]);

            Dictionary<string, string> currentFrame = eventsOfInterest.ToDictionary(
                eventName => eventName,
                eventName => eventName == "frame" ? groupFrameCount.ToString() : "FALSE"
            );

            foreach (string line in group)
            {
                string[] lineSplit = line.Split(separator);
                string eventName = lineSplit[2];
                string value = lineSplit[3];
                if (eventsOfInterest.Contains(eventName))
                {
                    currentFrame[eventName] = value;
                }
            }

            foreach (string eventName in eventsOfInterest)
            {
                csvLine.Add(currentFrame[eventName]);
            }
            tsv.Add(string.Join(separator, csvLine));
        }
        System.IO.File.WriteAllLines(filepath, tsv.ToArray());
    }
}

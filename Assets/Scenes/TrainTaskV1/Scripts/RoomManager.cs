using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the rooms in the train task, including loading and transitioning
/// between rooms.
/// </summary>
public class RoomManager : MonoBehaviour
{
    public GameObject track;
    public GameObject speedUpIndicator;
    public GameObject slowDownIndicator;
    public float trackSpacing = 2.25f;
    public GameObject occlusionObj;
    public GameObject jumpObj;
    public float occlusionObjSpacing = 1.14f;
    public Clickable player;
    public GameObject distractorContainer;
    public TaskStartOverlay Overlay;
    private Room[] rooms;

    private int currentRoom = 0;

    /// <summary>
    /// Initializes the RoomManager by loading the default experiment
    /// configuration, creating Room objects from the configuration, and
    /// starting the first room.
    /// </summary>
    void Start()
    {
        var config = ExperimentSerialization.LoadFromTxt(GlobalManager.Instance.configFile);

        string jsonConfig = config.ToJson();
        string jsonConfigOneLine = Regex.Replace(jsonConfig, @"\s+", " ");
        DataCaptureSystem.Instance.ReportEvent("RoomManager.Config.Loaded", jsonConfigOneLine);

        rooms = new Room[config.rooms.Length];

        for (var i = 0; i < rooms.Length; i++)
        {
            rooms[i] = Room.FromConfiguration(config.rooms[i], this, this.gameObject);
        }

        DataCaptureSystem.Instance.ReportEvent("RoomManager", "Initialized");

        DataCaptureSystem.Instance.ReportEvent("RoomManager.Room.Start", "0");
        rooms[0].StartRoom();
    }

    /// <summary>
    /// Called once per frame. If the current room is finished, stops the
    /// current room, increments the current room index, and starts the next
    /// room. If there are no more rooms, exports the data capture events to a
    /// file and loads the main menu scene.
    /// </summary>
    void Update()
    {
        if (rooms[currentRoom].finished)
        {
            rooms[currentRoom].StopRoom();
            currentRoom += 1;
            if (currentRoom >= rooms.Length)
            {
                currentRoom = 0;
                string devFilePath = Path.Combine(Application.persistentDataPath, "devFile.tsv");
                string analysisFilePath = Path.Combine(
                    Application.persistentDataPath,
                    "analysisFile.tsv"
                );
                DataCaptureSystem.Instance.ExportDevEvents(devFilePath);
                DataCaptureSystem.Instance.ExportAnalysisEvents(analysisFilePath);
                SceneManager.LoadScene("MainMenu");
            }
            DataCaptureSystem.Instance.ReportEvent("RoomManager.Room.Start", currentRoom);
            rooms[currentRoom].StartRoom();
        }
    }
}

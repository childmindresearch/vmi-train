using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public GameObject track;
    public float trackSpacing = 2.25f;
    public GameObject occlusionObj;
    public GameObject jumpObj;
    public float occlusionObjSpacing = 1.14f;
    public Clickable player;
    public GameObject distractorContainer;
    public TaskStartOverlay Overlay;
    private Room[] rooms;

    private int currentRoom = 0;

    // Start is called before the first frame update
    void Start()
    {
        var config = ExperimentSerialization.LoadDefault();

        DataCaptureSystem.Instance.ReportEvent("RoomManager.Config.Loaded", config.ToJson());

        rooms = new Room[config.rooms.Length];

        for (var i = 0; i < rooms.Length; i++)
        {
            rooms[i] = Room.FromConfiguration(config.rooms[i], this, this.gameObject);
        }

        DataCaptureSystem.Instance.ReportEvent("RoomManager", "Initialized");

        DataCaptureSystem.Instance.ReportEvent("RoomManager.Room.Start", 0);
        rooms[0].StartRoom();

    }

    // Update is called once per frame
    void Update()
    {
        if (rooms[currentRoom].finished)
        {
            rooms[currentRoom].StopRoom();
            currentRoom += 1;
            if (currentRoom >= rooms.Length)
            {
                currentRoom = 0;
                SceneManager.LoadScene("MainMenu");
            }
            DataCaptureSystem.Instance.ReportEvent("RoomManager.Room.Start", currentRoom);
            rooms[currentRoom].StartRoom();
        }
    }
}

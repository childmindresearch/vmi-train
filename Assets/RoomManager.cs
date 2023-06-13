using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject track;
    public float trackSpacing = 2.25f;
    public GameObject occlusionObj;
    public float occlusionObjSpacing = 2.25f;
    public GameObject player;
    public GameObject distractorContainer;
    private Room[] rooms;

    private int currentRoom = 0;

    // Start is called before the first frame update
    void Start()
    {
        var config = ExperimentSerialization.LoadDefault();

        rooms = new Room[config.rooms.Length];

        for (var i = 0; i < rooms.Length; i++)
        {
            rooms[i] = Room.FromConfiguration(config.rooms[i], this, this.gameObject);
        }

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
            }
            rooms[currentRoom].StartRoom();
        }
    }
}

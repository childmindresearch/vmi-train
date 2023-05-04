using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public Room[] rooms;
    public PathController controller;

    private int currentRoom = 0;

    // Start is called before the first frame update
    void Start()
    {
        rooms[0].StartRoom(controller);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.finished)
        {
            rooms[currentRoom].StopRoom();
            currentRoom += 1;
            if (currentRoom >= rooms.Length)
            {
                currentRoom = 0;
            }
            rooms[currentRoom].StartRoom(controller);
        }
    }
}

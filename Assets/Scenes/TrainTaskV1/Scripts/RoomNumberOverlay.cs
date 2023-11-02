using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomNumberOverlay : MonoBehaviour
{
    public GameObject text;
    public RoomManager manager;

    public void Update()
    {
        text.GetComponent<TMP_Text>().text =
            "Room " + (manager.currentRoom + 1) + " of " + manager.rooms.Length;
    }
}

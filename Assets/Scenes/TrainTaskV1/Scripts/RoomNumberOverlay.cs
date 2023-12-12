using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomNumberOverlay : MonoBehaviour
{
    public GameObject text;
    public RoomManager manager;
    public GameObject loadingBarInner;

    public void Update()
    {
        text.GetComponent<TMP_Text>().text =
            "Room " + (GlobalManager.Instance.currentRoom + 1) + " of " + manager.rooms.Length;
        loadingBarInner.GetComponent<Image>().fillAmount =
            ((float)GlobalManager.Instance.currentRoom + 1) / (float)manager.rooms.Length;
    }
}

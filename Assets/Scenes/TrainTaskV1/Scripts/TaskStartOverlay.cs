using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskStartOverlay : MonoBehaviour
{
    public Image ProgressBar;
    public GameObject Instructions;
    public string text;

    public void Start()
    {
        ProgressBar.fillAmount = 0;
    }

    public void Update()
    {
        Instructions.GetComponent<TMP_Text>().text = text;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskStartOverlay : MonoBehaviour
{
    public Image ProgressBar;

    public void Start() {
        ProgressBar.fillAmount = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the main menu scene and its functionality.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    /// <summary>
    /// Called before the first frame update. Reports an event to the data capture system.
    /// </summary>
    public void Start()
    {
        DataCaptureSystem.Instance.ReportEvent("MainMenuManager.Start", "MainMenu");
    }

    /// <summary>
    /// Loads the task scene.
    /// </summary>
    public void StartTask()
    {
        DataCaptureSystem.Instance.ReportEvent("MainMenuManager.StartTask", "TrainTaskV1");
        SceneManager.LoadScene("TrainTaskV1");
    }
}

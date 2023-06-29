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
    /// Clears assets in the current scene and loads the task scene.
    /// </summary>
    public void StartTask() {
        // Clear assets in current scene
        foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            Destroy(obj);
        }

        // Load task scene
        DataCaptureSystem.Instance.ReportEvent("MainMenuManager.StartTask", "TrainTaskV1");
        SceneManager.LoadScene("TrainTaskV1");
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    public void Quit() {
        DataCaptureSystem.Instance.ReportEvent("MainMenuManager.Quit", "Quit");
        Application.Quit();
    }
    
}

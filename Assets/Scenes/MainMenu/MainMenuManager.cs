using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartTask() {
        // Clear assets in current scene
        foreach (var obj in SceneManager.GetActiveScene().GetRootGameObjects()) {
            Destroy(obj);
        }

        // Load task scene
        SceneManager.LoadScene("Task");
    }

    public void Quit() {
        Application.Quit();
    }
    
}

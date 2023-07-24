using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents the dropdown menu in the main menu scene.
/// It is used for selection the level configuration file.
/// </summary>
public class MainMenuDropdown : MonoBehaviour
{
    private string[] configFiles;

    /// <summary>
    /// This method is called when the script instance is being loaded.
    /// It initializes the dropdown menu with the available configuration files.
    /// </summary>
    void Start()
    {
        this.configFiles = System.IO.Directory.GetFiles("Assets/Resources");
        this.configFiles = System.Array.FindAll(this.configFiles, (string name) => name.EndsWith(".txt"));
        this.configFiles = System.Array.ConvertAll(this.configFiles, (string name) => name.Replace("Assets/Resources/", ""));
        this.configFiles = System.Array.ConvertAll(this.configFiles, (string name) => name.Replace(".txt", ""));
        List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
        foreach (string file in this.configFiles) {
            options.Add(new TMPro.TMP_Dropdown.OptionData(file));
        }

        this.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options);
        this.GetComponent<TMPro.TMP_Dropdown>().onValueChanged.AddListener(delegate {
            this.SetSelectedFile();
        });
        this.SetSelectedFile(this.configFiles[0]);
    }

    /// <summary>
    /// Sets the selected configuration file based on the value of the dropdown menu.
    /// </summary>
    void SetSelectedFile() {
        GlobalManager.Instance.configFile = this.configFiles[this.GetComponent<TMPro.TMP_Dropdown>().value];
    }

}

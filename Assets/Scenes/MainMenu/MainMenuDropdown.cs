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
        TextAsset[] configFiles = Resources.LoadAll<TextAsset>("");
        List<string> validConfigFiles = new List<string>();
        foreach (TextAsset configFile in configFiles)
        {
            if (configFile.name.StartsWith("levelConfig"))
            {
                validConfigFiles.Add(configFile.name);
            }
        }
        this.configFiles = validConfigFiles.ToArray();
        List<TMPro.TMP_Dropdown.OptionData> options = new List<TMPro.TMP_Dropdown.OptionData>();
        foreach (string file in this.configFiles)
        {
            options.Add(new TMPro.TMP_Dropdown.OptionData(file));
        }

        this.GetComponent<TMPro.TMP_Dropdown>().AddOptions(options);
        this.GetComponent<TMPro.TMP_Dropdown>()
            .onValueChanged.AddListener(
                delegate
                {
                    this.SetSelectedFile();
                }
            );
        this.SetSelectedFile();
    }

    /// <summary>
    /// Sets the selected configuration file based on the value of the dropdown menu.
    /// </summary>
    void SetSelectedFile()
    {
        GlobalManager.Instance.configFile = this.configFiles[
            this.GetComponent<TMPro.TMP_Dropdown>().value
        ];
    }
}

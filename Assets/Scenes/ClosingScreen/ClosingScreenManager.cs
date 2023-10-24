using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Runtime.InteropServices;

/// <summary>
/// A helper class for downloading a file to the user's device.
/// </summary>
static class DownloadFileHelper
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void downloadToFile(string content, string filename);
#endif

    /// <summary>
    /// Downloads the specified content to a file with the specified filename.
    /// </summary>
    /// <param name="content">The content to download.</param>
    /// <param name="filename">The filename to save the content to.</param>
    public static void DownloadToFile(string content, string filename)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        downloadToFile(content, filename);
#endif
    }
}

/// <summary>
/// Manages the closing screen scene and its functionality.
/// </summary>
public class ClosingScreenManager : MonoBehaviour
{
    /// <summary>
    /// Downloads the development data and reports the event to the data capture
    /// system.
    /// </summary>
    public void DownloadDevelopmentData()
    {
        DataCaptureSystem.Instance.ReportEvent(
            "ClosingScreen.DownloadData",
            "StartDevelopmentDownload"
        );
        Downloader("development_logs.tsv");
    }

    /// <summary>
    /// Downloads the analysis data and reports the event to the data capture
    /// system.
    /// </summary>
    public void DownloadAnalysisData()
    {
        DataCaptureSystem.Instance.ReportEvent(
            "ClosingScreen.DownloadData",
            "StartAnalysisDownload"
        );
        Downloader("analysis_logs.tsv");
    }

    /// <summary>
    /// Downloads the configuration data and reports the event to the data
    /// capture system.
    /// </summary>
    public void DownloadConfigData()
    {
        DataCaptureSystem.Instance.ReportEvent("ClosingScreen.DownloadData", "StartConfigDownload");
        var config = ExperimentSerialization.LoadFromTxt(GlobalManager.Instance.configFile);
        string jsonConfig = config.ToJson();
        string configFilePath = Path.Combine(Application.persistentDataPath, "config.json");
        File.WriteAllText(configFilePath, jsonConfig);
        Downloader("config.json");
    }

    /// <summary>
    /// Downloads a file with the given local name from the persistent data path and saves it to the local machine.
    /// </summary>
    /// <param name="localName">The name of the file to download.</param>
    private void Downloader(string localName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, localName);
        string fileContent = File.ReadAllText(filePath);
        DownloadFileHelper.DownloadToFile(fileContent, localName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

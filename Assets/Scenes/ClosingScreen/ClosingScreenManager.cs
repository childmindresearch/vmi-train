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
    public void DownloadDevelopmentData()
    {
        DataCaptureSystem.Instance.ReportEvent(
            "ClosingScreen.DownloadData",
            "StartDevelopmentDownload"
        );
        Downloader("development_logs.tsv");
    }

    public void DownloadAnalysisData()
    {
        DataCaptureSystem.Instance.ReportEvent(
            "ClosingScreen.DownloadData",
            "StartAnalysisDownload"
        );
        Downloader("analysis_logs.tsv");
    }

    private void Downloader(string localName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, localName);
        string fileContent = File.ReadAllText(filePath);
        DownloadFileHelper.DownloadToFile(fileContent, localName);
    }
}

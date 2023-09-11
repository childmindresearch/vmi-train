using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using UnityEngine.SceneManagement;
using AOT;

public class OpenFileDialog : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void webOpenJsonFile(Action<string> callback);

    [MonoPInvokeCallback(typeof(Action<string>))]
    private static void webOpenJsonFileCallback(string jsonContent)
    {
        string configFilePath = Path.Combine(Application.persistentDataPath, "config.json");
        File.WriteAllText(configFilePath, jsonContent);
        GlobalManager.Instance.configFile = configFilePath;
        SceneManager.LoadScene("TrainTaskV1");
    }
#endif

    public Button UploadButton;

    void Start()
    {
        Button btn = UploadButton.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
#if !UNITY_EDITOR && UNITY_WEBGL
        webOpenJsonFile(webOpenJsonFileCallback);
#endif
    }
}

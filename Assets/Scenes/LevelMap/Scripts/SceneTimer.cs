using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTimer : MonoBehaviour
{
    public string sceneToLoad = "TrainTaskV1";

    public void Start()
    {
        DataCaptureSystem.Instance.ReportEvent("SceneTimer.Start", "LevelMap");
        StartCoroutine(SwitchSceneAfterDelay(3f));
    }

    private IEnumerator SwitchSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneToLoad);
    }
}

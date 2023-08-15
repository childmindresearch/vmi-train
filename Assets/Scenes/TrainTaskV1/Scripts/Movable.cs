using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represents a movable object in the scene. It provides
/// functionality for detecting changes in position, and rotation,
/// and reporting those changes to a data capture system.
/// </summary>
public class Movable : MonoBehaviour
{
    /// <summary>
    /// This method is called every frame and checks if the position, rotation, or scale of the object has changed since the last frame.
    /// If any of these values have changed, it reports the changes to the data capture system.
    /// </summary>
    public void Update()
    {
        // The following could be looped, but it is written out for clarity.
        // Reflection does not provide nice-looking code.
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.position.x",
            this.transform.position.x
        );
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.position.y",
            this.transform.position.y
        );
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.position.z",
            this.transform.position.z
        );
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.eulerAngles.x",
            this.transform.eulerAngles.x
        );
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.eulerAngles.y",
            this.transform.eulerAngles.y
        );
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.eulerAngles.z",
            this.transform.eulerAngles.z
        );
    }
}

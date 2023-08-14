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
    /// Initializes the last position, and rotation values to the current transform values.
    /// </summary>
    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }

    /// <summary>
    /// This method is called every frame and checks if the position, rotation, or scale of the object has changed since the last frame.
    /// If any of these values have changed, it reports the changes to the data capture system.
    /// </summary>
    public void Update()
    {
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.position",
            transform.position
        );
        DataCaptureSystem.Instance.ReportEvent(
            $"{this.name}.transform.eulerAngles",
            transform.eulerAngles
        );
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class represents a movable object in the scene. It provides 
/// functionality for detecting changes in position, rotation, and scale, 
/// and reporting those changes to a data capture system.
/// </summary>
public class Movable : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 lastRotation;
    private Vector3 lastScale;

    void Start() {
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
        lastScale = transform.localScale;
    }

    public void Update() {
        if (lastPosition != transform.position || lastRotation != transform.eulerAngles || lastScale != transform.localScale) {
            DataCaptureSystem.Instance.ReportEvent($"{this.name}", $"Position={transform.position}, Rotation={transform.eulerAngles}, Scale={transform.localScale}");
        }
        lastPosition = transform.position;
        lastRotation = transform.eulerAngles;
        lastScale = transform.localScale;
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class represents a clickable object in the scene. It inherits from the 
/// Movable class and provides functionality for detecting clicks on the object 
/// and reporting those clicks to a data capture system.
/// </summary>
public class Clickable : Movable
{
    /// <summary>
    /// Determines whether the object is currently being held by the user.
    /// </summary>
    /// <returns>True if the object is being held, false otherwise.</returns>
    public bool IsHeld()
    {
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            Vector2 ClickPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            Vector3 ClickWorldPosition = Camera.main.ScreenToWorldPoint(ClickPosition);
            Vector3 BoxSize = new Vector3(0, ClickWorldPosition.y, 0);
            LayerMask mask = 1 << 3;
            return Physics.CheckBox(ClickWorldPosition, BoxSize, Quaternion.identity, mask);
        }
        return false;
    }

    /// <summary>
    /// This method is called every frame. It checks if the object is being held 
    /// by the user and reports the event to the data capture system.
    /// </summary>
    new void Update() {
        base.Update();
        if (this.IsHeld()) {
            DataCaptureSystem.Instance.ReportEvent($"{this.name}", $"Touch=true");
        }
    }
}

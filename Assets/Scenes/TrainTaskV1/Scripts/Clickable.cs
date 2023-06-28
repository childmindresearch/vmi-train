using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : MonoBehaviour
{
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
}

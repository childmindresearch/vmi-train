using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.PlayerSettings;

public class Utils
{
    public static void DrawPath(Vector3[] points, Transform transform, bool loop = false)
    {
        if (points.Length == 0) return;

        for (int i = 0; i < points.Length; i++)
        {
            var p = transform.TransformPoint(points[i]);

            GUI.color = Color.black;
            Handles.Label(p, "" + i);
            Gizmos.DrawWireSphere(p, 1f);
            if (i > 0)
            {
                var p0 = transform.TransformPoint(points[i-1]);
                Gizmos.DrawLine(p0, p);
            }
        }

        if (loop && points.Length > 1)
        {
            Gizmos.DrawLine(
                transform.TransformPoint(points[points.Length - 1]),
                transform.TransformPoint(points[0])
            );
        }

    }


    public static void DrawYRect(float x, float z, float h, float v, float y = 0)
    {
        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x + h, y, z));
        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y, z + v));
        Gizmos.DrawLine(new Vector3(x + h, y, z + v), new Vector3(x + h, y, z));
        Gizmos.DrawLine(new Vector3(x + h, y, z + v), new Vector3(x, y, z + v));
    }
}

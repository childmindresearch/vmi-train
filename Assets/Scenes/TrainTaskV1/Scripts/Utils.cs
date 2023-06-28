using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Utils
{
    public static void DrawPath(Vector3[] points, Transform transform, bool loop = false)
    {
#if UNITY_EDITOR
        if (points.Length == 0) return;

        for (int i = 0; i < points.Length; i++)
        {
            var p = transform.TransformPoint(points[i]);

            GUI.color = Color.black;
            Handles.Label(p, "" + i);
            Gizmos.DrawWireSphere(p, 1f);
            if (i > 0)
            {
                var p0 = transform.TransformPoint(points[i - 1]);
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
#endif
    }


    public static void DrawYRect(float x, float z, float h, float v, float y = 0)
    {
        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x + h, y, z));
        Gizmos.DrawLine(new Vector3(x, y, z), new Vector3(x, y, z + v));
        Gizmos.DrawLine(new Vector3(x + h, y, z + v), new Vector3(x + h, y, z));
        Gizmos.DrawLine(new Vector3(x + h, y, z + v), new Vector3(x, y, z + v));
    }

    public static Vector2[] ScalarArr2Vec2Arr(float[] arr)
    {
        Vector2[] re = new Vector2[arr.Length / 2];
        for (var i = 0; i < re.Length; i++)
        {
            re[i] = new Vector2(
                arr[i * 2],
                arr[i * 2 + 1]
            );
        }
        return re;
    }

    public static Vector3[] ScalarArr2Vec3ArrXZ(float[] arr, float fill = 0)
    {
        Vector3[] re = new Vector3[arr.Length / 2];
        for (var i = 0; i < re.Length; i++)
        {
            re[i] = new Vector3(
                arr[i * 2],
                fill,
                arr[i * 2 + 1]
            );
        }
        return re;
    }

    public static void ClampArray(Vector2[] arr, float min, float max)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i] = new Vector2(
                Mathf.Clamp(arr[i].x, min, max),
                Mathf.Clamp(arr[i].y, min, max)
            );
        }
    }
    public static void ClampArray(float[] arr, float min, float max)
    {
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i] = Mathf.Clamp(arr[i], min, max);
        }
    }

    public class SimpleTimer
    {
        private float interval;
        private float t = 0;

        public SimpleTimer(float interval)
        {
            this.interval = interval;
        }

        public int Update(float delta)
        {
            int re = 0;
            this.t += delta;
            while (t > this.interval)
            {
                t -= this.interval;
                ++re;
            }
            return re;
        }
    }
}
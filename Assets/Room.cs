using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public float RoomWidth, RoomHeight = 50;

    public Vector3[] WayPoints;

    public bool generateTracks = true;
    public GameObject track;
    public float trackSpacing = 2;

    private void SetCameraToBounds(Camera cam)
    {
        cam.transform.position = new Vector3(
            gameObject.transform.position.x + RoomWidth * 0.5f,
            50,
            gameObject.transform.position.z + RoomHeight * 0.5f
        );
        cam.orthographicSize = RoomHeight * 0.5f;

        cam.transform.eulerAngles = new Vector3(90, 0, 0);

        // cam.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        /*cam.transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y,
            cam.transform.position.z
        );*/
        /*
        cam.transform.position = new Vector3(
            gameObject.transform.position.x + RoomWidth * 0.5f,
            50,
            gameObject.transform.position.z + RoomHeight * 0.5f
        );
        cam.orthographicSize = RoomHeight * 0.5f;

        cam.transform.eulerAngles = new Vector3(45, transform.eulerAngles.y, transform.eulerAngles.z);

        cam.transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y,
            cam.transform.position.z - 45
        );
        */
    }

    private Vector3[] RealWaypoints()
    {
        var l = new LagrangeInterpolation(WayPoints);
        var pp = new Vector3[30];

        for (int i = 0; i < pp.Length; i++)
        {
            var x = WayPoints[0].x + (i * ((WayPoints[WayPoints.Length - 1].x - WayPoints[0].x) / (pp.Length - 1)));
            pp[i] = new Vector3(
                x,
                WayPoints[0].y,
                l.get(x)
            );
        }

        var WayPoints2 = pp;

        Vector3[] wps = new Vector3[WayPoints2.Length];
        for (int i = 0; i < WayPoints2.Length; i++)
        {
            wps[i] = gameObject.transform.TransformPoint(WayPoints2[i]);
        }
        return wps;
    }

    public void StartRoom(PathController controller)
    {
        SetCameraToBounds(Camera.main);
        controller.StartPath(RealWaypoints());
    }

    public void StopRoom()
    {
    }

    private void Start()
    {
        if (track == null || !generateTracks) return;

        var wps = RealWaypoints();

        for (int i = 1; i < wps.Length; i++)
        {
            var trajectory = wps[i] - wps[i - 1];
            var trajectoryMag = trajectory.magnitude;

            var numSegments = Mathf.FloorToInt(trajectoryMag / trackSpacing);



            for (int n = 0; n < numSegments; n++)
            {
                var segment = Instantiate(track);
                segment.transform.position = wps[i - 1];
                segment.transform.LookAt(wps[i]);

                segment.transform.position = wps[i - 1] + (trajectory * (1 / trajectoryMag * ((n + .5f) * trackSpacing)));
            }
            var endSegment = Instantiate(track);
            endSegment.transform.position = wps[i] - (trajectory * (1 / trajectoryMag * .5f));
            endSegment.transform.LookAt(wps[i - 1]);
        }
    }

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        /*foreach (var wp in RealWaypoints())
        {
            Gizmos.DrawWireSphere(wp, 1.0f);
        }*/

        Gizmos.color = Color.blue;
        Utils.DrawYRect(
            gameObject.transform.position.x,
            gameObject.transform.position.z,
            RoomWidth,
            RoomHeight
        );
        Gizmos.color = Color.red;
        Utils.DrawPath(WayPoints, gameObject.transform);
    }
}

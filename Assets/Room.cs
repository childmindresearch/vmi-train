using System.Collections.Generic;
using UnityEngine;
using static ExperimentSerialization;

public class Room : MonoBehaviour
{
    public float RoomWidth, RoomHeight = 50;

    public Vector3[] WayPoints;
    public XPath TimePos;
    public bool TimePosInterpolation = false;
    public Vector2[] OcclusionStartStop;
    public RectifiedPath Path = null;

    public bool generateTracks = true;
    public RoomManager manager;

    public float Duration = 10f;
    private float StartTime = 0f;
    public bool finished = false;

    public int NumDistractors = 0;

    public int seed = 42;

    public float[] jumps;
    public float jumpDuration = 1f;
    public float jumpTimePosSlope = -0.5f;
    public GameObject[] jumpObjs;
    public XPath[] jumpPaths;

    public Utils.SimpleTimer DataCaptureTimer = new Utils.SimpleTimer(0.1f);

    private void SetCameraToBounds(Camera cam)
    {
        cam.transform.position = new Vector3(
            gameObject.transform.position.x + RoomWidth * 0.5f,
            50,
            gameObject.transform.position.z + RoomHeight * 0.5f
        );
        var roomAspect = RoomWidth / RoomHeight;
        var screenAspect = Screen.width / (float)Screen.height;
        cam.orthographicSize = (roomAspect > screenAspect) ? RoomHeight * 0.5f * (roomAspect / screenAspect) : RoomHeight * 0.5f;

        cam.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    private void OnRectTransformDimensionsChange()
    {
        SetCameraToBounds(Camera.main);
    }

    public static Room FromConfiguration(RoomConfiguration config, RoomManager manager, GameObject parent)
    {
        var roomObj = new GameObject("Room");
        roomObj.transform.parent = parent.transform;
        var room = roomObj.AddComponent<Room>();
        room.RoomWidth = config.width;
        room.RoomHeight = config.height;
        room.manager = manager;
        room.Duration = config.durationSec;
        room.NumDistractors = config.numDistractors;
        room.seed = config.seed;
        room.jumps = config.jumps;
        room.jumpTimePosSlope = config.jumpTimePosSlope;
        Utils.ClampArray(room.jumps, 0, 1);

        room.TimePosInterpolation = config.timeposInterpolation;
        var tempTimePos = Utils.ScalarArr2Vec2Arr(config.timepos);
        Utils.ClampArray(tempTimePos, 0, 1);

        if (room.TimePosInterpolation)
        {
            room.TimePos = new XPath(new LagrangeInterpolation(tempTimePos).rectify(0, 1, 0.01f, 0.001f, 1000).points);
        }
        else
        {
            room.TimePos = new XPath(new List<Vector2>(Utils.ScalarArr2Vec2Arr(config.timepos)));
        }


        room.OcclusionStartStop = Utils.ScalarArr2Vec2Arr(config.occlusionStartStop);
        Utils.ClampArray(room.OcclusionStartStop, 0, 1);

        room.WayPoints = Utils.ScalarArr2Vec3ArrXZ(config.path);
        for (var i = 0; i < room.WayPoints.Length; i++)
        {
            room.WayPoints[i] = new Vector3(
                room.WayPoints[i].x * config.width,
                room.WayPoints[i].y,
                room.WayPoints[i].z * config.height
            );
        }

        room.RecalculatePath();

        room.gameObject.SetActive(false);

        return room;
    }

    private void RecalculatePath()
    {
        Path = new LagrangeInterpolation(WayPoints).rectify(0, RoomWidth, 1, 0.01f, 1000);
    }

    public void StartRoom()
    {
        this.gameObject.SetActive(true);
        SetCameraToBounds(Camera.main);

        StartTime = Time.time;
        finished = false;
    }

    public void StopRoom()
    {
        this.gameObject.SetActive(false);
    }

    private void GenerateObjAlongPath(GameObject segmentPrefab, float spacing, float start = 0, float stop = 1)
    {
        var numSegments = (int) Path.arcLength / spacing;

        for (int i = 1; i < numSegments; i++)
        {
            float perc = (i - .5f) / (numSegments - 1f);
            if (perc < start || stop < perc) continue;

            var segment = Instantiate(segmentPrefab, transform);

            var p0 = Path.getLerp((i - 1) / ((float)numSegments - 1));
            var p1 = Path.getLerp(i / ((float)numSegments - 1));
            segment.transform.position = new Vector3(
                (p0.x + p1.x) * 0.5f,
                0,
                (p0.y + p1.y) * 0.5f
            );

            segment.transform.LookAt(new Vector3(p1.x, 0, p1.y));
        }
    }

    private void GenerateTracks()
    {
        if (manager.track == null || !generateTracks) return;

        GenerateObjAlongPath(manager.track, manager.trackSpacing);
    }

    private void GenerateOcclusions()
    {
        if (manager.occlusionObj == null) return;

        for (var i = 0; i < OcclusionStartStop.Length; i++)
        {
            GenerateObjAlongPath(
                manager.occlusionObj, manager.occlusionObjSpacing,
                OcclusionStartStop[i].x, OcclusionStartStop[i].y);
        }
    }

    private void GenerateDistractors()
    {
        Random.InitState(seed);

        var numDistractorPrototypes = manager.distractorContainer.transform.childCount;
        for (int i = 0; i < NumDistractors; i++)
        {
            int ri = Random.Range(0, numDistractorPrototypes);
            var dObj = manager.distractorContainer.transform.GetChild(ri).gameObject;
            Vector3 rPos = new Vector3(
                Random.Range(0, RoomWidth),
                0,
                Random.Range(0, RoomHeight)
            );

            Instantiate(dObj, rPos, dObj.transform.rotation, this.transform);
        }
    }

    private void GenerateJumps()
    {
        if (manager.jumpObj == null) return;

        jumpObjs = new GameObject[jumps.Length];
        jumpPaths = new XPath[jumps.Length];

        var posTime = TimePos.invert();

        for (var i = 0; i < jumps.Length; i++)
        {
            var jumpTime = posTime.getLerp(jumps[i]);

            float intercept = jumps[i] - jumpTimePosSlope * jumpTime;

            jumpObjs[i] = Instantiate(manager.jumpObj, transform);
            jumpPaths[i] = new XPath(new List<Vector2>(new Vector2[] {
                new Vector2(0, intercept),
                new Vector2(1, intercept + jumpTimePosSlope)
            }));
        }
    }

    private void Start()
    {
        if (Path == null) RecalculatePath();

        GenerateTracks();
        GenerateOcclusions();
        GenerateDistractors();
        GenerateJumps();
    }

    void Update()
    {
        float a = (Time.time - StartTime) / Duration;

        if (a >= 1) { finished = true; return; }

        float b = TimePos.getLerp(a);

        var p2d = Path.getLerp(b);
        var p2dTarget = Path.getLerp(b + 0.01f);
        manager.player.transform.position = new Vector3(p2d.x, 0, p2d.y);
        manager.player.transform.LookAt(new Vector3(p2dTarget.x, 0, p2dTarget.y));

        for (int i = 0; i < jumps.Length; ++i)
        {
            // j = status/percentage of jump: 0 = begin, 1 = end
            float j = (((b - jumps[i]) / (jumpDuration / Path.arcLength)) * .5f) + 0.5f;
            if (j > 0 && j < 1)
            {
                manager.player.transform.Rotate(new Vector3(0, 1, 0), j * 360);
            }

            var pos = Path.getLerp(jumpPaths[i].getLerp(a));
            jumpObjs[i].transform.position = new Vector3(pos.x, 0, pos.y);
        }


        for (int i = DataCaptureTimer.Update(Time.deltaTime); i > 0; --i)
        {
            DataCaptureSystem.Instance.ReportEvent("DataCapture",
                $"Position={manager.player.transform.position} " +
                $"Rotation={manager.player.transform.eulerAngles} " +
                $"Touch={(Input.touchCount > 0 ? Input.GetTouch(0).position.ToString() : "None")}");
        }
    }

    void OnDrawGizmos()
    {
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

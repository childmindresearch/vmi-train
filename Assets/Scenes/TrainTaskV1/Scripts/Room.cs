using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ExperimentSerialization;

public class Room : MonoBehaviour
{
    public float RoomWidth,
        RoomHeight = 50;

    public Vector3[] WayPoints;
    public XPath TimePos;
    public bool TimePosInterpolation = false;
    public Vector2[] OcclusionStartStop;
    public RectifiedPath Path = null;

    public GameObject AcceleratorOverlay;
    public bool generateTracks = true;
    public RoomManager manager;

    public float Duration = 10f;
    private float StartTime = 0f;
    private bool Started = false;
    public bool finished = false;

    public int NumDistractors = 0;

    public int seed = 42;

    public float[] jumps;
    public float jumpDuration = 1f;
    public float jumpTimePosSlope = -0.5f;
    public GameObject[] jumpObjs;
    public XPath[] jumpPaths;
    public string instructions;

    /// <summary>
    /// Sets the position and orthographic size of the given camera to match the dimensions of the Room.
    /// </summary>
    /// <param name="cam">The camera to set the position and orthographic size of.</param>
    private void SetCameraToBounds(Camera cam)
    {
        cam.transform.position = new Vector3(
            gameObject.transform.position.x + RoomWidth * 0.5f,
            50,
            gameObject.transform.position.z + RoomHeight * 0.5f
        );
        var roomAspect = RoomWidth / RoomHeight;
        var screenAspect = Screen.width / (float)Screen.height;
        cam.orthographicSize =
            (roomAspect > screenAspect)
                ? RoomHeight * 0.5f * (roomAspect / screenAspect)
                : RoomHeight * 0.5f;

        cam.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    /// <summary>
    /// This method is called when the dimensions of the RectTransform of the GameObject this script is attached to change.
    /// It sets the camera bounds to match the new dimensions of the Room.
    /// </summary>
    private void OnRectTransformDimensionsChange()
    {
        SetCameraToBounds(Camera.main);
    }

    /// <summary>
    /// Creates a new Room instance from a RoomConfiguration object.
    /// </summary>
    /// <param name="config">The RoomConfiguration object to use for creating the Room instance.</param>
    /// <param name="manager">The RoomManager instance that manages the Room.</param>
    /// <param name="parent">The parent GameObject to attach the Room to.</param>
    /// <returns>A new Room instance created from the RoomConfiguration object.</returns>
    public static Room FromConfiguration(
        RoomConfiguration config,
        RoomManager manager,
        GameObject parent
    )
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
        room.instructions = config.instructions;
        Utils.ClampArray(room.jumps, 0, 1);

        room.TimePosInterpolation = config.timeposInterpolation;
        var tempTimePos = Utils.ScalarArr2Vec2Arr(config.timepos);
        Utils.ClampArray(tempTimePos, 0, 1);

        if (room.TimePosInterpolation)
        {
            room.TimePos = new XPath(
                new LagrangeInterpolation(tempTimePos).rectify(0, 1, 0.01f, 0.001f, 1000).points
            );
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
        room.AcceleratorOverlay = Instantiate(new GameObject("Canvas"));
        room.AcceleratorOverlay.AddComponent<Canvas>();
        room.AcceleratorOverlay.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        room.AcceleratorOverlay.GetComponent<Canvas>().worldCamera = Camera.main;
        room.AcceleratorOverlay.GetComponent<Canvas>().planeDistance = 49;
        room.AcceleratorOverlay.transform.SetParent(roomObj.transform);

        room.gameObject.SetActive(false);

        return room;
    }

    /// <summary>
    /// Recalculates the path of the Room using Lagrange interpolation.
    /// </summary>
    private void RecalculatePath()
    {
        Path = new LagrangeInterpolation(WayPoints).rectify(0, RoomWidth, 1, 0.01f, 1000);
    }

    /// <summary>
    /// Activates the room game object.
    /// </summary>
    public void StartRoom()
    {
        this.gameObject.SetActive(true);
        SetCameraToBounds(Camera.main);
        DataCaptureSystem.Instance.ReportEvent(
            "Room.StartHash",
            ObjectHash.ComputeSha256Hash(this)
        );

        finished = false;
    }

    /// <summary>
    /// Deactivates the room game object.
    /// </summary>
    public void StopRoom()
    {
        this.gameObject.SetActive(false);
        DataCaptureSystem.Instance.ReportEvent("Room.EndHash", ObjectHash.ComputeSha256Hash(this));
    }

    /// <summary>
    /// Generates objects along the path of the room by instantiating object segments at regular intervals.
    /// </summary>
    /// <param name="segmentPrefab">The prefab of the object segment to instantiate.</param>
    /// <param name="spacing">The spacing between each object segment.</param>
    /// <param name="start">The starting percentage of the path to generate objects on (default is 0).</param>
    /// <param name="stop">The ending percentage of the path to generate objects on (default is 1).</param>
    private void GenerateObjAlongPath(
        GameObject segmentPrefab,
        float spacing,
        float start = 0,
        float stop = 1
    )
    {
        var numSegments = (int)Path.arcLength / spacing;

        for (int i = 1; i < numSegments; i++)
        {
            float perc = (i - .5f) / (numSegments - 1f);
            if (perc < start || stop < perc)
                continue;

            var segment = Instantiate(segmentPrefab, transform);

            var p0 = Path.getLerp((i - 1) / ((float)numSegments - 1));
            var p1 = Path.getLerp(i / ((float)numSegments - 1));
            segment.transform.position = new Vector3((p0.x + p1.x) * 0.5f, 0, (p0.y + p1.y) * 0.5f);

            segment.transform.LookAt(new Vector3(p1.x, 0, p1.y));
        }
    }

    /// <summary>
    /// Generates tracks in the room by instantiating track objects along the path of the room.
    /// </summary>
    private void GenerateTracks()
    {
        if (manager.track == null || !generateTracks)
            return;

        GenerateObjAlongPath(manager.track, manager.trackSpacing);
    }

    /// <summary>
    /// Generates occlusions in the room by instantiating occlusion objects along the path of the room.
    /// Occlusion objects are instantiated at random positions within the specified start and stop range.
    /// </summary>
    private void GenerateOcclusions()
    {
        if (manager.occlusionObj == null)
            return;

        for (var i = 0; i < OcclusionStartStop.Length; i++)
        {
            GenerateObjAlongPath(
                manager.occlusionObj,
                manager.occlusionObjSpacing,
                OcclusionStartStop[i].x,
                OcclusionStartStop[i].y
            );
        }
    }

    /// <summary>
    /// Generates distractors in the room by randomly selecting objects from the
    /// distractor container and instantiating them at random positions within
    /// the room.
    /// </summary>
    private void GenerateDistractors()
    {
        Random.InitState(seed);

        var numDistractorPrototypes = manager.distractorContainer.transform.childCount;
        for (int i = 0; i < NumDistractors; i++)
        {
            int ri = Random.Range(0, numDistractorPrototypes);
            var dObj = manager.distractorContainer.transform.GetChild(ri).gameObject;
            Vector3 rPos = new Vector3(Random.Range(0, RoomWidth), 0, Random.Range(0, RoomHeight));

            Instantiate(dObj, rPos, dObj.transform.rotation, this.transform);
        }
    }

    /// <summary>
    /// Generates speedup and slowdown indicators along the train's path and places them in the AcceleratorOverlay.
    /// </summary>
    private void GenerateBoostAndBrake()
    {
        // Place a speedup indicator wherever the train speeds up and a slowdown indicator
        // wherever it slows down.
        if (manager.speedUpIndicator == null || manager.slowDownIndicator == null)
            return;
        if (this.AcceleratorOverlay == null)
            return;

        var accelPoints = new List<float>();
        var decelPoints = new List<float>();
        var lastPos = Path.getLerp(0);
        float lastVel = 0f;
        float timeStep = 0.001f;

        for (float pos = 0.0f; pos < 0.95f; pos += timeStep)
        {
            var currentTimePos = TimePos.getLerp(pos);
            var currentPos = Path.getLerp(currentTimePos);
            var currentVel = (currentPos - lastPos).magnitude / timeStep;
            double currentAccel = currentVel - lastVel;

            if (currentAccel < -0.5)
                decelPoints.Add(currentTimePos);
            if (currentAccel > 0.5)
                accelPoints.Add(currentTimePos);

            lastPos = currentPos;
            lastVel = currentVel;
        }

        CreateIndicators(accelPoints, manager.speedUpIndicator);
        CreateIndicators(decelPoints, manager.slowDownIndicator);
    }

    private void CreateIndicators(List<float> points, GameObject indicatorPrefab)
    {
        foreach (var pos in points)
        {
            var indicator_pos = Path.getLerp(pos);
            var lookat_pos = Path.getLerp(pos + 0.01f);
            var indicator = Instantiate(indicatorPrefab, transform);
            indicator.transform.position = new Vector3(indicator_pos.x, 0.01f, indicator_pos.y);
            indicator.transform.LookAt(new Vector3(lookat_pos.x, 0.01f, lookat_pos.y));
            indicator.transform.Rotate(new Vector3(1, 0, 0), 90);
            indicator.transform.SetParent(this.AcceleratorOverlay.transform);
        }
    }

    /// <summary>
    /// Generates jumps in the room by instantiating jump objects along the path
    /// of the room. Jump objects are instantiated at positions determined by
    /// the jump time and the slope of the jump time position.
    /// </summary>
    private void GenerateJumps()
    {
        if (manager.jumpObj == null)
            return;

        jumpObjs = new GameObject[jumps.Length];
        jumpPaths = new XPath[jumps.Length];

        var posTime = TimePos.invert();

        for (var i = 0; i < jumps.Length; i++)
        {
            var jumpTime = posTime.getLerp(jumps[i]);

            float intercept = jumps[i] - jumpTimePosSlope * jumpTime;

            jumpObjs[i] = Instantiate(manager.jumpObj, transform);
            jumpObjs[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

            jumpPaths[i] = new XPath(
                new List<Vector2>(
                    new Vector2[]
                    {
                        new Vector2(0, intercept),
                        new Vector2(1, intercept + jumpTimePosSlope)
                    }
                )
            );
        }
    }

    /// <summary>
    /// Sets the position and orientation of the train based on the current time.
    /// </summary>
    private void SetTrainPosition()
    {
        float a = (Time.time - StartTime) / Duration;
        float b = TimePos.getLerp(a);

        var p2d = Path.getLerp(b);
        var p2dTarget = Path.getLerp(b + 0.01f);

        manager.player.transform.position = new Vector3(p2d.x, 0, p2d.y);
        manager.player.transform.LookAt(new Vector3(p2dTarget.x, 0, p2dTarget.y));
    }

    /// <summary>
    /// Sets the jumps for the room based on the current time and player position.
    /// </summary>
    private void SetJumps()
    {
        float a = (Time.time - StartTime) / Duration;
        float b = TimePos.getLerp(a);

        for (int i = 0; i < jumps.Length; ++i)
        {
            // j = status/percentage of jump: 0 = begin, 1 = end
            float j = (((b - jumps[i]) / (jumpDuration / Path.arcLength)) * .5f) + 0.5f;

            if (j > 0 && j < 1)
            {
                DataCaptureSystem.Instance.ReportEvent("jump", true);
                manager.player.transform.Rotate(new Vector3(0, 1, 0), j * 360);
            }

            var pos = Path.getLerp(jumpPaths[i].getLerp(a));
            jumpObjs[i].transform.position = new Vector3(pos.x, 0, pos.y);
        }
    }

    /// <summary>
    /// Waits for the player to hold the train for a certain amount of time before starting the room.
    /// </summary>
    private void AwaitStart()
    {
        manager.Overlay.gameObject.SetActive(true);
        StartTime = Time.time;
        SetTrainPosition();
        StartCoroutine(AwaitStartCoroutine());
    }

    /// <summary>
    /// Waits for the player to hold the train for a certain amount of time before starting the room.
    /// </summary>
    /// <returns>An IEnumerator that waits for the player to hold the train for a certain amount of time before starting the room.</returns>
    private IEnumerator AwaitStartCoroutine()
    {
        float timeHeld = 0; // seconds
        float timeToHold = 0.5f; // seconds
        while (timeHeld < timeToHold)
        {
            if (manager.player.IsHeld())
            {
                timeHeld += Time.deltaTime;
            }
            else
            {
                timeHeld = 0;
            }
            manager.Overlay.ProgressBar.fillAmount = timeHeld / timeToHold;

            yield return null;
        }

        Started = true;
        StartTime = Time.time;
        manager.Overlay.gameObject.SetActive(false);
    }

    /// <summary>
    /// Logs special events during gameplay, such as occlusions and accelerator/decelerator events.
    /// Logging at the object level is preferred; this function is for logging events that are not
    /// associated with a particular object, but rather interactions between them.
    /// </summary>
    private void LogSpecialEvents()
    {
        // Log occlusion events
        for (int i = 0; i < this.OcclusionStartStop.Length; i++)
        {
            float currentPos = TimePos.getLerp((Time.time - StartTime) / Duration);
            if (
                currentPos > this.OcclusionStartStop[i][0]
                && currentPos < this.OcclusionStartStop[i][1]
            )
            {
                DataCaptureSystem.Instance.ReportEvent("occlusion", true);
            }
        }

        // Log accelerator/decelerator events
        int nearestIndicator = 0;
        float nearestIndicatorDistance = 999;
        bool isAccel = false;

        for (int i = 0; i < this.AcceleratorOverlay.transform.childCount; i++)
        {
            Vector3 indicatorPos = this.AcceleratorOverlay.transform.GetChild(i).position;
            float indicatorDistance = Vector3.Distance(
                this.manager.player.transform.position,
                indicatorPos
            );
            if (indicatorDistance < nearestIndicatorDistance)
            {
                nearestIndicator = i;
                nearestIndicatorDistance = indicatorDistance;
                isAccel = this.AcceleratorOverlay.transform.GetChild(i).name == "Booster(Clone)";
            }
        }

        if (nearestIndicatorDistance < 0.05)
        {
            if (isAccel)
            {
                DataCaptureSystem.Instance.ReportEvent("acceleration", true);
            }
            else
            {
                DataCaptureSystem.Instance.ReportEvent("deceleration", true);
            }
        }

        // Log clicks
        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            Vector2 ClickPosition =
                Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            Vector3 ClickWorldPosition = Camera.main.ScreenToWorldPoint(ClickPosition);
            DataCaptureSystem.Instance.ReportEvent("Click.x", ClickWorldPosition.x);
            DataCaptureSystem.Instance.ReportEvent("Click.y", ClickWorldPosition.y);
            DataCaptureSystem.Instance.ReportEvent("Click.z", ClickWorldPosition.z);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if (Path == null)
            RecalculatePath();

        GenerateTracks();
        GenerateOcclusions();
        GenerateDistractors();
        GenerateJumps();
        GenerateBoostAndBrake();
        AwaitStart();
    }

    /// <summary>
    /// Update is called once per frame. It updates the train's position, sets the jumps, and logs special events.
    /// </summary>
    void Update()
    {
        if (!Started)
            return;
        if ((Time.time - StartTime) / Duration >= 1)
        {
            finished = true;
            return;
        }

        SetTrainPosition();
        SetJumps();
        LogSpecialEvents();
    }

    /// <summary>
    /// Draws Gizmos in the Scene view for the Room object, including a blue rectangle representing the room's bounds
    /// and a red path representing the room's waypoints.
    /// </summary>
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

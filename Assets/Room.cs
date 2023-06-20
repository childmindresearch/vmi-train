using UnityEngine;
using static ExperimentSerialization;

public class Room : MonoBehaviour
{
    public float RoomWidth, RoomHeight = 50;

    public Vector3[] WayPoints;
    public Vector2[] TimePos;
    public bool TimePosInterpolation = false;
    public Vector2[] OcclusionStartStop;
    public RectifiedPath Path = null;

    public bool generateTracks = true;
    public RoomManager manager;

    public float Duration = 10f;
    private float StartTime = 0f;
    public bool finished = false;

    public int numStaticDistractors = 0;
    public int numMovingDistractors = 0;

    public int seed = 42;

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
        room.numStaticDistractors = config.numStaticDistractors;
        room.numMovingDistractors = config.numMovingDistractors;
        room.seed = config.seed;

        room.TimePosInterpolation = config.timeposInterpolation;
        room.TimePos = Utils.ScalarArr2Vec2Arr(config.timepos);

        if (room.TimePosInterpolation)
        {
            room.TimePos = new LagrangeInterpolation(room.TimePos).rectify(0, 1, 0.01f, 0.001f, 1000).GetPoints();
        }
        Utils.ClampArrInPlace(room.TimePos, 0, 1);


        room.OcclusionStartStop = Utils.ScalarArr2Vec2Arr(config.occlusionStartStop);
        Utils.ClampArrInPlace(room.OcclusionStartStop, 0, 1);

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
        if (manager.track == null || !generateTracks) return;

        var pp = new Vector3[(int)(Path.arcLength / spacing)];

        for (int i = 1; i < pp.Length; i++)
        {
            float perc = (i - .5f) / (pp.Length - 1f);
            if (perc < start || stop < perc) continue;

            var segment = Instantiate(segmentPrefab, transform);

            var p0 = Path.getLerp((i - 1) / ((float)pp.Length - 1));
            var p1 = Path.getLerp(i / ((float)pp.Length - 1));
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
            GenerateObjAlongPath(manager.occlusionObj, manager.occlusionObjSpacing, OcclusionStartStop[i].x, OcclusionStartStop[i].y);
        }


    }

    private void GenerateStaticDistractors()
    {
        Random.InitState(seed);
        
        var numDistractorPrototypes = manager.staticDistractorContainer.transform.childCount;
        for (int i = 0; i < numStaticDistractors; i++)
        {
            int ri = Random.Range(0, numDistractorPrototypes);
            var dObj = manager.staticDistractorContainer.transform.GetChild(ri).gameObject;
            Vector3 rPos = new Vector3(
                Random.Range(0, RoomWidth),
                0,
                Random.Range(0, RoomHeight)
            );

            Instantiate(dObj, rPos, dObj.transform.rotation, this.transform);
        }
    }

    private void GenerateMovingDistractors()
{
    var numDistractorPrototypes = manager.movingDistractorContainer.transform.childCount;
    for (int i = 0; i < numMovingDistractors; i++)
    {
        int ri = Random.Range(0, numDistractorPrototypes);
        var dObj = manager.movingDistractorContainer.transform.GetChild(ri).gameObject;
        Vector3 rPos = new Vector3(
            Random.Range(0, RoomWidth),
            0,
            Random.Range(0, RoomHeight)
        );
        
        var obj = Instantiate(dObj, rPos, dObj.transform.rotation, this.transform);
        obj.GetComponent<Rigidbody>().velocity = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ) * 10;
        

    }
}

    private void Start()
    {
        if (Path == null) RecalculatePath();

        GenerateTracks();
        GenerateOcclusions();
        GenerateStaticDistractors();
        GenerateMovingDistractors();
    }

    void Update()
    {
        float a = (Time.time - StartTime) / Duration;

        if (a >= 1) { finished = true; return; }

        float b = 1;
        for (int i = 1; i < TimePos.Length; i++)
        {
            if (a > TimePos[i - 1].x && a < TimePos[i].x)
            {
                float f = (a - TimePos[i - 1].x) / (TimePos[i].x - TimePos[i - 1].x);
                b = Mathf.Lerp(TimePos[i - 1].y, TimePos[i].y, f);
                break;
            }
        }

        var p2d = Path.getLerp(b);
        var p2dTarget = Path.getLerp(b + 0.01f);
        manager.player.transform.position = new Vector3(p2d.x, 0, p2d.y);
        manager.player.transform.LookAt(new Vector3(p2dTarget.x, 0, p2dTarget.y));
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

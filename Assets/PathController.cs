using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

public enum PathMovementStyle
{
    Continuous,
}
public class PathController : MonoBehaviour
{
    public float MovementSpeed = 30;

    public PathMovementStyle MovementStyle = PathMovementStyle.Continuous;
    public bool LoopThroughPoints = false;

    private Vector3[] wayPoints = new Vector3[0];

    private int currentTarget = 0;

    public bool finished
    {
        get;
        private set;
    } = false;


    public void StartPath(Vector3[] points)
    {
        finished = false;
        wayPoints = points;
        currentTarget = 0;
        if (wayPoints.Length == 0) return;
        gameObject.transform.position = wayPoints[0];
        transform.LookAt(wayPoints[0]);
    }

    public void StopPath()
    {
        finished = true;
        currentTarget = 0;
        wayPoints = new Vector3[0];
    }

    private void Update()
    {
        if (wayPoints.Length == 0) return;

        var distance = Vector3.SqrMagnitude(transform.position - wayPoints[currentTarget]);
        if (Mathf.Abs(distance) < MovementSpeed * Time.deltaTime)
        {
            transform.position = wayPoints[currentTarget];
            currentTarget++;
            if (currentTarget >= wayPoints.Length)
            {
                currentTarget = LoopThroughPoints ? 0 : wayPoints.Length - 1;
                finished = true;
            }
            transform.LookAt(wayPoints[currentTarget]);
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentTarget], (MovementSpeed) * Time.deltaTime);
    }
}
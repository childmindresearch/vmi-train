using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class XPath
{
    [SerializeField]
    public List<Vector2> points;

    public XPath(List<Vector2> points)
    {
        this.points = points.OrderBy(o=>o.x).Select(o=>new Vector2(o.x,o.y)).ToList();
    }

    public float getLerp(float a)
    {
        float b = 1;
        for (int i = 1; i < points.Count; i++)
        {
            if (a >= points[i - 1].x && a <= points[i].x)
            {
                float f = (a - points[i - 1].x) / (points[i].x - points[i - 1].x);
                b = Mathf.Lerp(points[i - 1].y, points[i].y, f);
                break;
            }
        }
        return b;
    }

    public XPath invert()
    {
        return new XPath(points.Select(o => new Vector2(o.y, o.x)).ToList());
    }
        
}

[Serializable]
public class RectifiedPath
{
    [SerializeField]
    public List<Vector2> points;
    [SerializeField]
    public float arcLength;

    public RectifiedPath(List<Vector2> points, float arcLength)
    {
        this.points = points;
        this.arcLength = arcLength;
    }

    /// Get linearly interpolated point on path for a [0; 1]
    public Vector2 getLerp(float a)
    {
        float findex = (this.points.Count - 1) * a;
        int index = (int)findex;
        float remainder = findex - index;


        if (index < 0) return this.points[0];
        if (index >= this.points.Count - 1) return this.points[this.points.Count - 1];

        return new Vector2(
            (this.points[index].x * (1f - remainder) + this.points[index+1].x * remainder),
            (this.points[index].y * (1f - remainder) + this.points[index+1].y * remainder)
        );
    }

    public Vector2 getNearest(float a)
    {
        float findex = (this.points.Count - 1) * a;
        int index = (int)findex;

        if (index >= this.points.Count - 1) return this.points[this.points.Count - 1];
        return this.points[index];
    }

    public Vector2[] GetPoints()
    {
        return this.points.ToArray();
    }
}

public class LagrangeInterpolation
{
    private readonly float[] x;
    private readonly float[] y;
    private readonly float[] w;

    public LagrangeInterpolation(Vector3[] points)
    {
        this.x = new float[points.Length];
        this.y = new float[points.Length];
        for (var j = 0; j < points.Length; j++)
        {
            this.x[j] = points[j].x;
            this.y[j] = points[j].z;
        }
        this.w = lagrange_weights(this.x);
    }
    public LagrangeInterpolation(Vector2[] points)
    {
        this.x = new float[points.Length];
        this.y = new float[points.Length];
        for (var j = 0; j < points.Length; j++)
        {
            this.x[j] = points[j].x;
            this.y[j] = points[j].y;
        }
        this.w = lagrange_weights(this.x);
    }

    public LagrangeInterpolation(float[] xp, float[] yp)
    {
        if (xp.Length != yp.Length)
        {
            throw new InvalidOperationException("point x and y coordinate arrays must be the same length");
        }

        this.x = xp;
        this.y = yp;
        this.w = lagrange_weights(xp);
    }

    public float get(float x)
    {
        var a = 0.0f;
        var b = 0.0f;
        for (var j = 0; j < this.w.Length; j++)
        {
            if (x == this.x[j]) return (this.y[j]);
            a += (this.w[j] / (x - this.x[j])) * this.y[j];
            b += (this.w[j] / (x - this.x[j]));
        }
        return a / b;
    }

    private static float[] lagrange_weights(float[] x)
    {
        var w = new float[x.Length];
        for (var j = 0; j < w.Length; j++)
        {
            w[j] = 1.0f;
            for (var m = 0; m < w.Length; m++)
            {
                if (m == j) continue;
                w[j] *= (1.0f / (x[j] - x[m]));
            }
        }
        return w;
    }

    public RectifiedPath rectify(
        float start,
        float stop,
        float lenInterval,
        float step,
        int maxIntervals
    )
    {
        var res = new List<Vector2>();

        float last_x = start;
        float last_y = get(start);
        res.Add(new Vector2(last_x, last_y));
        float sum = 0;
        float totalSum = 0;
        for (float x = start + step; x < stop; x += step)
        {
            float y = get(x);
            float stepLength = Mathf.Sqrt(Mathf.Pow(x - last_x, 2) + Mathf.Pow(y - last_y, 2));
            sum += stepLength;
            totalSum += stepLength;

            if (sum > lenInterval)
            {
                
                if (res.Count > maxIntervals - 2)
                {
                    throw new Exception("Too many intervals.");
                }
                res.Add(new Vector2(x, y));
                sum = 0;
            }

            last_x = x;
            last_y = y;
        }
        res.Add(new Vector2(stop, get(stop)));

        return new RectifiedPath(res, totalSum);
    }
}

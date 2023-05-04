using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

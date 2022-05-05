using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineGenerator
{
    SplineSettings settings;
    Spline spline;

    public void UpdateSettings(SplineSettings settings)
    {
        this.settings = settings;
    }

    public void UpdateSpline()
    {
        List<Vector3> points = new List<Vector3>(settings.points);
        Vector3 initialVel = settings.initialVelocity;
        Vector3 finalVel = settings.finalVelocity;

        
        // points.Add(settings.points[0]);

        int N = points.Count;
        float[,] m = new float[N, N];

        // Populate Matrix
        m[0, 0] = 2;
        m[0, 1] = 1;

        for(int i = 1; i < N - 1; ++i)
        {
            m[i, i - 1] = 1;
            m[i, i]     = 4;
            m[i, i + 1] = 1;
        }

        m[N - 1, N - 2] = 1;
        m[N - 1, N - 1] = 2;

        List<Vector3> data = new List<Vector3>();

        // Populate data
        data.Add(points[1] - points[0] - initialVel);

        for(int i = 1; i < N - 1; ++i)
        {
            data.Add(points[i + 1] - 2.0f * points[i] +points[i - 1]);
        }

        data.Add(finalVel - points[N - 1] + points[N - 2]);

        for(int i = 0; i < N; ++i)
        {
            data[i] *= 6;
        }

        // Execute pseudo gauss jordan
        float tempMult = 1.0f / m[0, 0];

        m[0, 0] *= tempMult;
        m[0, 1] *= tempMult;
        data[0] *= tempMult;

        // Solve downwards
        for(int i = 1; i < N - 1; ++i)
        {
            m[i, i - 1] -= m[i - 1, i - 1];
            m[i, i] -= m[i - 1, i];
            m[i, i + 1] -= m[i - 1, i + 1];

            data[i] -= data[i - 1];

            tempMult = 1.0f / m[i, i];

            m[i, i] *= tempMult;
            m[i, i + 1] *= tempMult;

            data[i] *= tempMult;
        }

        m[N - 1, N - 2] -= m[N - 2, N - 2];
        m[N - 1, N - 1] -= m[N - 2, N - 1];

        data[N - 1] -= data[N - 2];

        tempMult = 1.0f / m[N - 1, N - 1];

        m[N - 1, N - 1] *= tempMult;

        data[N - 1] *= tempMult;

        // Solve upwards
        for (int i = N - 2; i >= 0; --i)
        {
            data[i] -= data[i + 1] * m[i, i + 1];
            m[i, i + 1] = 0.0f;
        }

        spline = new Spline(points, data);
    }

    public Vector3 GeneratePoint(float t)
    {
        return spline.GeneratePoint(t);
    }
}

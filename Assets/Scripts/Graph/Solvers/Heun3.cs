using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heun3 : ODESolver
{
    public Heun3()
    {
        ThisSolver = Solvers.Heun3;
    }

    public override Vector2[] Solve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        return Heun3Stage1D(ti, tf, N, x0, dxdt);
    }
    public Vector2[] Heun3Stage1D(float xi, float xf, int N, float v0, Func<float, float, float> dxdt)
    {
        Vector2[] pts = new Vector2[N + 1];
        pts[0] = new Vector2(xi, v0);
        float h = (xf - xi) / N;
        for (int i = 1; i < N + 1; i++)
        {
            float x = Mathf.Lerp(xi, xf, (float)i / N);
            float k1 = dxdt(pts[i - 1].y, pts[i - 1].x);
            float k2 = dxdt(pts[i - 1].y + (h / 3) * k1, pts[i - 1].x + h / 3);
            float k3 = dxdt(pts[i - 1].y + (2 * h / 3) * k2, pts[i - 1].x + (2 * h / 3));
            float y = pts[i - 1].y + (h / 4) * (k1 + 3 * k3);
            pts[i] = new Vector2(x, y);
        }
        SetParameters(xi, xf, N, v0, dxdt, pts);
        return pts;
    }

    public static Vector2[] Heun3Stage1Step(float xi, float xf, int N, float v0, Func<float, float, float> dxdt)
    {
        Vector2[] pts = new Vector2[2];
        pts[0] = new Vector2(xi, v0);
        float h = (xf - xi) / N;
        float x = xi + h;
        float k1 = dxdt(pts[0].y, pts[0].x);
        float k2 = dxdt(pts[0].y + (h / 3) * k1, pts[0].x + h / 3);
        float k3 = dxdt(pts[0].y + (2 * h / 3) * k2, pts[0].x + (2 * h / 3));
        float y = pts[0].y + (h / 4) * (k1 + 3 * k3);
        pts[1] = new Vector2(x, y);
        return pts;
    }
    public static Vector2[] Heun3StageNStep(float stepsize, float x0, float v0, int numofSteps, Func<float, float, float> dxdt)
    {
        Vector2[] pts = new Vector2[numofSteps + 1];
        pts[0] = new Vector2(x0, v0);
        float h = stepsize;
        for (int i = 1; i < numofSteps + 1; i++)
        {
            float x = x0 + i * h;
            float k1 = dxdt(pts[i - 1].y, pts[i - 1].x);
            float k2 = dxdt(pts[i - 1].y + (h / 3) * k1, pts[i - 1].x + h / 3);
            float k3 = dxdt(pts[i - 1].y + (2 * h / 3) * k2, pts[i - 1].x + (2 * h / 3));
            float y = pts[i - 1].y + (h / 4) * (k1 + 3 * k3);
            pts[i] = new Vector2(x, y);
        }
        return pts;
    }

    public override Vector2[] UpdateSolve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        Vector2[] output;
        if (ti != tInitial || tf != tFinal || N != numOfSteps || x0 != initialValue || dxdt != Function)
        {
            output = Heun3Stage1D(ti, tf, N, x0, dxdt);
            return output;
        }
        else
        {
            return CurrentPoints;
        }
    }
}

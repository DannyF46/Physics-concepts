using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heun2 : ODESolver
{
    public Heun2()
    {
        ThisSolver = Solvers.Heun2;
    }

    public override Vector2[] Solve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        return Heun2Stage1D(ti, tf, N, x0, dxdt);
    }
    public Vector2[] Heun2Stage1D(float xi, float xf, int N, float v0, Func<float, float, float> dxdt)//not static -- requires an instance (and can store parameters)
    {
        Vector2[] pts = new Vector2[N + 1];
        pts[0] = new Vector2(xi, v0);
        float h = (xf - xi) / N;
        for (int i = 1; i < N+1; i++)
        {
            float x = Mathf.Lerp(xi, xf, (float)i / N);
            float k1 = dxdt(pts[i - 1].y, pts[i - 1].x);

            float y = pts[i - 1].y + (h / 2) * (k1 + dxdt(pts[i - 1].y + h * k1, pts[i - 1].x));
            pts[i] = new Vector2(x, y);
        }
        SetParameters(xi, xf, N, v0, dxdt, pts);
        return pts;
    }
    public static Vector2[] Heun2StageNStep(float xi, float xf, int N, float v0, Func<float, float, float> dxdt)//static -- makes usable without an instance
    {
        
        Vector2[] pts = new Vector2[N + 1];
        pts[0] = new Vector2(xi, v0);
        float h = (xf - xi) / N;
        for (int i = 1; i < N; i++)
        {
            float x = Mathf.Lerp(xi, xf, (float)i / N);
            float k1 = dxdt(pts[i - 1].y, pts[i - 1].x);

            float y = pts[i - 1].y + (h / 2) * (k1 + dxdt(pts[i - 1].y + h * k1, pts[i - 1].x));
            pts[i] = new Vector2(x, y);
        }
        return pts;
    }
    
    public override Vector2[] UpdateSolve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        Vector2[] output;
        if (ti != tInitial || tf != tFinal || N != numOfSteps || x0 != initialValue || dxdt != Function)
        {
            output = Heun2Stage1D(ti, tf, N, x0, dxdt);
            return output;
        }
        else 
        {
            return CurrentPoints;
        }
    }
}

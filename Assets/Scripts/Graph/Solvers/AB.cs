using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AB : ODESolver
{
    public AB()
    {
        ThisSolver = Solvers.AdamsBashforth;
    }
    public override Vector2[] Solve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        return AdamsBashforth1D(ti, tf, N, x0, dxdt);
    }
    public static Vector2[] AdamsBashforth1D(float xi, float xf, int N, float v0, Func<float, float, float> dxdt)
    {
        Vector2[] pts = new Vector2[N + 1];
        float[] f = new float[N + 1];
        pts[0] = new Vector2(xi, v0);
        pts[1] = Heun3.Heun3Stage1Step(xi, xf, N, v0, dxdt)[1];
        f[0] = dxdt(v0, xi);
        float h = (xf - xi) / N;

        for (int i = 2; i < N+1; i++)
        {

            float x = Mathf.Lerp(xi, xf, (float)i / N);
            f[i - 1] = dxdt(pts[i - 1].y, pts[i - 1].x);

            float y = pts[i - 1].y + (h / 2) * (3 * f[i - 1] - f[i - 2]);
            pts[i] = new Vector2(x, y);
        }
        return pts;
    }

    public override Vector2[] UpdateSolve(float ti, float tf, int N, float v0, Func<float, float, float> dxdt)
    {
        throw new NotImplementedException();
    }
}

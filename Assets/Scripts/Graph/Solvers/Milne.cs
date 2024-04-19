using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Milne : ODESolver
{
    public Milne()
    {
        ThisSolver = Solvers.Milne ;
    }
    public override Vector2[] Solve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        return Milne1D(ti, tf, N, x0, dxdt);
    }
    public static Vector2[] Milne1D(float ti, float tf, int N, float v0, Func<float, float, float> dxdt)
    {
        float h = (tf - ti) / N;
        Vector2[] pts = new Vector2[N + 1];
        Vector2[] predictorpts = new Vector2[N + 1];
        Vector2[] firststeps = Heun3.Heun3StageNStep(h, ti, v0, 3, dxdt); // first 3 points provided by H3
        pts[0] = new Vector2(ti, v0);

        float[] f = new float[N + 1];
        f[0] = dxdt(v0, ti);
        for (int i = 1; i < N + 1; i++)
        {
            float x = Mathf.Lerp(ti, tf, (float)i / N);


            if (i > 3)
            {

                f[i - 1] = dxdt(pts[i - 1].y, x - h);

                float predictory = pts[i - 4].y + (4f / 3f) * h * (2 * f[i - 3] - f[i - 2] + 2 * f[i - 1]);
                float y = pts[i - 2].y + (h / 3) * (f[i - 2] + 4 * f[i - 1] + dxdt(predictory, x));

                predictorpts[i] = new Vector2(x, predictory);
                pts[i] = new Vector2(x, y);
            }
            else
            {
                f[i] = dxdt(pts[i].y, x);
                pts[i] = firststeps[i];
            }

        }
        return pts;
    }

    public override Vector2[] UpdateSolve(float ti, float tf, int N, float v0, Func<float, float, float> dxdt)
    {
        throw new NotImplementedException();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardEuler : ODESolver
{

    
    public ForwardEuler()
    {
        ThisSolver = Solvers.ForwardEuler;
    }
    
    public override Vector2[] Solve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        return ForwardEuler1D(ti, tf, N, x0, dxdt);
    }
    public Vector2[] ForwardEuler1D(float xi, float xf, int N, float v0, Func<float, float, float> dxdt)
    {
        /*
         * FE: y_{n+1} = y_n + h*f(y_n,t_n); f(y,t) = dy/dt
         */
        Vector2[] pts = new Vector2[N + 1];
        pts[0] = new Vector2(xi, v0);
        float h = (xf - xi) / N;
        for (int i = 0; i < N; i++)
        {
            float x = Mathf.Lerp(xi, xf, (float)(i+1)/ N);
            float y = pts[i].y + h * dxdt(pts[i].y, x);

            pts[i + 1] = new Vector2(x, y);
        }

        SetParameters(xi, xf, N, v0, dxdt, pts);
        return pts;
    }
    /*
    public static Vector2[,] ForwardEulerND(float ti, float tf, int N, float[] x0, Func<float[], float, float[]> dxdt) //public float[] dxdt(float[] pos, float t)
    {

        int dim = x0.Length;
        Vector2[,] pts = new Vector2[dim, N + 1];
        float h = (tf - ti) / N;
        
        for(int i = 0; i < dim; i++)
        {
            pts[i, 0] = new Vector2(ti, x0[i]);

            for (int j = 0; j < N; j++)
            {
                float x = Mathf.Lerp(ti, tf, (float)j / N);
                float y = pts[i,j].y + h * dxdt(pts[i,j], x);

                pts[i, j + 1] = new Vector2(x, y);
            }
        }    

        return pts;
    }*/
    public override Vector2[] UpdateSolve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt)
    {
        Vector2[] output;
        if (ti != tInitial || tf != tFinal || N != numOfSteps || x0 != initialValue || dxdt != Function)
        {
            output = ForwardEuler1D(ti, tf, N, x0, dxdt);
            return output;
        }
        else
        {
            return CurrentPoints;
        }
    }
}

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class ODESolver
{
    public enum Solvers { ForwardEuler, Heun2, Heun3, AdamsBashforth, Milne, NA };
    public Solvers ThisSolver { get; protected set; } = Solvers.NA;
    protected float tInitial { get; private set; }
    protected float tFinal { get; private set; }
    protected int numOfSteps { get; private set; }
    protected float initialValue { get; private set; }
    protected Func<float, float, float> Function { get; private set; }
    public Vector2[] CurrentPoints { get; private set; }
    protected void SetParameters(float ti, float tf, int N, float v0, Func<float, float, float> dxdt, Vector2[] points)
    {
        tInitial = ti;
        tFinal = tf;
        numOfSteps = N;
        initialValue = v0;
        Function = dxdt;
        CurrentPoints = points;
    }

    abstract public Vector2[] Solve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt);
    abstract public Vector2[] UpdateSolve(float ti, float tf, int N, float x0, Func<float, float, float> dxdt);
}

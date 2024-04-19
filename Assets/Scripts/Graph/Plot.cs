using MathNet.Numerics.OdeSolvers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.PlayerSettings;

public class Plot : MonoBehaviour //where everything comes together. eventually user will use UI elements that interact with this component and change parameters
{

    public Axis _xAxis;
    public Axis _yAxis;
    private Axis[] Axes = new Axis[2]; //collect x- and y-axes into a single list (for DataPoint.UpdateNumberOfPoints method)
    public Ticks _xTicks;
    public Ticks _yTicks;
    public Gridlines _xGridlines;
    public Gridlines _yGridlines;
    public DataPoint _Points;
    private GameObject AxesParent, GridlineParent, TickParent, TickLabelParent, PointsParent;

    //User parameters
    [SerializeField] public Vector2 minValues;
    [SerializeField] public Vector2 maxValues;
    [SerializeField] public Vector2Int numberOfTicks;
    [SerializeField] public int numOfPts = 10;
    [SerializeField] public float ptSize = 0.05f;
    [SerializeField] public Color ptColor = Color.black;

    private float delta = 0;
    private DampOsc oscillator;
    public float naturalFrequency = 1f;
    public float Dampening = 0.1f;
    public float Response = 0f;
    public Vector2[] ICs = new Vector2[] { Vector2.up, Vector2.down };
    public ODESolver solverData;
    public DataPoint plotData;

    //private Heun3 HeunSolver = new Heun3();

    [SerializeField] private ODESolver.Solvers Solver;

    // Start is called before the first frame update
    void Start()
    {
        delta = (maxValues.x - minValues.x) / numOfPts;
        //Assign parents
        AxesParent = Globals.Instance.AxesParent;
        TickParent = Globals.Instance.TickParent;
        GridlineParent = Globals.Instance.GridlineParent;
        PointsParent = Globals.Instance.DataParent;

        //initialize y-axis, ticks, and gridlines
        _yAxis = AxesParent.AddComponent<Axis>();
        _yAxis.MakeAxis(minValues.y, maxValues.y, Vector2.right);
        _yAxis.ZeroLine.tag = "yAxis";
        _yTicks = TickParent.AddComponent<Ticks>();
        _yTicks._Axis = _yAxis;
        _yTicks.SetTicks(numberOfTicks.y, Vector2.up);

        _yGridlines = GridlineParent.AddComponent<Gridlines>();
        _yGridlines._Axis = _yAxis;
        _yGridlines.SetGridlines(numberOfTicks.y, Vector2.up);

        //initialize x-axis, ticks, and gridlines
        _xAxis = AxesParent.AddComponent<Axis>();
        _xAxis.MakeAxis(minValues.x, maxValues.x, Vector2.up);
        _xAxis.ZeroLine.tag = "xAxis";
        _xTicks = TickParent.AddComponent<Ticks>();
        _xTicks._Axis = _xAxis;
        _xTicks.SetTicks(numberOfTicks.x, Vector2.right);

        _xGridlines = GridlineParent.AddComponent<Gridlines>();
        _xGridlines._Axis = _xAxis;
        _xGridlines.SetGridlines(numberOfTicks.x, Vector2.right);
        ODESolver H3solver = new Heun3();
        //Put axes into Axes list
        Axes[0] = _xAxis;
        Axes[1] = _yAxis;
        (plotData, solverData) = StartODEDraw(minValues.x, maxValues.x, numOfPts, ICs[0].y, dxdt, Solver);
      


        //Damped Oscillator
        //oscillator = new DampOsc(naturalFrequency, Dampening, Response);
 
        //Vector2[] H3 = HeunSolver.Heun3Stage1D(minValues.x, maxValues.x, numOfPts, ICs[0].y, dxdt);

        //_Points = new DataPoint(Axes, DampenedOscillator(oscillator,ICs), ptSize, ptColor); //instantiates and plots the points pts (make it easier to sort/layer?)
        //_Points = new DataPoint(Axes, H3, ptSize, ptColor);

        //DataPoint _driverPts = new DataPoint(Axes, Function(OscillatorDriver), 0.02f, Color.red);
        //sample: plot 2 sets of data
        /*
        Vector2[] pts = new Vector2[numOfPts+1];
        Vector2[] pts2 = new Vector2[numOfPts+1];
        for (int i = 0; i < numOfPts+1; i++) 
        {
            float x = Mathf.Lerp(minValues.x, maxValues.x, (float)i / numOfPts);
            float y = Mathf.Sin(x);
            pts[i] = new Vector2(x, y);
            pts2[i] = new Vector2(x, 2*y);
        }
        _Points = new DataPoint(Axes, pts, 0.05f, UnityEngine.Color.black); //instantiates and plots the points pts
        DataPoint Points2 = new DataPoint(Axes, pts2, 0.025f, UnityEngine.Color.red); //instantiate and plot the points pts
        */
    }
    private void Update()
    {
        //if a parameter changes, update the bounds of the plot accordingly. Kinda brute-forced, theres probably a better way
        if (_xAxis.minValue != minValues.x || _xAxis.maxValue != maxValues.x || _yAxis.minValue != minValues.y || _yAxis.maxValue != maxValues.y || numberOfTicks.x != _xTicks.tickCount || numberOfTicks.y != _yTicks.tickCount)
        {
            _xAxis.UpdateAxis(minValues.x, maxValues.x, Vector2.right);
            _yAxis.UpdateAxis(minValues.y, maxValues.y, Vector2.up);
            Axes[0] = _xAxis;
            Axes[1] = _yAxis;
            _xTicks.UpdateTicks(numberOfTicks.x, _xAxis, Vector2.right);
            _yTicks.UpdateTicks(numberOfTicks.y, _yAxis, Vector2.up);
            _xGridlines.UpdateGridlines(numberOfTicks.x, _xAxis, Vector2.right);
            _yGridlines.UpdateGridlines(numberOfTicks.y, _yAxis, Vector2.up);
            delta = (maxValues.x - minValues.x) / numOfPts;
        }
        UpdateODEDraw(minValues.x, maxValues.x, numOfPts, ICs[0].y, dxdt, Solver);

        /*
        if (oscillator.NatFreq != naturalFrequency || oscillator.Damping != Dampening || oscillator.Response != Response)
        {
            oscillator.SetConstants(naturalFrequency, Dampening, Response);
            _Points.SetPositions(DampenedOscillator(oscillator, ICs));
        }*/
        if (plotData.allPoints[0].transform.localScale.x != ptSize)
        {
            plotData.SetScales(ptSize);
        }
    }
    public Vector2[] Expoenential()
    {
        Vector2[] pts = new Vector2[numOfPts + 1];
        for (int i = 0; i < numOfPts + 1; i++)
        {
            float x = Mathf.Lerp(minValues.x, maxValues.x, (float)i / numOfPts);
            //float x = Mathf.Sin(t + Mathf.Sin(Time.realtimeSinceStartup));
            //float y = funct(x).y;

            pts[i] = new Vector2(x, Mathf.Exp(x));
        }
        return pts;
    }
    public Vector2[] Function(Func<float, Vector2> funct) //whatever mathematical function one wants to plot. currently returns a sine wave
    {
        Vector2[] pts = new Vector2[numOfPts + 1];
        for (int i = 0; i < numOfPts + 1; i++)
        {
            float x = Mathf.Lerp(minValues.x, maxValues.x, (float)i / numOfPts);
            //float x = Mathf.Sin(t + Mathf.Sin(Time.realtimeSinceStartup));
            //float y = funct(x).y;

            pts[i] = funct(x);
        }
        return pts;

    }
    public Vector2[] DampenedOscillator(DampOsc osc, Vector2[] oscICs)
    {
        Vector2[] driverICs = DriverIC();
        osc.SetICs(oscICs, driverICs);

        Vector2[] driverPts = Function(OscillatorDriver);
        Vector2[] pts = new Vector2[numOfPts + 1];

        for (int i = 0; i < numOfPts + 1; i++)
        {
            float x = Mathf.Lerp(minValues.x, maxValues.x, (float)i / numOfPts);
            float y = osc.UpdateSolver(delta, driverPts[i]).y;
            pts[i] = new Vector2(x, y);
        }
        return pts;
    }
    public Vector2 OscillatorDriver(float t)
    {
        Vector2 driverPt;

        driverPt = new Vector2(t, t * Heaviside(t, 2f) * Heaviside(-t, -4f));
        //driverPt = new Vector2(t, Mathf.PerlinNoise1D(t));
        return driverPt;
    }
    public Vector2[] DriverIC()
    {
        Vector2[] ics = new Vector2[2];
        ics[0] = OscillatorDriver(minValues.x);
        ics[1] = (OscillatorDriver(minValues.x + delta) - ics[0]) / delta;
        return ics;
    }
    public float Heaviside(float x, float a)
    {
        float Step;
        if (x <= a)
        {
            Step = 0f;
        }
        else if (x > a)
        {
            Step = 1f;
        }
        else
        {
            Step = -1;
        }
        return Step;
    }

    public float dxdt(float x, float t)
    {
        return (x);
    }

    public (DataPoint, ODESolver) StartODEDraw(float xi, float xf, int N, float v0, Func<float, float, float> dxdt, ODESolver.Solvers solver)
    {
        DataPoint plotData;
        ODESolver solverData;
        switch (solver)
        {
            case (ODESolver.Solvers.ForwardEuler):
                solverData = new ForwardEuler();
                break;
            case (ODESolver.Solvers.Heun3):
                solverData = new Heun3();
                break;
            case (ODESolver.Solvers.Heun2):
                solverData = new Heun2();
                break;
            case (ODESolver.Solvers.AdamsBashforth):
                solverData = new AB();
                break;
            case (ODESolver.Solvers.Milne):
                solverData = new Milne();
                break;
            default:
                solverData = null;
                break;
            
        }
        solverData.Solve(xi, xf, N, v0, dxdt);
        plotData = new DataPoint(Axes, solverData.CurrentPoints, ptSize, ptColor);
        return (plotData,solverData);
    }
    public void UpdateODEDraw(float xi, float xf, int N, float v0, Func<float, float, float> dxdt, ODESolver.Solvers solver)//make more flexible -- easy way to make multiple lines? (also legend)
    {

        if (solver == solverData.ThisSolver)
        {
            solverData.UpdateSolve(xi, xf, N, v0, dxdt);   
        }
        else
        {
            plotData.ClearPoints();
            (plotData, solverData) = StartODEDraw(xi, xf, N, v0, dxdt, solver);

        }
        if (N + 1 != plotData.allPoints.Length)
        {
            plotData.UpdateNumberOfPoints(Axes, solverData.CurrentPoints, ptSize, ptColor);
        }

        plotData.SetPositions(solverData.CurrentPoints);
    }
}

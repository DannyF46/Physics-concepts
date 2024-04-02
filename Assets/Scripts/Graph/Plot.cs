using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    // Start is called before the first frame update
    void Start()
    {
        //Assign parents
        AxesParent = Globals.Instance.AxesParent;
        TickParent = Globals.Instance.TickParent;
        GridlineParent = Globals.Instance.GridlineParent;
        PointsParent = Globals.Instance.DataParent;

        //initialize y-axis, ticks, and gridlines
        _yAxis = AxesParent.AddComponent<Axis>();
        _yAxis.MakeAxis(minValues.y,maxValues.y, Vector2.right);
        _yAxis.ZeroLine.tag = "yAxis";
        _yTicks = TickParent.AddComponent<Ticks>();
        _yTicks._Axis = _yAxis;
        _yTicks.SetTicks(numberOfTicks.y, Vector2.up);
        
        _yGridlines = GridlineParent.AddComponent<Gridlines>();
        _yGridlines._Axis = _yAxis;
        _yGridlines.SetGridlines(numberOfTicks.y, Vector2.up);

        //initialize x-axis, ticks, and gridlines
        _xAxis = AxesParent.AddComponent<Axis>();
        _xAxis.MakeAxis(minValues.x,maxValues.x, Vector2.up);
        _xAxis.ZeroLine.tag = "xAxis";
        _xTicks = TickParent.AddComponent<Ticks>();
        _xTicks._Axis = _xAxis;
        _xTicks.SetTicks(numberOfTicks.x, Vector2.right);

        _xGridlines = GridlineParent.AddComponent<Gridlines>();
        _xGridlines._Axis = _xAxis;
        _xGridlines.SetGridlines(numberOfTicks.x, Vector2.right);
        
        //Put axes into Axes list
        Axes[0] = _xAxis;
        Axes[1] = _yAxis;

        //sample: plot 2 sets of data
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
        DataPoint Points2 = new DataPoint(Axes, pts2, 0.025f, UnityEngine.Color.red); //instantiate and plot the points pts2
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
        }
        if(numOfPts != _Points.allPoints.Length)
        { 
            _Points.UpdateNumberOfPoints(Axes, Function(), 0.05f, Color.black);
        }
        
    }

    public Vector2[] Function() //whatever mathematical function one wants to plot. currently returns a sine wave
    {
        Vector2[] pts = new Vector2[numOfPts + 1];
        for (int i = 0; i < numOfPts + 1; i++)
        {
            float x = Mathf.Lerp(minValues.x, maxValues.x, (float)i / numOfPts);
            //float x = Mathf.Sin(t + Mathf.Sin(Time.realtimeSinceStartup));
            float y = Mathf.Sin(x - Mathf.Sin(Time.realtimeSinceStartup));

            pts[i] = new Vector2(x, y);
        }
        return pts;

    }
    
}

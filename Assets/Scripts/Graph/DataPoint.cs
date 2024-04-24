using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
//using static UnityEditor.PlayerSettings;

public class DataPoint //handles plotted points (want to eventually include a line plot class)
{
    [SerializeField] private GameObject dotPrototype; //prototype for datapoints
    [SerializeField] private GameObject DataParent; //parent for datapoints
    public GameObject[] allPoints; //list containing all the points

    public Axis[] Axes = new Axis[2]; //axes to plot on, given by Plot.cs 
    public Axis xAxis;
    public Axis yAxis;

    protected RectTransform containerTransform;
    public DataPoint() //only constructor
    {

    }

    public DataPoint(int numofPts) //only constructor that only specifies the number of points 
    {
        allPoints = new GameObject[numofPts];
        for (int i = 0; i < numofPts; i++)
        {
            allPoints[i] = CreatePoint();
        }
    }
    public DataPoint(Axis[] Axes, Vector2[] Points, float[] sizes, UnityEngine.Color[] colors) //constructor allowing for each point to have different sizes and colors
    {
        PlotPoints(Axes, Points, sizes, colors);
    }
    public DataPoint(Axis[] Axes, Vector2[] Points, float size, UnityEngine.Color color) //constructor for when all points have the same size and color
    {
        PlotPoints(Axes, Points, size, color);
    }

    public void PlotPoints(Axis[] Axes, Vector2[] Points, float[] sizes, UnityEngine.Color[] colors) //plots the points on the given axes (for when each point has a different size/color)
    {
        InitGameObjs(Axes);
        allPoints = new GameObject[Points.Length+1];
        for (int i=0; i < Points.Length; i++)
        {
            allPoints[i] = CreatePoint();
            SetParameters(allPoints[i], Points[i], sizes[i], colors[i]);
        }
    }
    public void PlotPoints(Axis[] Axes, Vector2[] Points, float size, UnityEngine.Color color)//plots the points on the given axes (for when each point has the same size/color)
    {
        InitGameObjs(Axes);
        allPoints = new GameObject[Points.Length];
        for (int i = 0; i < Points.Length; i++)
        {
            allPoints[i] = CreatePoint();
            SetParameters(allPoints[i], Points[i], size, color);
        }
    }
    public void InitGameObjs(Axis[] Axes) //get all the needed objects
    {
        DataParent = Globals.Instance.DataParent;
        dotPrototype = Globals.Instance.dotPrototype;
        containerTransform = Globals.Instance.containerRectTrans;
      
        xAxis = Axes[0];
        yAxis = Axes[1];

    }

    public GameObject CreatePoint()//spawns a point but doesnt do anything with it besides parent it 
    {
        GameObject pt = GameObject.Instantiate(dotPrototype, DataParent.transform);
        return pt;
    }
    public void SetParameters(GameObject pt, Vector2 pos, float size, UnityEngine.Color col) //set the position, size, and color of 1 datapoint
    {
        SetPosition(pt, pos);
        SetScale(pt, size);
        SetColor(pt, col);        
    }

    public void SetPosition(GameObject pt, Vector2 pos) //sets position of 1 datapoint
    {
        pt.transform.position = Globals.Instance.PositionFromAxisSpace(pos);
    }

    public void SetPositions(Vector2[] pos) //set the position for every datapoint (i.e. for animation or plotting a time-dependant function)
    {
        for(int i = 0; i < allPoints.Length; i++)
        {
            allPoints[i].transform.position = Globals.Instance.PositionFromAxisSpace(pos[i]);
        }  
    }
    public Vector2[] GetPositions()
    {
        Vector2[] CurrentPoints = new Vector2[allPoints.Length];
        for (int i = 0; i < allPoints.Length; i++)
        {
            CurrentPoints[i] = allPoints[i].transform.position;
        }
        return CurrentPoints;
    }
    public void SetScale(GameObject pt, float size) //set the size for 1 points
    {
        pt.transform.localScale = size*Vector2.one ;
    }
    public void SetScales(float size) //set the size for all points
    {
        for (int i = 0; i < allPoints.Length; i++)
        {
            SetScale(allPoints[i], size);
        }
        
    }
    public void SetColor(GameObject pt, UnityEngine.Color col) //set color for 1 point
    {
        Image dotimage = pt.GetComponent<Image>();
        dotimage.color = col;
    }
    public void SetColors (UnityEngine.Color col) //set the size for all points
    {
        for (int i = 0; i < allPoints.Length; i++)
        {
            SetColor(allPoints[i], col);
        }

    }
    public void UpdateNumberOfPoints(Axis[] Axes, Vector2[] Points, float size, UnityEngine.Color color) //if the number of points to plot changes at runtime, clear the old points and make the new ones
    {
        ClearPoints();
        PlotPoints(Axes, Points, size, color);
    }
    public void ClearPoints() //if the number of points to plot changes at runtime, clear the old points and make the new ones
    {
        for (int i = 0; i < allPoints.Length; i++)
        {
            GameObject.Destroy(allPoints[i]);
            
        }

    }
}

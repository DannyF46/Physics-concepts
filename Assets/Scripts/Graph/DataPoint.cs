using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class DataPoint
{
    [SerializeField] private GameObject dotPrototype;
    private GameObject singlePoint;
    public GameObject[] allPoints;
    [SerializeField] private GameObject DataParent;
    [SerializeField] private float size;
    [SerializeField] private UnityEngine.Color color;
    [SerializeField] private Vector2 position;
    private Vector3[] canvasCorners;
    private xAxis _xAxis;
    private yAxis _yAxis;
    private float xoffset = 0;
    private float yoffset = 0;
    private float xScaleFactor;
    private float yScaleFactor;
    private float xRange;
    private float yRange;
    private float containerWidth;
    private float containerHeight;
    protected RectTransform containerTransform;
    // Start is called before the first frame update
    void Start()
    {
        //CreatePoint();
        //SetParameters(singlePoint, position, size, color);
    }


    // Update is called once per frame
    void Update()
    {/*
        if(Points.Length!= 0)
        {
            if (Points[Points.Length - 1].transform.position.x != position.x || Points[Points.Length - 1].transform.position.y != position.y)
            {
                Points[0].transform.position = position;
            }
        }*/

    }


    public DataPoint(int numofPts)
    {
        allPoints = new GameObject[numofPts];
        for (int i = 0; i < numofPts; i++)
        {
            allPoints[i] = CreatePoint();

        }
    }
    public DataPoint(Vector2[] Points, float[] sizes, UnityEngine.Color[] colors)
    {

        //CreatePoint();
        //SetParameters(singlePoint, position, size, color);
        PlotPoints(Points, sizes, colors);
    }
    public DataPoint(Vector2[] Points, float size, UnityEngine.Color color)
    {

        //CreatePoint();
        //SetParameters(singlePoint, position, size, color);
        PlotPoints(Points, size, color);
    }

    public void PlotPoints(Vector2[] Points, float[] sizes, UnityEngine.Color[] colors)
    {
        InitGameObjs();
        allPoints = new GameObject[Points.Length];
        for (int i=0; i < Points.Length; i++)
        {
            allPoints[i] = CreatePoint();
            SetParameters(allPoints[i], Points[i], sizes[i], colors[i]);
        }
    }
    public void PlotPoints(Vector2[] Points, float size, UnityEngine.Color color)
    {
        InitGameObjs();
        allPoints = new GameObject[Points.Length];
        for (int i = 0; i < Points.Length; i++)
        {
            allPoints[i] = CreatePoint();
            SetParameters(allPoints[i], Points[i], size, color);
        }
    }
    public void InitGameObjs()
    {
        canvasCorners = new Vector3[4];
        DataParent = GameObject.FindGameObjectWithTag("DataPoints");
        dotPrototype = GameObject.FindGameObjectWithTag("datapointPrototype");
        containerTransform = GameObject.FindGameObjectWithTag("GraphContainer").GetComponent<RectTransform>();
        containerTransform.GetWorldCorners(canvasCorners); //corners gives rendered worldspace points; width or sizedelta give screenspace (local?)
        _xAxis = GameObject.FindGameObjectWithTag("Axes").GetComponent<xAxis>();
        _yAxis = GameObject.FindGameObjectWithTag("Axes").GetComponent<yAxis>();
        RefreshParams();


    }
    public void RefreshParams()
    {
        xRange = _xAxis.maxValue - _xAxis.minValue;
        yRange = _yAxis.maxValue - _yAxis.minValue;
        containerWidth = canvasCorners[2].x - canvasCorners[0].x;
        containerHeight = canvasCorners[1].y - canvasCorners[0].y;
        xScaleFactor = containerWidth / xRange;
        yScaleFactor = containerHeight / yRange;
    }
    public GameObject CreatePoint()
    {
        Debug.Log(dotPrototype);
        dotPrototype.SetActive(true) ;
        //InitGameObjs();
        GameObject pt = GameObject.Instantiate(dotPrototype, DataParent.transform);
        dotPrototype.SetActive(false);
        return pt;
    }
    public void SetParameters(GameObject pt, Vector2 pos, float size, UnityEngine.Color col)
    {
        //if(singlePoint != null)
        {
            RefreshParams();
            SetPosition(pt, pos);
            SetScale(pt, size);
            SetColor(pt, col);
        }
        
    }
    public void SetPosition(GameObject pt, Vector2 pos)
    {
        pt.transform.position = PositionFromGraphSpace(pos);
    }
    public void SetPositions(GameObject[] pt, Vector2[] pos)
    {
        for(int i = 0; i < pt.Length; i++)
        {
            pt[i].transform.position = PositionFromGraphSpace(pos[i]);
        }
        
    }
    public void SetScale(GameObject pt, float size)
    {
        pt.transform.localScale = size*Vector2.one ;
    }
    public void SetColor(GameObject pt, UnityEngine.Color col)
    {
        Image dotimage = pt.GetComponent<Image>();
        dotimage.color = col;
    }

    public Vector2 PositionFromGraphSpace(Vector2 pos)
    {
        Vector2 offsetL = canvasCorners[0];
        Vector2 offsetR = canvasCorners[1];
        Vector2 mins = new Vector2(_xAxis.minValue, _yAxis.minValue);
        Vector2 maxs = new Vector2(_xAxis.maxValue, _yAxis.maxValue);
        Vector2 scaleFactors = new Vector2(xScaleFactor, yScaleFactor);
        //Vector2 finalpos2 = pos - _xAxis.minValue; 
        
        Vector2 finalPosL = offsetL + Vector2.Scale((pos - mins), scaleFactors);
        Vector2 finalPosR = offsetR + Vector2.Scale((pos + maxs), scaleFactors);
        Debug.Log((finalPosL, finalPosR));
        return finalPosL;
    }
}

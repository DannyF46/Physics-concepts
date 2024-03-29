using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Plot : MonoBehaviour
{
    public xAxis _xAxis;
    public yAxis _yAxis;
    public Gridlines _Gridlines;
    public DataPoint _Points;

    [SerializeField] public float xmin = 0;
    [SerializeField] public float xmax = 10;
    [SerializeField] public int xTicks = 10;
    [SerializeField] public float ymin = 0;
    [SerializeField] public float ymax = 10;
    [SerializeField] public int yTicks = 10;
    [SerializeField] public int numOfPts = 10;

    // Start is called before the first frame update
    void Start()
    {
        _xAxis = GameObject.FindGameObjectWithTag("Axes").GetComponent<xAxis>();
        _yAxis = GameObject.FindGameObjectWithTag("Axes").GetComponent<yAxis>();
        _Gridlines =  GameObject.FindGameObjectWithTag("Gridlines").GetComponent<Gridlines>();
        //_Points = new DataPoint(numOfPts);
        /*Vector2[] pts = new Vector2[40];
        Vector2[] pts2 = new Vector2[40];
        for (int i = 0; i < 40; i++)
        {
            float x = Mathf.Lerp(-xmin, xmax, (float)i / 40);
            float y = Mathf.Sin(x);
            pts[i] = new Vector2(x, y);
            pts2[i] = new Vector2(x, 2*y);
        }
        _Points = new DataPoint(pts, 0.05f, UnityEngine.Color.black);*/
        //DataPoint _Points2 = new DataPoint(pts2, 0.04f, UnityEngine.Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        if(_xAxis.labelCount != xTicks || _xAxis.minValue != xmin || _xAxis.maxValue != xmax)
        {
            _xAxis.UpdateAxis(xmin, xmax, xTicks, Vector2.right);
        }
       

        Vector2[] pts = new Vector2[numOfPts];
        for (int i = 0; i < numOfPts; i++)
        {
            float t = Mathf.Lerp(-xmin, xmax, (float)i / numOfPts);
            float x = Mathf.Sin(t - 5 * Time.realtimeSinceStartup);
            float y = Mathf.Sin(t - 10*Time.realtimeSinceStartup);
            pts[i] = new Vector2(x, y);

        }
        if (_Points == null || _Points.allPoints.Length != numOfPts)
        {
            _Points = new DataPoint(pts, 0.05f, Color.black);
        }
        else
        {
            _Points.SetPositions(_Points.allPoints,pts);
        }
    }
}

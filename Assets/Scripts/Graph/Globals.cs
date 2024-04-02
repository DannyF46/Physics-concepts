using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[InitializeOnLoad]
public class Globals : MonoBehaviour //make some frequently-used objects/methods globally accessable (so we don't need to constantly re-call things like FindGameObjectWithTag)
{

    public GameObject _Container;
    public GameObject gridlinePrototype, tickPrototype, tickLabelPrototype, dotPrototype; //prototypes
    public GameObject AxesParent, GridlineParent, TickParent, TickLabelParent, DataParent; //parents
    public static Globals Instance; //this instance, access public properties/methods using Global.Instance.XYZ

    public Vector3[] containerCorners = new Vector3[4]; //worldspace coordinates of the corners of the graphcontainer
    public Vector2 containerSize = new Vector2(); //worldspace x and y dimensions of the graphcontainer
    public Vector2 containerOffset = new Vector2(); //worldspace coordinates of corner[0] (bottom left)
    public Vector2 containerCenter = new Vector2(); //worldspace coorrdindates of the center of the container
    public RectTransform containerRectTrans; //RectTransform of the container
    public Plot plot; //component on PlotMaster gameobject

    void Awake() //Awake() runs before Start(). We need Globals to run before attempting to make a plot
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        GetParents();

        GetPrototypes();

        _Container = GameObject.FindGameObjectWithTag("GraphContainer"); 
        containerRectTrans = _Container.GetComponent<RectTransform>();
        _Container.GetComponent<RectTransform>().GetWorldCorners(containerCorners);
        containerSize = new Vector2(containerCorners[2].x - containerCorners[0].x, containerCorners[1].y - containerCorners[0].y);
        containerOffset = containerCorners[0];
        containerCenter = containerOffset + containerSize / 2;
    }

    public Vector2 PositionFromAxisSpace(Vector2 pos) //using the user-specified plotting range (from the Plot component on the PlotMaster gameobject),
                                                      //determine the world-space coordinates corresponding to the inputted graph-space position. Does not need Axes to be generated first
    {
        
        Vector3[] canvasCorners = new Vector3[4];
        _Container.GetComponent<RectTransform>().GetWorldCorners(canvasCorners);
        Vector2 offset = canvasCorners[0];
        Vector2 Range = plot.maxValues - plot.minValues;
        Vector2 scaleFactor = new Vector2(containerSize.x / Range.x, containerSize.y / Range.y);
        Vector2 finalpos;
            
        finalpos = offset + Vector2.Scale((pos - plot.minValues), scaleFactor);

        return finalpos;
    }
    
    public void GetPrototypes()//get all the prototypes and assign them to variables
    {
        gridlinePrototype = GameObject.FindGameObjectWithTag("gridlinePrototype");
        tickPrototype = GameObject.FindGameObjectWithTag("tickPrototype");
        tickLabelPrototype = GameObject.FindGameObjectWithTag("tickLabelPrototype");
        dotPrototype = GameObject.FindGameObjectWithTag("datapointPrototype");
    }
    public void GetParents()//get all the parents and assign them to variables
    {
        AxesParent = GameObject.FindGameObjectWithTag("Axes");
        GridlineParent = GameObject.FindGameObjectWithTag("Gridlines");
        TickParent = GameObject.FindGameObjectWithTag("Ticks");
        TickLabelParent = GameObject.FindGameObjectWithTag("TickLabels");
        DataParent = GameObject.FindGameObjectWithTag("DataPoints");
    }
}

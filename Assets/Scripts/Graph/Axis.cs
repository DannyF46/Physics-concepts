using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Axis : MonoBehaviour //responsible for managing axes (bounds, x=0 and y=0 lines). Would like to allow for log plots as well
{

    [SerializeField] protected GameObject gridlinePrototype; //Uses the gridline prototype to create the axis lines

    [SerializeField] protected GameObject axisLabel; //axis labeling (TBD)
    protected TMP_Text axisLabelText;

    protected GameObject AxesParent; //where to parent the axes
    public GameObject ZeroLine; //the axis line
    public RectTransform containerTransform;
    public Vector2 containerCenter;

    public float minValue = 0; //the bounds of the axis
    public float maxValue = 10;

    public void SetLabel(string label) //sets axis label (TBD)
    {
        axisLabelText = axisLabel.GetComponent<TMP_Text>();
        axisLabelText.text = label;
    }
  
    public void MakeAxis(float min, float max, Vector2 axis) //creates an x- or y-axis, depending on axis, with the given min and max
    {
        minValue = min; //set the properties to be whatever the inputted bounds are
        maxValue = max;
        GetObjects();

        Vector2 Origin = Globals.Instance.PositionFromAxisSpace(Vector2.zero); //Find (0,0) in worldspace

        RectTransform containerTransform = Globals.Instance.containerRectTrans;
        Vector2 containerCenter = Globals.Instance.containerCenter;
        
        
        if (axis == Vector2.right || axis == Vector2.left) //generate y-axis (x=0 line) [i want this to be consistent with the rest of the class, so need to make it so this makes the x-axis (y=0 line)]
        {
            Vector2 x0line = new Vector3(Origin.x, containerCenter.y, 0);
            ZeroLine = Instantiate(gridlinePrototype, x0line, gridlinePrototype.transform.rotation, AxesParent.transform);
            ZeroLine.GetComponent<RectTransform>().sizeDelta = new Vector2(3, containerTransform.sizeDelta.y);
        }
        else if (axis == Vector2.up || axis == Vector2.down) //generate x-axis (y=0 line) [...]
        {
            Vector2 y0line = new Vector3(containerCenter.x, Origin.y, 0);
            ZeroLine = Instantiate(gridlinePrototype, y0line, Quaternion.Euler(0,0,90), AxesParent.transform);
            ZeroLine.GetComponent<RectTransform>().sizeDelta = new Vector2(3, containerTransform.sizeDelta.x);
        }

        ZeroLine.GetComponent<Image>().color = Color.black; //set the coordinate axis black

    }
    public void UpdateAxis(float min, float max, Vector2 direction) // deletes current axis and creates a new one with the specified min/max
    {
        Destroy(ZeroLine);
        MakeAxis(min, max, direction);
    }
    public virtual void LinearArrayAlongAxis(int N, GameObject graphElementToPlace, Vector2 direction, GameObject[] elementList, GameObject parent) //distributes N graphElements along the given direction,
                                                                                                                                                    //saving them to specified elementList and parenting them
                                                                                                                                                    //to the specified parent 
    {
        Vector2 containerSize = Globals.Instance.containerSize;
        Vector2 offset = Globals.Instance.containerOffset;
        RectTransform elementRectTrans = graphElementToPlace.GetComponent<RectTransform>();  
        float xSpacing = containerSize.x / N;
        float ySpacing = containerSize.y / N;
        Vector2 pos;

        if(direction == Vector2.right) //distributes in x direction
        {
            for (int i = 0; i < N + 1; i++)
            {
                pos = offset + new Vector2((float)i * xSpacing, elementRectTrans.sizeDelta.y / 2);

                elementList[i] = Instantiate(graphElementToPlace, pos, Quaternion.identity);
                elementList[i].transform.SetParent(parent.transform);
            }
        }
        else if(direction == Vector2.up) //y direction
        {
            for (int i = 0; i < N + 1; i++)
            {
                pos = offset + new Vector2(elementRectTrans.sizeDelta.y / 2, (float)i * ySpacing);

                elementList[i] = Instantiate(graphElementToPlace, pos, Quaternion.Euler(0,0,90));
                elementList[i].transform.SetParent(parent.transform);
            }
        }
       
    }
    public void GetObjects() //note to self: i think these can be declared when intitiliazing properties
    {
        AxesParent = Globals.Instance.AxesParent;
        gridlinePrototype = Globals.Instance.gridlinePrototype;
        containerTransform = Globals.Instance.containerRectTrans;
        containerCenter = Globals.Instance.containerCenter;

    }
}

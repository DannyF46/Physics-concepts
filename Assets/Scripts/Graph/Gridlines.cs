using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridlines : MonoBehaviour //handles gridlines
{
    [SerializeField] protected GameObject gridlinePrototype; //prototype to be instantiated as gridlines

    protected GameObject[] gridlines; //list storing each line
    protected GameObject GridlineParent; //parent for gridlines
    protected RectTransform containerTransform; 

    [SerializeField] int numHorizLines = 10;
    [SerializeField] int numVertLines = 10;

    public Axis _Axis; //the axis along which the gridlines will be distributed (specified in Plot.cs)


    public void SetGridlines(int numOfGridlines, Vector2 direction) //creates the specified number of gridlines along the specified direction
    {
        GetObjects();

        gridlines = new GameObject[numOfGridlines+1];
        Vector2 containerCenter = Globals.Instance.containerCenter;
        _Axis.LinearArrayAlongAxis(numOfGridlines, gridlinePrototype, direction, gridlines, GridlineParent); //distribute lines along specified axis 

        //LinearArrayAlongAxis was made for gridlines/axes, so placements for other objects need to be adjusted
        //as well, since one prototype is used for either direction of gridline, we must adjust the size accordingly
        for (int i = 0; i < numOfGridlines+1; i++)
        {
            if (direction == Vector2.right) //gridlines along the x-axis (i.e. veritcal gridlines)
            {
                gridlines[i].transform.position = new Vector2(gridlines[i].transform.position.x, containerCenter.y); 
                gridlines[i].GetComponent<RectTransform>().sizeDelta = new Vector2(gridlines[i].GetComponent<RectTransform>().sizeDelta.x, Globals.Instance.containerSize.y); 
            }
            else if (direction == Vector2.up)//gridlines along y-axis (horizontal gridlines)
            {
                gridlines[i].transform.position = new Vector2(containerCenter.x, gridlines[i].transform.position.y);
                gridlines[i].GetComponent<RectTransform>().sizeDelta = new Vector2(gridlines[i].GetComponent<RectTransform>().sizeDelta.x, Globals.Instance.containerSize.x);
            }
            
        }

    }
   
    public void UpdateGridlines(int numofGridlines, Axis axis, Vector2 dir) //update whe parameters change. _Axis may not need to be re-specified here. Would like to make it so gridlines/ticks
                                                                            //move with the axis while changing the plotting range (i.e. lock ticks/gridlines to 'nice' numbers that move while changing the 
                                                                            //plotting range, rather than locking ticks/lines to a location and changing the number while changing plotting range)
    { 
        for (int i = 0;i < gridlines.Length;i++)
        {
            Destroy(gridlines[i]);
        }
        _Axis = axis;
        SetGridlines(numofGridlines, dir);
    }
    
    public void GetObjects()
    {
        containerTransform = Globals.Instance.containerRectTrans;
        gridlinePrototype = Globals.Instance.gridlinePrototype;
        GridlineParent = Globals.Instance.GridlineParent;

    }
}

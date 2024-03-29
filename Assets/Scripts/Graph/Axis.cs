using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class Axis : MonoBehaviour
{
    [SerializeField] protected GameObject tickPrototype, tickLabelPrototype, gridlinePrototype, axisLabel;
    protected GameObject[] ticks, ticklabels, gridlines;
    protected TMP_Text axisLabelText;
    protected RectTransform containerTransform;

    protected GameObject AxesParent, GridlineParent, TickParent, TickLabelParent;
    protected GameObject ZeroLine;

    [SerializeField] public int labelCount = 10;
    protected float defaultMinValue = 0f;
    protected float defaultMaxValue = 10f;
    [SerializeField] public float minValue = 0;
    [SerializeField] public float maxValue = 0;
    private float Range;
    public virtual void Start()
    {
        containerTransform = GameObject.FindGameObjectWithTag("GraphContainer").GetComponent<RectTransform>();
        AxesParent = GameObject.FindGameObjectWithTag("Axes");
        GridlineParent = GameObject.FindGameObjectWithTag("Gridlines");
        TickParent = GameObject.FindGameObjectWithTag("Ticks");
        TickLabelParent = GameObject.FindGameObjectWithTag("TickLabels");
        
        //x-axis
        //SetTicks(labelCount,Vector2.right);
        //SetTickLabels(labelCount, Vector2.right);
        //y-axis
        //SetTicks(labelCount, Vector2.up);
        //SetTickLabels(labelCount, Vector2.up);
    }

    public void SetLabel(string label)
    {
        axisLabelText = axisLabel.GetComponent<TMP_Text>();
        axisLabelText.text = label;
    }
    
    public void UpdateAxis(float min, float max, int numOfLabels, Vector2 direction)
    {
        ClearAxis();
        minValue = min;
        maxValue = max;
        labelCount = numOfLabels;
        MakeAxis(labelCount,direction);

        /*
        if (minValue == maxValue)
        {
            minValue = defaultMinValue;
            maxValue = defaultMaxValue;
            Range = maxValue - minValue;
        }*/

    }
    public void SetTicks(int numofTicks, Vector2 direction)
    {
        ticks = new GameObject[numofTicks + 1];
        tickPrototype.SetActive(true);
        LinearArrayAlongAxis(numofTicks, tickPrototype, direction, ticks, TickParent);
        tickPrototype.SetActive(false);
    }
   /* public void UpdateTickLabels(float min, float max, int numofTicks, Vector2 direction)
    {
        

        tickPrototype.SetActive(false);
    }*/
    public void MakeAxis(int numOfLabels, Vector2 axis)
    {

        gridlinePrototype.SetActive(true);
        tickLabelPrototype.SetActive(true);
        tickPrototype.SetActive(true);
        SetTicks(numOfLabels, axis);
        SetTickLabels(numOfLabels, axis);
        if (axis == Vector2.right || axis == Vector2.left)
        {
            ZeroLine = Instantiate(gridlinePrototype, new Vector3(PositionFromAxisSpace(0, axis), gridlinePrototype.transform.position.y, 0), Quaternion.identity, AxesParent.transform);
        }
        else if (axis == Vector2.up || axis == Vector2.down)
        {
            ZeroLine = Instantiate(gridlinePrototype, new Vector3(gridlinePrototype.transform.position.x, PositionFromAxisSpace(0, axis), 0), gridlinePrototype.transform.rotation, AxesParent.transform);
        }
        ZeroLine.GetComponent<RectTransform>().sizeDelta = new Vector2(3, ZeroLine.GetComponent<RectTransform>().sizeDelta.y);
        ZeroLine.GetComponent<Image>().color = Color.black;

        gridlinePrototype.SetActive(false);
        tickLabelPrototype.SetActive(false);
        tickPrototype.SetActive(false);
    }
    public void ClearAxis()
    {
        for(int i = 0; i<ticklabels.Length; i++)
        {
            Destroy(ticklabels[i]);
            Destroy(ticks[i]);
            Destroy(ZeroLine);
        }
        
    }

    public void SetTickLabels(int numOfLabels, Vector2 direction)
    {
        ticklabels = new GameObject[numOfLabels + 1];
        LinearArrayAlongAxis(numOfLabels, tickLabelPrototype, direction,  ticklabels, TickLabelParent);
        float DeltaX = (maxValue - minValue) / numOfLabels;

        for (int i = 0; i < numOfLabels + 1; i++)
        {
            string label = (minValue + i * DeltaX).ToString();
            ticklabels[i].GetComponent<TMP_Text>().text = label;
        }
        tickLabelPrototype.SetActive(false);
    }

    public void LinearArrayAlongAxis(int N, GameObject graphElementToPlace, Vector2 direction, GameObject[] elementList, GameObject parent)
    {
        Vector3[] canvasCorners = new Vector3[4];
        containerTransform.GetWorldCorners(canvasCorners); //corners gives rendered worldspace points; width or sizedelta give screenspace (local?)
        float containerWidth = canvasCorners[2].x - canvasCorners[0].x;
        float containerHeight = canvasCorners[1].y - canvasCorners[0].y;
        float xSpacing = containerWidth / N;
        float ySpacing = containerHeight / N;
        float posx;
        float posy;
        if(direction == Vector2.right || direction == Vector2.left) //x direction
        {
            for (int i = 0; i < N + 1; i++)
            {
                PositionFromAxisSpace(i , direction);
                posx = (containerTransform.transform.position.x - containerWidth / 2 + (float)i * xSpacing);
                posy = (graphElementToPlace.transform.position.y);
                elementList[i] = Instantiate(graphElementToPlace);
                elementList[i].transform.SetParent(parent.transform, false);
                elementList[i].transform.position = new Vector2(posx, posy);
            }
        }
        else if(direction == Vector2.up) //y direction
        {
            for (int i = 0; i < N + 1; i++)
            {
                posy = (containerTransform.transform.position.y - containerHeight / 2 + (float)i * ySpacing);
                posx = (graphElementToPlace.transform.position.x);
                elementList[i] = Instantiate(graphElementToPlace);
                elementList[i].transform.SetParent(parent.transform, false);
                elementList[i].transform.position = new Vector2(posx, posy);

            }
        }
       
    }
    public float PositionFromAxisSpace(float pos, Vector2 dir)
    {
        Vector3[] canvasCorners = new Vector3[4];
        containerTransform.GetWorldCorners(canvasCorners);
        Vector2 offset = canvasCorners[0];
        Range = maxValue - minValue;
        float finalpos;

        if (dir == Vector2.up)
        {
            float containerHeight = canvasCorners[1].y - canvasCorners[0].y;
            float scaleFactor = containerHeight / Range;

            finalpos = offset.y + (pos - minValue) * scaleFactor;
        }
        else if (dir == Vector2.right)
        {
            float containerWidth = canvasCorners[2].x - canvasCorners[0].x;
            float scaleFactor = containerWidth / Range;
            Debug.Log(offset.x + ", " + pos + ", " + minValue + ", " + scaleFactor);
            finalpos = offset.x + (pos - minValue) * scaleFactor;

        }
        else
        {
            finalpos = pos;
        }
       
        return finalpos;
    }

}

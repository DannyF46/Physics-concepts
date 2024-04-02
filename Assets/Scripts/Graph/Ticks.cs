using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ticks : MonoBehaviour //generates the ticks and ticklabels that label points on the graph 
{
    [SerializeField] protected GameObject tickPrototype, tickLabelPrototype; //prototypes

    protected GameObject[] ticks, ticklabels; //lists that store the tick/ticklabel objects, respectively

    protected GameObject TickParent, TickLabelParent; //Objects to parent the ticks and ticklabels to

    public Axis _Axis; //the axis along which the ticks will be distributed (specified in Plot.cs)

    [SerializeField] public int tickCount = 10; //default number of ticks


    public void GetObjects() //gets the necessary parents and prototypes
    {
        tickPrototype = Globals.Instance.tickPrototype;
        tickLabelPrototype = Globals.Instance.tickLabelPrototype;
        TickParent = Globals.Instance.TickParent;
        TickLabelParent = Globals.Instance.TickLabelParent;
    }
    public void SetTicks(int numofTicks, Vector2 direction) //Creates the ticks and their labels
    {
        GetObjects();//make sure parents/prototoypes have been assigned before making the ticks

        ticks = new GameObject[numofTicks + 1];
        _Axis.LinearArrayAlongAxis(numofTicks, tickPrototype, direction, ticks, TickParent); //distribute ticks along the specified axis
        SetTickLabels(numofTicks, direction);
        tickCount = numofTicks; //set tickCount property

    }
    public void SetTickLabels(int numOfLabels, Vector2 direction) //Assigns ticks a numeric label
    {
        ticklabels = new GameObject[numOfLabels + 1];
        _Axis.LinearArrayAlongAxis(numOfLabels, tickLabelPrototype, direction, ticklabels, TickLabelParent);//distribute ticks along the specified axis
        float DeltaX = (_Axis.maxValue - _Axis.minValue) / numOfLabels; //spaceing between labels

        for (int i = 0; i < numOfLabels + 1; i++)
        {
            string label = (_Axis.minValue + i * DeltaX).ToString(); 
            ticklabels[i].GetComponent<TMP_Text>().text = label; //set label text
            ticklabels[i].GetComponent <TMP_Text>().fontSize = 32; //set label size (should make customizable, TBD)

            //LinearArrayAlongAxis was made for gridlines/axes, so placements for other objects need to be adjusted:
            if (direction == Vector2.right) //along the x-axis
            {
                Vector2 dist = ticklabels[i].transform.position - Globals.Instance.containerCorners[0];
                ticklabels[i].transform.position += new Vector3(0, -2f * dist.y,0); 
            }
            else if (direction == Vector2.up)//along y-axis
            {
                Vector2 dist = ticklabels[i].transform.position - Globals.Instance.containerCorners[0];
                ticklabels[i].transform.position += new Vector3(-2f*dist.x, 0, 0);
            }
        }
    }
    public void UpdateTicks(int numOfLabels, Axis axis, Vector2 direction) //updates ticks if parameters change
    {
        for (int i = 0; i < ticks.Length; i++)
        {
            Destroy(ticks[i]);
            Destroy(ticklabels[i]);
        }
        _Axis = axis;
        SetTicks(numOfLabels, direction);

    }
    
}

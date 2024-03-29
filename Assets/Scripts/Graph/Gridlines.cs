using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gridlines : MonoBehaviour
{
    [SerializeField] protected GameObject xgridlinePrototype;
    [SerializeField] protected GameObject ygridlinePrototype;
    protected GameObject[] gridlines;
    protected GameObject GridlineParent;
    protected RectTransform containerTransform;

    [SerializeField] int numHorizLines = 10;
    [SerializeField] int numVertLines = 10;

    private xAxis _xAxis;
    private yAxis _yAxis;
    // Start is called before the first frame update
    void Start()
    {
        containerTransform = GameObject.FindGameObjectWithTag("GraphContainer").GetComponent<RectTransform>();
        GridlineParent = GameObject.FindGameObjectWithTag("Gridlines");
        SetGridlines(numHorizLines, 0);
        SetGridlines(numVertLines, 1);
    }
    
    public void SetGridlines(int numOfGridlines, int direction)
    {
        gridlines = new GameObject[numOfGridlines + 1];
        if(direction == 0)
        {
            LinearArrayAlongAxis(numOfGridlines, xgridlinePrototype, gridlines, GridlineParent, direction);
            xgridlinePrototype.SetActive(false);
        }
        else if (direction == 1)
        {
            LinearArrayAlongAxis(numOfGridlines, ygridlinePrototype, gridlines, GridlineParent, direction);
            ygridlinePrototype.SetActive(false);
        }

    }

    public void LinearArrayAlongAxis(int N, GameObject graphElementToPlace, GameObject[] elementList, GameObject parent, int direction)
    {
        Vector3[] canvasCorners = new Vector3[4];
        containerTransform.GetWorldCorners(canvasCorners); //corners gives rendered worldspace points; width or sizedelta give screenspace (local?)
        float containerWidth = canvasCorners[2].x - canvasCorners[0].x;
        float containerHeight = canvasCorners[1].y - canvasCorners[0].y;
        float xSpacing = containerWidth / N;
        float ySpacing = containerHeight / N;
        float posx;
        float posy;
        if (direction == 0) //x direction
        {
            for (int i = 0; i < N + 1; i++)
            {
                posx = (containerTransform.transform.position.x - containerWidth / 2 + (float)i * xSpacing);
                posy = (graphElementToPlace.transform.position.y);
                elementList[i] = Instantiate(graphElementToPlace);
                elementList[i].transform.SetParent(parent.transform, false);
                elementList[i].transform.position = new Vector2(posx, posy);
            }
        }
        else if (direction == 1) //y direction
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
}

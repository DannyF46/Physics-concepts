using MathNet.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UniversalGraviation : MonoBehaviour
{

    [SerializeField] private float gravCoupling = 1;
    [SerializeField] private Rigidbody[] bodies;
    [SerializeField] private float xSpacing = 1;
    [SerializeField] private float ySpacing = 1;
    [SerializeField] private float zSpacing;
    [SerializeField] private float spacingOffsetScale = 2;
    [SerializeField] private float initialVelocityScale = 1f;
    [SerializeField] private float massScale = 1f;

    // Start is called before the first frame update
    void Start()
    {

        bodies = GameObject.FindObjectsOfType<Rigidbody>();
        int S = Mathf.FloorToInt(Mathf.Pow(bodies.Length,1f/3f));
        int bodyNum = 0;
        Vector3[,,] points = new Vector3[S,S,S];
        Vector3[] grid = new Vector3[bodies.Length];
       
        for (int i = 0; i < S; i++)
        {
            for (int j = 0; j < S; j++)
            {
                for (int k = 0; k < S; k++)
                {
                    float spaceOffset = spacingOffsetScale*Random.value;
                    points[i,j,k] += new Vector3(i*xSpacing, j*ySpacing, k*zSpacing) + spaceOffset * Random.insideUnitSphere;
                    
                    Rigidbody body = bodies[bodyNum];
                    //body.position = grid[i];
                    body.position = points[i, j, k];
                    body.mass *= massScale * Random.value;
                    body.velocity += initialVelocityScale * Random.insideUnitSphere;
                    bodyNum++;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // for each rigidbody, calculate and apply the force
        //bodies = GameObject.FindObjectsOfType<Rigidbody>();

        for (int i = 0; i < bodies.Length; i++)
        {
            Rigidbody body = bodies[i];

            for (int j = 0; j < bodies.Length; j++)
            {
                if (body == bodies[j])
                {

                }
                else
                {
                    Vector3 itoj = (bodies[j].position - body.position).normalized;
                    Vector3 force = gravCoupling * body.mass * bodies[j].mass / ((bodies[j].position - body.position).sqrMagnitude) * itoj;
                    body.AddForce(force);
                }
            }
        }
    
    
    }

    private Vector3 Fnet()
    {
        Vector3 Force = new Vector3();

        return Force;
    }
}

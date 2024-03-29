using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

public class ColisionManager : MonoBehaviour
{
    private GameObject[] collisionObjects;
    [SerializeField] public Rigidbody2D[] bodies;
    [SerializeField] public float[] masses;
    [SerializeField] public Vector2[] velocities;

    [SerializeField] private float[] massInput;
    [SerializeField] private Vector2[] velocityInput;

    [SerializeField] private CoM CenterofMass;
    [SerializeField] private bool CoMFrame = false;
    [SerializeField] private FollowCoM camFollowCoM;
    private List<Vector2> bodiesStartPos = new List<Vector2>();
    private Vector2 CoMVel;

    // Start is called before the first frame update
    void Start()
    {
        collisionObjects = GameObject.FindGameObjectsWithTag("CollisionObject");
        bodies = new Rigidbody2D[collisionObjects.Length];
        //bodies = FindObjectsOfType<Rigidbody2D>();
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i] = collisionObjects[i].GetComponent<Rigidbody2D>();
            bodiesStartPos.Add(bodies[i].position);
        }

    }

    // Update is called once per frame
    void Update()
    {
        collisionObjects = GameObject.FindGameObjectsWithTag("CollisionObject");
        bodies = new Rigidbody2D[collisionObjects.Length];
        for (int i = 0 ; i < bodies.Length ; i++)
        {
            bodies[i] = collisionObjects[i].GetComponent<Rigidbody2D>();
        }

        if(velocityInput.Length != bodies.Length || massInput.Length != bodies.Length)
        {
            velocityInput = new Vector2[bodies.Length];
            massInput = new float[bodies.Length];
            masses = new float[bodies.Length];            

        }
        for (int i = 0 ; i < massInput.Length ; i++)
        {

            if (massInput[i] != masses[i] && massInput[i] != 0)
            {
                velocities[i] = masses[i] * velocities[i]/massInput[i];
                bodies[i].velocity = velocities[i];
                bodies[i].mass = massInput[i];
            }
            masses[i] = bodies[i].mass;
        }
        velocities = new Vector2[bodies.Length];
        CoMVel = CenterofMass.CenterofMassVelocity;

        if (CoMFrame)
        {
            camFollowCoM.followCenterOfMass = true;
            for (int i = 0; i < bodies.Length; i++)
            {
                velocities[i] = bodies[i].velocity - CoMVel;
            }
        }
        else if (!CoMFrame)
        {
            camFollowCoM.followCenterOfMass = false;
            for (int i = 0; i < bodies.Length; i++)
            {
                velocities[i] = bodies[i].velocity;
            }
        }

    }


    public void FollowButton()
    {

        if (CoMFrame)
        {
            CoMFrame = false;
            camFollowCoM.followCenterOfMass = false;
        }
        else if (!CoMFrame)
        {
            CoMFrame = true;
            camFollowCoM.followCenterOfMass = true;
        }
    }

    public void SetVelocityButton()
    {
        bodiesStartPos.Clear();
        for (int i = 0; i < bodies.Length; i++)
        {
            
            bodiesStartPos.Add(bodies[i].position);
            bodies[i].velocity = velocityInput[i];
        }
    }
    public void ResetPositionButton()
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].position = bodiesStartPos[i];
            bodies[i].rotation = 0;
            bodies[i].angularVelocity = 0;
            bodies[i].velocity = Vector2.zero;
        }
    }
}

using MathNet.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UniversalGraviation : MonoBehaviour
{
    // $$ F = G M m / r^2 $$ -- G: Newton's gravitational constant, M: mass of body recieveing the force, m: mass of body generating the gravitational field, r: distance between the two masses. 

    [SerializeField] private float gravCoupling = 1; //plays the role of G, but not called G because G has a specific value associated with it
    [SerializeField] private Rigidbody[] bodies; //contains rigidbodies of all the masses in the scene
    [SerializeField] private float xSpacing = 1; //spacing between masses
    [SerializeField] private float ySpacing = 1;
    [SerializeField] private float zSpacing;
    [SerializeField] private float spacingOffsetScale = 2; //strength of intial position perturbation
    [SerializeField] private float initialVelocityScale = 1f; //strength of initial random velcocity  
    [SerializeField] private float massScale = 1f; //level of mass variation 

    public ComputeShader gravCompShader; //compute shader will be responsible for doing the force calculations on the GPU. We then apply the forces on the CPU (here)
    public Particle[] Particles; //Particle data that will be sent to GPU
    private Vector3[] netForces; //array that will recieve the net forces from the GPU

    //struct that will contain the particle data to send to the compute shader
    public struct Particle 
    {
        public Vector3 position;
        public float mass;
    };

    void Awake()
    {
        //Get all the masses in the scene
        bodies = GameObject.FindObjectsOfType<Rigidbody>();

        //initialize arrays to be used for compute shader buffers
        Particles = new Particle[bodies.Length];
        netForces = new Vector3[bodies.Length];

        //Particles are (currently) arranged in a cubic 3D grid, so S is the number of particles per edge
        int S = Mathf.FloorToInt(Mathf.Pow(bodies.Length,1f/3f));
        int bodyNum = 0; //used as a counter, to convert from [x,y,z] to [i]
        Vector3[,,] points = new Vector3[S,S,S];

        //set particle initial positions/velocity/mass, fill arrays for CS buffers 
        for (int i = 0; i < S; i++)
        {
            for (int j = 0; j < S; j++)
            {
                for (int k = 0; k < S; k++)
                {
                    points[i,j,k] += new Vector3((i-S/2)*xSpacing, (j - S / 2) *ySpacing, (k - S / 2) *zSpacing) + spacingOffsetScale * Random.insideUnitSphere; //initial position, a site on a cubic grid + some random offset
                    //Get 'i'th rigidbody, set its initial position, velocity, and mass
                    Rigidbody body = bodies[bodyNum];
                    body.GetComponent<TrailRenderer>().enabled = false; //disable trails before before setting positions to avoid a mess of trals on start
                    body.position = points[i, j, k];
                    body.mass *= massScale * Random.value;
                    //body.velocity = initialVelocityScale * Random.insideUnitSphere;
                    //body.velocity = -initialVelocityScale * VelocityField.VectorFieldFromPotential(VelocityField.Potential, points[i,j,k], 0.01f);
                    float x = points[i, j, k].x;
                    float y = points[i, j, k].y;
                    float z = points[i, j, k].z;
                    body.velocity = VelocityField.VectorField(- z,  x, -y) /10;

                    //Create a Particle struct, set its position and mass
                    Particles[bodyNum] = new Particle();  
                    Particles[bodyNum].position = body.position;
                    Particles[bodyNum].mass = body.mass;
                    bodyNum++;//each time the loop runs, bodyNum goes up, essentially converting an [x,y,z] index to a simple [i] index
                }
            }
        }
    }

    void Update()
    {
        // for each rigidbody, calculate and apply the force

        //UpdatewithCPU();
        UpdatewithCS();

    }

    //Originally this sim was done on the CPU. Pretty brute force I think
    public void UpdatewithCPU()
    {
        for (int i = 0; i < bodies.Length; i++) //Net force for the 'i'th particle
        {
            Rigidbody body = bodies[i];
            Vector3 field = Vector3.zero;
            
            for (int j = 0; j < bodies.Length; j++) //Add up the force from each of the 'j' particles to get the net force
            {
                if (body != bodies[j]) //if i=j, then dont calculate (it would result in a divide by 0)
                {
                    Vector3 itoj = (bodies[j].position - body.position).normalized; //Gravity is attractive, so it pulls 'i' towards 'j' (and eventually 'j' will be pulled towards 'i', once the loop comes around to it)
                    field += bodies[j].mass / ((bodies[j].position - body.position).sqrMagnitude) * itoj; //if F = G M m / r^2, then here we are just calculating m/r^2, which is more like a gravitational FIELD rather than FORCE
                }

            }
            
            netForces[i] = gravCoupling * body.mass * field; //F = G*M*g, where g = m/r^2
            bodies[i].AddForce(netForces[i]);
        }

    }
    public void UpdatewithCS()
    {

        int massSize = sizeof(float);
        int vecSize = sizeof(float)*3;
        int totalSize = massSize + vecSize;//size of each struct instance, in bytes

        for (int i = 0; i < Particles.Length; i++)
        {
            if (bodies[i].GetComponent<TrailRenderer>().enabled == false)//turn on particle trails once the sim begins
            {
                bodies[i].GetComponent<TrailRenderer>().enabled = true;
            }
            //Refresh particle positions (and masses too I guess tho they're not changing...) so buffer has the current data
            Particles[i].position = bodies[i].position;
            Particles[i].mass = bodies[i].mass; //for some reason, if I dont include this line then the compute shader only acts on 7 particles??? 
        }

        int kernal = gravCompShader.FindKernel("CSMain"); //im understanding this as something like the index for the shader version of a method 
        Vector3[] CSOutput = new Vector3[Particles.Length]; //output from the compute shader

        //create input and output buffers. Input contains Particle structs to sent to GPU, output contains net force/G sent from the GPU
        ComputeBuffer inputbuff = new ComputeBuffer(Particles.Length, totalSize); 
        ComputeBuffer outputbuff = new ComputeBuffer(Particles.Length, vecSize);
        
        //load up the input buffer with particle data, set shader counterpart variables,
        inputbuff.SetData(Particles);
        gravCompShader.SetBuffer(kernal, "particles", inputbuff);
        gravCompShader.SetBuffer(kernal, "output", outputbuff);
        gravCompShader.SetFloat("numberofParticles", Particles.Length);

        //Number of work groups to make
        //int numWorkGroups = Mathf.CeilToInt(Particles.Length / (8*8));
        int numWorkGroups = 1;
        //Get the shader to execute (im understanding this as something like calling the CSMain "method", with the stated number of workgroups)
        gravCompShader.Dispatch(kernal,numWorkGroups,numWorkGroups,numWorkGroups);

        //get the calculated net force data back form the shader, and store in the CSOutput array
        outputbuff.GetData(CSOutput);

        //Scale the forces by G, then apply them to their associated masses
        for (int i = 0; i < Particles.Length; i++)
        {
            netForces[i]= gravCoupling * CSOutput[i];
            bodies[i].AddForce(netForces[i]);
        }
        //dispose the bufffers
        inputbuff.Dispose();
        outputbuff.Dispose();
    }

}

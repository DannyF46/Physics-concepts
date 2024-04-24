using MathNet.Numerics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.ParticleSystemJobs;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UIElements;
//idea: instead of comparing each particle with every other particle, take samples that average over larger volumes as you move out from each particle
//       O (   )                                                                                                Nearby, more samples of smaller volumes. Far, less samples of larger volumes
//    O o.o O (   )
//  O o.....o O (   )
//    O o.o O(    )
//       O (    ) ... and so on 
//
public class UniversalGraviation : MonoBehaviour
{
    const float pi = Mathf.PI;

    [SerializeField] private GameObject _Particle;
    // $$ F = G M m / r^2 $$ -- G: Newton's gravitational constant, M: mass of body recieveing the force, m: mass of body generating the gravitational field, r: distance between the two masses. 
    [SerializeField] private int particlesPerSide = 8;
    [SerializeField] private float gravCoupling = 1; //plays the role of G, but not called G because G has a specific value associated with it
    [SerializeField] public Rigidbody[] bodies { get; private set; }  //contains rigidbodies of all the masses in the scene
    [SerializeField] private float xSpacing = 1; //spacing between masses
    [SerializeField] private float ySpacing = 1;
    [SerializeField] private float zSpacing;
    [SerializeField] private float spacingOffsetScale = 2; //strength of intial position perturbation
    [SerializeField] private float initialVelocityScale = 1f; //strength of initial random velcocity  
    [SerializeField] private float massScale = 1f; //level of mass variation 

    public ComputeShader gravCompShader; //compute shader will be responsible for doing the force calculations on the GPU. We then apply the forces on the CPU (here)
    public Particle[] Particles { get; private set; } //Particle data that will be sent to GPU
    private Vector3[] netForces; //array that will recieve the net forces from the GPU
    private int numberOfParticles;
    public float range = 10;
    //struct that will contain the particle data to send to the compute shader
    public struct Particle 
    {
        public Vector3 position;
        public float mass;
    };

    void Awake()
    {
        //Get all the masses in the scene
        //bodies = GameObject.FindObjectsOfType<Rigidbody>();
        numberOfParticles = particlesPerSide * particlesPerSide * particlesPerSide;

        //initialize arrays to be used for compute shader buffers
        bodies = new Rigidbody[numberOfParticles];
        Particles = new Particle[numberOfParticles];
        netForces = new Vector3[numberOfParticles];

        CreateParticles(numberOfParticles);
        ArrangeParticles(particlesPerSide);

        _Particle.SetActive(false);
        
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
            if (!bodies[i].GetComponent<TrailRenderer>().enabled)
            {
                bodies[i].GetComponent<TrailRenderer>().enabled = true;
            }

            //Refresh particle positions so buffer has the current data
            Particles[i].position = bodies[i].position;
        }

        int kernal = gravCompShader.FindKernel("CSMain"); //im understanding this as something like the index for the shader version of a method 
        //Vector3[] CSOutput = new Vector3[Particles.Length]; //output from the compute shader

        //create input and output buffers. Input contains Particle structs to sent to GPU, output contains net force/G sent from the GPU
        ComputeBuffer inputbuff = new ComputeBuffer(Particles.Length, totalSize);
        ComputeBuffer outputbuff = new ComputeBuffer(Particles.Length, vecSize);
        
        //load up the input buffer with particle data, set shader counterpart variables,
        inputbuff.SetData(Particles);
        outputbuff.SetData(netForces);

        gravCompShader.SetBuffer(kernal, "particles", inputbuff);
        gravCompShader.SetBuffer(kernal, "output", outputbuff);
        gravCompShader.SetFloat("numberofParticles", Particles.Length);
        gravCompShader.SetFloat("G", gravCoupling);

        //Number of work groups to make
        //int numWorkGroups = Mathf.CeilToInt((float)numberOfParticles/ 1000);//total number of particles divided by the thread number volume in the CS seems to be ideal
        int numWorkGroups = Mathf.CeilToInt((float)numberOfParticles/ 1024);
        //Get the shader to execute (im understanding this as something like calling the CSMain "method", with the stated number of workgroups)
        gravCompShader.Dispatch(kernal, numWorkGroups, numWorkGroups, 1);

        //get the calculated net force data back form the shader, and store in the CSOutput array
        outputbuff.GetData(netForces);

        
        //Scale the forces by G, then apply them to their associated masses
        for (int i = 0; i < Particles.Length; i++) 
        {
            bodies[i].AddForce(netForces[i]);
            bodies[i].GetComponent<Renderer>().material.SetFloat("_Speed", bodies[i].velocity.magnitude);
        }
        //dispose the bufffers
        inputbuff.Release();
        outputbuff.Release();
    }
    
    public void CreateParticles(float numOfParticles)
    {

        for (int i = 0; i < numOfParticles; i++)
        { 
            //create a body and set its mass
            GameObject particle = GameObject.Instantiate(_Particle);
            bodies[i] = particle.GetComponent<Rigidbody>();
            bodies[i].mass *= massScale * (1.9f*Random.value + 0.1f); //(1.5*x+0.5) gives a value between [0.5,2] for x between 0 and 1 //1.9x+0.1 ==> 0.1 to 2
            bodies[i].transform.localScale = Mathf.Pow(bodies[i].mass,1f/3f)*Vector3.one;
            particle.GetComponent<TrailRenderer>().enabled = false;//disable trails before before setting positions to avoid a mess of trals on start

            //Create a Particle struct, set its mass
            Particles[i] = new Particle();
            Particles[i].mass = bodies[i].mass;
        }

    }
    
    public void ArrangeParticles(int numPerSide)
    {
        int bodyNum = 0; //used as a counter, to convert from [x,y,z] to [i]
        Vector3[,,] points = new Vector3[numPerSide, numPerSide, numPerSide];

        for (int i = 0; i < numPerSide; i++)
        {
            for (int j = 0; j < numPerSide; j++)
            {
                for (int k = 0; k < numPerSide; k++)
                {
                    points[i, j, k] += new Vector3((i - numPerSide / 2) * xSpacing, (j - numPerSide / 2) * ySpacing, (k - numPerSide / 2) * zSpacing) + spacingOffsetScale * Random.insideUnitSphere; //initial position, a site on a cubic grid + some random offset
                    
                    //Get 'i'th rigidbody, set its initial position and velocity
                    //Rigidbody body = bodies[bodyNum];
                    bodies[bodyNum].position = points[i, j, k];
                    
                    //body.velocity = initialVelocityScale * Random.insideUnitSphere;
                    //body.velocity = initialVelocityScale * VelocityField.VectorFieldFromPotential(VelocityField.Potential, points[i,j,k], 0.01f);
                    float x = points[i, j, k].x;
                    float y = points[i, j, k].y;
                    float z = points[i, j, k].z;
                    bodies[bodyNum].velocity = initialVelocityScale * VelocityField.VectorField(y, -x, z);

                    //Set particle position
                    Particles[bodyNum].position = points[i, j, k];

                    bodyNum++;//each time the loop runs, bodyNum goes up, essentially converting an [x,y,z] index to a simple [i] index
                }
            }
        }
        Debug.Log(bodyNum);
    }

    //starting to think about an averaging procedure to speed up computations. commit before starting to implement
    public Collider[] GetCollidersInRangeOf(Vector3 pos, float radius)
    { 
        Collider[] neighborhood = Physics.OverlapSphere(pos, radius);
        return neighborhood;
    }
    public float NeighborhoodMass(Vector3 center, float radius)//
    {
        Collider[] neighborhood = GetCollidersInRangeOf(center, radius);
        float neighborhoodMass = 0;

        foreach (Collider neighbor in neighborhood)
        {
            neighborhoodMass += neighbor.GetComponent<Rigidbody>().mass;//i think we just want to add the masses, but if things go wrong consider using average mass?
        }

        return neighborhoodMass;
    }
    public Particle CreateNeihborhoodParticle(Vector3 center, float range)
    {
        Particle neighborParticle;

        neighborParticle.mass = NeighborhoodMass(center, range);
        neighborParticle.position = center;

        return neighborParticle;
    }
    public Vector3 ForceFromNeighborhood(Particle particle, Vector3 center, float range)
    {
        Particle neighborparticle = CreateNeihborhoodParticle(center, range);
        Vector3 r = (neighborparticle.position - particle.position);
        float distancesqr = r.sqrMagnitude;
        Vector3 dir = r.normalized;


        Vector3 neighborhoodforce = gravCoupling*particle.mass * neighborparticle.mass/distancesqr * dir;

        return neighborhoodforce;
    }
    public Vector3 GetForceFromNeighborShell(Particle particle, float innerShellRadius, float outerShellRadius)
    {
        Vector3 shellforce = Vector3.zero;
        float neibhorhoodDiameter = outerShellRadius - innerShellRadius;
        float neighborhoodVol = (4f / 3f) * pi * (neibhorhoodDiameter / 2) * (neibhorhoodDiameter / 2) * (neibhorhoodDiameter / 2);
        float shellVol = (4f / 3f) * pi * (outerShellRadius * outerShellRadius * outerShellRadius - innerShellRadius * innerShellRadius * innerShellRadius); //Vol = (4/3)*pi*(r_2^3 - r_1^3)
        int numberOfNeihborhoods = Mathf.FloorToInt(shellVol/neighborhoodVol);
        
        for (int i = 0; i < numberOfNeihborhoods; i++)
        {
            Vector3 neiborhoodPos = particle.position + (innerShellRadius - neibhorhoodDiameter / 2f) * Random.onUnitSphere;// randomize where in the shell to sample --should average out errors
            shellforce += ForceFromNeighborhood(particle, neiborhoodPos, neibhorhoodDiameter/2f);
        }
        return shellforce;
    }
}

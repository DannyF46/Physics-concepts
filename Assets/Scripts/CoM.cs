using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
[ExecuteAlways]
public class CoM : MonoBehaviour
{
    private GameObject[] collisionObjects;
    [SerializeField] public Rigidbody2D[] bodies;
    private List<Vector2> momenta = new List<Vector2>();
    private Vector2 CoMmomentum;
    [HideInInspector] public Vector2 CenterofMass;
    private Vector2 prevCenterofMass;
    [HideInInspector] public Vector2 CenterofMassVelocity;
    private float TotalMass;
    private Transform gizmo;
    // Start is called before the first frame update
    
    void Start()
    {
        gizmo = this.GetComponentInChildren<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        collisionObjects = GameObject.FindGameObjectsWithTag("CollisionObject");
        bodies = new Rigidbody2D[collisionObjects.Length];
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i] = collisionObjects[i].GetComponent<Rigidbody2D>();
        }
        CenterofMass = Vector2.zero;
        CoMmomentum = Vector2.zero;
        momenta.Clear();
        TotalMass = 0;
        for (int i = 0; i < bodies.Length; i++)
        {
            TotalMass += bodies[i].mass;
        }
        for (int i = 0; i < bodies.Length; i++)
        {
            momenta.Add(bodies[i].mass * bodies[i].velocity);
            CenterofMass += bodies[i].mass * bodies[i].worldCenterOfMass / TotalMass;
            CoMmomentum += momenta[i] / TotalMass;
        }
        gizmo.position = CenterofMass;
        
        if(prevCenterofMass != null)
        {
            CenterofMassVelocity = (CenterofMass - prevCenterofMass)/Time.fixedDeltaTime;

        }


        prevCenterofMass = CenterofMass;


    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine.Utility;


//[ExecuteAlways]

public class SecondOrder : MonoBehaviour
{
    [SerializeField] private GameObject objToMove;
    [SerializeField] private GameObject objToFollow;
    [SerializeField] float NatFreq = 0;
    [SerializeField] float Damping = 0;
    [SerializeField] float Response = 0;
    [SerializeField] float TimeStep = 0.0001f;


    //private Vector<float> movePosition;
    private Rigidbody rbToMove;
    private Vector3 prevFollowPos;
    private Vector3 movePos, moveVel;
    private Vector3 followPos, followVel;
    private float k1, k2, k3;

    private Vector3 NaNVec = new(float.NaN, float.NaN, float.NaN);
    // Start is called before the first frame update
    void Start()
    {
        rbToMove = objToMove.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TimeStep = Time.deltaTime;
        movePos = objToMove.transform.position;
        moveVel = rbToMove.velocity;
        followPos = objToFollow.transform.position;

        Variables(NatFreq, Damping, Response);
        TimeStep = Time.fixedDeltaTime;

        Vector3 addVel;
            
        if(NatFreq > 0)
        {
            addVel = UpdateSolver(TimeStep);
        }
        else
        {
            addVel = Vector3.zero;
        }

        if (!addVel.IsNaN())
        {
            rbToMove.velocity += addVel;
            //rbToMove.AddForce(addVel);
        }
        

        prevFollowPos = objToFollow.transform.position;
    }
    
    public void Variables(float f, float z, float r)
    {
        if (f != 0)
        {
            k1 = z / (Mathf.PI * f);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = r * z / (2 * Mathf.PI * f);
        }


    }

    public Vector3 UpdateSolver(float T)
    {
        Vector3 AddVel;
        Vector3 AddForce;
        followVel = (followPos - prevFollowPos) / T;

        //movePos += T * moveVel;
        AddForce = rbToMove.mass*(followPos + k3 * followVel - movePos - k1 * moveVel) / k2;
        AddVel = T * (followPos + k3 * followVel - movePos - k1 * moveVel) / k2;

        return AddVel;
        //return AddForce;
    }
        

}


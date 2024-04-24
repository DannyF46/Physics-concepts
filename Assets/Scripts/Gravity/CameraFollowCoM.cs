using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowCoM : MonoBehaviour
{
    public UniversalGraviation gravityHandler;
    public GameObject camera;
    public Vector3 centerOfMass;
    public float totalMass = 0;
    UniversalGraviation.Particle[] particles;

    private void Start()
    {
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        particles = gravityHandler.Particles;

        for (int i = 0; i < particles.Length; i++)
        {
            totalMass += particles[i].mass;
        }
    }
   
    void FixedUpdate()
    {
        particles = gravityHandler.Particles;
        centerOfMass = Vector3.zero;
        for (int i = 0; i < particles.Length; i++)
        {
            //centerOfMass += particles[i].mass * particles[i].position;
            centerOfMass += particles[i].mass * gravityHandler.bodies[i].position;
        }
        centerOfMass = centerOfMass/totalMass;

        Vector3 camToCoM = (centerOfMass - camera.transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(camToCoM);
        camera.transform.rotation = rot;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(centerOfMass,1);
        
    }
}

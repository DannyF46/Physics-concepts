using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollowCoM : MonoBehaviour
{
    public UniversalGraviation gravityHandler;
    public GameObject camera;
    public Vector3 centerOfMass = Vector3.zero;
    public float totalMass = 0;
    UniversalGraviation.Particle[] particles;

    // Update is called once per frame
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
        centerOfMass = Vector3.zero;
        for (int i = 0; i < particles.Length; i++)
        {
            centerOfMass += particles[i].mass * particles[i].position;
        }
        centerOfMass /= totalMass;

        Vector3 camToCoM = (centerOfMass - this.transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(camToCoM);
        //Quaternion rotateToCoM = Quaternion.FromToRotation(camera.transform.forward, camToCoM);
        camera.transform.rotation = rot;
    }
}

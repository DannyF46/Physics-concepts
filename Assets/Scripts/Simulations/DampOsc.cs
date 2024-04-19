/*
 * Based on work done by YouTube user t3ssel8r: https://www.youtube.com/watch?v=KPoeNZZ6H4s
 * 
 * 
 */


using System;
using Unity.VisualScripting;
using UnityEngine;

public class DampOsc
{

    [SerializeField] public float NatFreq = 0;
    [SerializeField] public float Damping = 0;
    [SerializeField] public float Response = 0;
    //[SerializeField] float TimeStep = 0.0001f;

    public Vector2 prevOscPos, prevDriverPos;
    public Vector2 oscPos, oscVel;
    public Vector2 driverPos, driverVel;
    public Vector2[] oscIC, driverIC = new Vector2[2]; //initial conditions
    public float k1, k2, k3;
    public bool ConstSet = false;
    public bool ICSet = false;

    public DampOsc(float freq, float damp, float response, Vector2[] oscillatorICs, Vector2[] driverICs)
    {
        NatFreq = freq;
        Damping = damp;
        Response = response;
        SetConstants(NatFreq, Damping, Response);
        SetICs(oscillatorICs, driverICs);

    }
    public DampOsc(float freq, float damp, float response)
    {
        NatFreq = freq;
        Damping = damp;
        Response = response;
        SetConstants(NatFreq, Damping, Response);

    }
    public void SetConstants(float f, float z, float r)
    {
        if (f != 0)
        {
            k1 = z / (Mathf.PI * f);
            k2 = 1 / ((2 * Mathf.PI * f) * (2 * Mathf.PI * f));
            k3 = r * z / (2 * Mathf.PI * f);
            ConstSet = true;
        }
    }
    public void SetICs(Vector2[] oscillatorICs, Vector2[] driverICs) //IC[0] = initial position; IC[1] = initial velocity
    {
        oscIC = oscillatorICs;
        driverIC = driverICs;
        oscPos = oscIC[0];
        oscVel = oscIC[1];
        driverPos = driverIC[0];
        driverVel = driverIC[1];
        ICSet = true;
    }

    public Vector2 UpdateSolver(float timestep, Vector2 driverPosition)
    {
        if (!ConstSet || !ICSet)
        {
            Debug.LogWarning("Set constants or initial conditions");
        }
        else
        {
            prevDriverPos = driverPos;
            driverPos = driverPosition;

            Vector2 deltaV = timestep * (driverPos - oscPos + k3 * driverVel - k1 * oscVel) / k2;

            oscVel += deltaV;
            oscPos += timestep * oscVel;

            driverVel = (driverPos - prevDriverPos) / timestep;
        }

        return oscPos;
    }


   
}

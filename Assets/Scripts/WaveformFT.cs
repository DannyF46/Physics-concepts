using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MathNet;
using MathNet.Numerics;
using UnityEngine.Rendering;
using System.Linq;

public class WaveformFT : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private int numberOfPoints = 20;
    [SerializeField] private float plotFrom = -1;
    [SerializeField] private float plotTo = 1;

    [SerializeField] private float freq = 2;

    [SerializeField] private float minFreq = 0;
    [SerializeField] private float maxFreq = 100;
    private float[] xvalues;
    private float[] yvalues;
    //private List<Vector3>
    //[SerializeField] Color lineColor;
    //[SerializeField] Color color;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (numberOfPoints > 0)
        {
            if(line.positionCount != numberOfPoints + 1) 
            {
                line.positionCount = numberOfPoints + 1;
            }
            
            xvalues = new float[numberOfPoints + 1];
            yvalues = new float[numberOfPoints + 1];
            Complex32[] PtsforFFT = new Complex32[numberOfPoints + 1];

            for (int i = 0; i < numberOfPoints + 1; i++)
            {
                xvalues[i] = Mathf.Lerp(plotFrom,plotTo, i / (float)numberOfPoints);
                Vector3 FunctPt = Function(xvalues[i]);
                yvalues[i] = FunctPt.y;
                PtsforFFT[i] = yvalues[i];
            }

            MathNet.Numerics.IntegralTransforms.Fourier.Forward(PtsforFFT);
            float[] LinePos = new float[numberOfPoints + 1];
            for (int i = 0; i < numberOfPoints + 1; i++)
            {
                xvalues[i] = Mathf.Lerp(minFreq, maxFreq, i / (float)numberOfPoints);
                float FFTPt = (float)Complex32.Abs(PtsforFFT[i]);
                line.SetPosition(i, new Vector3(xvalues[i], FFTPt));
                LinePos[i] = FFTPt;
            }

            int maxIndex = (int)Array.IndexOf(LinePos,LinePos.Max()) ;
            Debug.Log(xvalues[maxIndex]);
        }
        
    }
    
    private Vector3 Function(float x)
    {
        
        Vector3 pt = new Vector3();
        
        pt.x = x;
        pt.y = Mathf.Sin(freq*x) + Mathf.Sin(2*freq * x);

        return pt;
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MathNet;
using MathNet.Numerics;
using UnityEngine.Rendering;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using System.Xml.Schema;
using MathNet.Numerics.LinearAlgebra.Complex;
using MathNet.Numerics.Distributions;
using Unity.Mathematics;
using MathNet.Numerics.Providers.LinearAlgebra;

public class Schroedinger : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    [SerializeField] private int numberOfPoints = 20;
    [SerializeField] private float xMin = -1;
    [SerializeField] private float xMax = 1;
    [SerializeField] private float timeStep = 0.1f;
    [SerializeField] private float Freq = 1;
    private float spatialStepSize;
    private float[] xvalues;
    private Complex32[] yvalues;
    private Complex32[] prevValues;
    //private List<Vector3>
    //[SerializeField] Color lineColor;
    //[SerializeField] Color color;

    readonly Complex32 I = Complex32.ImaginaryOne;
    readonly float PI = Mathf.PI;

    // Start is called before the first frame update
    void Start()
    {
        spatialStepSize = (xMax - xMin) / numberOfPoints;
        xvalues = new float[numberOfPoints + 1];
        prevValues = new Complex32[numberOfPoints + 1];
        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            xvalues[i] = Mathf.Lerp(xMin, xMax, i / (float)numberOfPoints);
            //prevValues[i] = (Complex32)Mathf.Sin(2 * PI * xvalues[i] / (xMax - xMin));
            //prevValues[i] = Complex32.Exp(I*2 * PI * Freq * xvalues[i] / (xMax - xMin));
            prevValues[i] = GaussianPacket(xvalues[i], 0 ,2f, 1);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (numberOfPoints > 0)
        {
            if (line.positionCount != numberOfPoints + 1)
            {
                line.positionCount = numberOfPoints + 1;
            }


            yvalues = UnitaryShrodingerNextStep(xvalues, 0, prevValues);

            prevValues = yvalues;

            for (int i = 0; i < numberOfPoints + 1; i++)
            {
                line.SetPosition(i, new Vector3(xvalues[i], yvalues[i].MagnitudeSquared));
            }

        }

    }
    private Complex32[] UnitaryShrodingerNextStep(float[] x, float t, Complex32[] prevStep)
    {   
        Complex32[] pts = new Complex32[numberOfPoints + 1];
        float T = timeStep;
        Complex32[,] TimeEvol = TimeEvolutionOp(x, t);


        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            pts[i] = TimeEvol[i, i] * prevStep[i];

        }
        return pts;
    }


    private Complex32[] ShrodingerNextStep(float[] x, float t, Complex32[] prevStep)
    {
        Complex32[] pts = new Complex32[numberOfPoints + 1];
        float T = timeStep;
        Complex32[,] schroMatrix = new Complex32[numberOfPoints + 1, numberOfPoints + 1];
        float deltas = T / (spatialStepSize * spatialStepSize);
        

        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            Complex32 diag = 1 - I * (2 * deltas + T * Potential(x[i], t));
            schroMatrix[i, i] = diag;
            if (i == 0)
            {
                schroMatrix[i, i + 1] = I * deltas;
                pts[i] = schroMatrix[i, i] * prevStep[i] + schroMatrix[i, i + 1] * prevStep[i + 1];
            }
            else if (i == numberOfPoints)
            {
                schroMatrix[i, i - 1] = I * deltas;
                pts[i] = schroMatrix[i, i] * prevStep[i] + schroMatrix[i, i - 1] * prevStep[i - 1];
            }
            else
            {
                schroMatrix[i, i - 1] = schroMatrix[i, i + 1] = I * deltas;
                pts[i] = schroMatrix[i, i] * prevStep[i] + schroMatrix[i, i + 1] * prevStep[i + 1] + schroMatrix[i, i - 1] * prevStep[i - 1];
            }
        }
        /*
        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            if (i == 0)
            {
                pts[i] = schroMatrix[i, i] * prevStep[i] + schroMatrix[i, i + 1] * prevStep[i + 1];
            }
            else if (i == numberOfPoints)
            {
                pts[i] = schroMatrix[i, i] * prevStep[i] + schroMatrix[i, i - 1] * prevStep[i - 1];
            }
            else
            {
                pts[i] = schroMatrix[i, i] * prevStep[i] + schroMatrix[i, i + 1] * prevStep[i + 1] + schroMatrix[i, i - 1] * prevStep[i - 1];
            }
        }*/

        return pts;
    }
    float Potential(float x, float t)
    {
        float Vnj = 0;

        return Vnj;
    }
    Complex32[,] TimeEvolutionOp(float[] x, float t)
    {
        //e^{-iHt}
        float T = timeStep;
        Complex32[,] H = Hamiltonian(x, t);
        Complex32[,] ExpHfirstorder = new Complex32[numberOfPoints + 1, numberOfPoints + 1];
        Matrix<Complex32> HamMat = MathNet.Numerics.LinearAlgebra.CreateMatrix.SparseOfArray<Complex32>(H);
        Matrix<Complex32> A = HamMat * HamMat;
        Matrix<Complex32> SecondOrderExp = CreateMatrix.DiagonalIdentity<Complex32>(numberOfPoints + 1) - I*HamMat*T + A*T*T/2;
       
        //Complex32[,] ExpHfirstorder = new Complex32[numberOfPoints + 1, numberOfPoints + 1];
        for (int i = 0; i < numberOfPoints + 1; i++)
        {/*
            ExpHfirstorder[i,i] = (1 - I* H[i,i]*T/2) / (1 + I * H[i, i] * T / 2);
            if (i == 0)
            {
                ExpHfirstorder[i, i + 1] = (- I * H[i, i+1] * T / 2) / ( I * H[i, i+1] * T / 2);

            }
            else if (i == numberOfPoints)
            {
                ExpHfirstorder[i, i - 1] = (-I * H[i, i - 1] * T / 2) / (I * H[i, i - 1] * T / 2);

            }
            else
            {
                ExpHfirstorder[i, i - 1] = (-I * H[i, i - 1] * T / 2) / (I * H[i, i - 1] * T / 2);
                ExpHfirstorder[i, i + 1] = (-I * H[i, i + 1] * T / 2) / (I * H[i, i + 1] * T / 2);

            }*/
            
            ExpHfirstorder[i, i] = (1 - I * H[i, i] * T / 2) ;
            if (i == 0)
            {
                ExpHfirstorder[i, i + 1] = (-I * H[i, i + 1] * T / 2);

            }
            else if (i == numberOfPoints)
            {
                ExpHfirstorder[i, i - 1] = (-I * H[i, i - 1] * T / 2);

            }
            else
            {
                ExpHfirstorder[i, i - 1] = (-I * H[i, i - 1] * T / 2);
                ExpHfirstorder[i, i + 1] = (-I * H[i, i + 1] * T / 2);

            }
        }
        //return ExpHfirstorder;
        return SecondOrderExp.ToArray();
    }
    Complex32[,] Hamiltonian(float[] x, float t)
    {
        Complex32[,] H = new Complex32[numberOfPoints + 1, numberOfPoints + 1];
        Complex32[,] LaplaceOp = SecondDerivFiniteDifferences();
        
        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            float Vnj = Potential(x[i], t);
            H[i, i] = -LaplaceOp[i, i] / 2 + Vnj;
            if (i == 0)
            {
                H[i, i + 1] = -LaplaceOp[i,i+1]/2;
               
            }
            else if (i == numberOfPoints)
            {
                H[i, i - 1] =  -LaplaceOp[i, i - 1] / 2;

            }
            else
            {
                H[i, i - 1] = -LaplaceOp[i, i - 1] / 2;
                H[i, i + 1] = -LaplaceOp[i, i + 1] / 2;

            }
        }
        return H;
    }
    
    Complex32[,] SecondDerivFiniteDifferences()
    {
        Complex32[,] LaplaceOp = new Complex32[numberOfPoints + 1, numberOfPoints + 1];
  
        //var vec = MathNet.Numerics.LinearAlgebra.CreateVector.DenseOfArray<Complex32>();
        

        float dxsqr = (spatialStepSize * spatialStepSize);
        for (int i = 0; i < numberOfPoints + 1; i++)
        {
            LaplaceOp[i, i] = -2 / dxsqr;
            if (i == 0)
            {
                LaplaceOp[i, i + 1] = 1/dxsqr;
            }
            else if (i == numberOfPoints)
            {
                LaplaceOp[i, i - 1] = 1/dxsqr;
            }
            else
            {   
                LaplaceOp[i, i - 1] = LaplaceOp[i,i+1] = 1 / dxsqr;
            }
        }
        //Matrix<Complex32> Mat = MathNet.Numerics.LinearAlgebra.CreateMatrix.SparseOfArray<Complex32>(LaplaceOp);
        return LaplaceOp;
    }

    Complex32 GaussianPacket(float x, float x0 ,float sigma, float p)
    {
        Complex32 Gaussian = Complex32.Pow(1 / (2 * PI * sigma * sigma), 1f / 4f) * Complex32.Exp(-(x - x0)*(x-x0) / (4 * sigma * sigma) + I * p * x);
        return Gaussian;
    }
}

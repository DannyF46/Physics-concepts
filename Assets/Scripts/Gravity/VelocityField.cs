using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public static class VelocityField 
{
    // Start is called before the first frame update

    public static Vector3 Gradient(Func<Vector3, float> function, Vector3 position, float delta)
    {
        float dfdx = DirectionalDerivative(function, position, Vector3.right, delta);
        float dfdy = DirectionalDerivative(function, position, Vector3.up, delta);
        float dfdz = DirectionalDerivative(function, position, Vector3.forward, delta);
        Vector3 grad = new Vector3(dfdx, dfdy, dfdz);
        return grad;

    }
    public static float DirectionalDerivative(Func<Vector3, float> function, Vector3 position, Vector3 direction, float delta)
    {
        float fx = function(position);
        float fx2 = function(position + delta*direction);
        float deriv = (fx2 - fx) / delta;
        return deriv;
    }
    public static Vector3 VectorField(Vector3 position)
    {
        float x = position.x;
        float y = position.y;
        float z = position.z;

        Vector3 field = new Vector3(x, y, z);
        return field;
    }
    public static Vector3 VectorField(float xcomp, float ycomp, float zcomp)
    {
        Vector3 field = new Vector3(xcomp, ycomp, zcomp);
        return field;
    }
    public static Vector3 VectorFieldFromPotential(Func<Vector3,float> potential, Vector3 position, float delta)
    {
        return -Gradient(potential, position, delta);
    }
    public static float Potential(Vector3 position)
    {
        float x = position.x;
        float y = position.y;
        float z = position.z;

        float potential = position.sqrMagnitude;
        return potential;

    }
}

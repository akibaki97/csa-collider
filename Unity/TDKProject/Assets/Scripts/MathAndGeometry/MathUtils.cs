using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtils
{
    public enum Coordinate
    {
        X,
        Y,
        Z
    }

    public static Vector3 CenterOfMass(IEnumerable<Vector3> collection)
    {
        uint count = 0;
        Vector3 sum = Vector3.zero;
        foreach (Vector3 vec in collection)
        {
            count++;
            sum += vec;
        }  

        return sum / count;
    }
}

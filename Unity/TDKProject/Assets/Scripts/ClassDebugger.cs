using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;
using Assets.Scripts.MathAndGeometry;

public class ClassDebugger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("hello world");

        AlgPoly algPoly = new AlgPoly(10);
        algPoly.p[0] = new Complex(0.04455f, -0.07194f);
        algPoly.p[1] = new Complex(-0.05742f, 0.08978f);
        algPoly.p[2] = new Complex(0.17168f, -0.28283f);
        algPoly.p[3] = new Complex(-0.12969f, 0.08158f);
        algPoly.p[4] = new Complex(-0.33418f, 0.26873f);
        algPoly.p[5] = new Complex(0.14186f, 0.00000f);
        algPoly.p[6] = new Complex(-0.33418f, -0.26873f);
        algPoly.p[7] = new Complex(-0.12969f, -0.08158f);
        algPoly.p[8] = new Complex(0.17168f, 0.28283f);
        algPoly.p[9] = new Complex(-0.05742f, -0.08978f);
        algPoly.p[10] = new Complex(0.04455f, 0.07194f);

        Complex[] roots = algPoly.AberthEhrlich();

        //for (int k = 0; k < algPoly.N; k++)
        //{
        //    Debug.Log(roots[k].ToString() + "; abs: " + roots[k].Abs);
        //}

        //Complex val = algPoly.Eval(new Complex(4, 2));
        //Debug.Log(val.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

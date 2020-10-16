using Model.MathAndGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MathAndGeometry
{
    /// <summary>
    /// Algebraic polynomial class with complex coefficients
    /// </summary>
    public class AlgPoly
    {
        public AlgPoly(Complex[] p)
        {
            this.p = p;
            N = p.Length - 1;
        }

        public AlgPoly(TrigPoly f)
        {
            p = (Complex[]) f.c.Clone();
        }

        public AlgPoly(int N)
        {
            p = new Complex[N + 1];
            this.N = N;
        }

        /// <summary>
        /// coefficients
        /// </summary>
        public Complex[] p;

        /// <summary>
        /// degree
        /// </summary>
        public int N;

        public Complex Eval(Complex x)
        {
            Complex y = p[N];
            for (int k = N-1; k > -1; k--)
            {
                y.Mul(x);
                y.Add(p[k]);
            }

            return y;
        }

        public Complex EvalD(Complex x)
        {
            Complex y = N * p[N];
            for (int k = N-1; k > 0; k--)
            {
                y.Mul(x);
                y.Add(k * p[k]);
            }

            return y;
        }

        public Complex[] AberthEhrlich()
        {
            Complex[] w = new Complex[N];
            Complex[] z = new Complex[N];
            bool[] b = new bool[N];

            // initial points
            float t = 0;

            for (int k = 0; k < N; k++)
            {
                t += 2f * Mathf.PI / N;
                z[k] = new Complex(1.1f * Mathf.Cos(t), 1.1f * Mathf.Sin(t));
            }

            // algorithm start

            int max_it = 25;
            bool stop;

            for (int i = 0; i < max_it; i++)
            {
                stop = true;
                for (int k = 0; k < N; k++)
                {
                    if (b[k]) continue;

                    Complex frac = Eval(z[k]);
                    frac.Div(EvalD(z[k]));

                    Complex srd = new Complex(0, 0);
                    for (int j = 0; j < N; j++)
                    {
                        if (k == j) continue;

                        Complex v = z[k] - z[j];
                        v.Rec();
                        srd.Add(v);
                    }

                    w[k] = frac;
                    frac.Mul(srd);
                    w[k].Div(Complex.realUnit - frac);
                    z[k].Sub(w[k]);

                    b[k] = w[k].SqAbs < Mathf.Epsilon * Mathf.Epsilon;
                    stop = stop && b[k];
                }

                if (stop) 
                {
                    //Debug.Log(i);
                    break; 
                }
            }

            return z;
        }

    }
}

using Assets.Scripts.MathAndGeometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// Represents a trigonometric polynomial with its coefficients.
        /// </summary>
        public class TrigPoly
        {
            public TrigPoly(float[] a, float[] b)
            {
                N = a.Length - 1;
                this.a = a;
                this.b = b;
                this.c = new Complex[2 * N + 1];

                ComputeComplexCoeffs();
            }

            public TrigPoly(Complex[] c)
            {
                N = (c.Length - 1)/2;
                this.c = c;
            }

            public TrigPoly(int N)
            {
                a = new float[N + 1];
                b = new float[N + 1];
                c = new Complex[2 * N + 1];
                this.N = N;
            }

            public void ComputeComplexCoeffs()
            {
                for (int k = -N; k < 0; k++)
                {
                    c[k + N] = new Complex(a[-k] / 2f, b[-k] / 2f);
                }

                c[N] = new Complex(a[0], 0);

                for (int k = 1; k < N + 1; k++)
                {
                    c[k + N] = new Complex(a[k] / 2f, -b[k] / 2f);
                }
            }

            public AlgPoly ToAlgPoly()
            {
                return new AlgPoly((Complex[])c.Clone());
            }

            public Vector2 this[int index]
            {
                get { return new Vector2(a[index], b[index]); }
                set { a[index] = value.x; b[index] = value.y; }
            }

            /// <summary>
            /// Coefficients for real representation
            /// </summary>
            public float[] a;
            public float[] b;

            /// <summary>
            /// Coefficients for complex representation
            /// </summary>
            public Complex[] c;

            public int N { get; private set; }

            /// <summary>
            /// Evaluate the trigonometric polynomial at a given point.
            /// </summary>
            /// <param name="t"></param>
            /// <returns> The value at t. </returns>
            public float Eval(float t)
            {
                float c1 = Mathf.Cos(t);
                float s1 = Mathf.Sin(t);
                float c = c1, s = s1;
                float y = a[0] + c1 * a[1] + s1 * b[1];
                for (int k = 2; k <= N; k++)
                {
                    float tmp = c;
                    c = c * c1 - s * s1;
                    s = s * c1 + tmp * s1;
                    y += a[k] * c + b[k] * s;
                }

                return y;
            }
        }
    }
}

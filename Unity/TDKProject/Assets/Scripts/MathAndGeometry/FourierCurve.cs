using Assets.Scripts.MathAndGeometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {        
        /// <summary>
        /// A parametric curve represented with trigonometric polynomials.
        /// </summary>
        public class FourierCurve
        {
            /// <summary>
            /// Constructing the curve which is approximating a polygon.
            /// </summary>
            /// <param name="polygon"> The polygon it approximates. </param>
            /// <param name="N"> The last index of the coefficients. </param>
            public FourierCurve(Polygon polygon, int N = 10)
            {
                LastInd = N;
                x = new TrigPoly(N);
                y = new TrigPoly(N);
                List<Vector2> vertices = new List<Vector2>();
                List<float> t = new List<float>();
                float length = 0;

                t.Add(0);
                vertices.Add(polygon[0]);

                int n = 1;
                for (int i = 1; i < polygon.Count; i++)
                {
                    float d = Vector2.Distance(polygon[i], vertices[n - 1]);
                    if (d > Mathf.Epsilon)
                    {
                        length += d;
                        t.Add(2 * Mathf.PI * length);
                        vertices.Add(polygon[i]);
                        n++;
                    }
                }

                t[0] /= length;
                for (int i = 1; i < n; i++)
                {
                    t[i] /= length;
                    x.a[0] += (t[i] - t[i - 1]) * (vertices[i].x + vertices[i - 1].x) / (4 * Mathf.PI);
                    y.a[0] += (t[i] - t[i - 1]) * (vertices[i].y + vertices[i - 1].y) / (4 * Mathf.PI);
                }

                for (int k = 1; k <= N; k++)
                {
                    x.a[k] = x.b[k] = y.a[k] = y.b[k] = 0;
                    float denom = (k * k * Mathf.PI);
                    float dt, dx, dy, dCos, dSin;
                    for (int i = 0; i < n - 1; i++)
                    {
                        dt = t[i + 1] - t[i];
                        dx = vertices[i + 1].x - vertices[i].x;
                        dy = vertices[i + 1].y - vertices[i].y;
                        dCos = Mathf.Cos(k * t[i + 1]) - Mathf.Cos(k * t[i]);
                        dSin = Mathf.Sin(k * t[i + 1]) - Mathf.Sin(k * t[i]);

                        x.a[k] += (dx / dt) * dCos / denom;
                        x.b[k] += (dx / dt) * dSin / denom;
                        y.a[k] += (dy / dt) * dCos / denom;
                        y.b[k] += (dy / dt) * dSin / denom;
                    }
                }

                x.ComputeComplexCoeffs();
                y.ComputeComplexCoeffs();

                transform = new Transform2D(new Vector2(0, 0), 1, 0);

                AABB = polygon.AABB;
            }

            public TrigPoly x;
            public TrigPoly y;
            private Transform2D transform;

            public int LastInd { get; private set; }
            public Vector2[] AABB { get; private set; }
            public Vector2 AABBMin { get { return AABB[0]; } }
            public Vector2 AABBMax { get { return AABB[2]; } }
            public Vector2 AABBCenter { get { return 0.5f * (AABB[0] + AABB[2]); } }

            /// <summary>
            /// Setting the transform for the curve, when convenient coordinate system needed.
            /// </summary>
            /// <param name="translate"></param>
            /// <param name="cosAlpha"></param>
            /// <param name="sinAlpha"></param>
            public void SetTransform(Vector2 translate, float cosAlpha, float sinAlpha)
            {
                transform.position = translate;
                transform.rotation.Set(cosAlpha, sinAlpha);
            }

            //public void SetTransform(Vector2 translate, Complex rotate)
            //{
            //    transform.Translation = translate;
            //    transform.Orientation = rotate;
            //}

            /// <summary>
            /// Resets the adjusted transform.
            /// </summary>
            public void ResetTransform()
            {
                transform.position.Set(0, 0);
                transform.rotation.Set(1, 0);
            }

            //public void ResetTransform()
            //{
            //    transform.Translation = Vector2.zero;
            //    transform.Orientation = Complex.identity;
            //}

            public AlgPoly Y2AlgPoly()
            {              
                int N = LastInd;
                AlgPoly outPoly = new AlgPoly(2 * N);

                for (int k = -N; k < N+1; k++)
                {
                    outPoly.p[k + N] = transform.rotation.y * x.c[k + N] + transform.rotation.x * y.c[k + N];
                }

                float X = transform.position.x + x.a[0];
                float Y = transform.position.y + y.a[0];
                outPoly.p[N] = new Complex(transform.rotation.y * X + transform.rotation.x * Y, 0f);

                return outPoly;
            }

            /// <summary>
            /// Evaluates the curve at a given argument.
            /// </summary>
            /// <param name="t"></param>
            /// <returns> The (x(t),y(t)) point. </returns>
            public Vector2 Eval(float t)
            {
                return new Vector2(EvalX(t), EvalY(t));
            }

            /// <summary>
            /// Evaluate the x component of the transformed function derivative.
            /// </summary>
            /// <param name="t"></param>
            /// <returns> The x'(t) value. </returns>
            public float EvalTransformedDerivativeX(float t)
            {
                float Val = 0;
                for (int k = 1; k <= LastInd; k++)
                {
                    Val += (x.b[k] * transform.rotation.x - y.b[k] * transform.rotation.y) * k * Mathf.Cos(k * t) -
                           (x.a[k] * transform.rotation.x - y.a[k] * transform.rotation.y) * k * Mathf.Sin(k * t);
                }

                return Val;
            }

            /// <summary>
            /// Evaluate the y component of the transformed function derivative.
            /// </summary>
            /// <param name="t"></param>
            /// <returns> The x'(t) value. </returns>
            public float EvalTransformedDerivativeY(float t)
            {
                float Val = 0;
                for (int k = 1; k < LastInd; k++)
                {
                    Val += (x.b[k] * transform.rotation.y + y.b[k] * transform.rotation.x) * k * Mathf.Cos(k * t) -
                           (x.a[k] * transform.rotation.y + y.a[k] * transform.rotation.x) * k * Mathf.Sin(k * t);
                }

                return Val;
            }


            public float EvalTransformedX(float t)
            {
                float ValX = x.Eval(t) + transform.position.x;
                float ValY = y.Eval(t) + transform.position.y;
                float Val = ValX * transform.rotation.x - ValY * transform.rotation.y;
                return Val;
            }

            public float EvalTransformedY(float t)
            {
                float ValX = x.Eval(t) + transform.position.x;
                float ValY = y.Eval(t) + transform.position.y;
                float Val = ValX * transform.rotation.y + ValY * transform.rotation.x;
                return Val;
            }


            public float EvalX(float t)
            {
                return x.Eval(t);
            }

            public float EvalY(float t)
            {
                return y.Eval(t);
            }
        }
    }
}

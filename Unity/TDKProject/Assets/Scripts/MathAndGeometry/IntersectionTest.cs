using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Line = UnityEngine.Ray;
using System;
using Random = UnityEngine.Random;
using Assets.Scripts.MathAndGeometry;

namespace Model
{
    namespace MathAndGeometry
    {
        enum PrincipalAxes
        {
            X,
            Y,
            Z
        }

        /// <summary>
        /// Implementing the intersection algorithms between geometric elements.
        /// </summary>
        public static class IntersectionTest
        {
            /// <summary>
            /// Implementing Sphere-Triangle intersection algorithm with closest point calculations.
            /// </summary>
            /// <param name="sphereCenter"> The center of the given sphere in world coordinates. </param>
            /// <param name="sphereRadius"> The radius of the given sphere. </param>
            /// <param name="triangle"> The triangle object. </param>
            /// <returns>
            /// True if there is intersection, False otherwise.
            /// </returns>
            public static bool SphereVsTriangle(Vector3 sphereCenter, float sphereRadius, Triangle triangle)
            {
                return Vector3.Distance(ClosestPtPointToTriangle(sphereCenter, triangle), sphereCenter) <= sphereRadius;
            }


            /// <summary>
            /// Implementing the closest point algorithm.
            /// </summary>
            /// <param name="q"> The given point. </param>
            /// <param name="tri"> The given triangle. </param>
            /// <returns>
            /// The point on the triangle with minimal distance from the q.
            /// </returns>
            private static Vector3 ClosestPtPointToTriangle(Vector3 q, Triangle tri)
            {
                float ab = Vector3.SqrMagnitude(tri[0] - tri[1]);
                float ac = Vector3.SqrMagnitude(tri[0] - tri[2]);
                float bc = Vector3.SqrMagnitude(tri[1] - tri[2]);
                float aq = Vector3.SqrMagnitude(tri[0] - q);
                float bq = Vector3.SqrMagnitude(tri[1] - q);
                float cq = Vector3.SqrMagnitude(tri[2] - q);
                float b = Vector3.SqrMagnitude(tri[1]);

                float alpha = ac;
                float beta = ab;
                float gamma = ab + ac - bc;
                float delta = cq - aq - ac;
                float eps = bq - aq - ab;
                float zeta = aq + b;

                float det = 4 * alpha * beta - gamma * gamma;
                float s = (-2 * beta * delta + eps * gamma) / det;
                float t = (-2 * alpha * eps + delta * gamma) / det;
                if (0 <= s && s <= 1 && 0 <= t && t <= 1 && t + s <= 1)
                {
                    return (1 - s - t) * tri[0] + t * tri[1] + s * tri[2];
                }
                else
                {
                    float d1, d2, d3;

                    // s = 0
                    t = -eps / (2 * beta);
                    if (t < 0) { t = 0; d1 = zeta; }
                    else if (t > 1) { t = 1; d1 = beta + eps + zeta; }
                    else d1 = zeta + eps * t;

                    // t = 0
                    s = -delta / (2 * alpha);
                    if (s < 0) { s = 0; d2 = zeta; }
                    else if (s > 1) { s = 1; d2 = alpha + delta + zeta; }
                    else d2 = zeta + delta * s;


                    // s + t = 1
                    float denom = alpha + beta - gamma;
                    float u = (2 * alpha - gamma + delta - eps) / (2 * denom);
                    if (u < 0) u = 0;
                    else if (u > 1) u = 1;

                    d3 = u * u * denom + u * (-2 * alpha + gamma - delta + eps) + alpha + delta + zeta;

                    if (d1 <= d2 && d1 <= d3)
                        return (1 - t) * tri[0] + t * tri[1];
                    if (d2 <= d1 && d2 <= d3)
                        return (1 - s) * tri[0] + s * tri[2];
                    else
                        return u * tri[1] + (1 - u) * tri[2];
                }
            }

            /// <summary>
            /// Implements the PlaneVsTriangle intersection algorithm.
            /// </summary>
            /// <param name="plane"> The given plane. </param>
            /// <param name="triangle"> The given triangle </param>
            /// <param name="vec1"> Out parameter which holds on end of the intersection segment. </param>
            /// <param name="vec2"> Out parameter which holds the other end of the intersection segment. </param>
            /// <returns>
            /// True if the two objects are intersecting.
            /// </returns>
            public static bool PlaneVsTriangle(Plane plane, Triangle triangle, out Vector3 vec1, out Vector3 vec2)
            {
                bool firstFound = false;
                vec1 = vec2 = new Vector3(0, 0, 0);
                if (!Mathf.Approximately(Mathf.Abs(Vector3.Dot(plane.Normal, triangle.Normal)), 1))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector3[] segment = new Vector3[] { triangle[i % 3], triangle[(i + 1) % 3] };

                        if (Mathf.Approximately(Vector3.Dot(segment[1] - segment[0], plane.Normal), 0))
                        {
                            if (Mathf.Approximately(Vector3.Dot(segment[0], plane.Normal), plane.PlaneConst))
                            {
                                vec1 = segment[0];
                                vec2 = segment[1];
                                return true;
                            }
                        }
                        else
                        {
                            float t = (plane.PlaneConst - Vector3.Dot(segment[0], plane.Normal)) /
                                       Vector3.Dot(segment[1] - segment[0], plane.Normal);

                            if (0 <= t && t <= 1)
                            {
                                if (firstFound = !firstFound)
                                {
                                    vec1 = segment[0] + t * (segment[1] - segment[0]);
                                }
                                else
                                {
                                    vec2 = segment[0] + t * (segment[1] - segment[0]);
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }

            /// <summary>
            /// Searches for collision points between a rectangle and a line segment
            /// on the 2D plane.
            /// </summary>
            /// <param name="Rect"></param>
            /// <param name="translate"></param>
            /// <param name="abLength"></param>
            /// <param name="cosPhi"></param>
            /// <param name="sinPhi"></param>
            /// <returns></returns>
            public static bool SegmentVsRectangle(Vector2[] Rect, Transform2D segTrf, float abLength
                /*Vector2[] Rect, Vector2 translate, float abLength, float cosPhi, float sinPhi*/)
            {
                Vector2[] TrdRect = new Vector2[4];

                for (int i = 0; i < 4; i++)
                {
                    //TrRect[i] = new Vector2(
                    //    cosPhi * (Rect[i].x + translate.x) + sinPhi * (Rect[i].y + translate.y),
                    //    -sinPhi * (Rect[i].x + translate.x) + cosPhi * (Rect[i].y + translate.y)
                    //    );

                    TrdRect[i] = Complex.Conjugated(segTrf.rotation) * (Rect[i] + segTrf.position);
                }
                

                bool smallerFound = false;
                bool biggerFound = false;

                for (int i = 0; i < 4; i++)
                {
                    if (TrdRect[i].y * TrdRect[(i + 1) % 4].y < 0)
                    {
                        float zeroSite = -(TrdRect[(i + 1) % 4].x - TrdRect[i].x) / (TrdRect[(i + 1) % 4].y - TrdRect[i].y) * TrdRect[i].y + TrdRect[i].x;
                        if (0 <= zeroSite && zeroSite <= abLength)
                        {
                            return true;
                        }
                        else if (zeroSite < 0)
                        {
                            smallerFound = true;
                        }
                        else if (abLength < zeroSite)
                        {
                            biggerFound = true;
                        }
                    }
                }
                return smallerFound && biggerFound;
            }

            /// <summary>
            /// Searches for intersection between a line segment
            /// and a curve represented with trigonometric polynomials.
            /// It uses simulated annealing, with the support of
            /// gradient descent.
            /// </summary>
            /// <param name="curve"></param>
            /// <param name="vecA"></param>
            /// <param name="vecB"></param>
            /// <param name="abLength"></param>
            /// <param name="cosTheta"></param>
            /// <param name="sinTheta"></param>
            /// <param name="up"></param>
            /// <param name="contactPt"></param>
            /// <param name="penetration"></param>
            /// <returns>
            /// True if there's intersection, false otherwise.
            /// </returns>
            public static bool SegmentVsCurveSA(
                FourierCurve curve,
                Vector2 vecA,
                Vector2 vecB,
                float abLength,
                Complex abRotation,
                Vector2 up,
                out Vector2 contactPt,
                out float penetration,
                int annealingTemperature,
                float limit
                )
            {
                float ySign = Mathf.Sign(-abRotation.y * (up.x - vecA.x) + abRotation.x * (up.y - vecA.y));

                if (ySign < 0)
                    curve.SetTransform(-vecB, -abRotation.x, abRotation.y);
                else
                    curve.SetTransform(-vecA, abRotation.x, -abRotation.y);

                // find global min on the curve
                SimulatedAnnealing2D(
                    curve,
                    abLength,
                    out float mint,
                    out float minx,
                    out float miny,
                    annealingTemperature,
                    limit);

                penetration = -miny;
                if (miny <= safetyMargin && 0 <= minx && minx <= abLength && miny >= -limit + safetyMargin)
                {
                    contactPt = curve.Eval(mint);
                    curve.ResetTransform();
                    return true;
                }

                contactPt = Vector2.zero;
                curve.ResetTransform();
                return false;
            }

            /// <summary>
            /// Implementation of  the simulated annealing algorithm designed for the Segment-Curve problem. 
            /// </summary>
            /// <param name="curve"></param>
            /// <param name="d"></param>
            /// <param name="mint"></param>
            /// <param name="minx"></param>
            /// <param name="miny"></param>
            /// <param name="initTemp"></param>
            /// <remarks>
            /// The minimalised function is the y component of the curve.
            /// </remarks>
            private static void SimulatedAnnealing2D(FourierCurve curve, float d, out float mint, out float minx, out float miny, int initTemp, float limit)
            {
                float currt = 2 * Mathf.PI * Random.value;
                float currx = curve.EvalTransformedX(currt);
                float curry = curve.EvalTransformedY(currt);
                float currf = curry;

                float penalty = 0;
                if (currx < 0) penalty = Mathf.Exp(-currx);
                else if (currx > d) penalty = Mathf.Exp(currx - d);

                float absCurry = Mathf.Abs(curry);
                if (absCurry > limit) penalty += PenaltyFunc(absCurry);

                currf += penalty;

                mint = currt;
                minx = currx;
                miny = curry;
                float minf = currf;

                int temp = initTemp;

                float newt, newx, newy, newf, tempInitTempSqRatio, absNewy;
                while (temp > 0)
                {
                    // calculating the new candidate
                    tempInitTempSqRatio = ((float)temp * temp) / ((float)initTemp * initTemp);

                    if (temp > initTemp * 0.25)
                        newt = currt + Mathf.PI * tempInitTempSqRatio * Mathf.Sign(Random.value - 0.5f);
                    else // gradient descent
                        newt = mint - tempInitTempSqRatio * curve.EvalTransformedDerivativeY(mint);

                    // using periodicity
                    if (newt < 0) newt += 2 * Mathf.PI;
                    else if (newt > 2 * Mathf.PI) newt -= 2 * Mathf.PI;

                    // calculating value at the new candidate
                    newx = curve.EvalTransformedX(newt);
                    newy = curve.EvalTransformedY(newt);
                    newf = newy;

                    // applying penalties
                    if (newx < 0) penalty = PenaltyFunc(newx);
                    else if (newx > d) penalty = PenaltyFunc(newx - d);

                    absNewy = Mathf.Abs(newy);
                    if (absNewy > limit) penalty += PenaltyFunc(absNewy);

                    newf += penalty;

                    if (minf > newf)
                    {
                        mint = currt = newt;
                        minx = currx = newx;
                        miny = curry = newy;
                        minf = currf = newf;
                    }
                    else if (Mathf.Exp((currf - newf) / temp) > Random.value)
                    {
                        currt = newt;
                        currx = newx;
                        curry = newy;
                        currf = newf;
                    }

                    --temp;
                }
            }

            public static bool SegmentVsCurveAE(
                FourierCurve curve,
                Vector2 vecA,
                Vector2 vecB,
                float abLength,
                Complex abRotation,
                Vector2 up,
                List<Vector2> intersections
                )
            {
                float ySign = Mathf.Sign(-abRotation.y * (up.x - vecA.x) + abRotation.x * (up.y - vecA.y));

                if (ySign < 0)
                    curve.SetTransform(-vecB, -abRotation.x, abRotation.y);
                else
                    curve.SetTransform(-vecA, abRotation.x, -abRotation.y);

                AlgPoly algPoly = curve.Y2AlgPoly();
                Complex[] roots = algPoly.AberthEhrlich();

                for (int i = 0; i < roots.Length; i++)
                {
                    //Debug.Log(roots[i].Abs);
                    if (Mathf.Abs(roots[i].Abs -1f) < Mathf.Epsilon)
                    {
                        float arg = roots[i].Arg;
                        Vector2 intxy = curve.Eval(arg);

                        intersections.Add(intxy);
                    }
                }
                
                return intersections.Count > 0;
            }



            private static float PenaltyFunc(float t) => 1 + t * t * t * t;

            public static bool SphereVsLine(Sphere sphere, Line line, out float signedDist)
            {
                float b = 2 * Vector3.Dot(line.origin - sphere.CenterW, line.direction);
                float c = Vector3.SqrMagnitude(line.origin - sphere.CenterW) - sphere.Radius * sphere.Radius;
                float D = b * b - 4 * c;

                if (D >= 0)
                {
                    float t1 = (-b + Mathf.Sqrt(D)) / 2;
                    float t2 = (-b - Mathf.Sqrt(D)) / 2;

                    signedDist = Mathf.Abs(t1) < Mathf.Abs(t2) ? t1 : t2;
                    return true;
                }

                signedDist = float.PositiveInfinity;
                return false;
            }

            public static bool SphereVsRay(Sphere sphere, Ray ray, out float signedDist)
            {
                float b = 2 * Vector3.Dot(ray.origin - sphere.CenterW, ray.direction);
                float c = Vector3.SqrMagnitude(ray.origin - sphere.CenterW) - sphere.Radius * sphere.Radius;
                float D = b * b - 4 * c;

                if (D >= 0)
                {
                    float t1 = (-b + Mathf.Sqrt(D)) / 2;
                    float t2 = (-b - Mathf.Sqrt(D)) / 2;

                    if (t1 > 0 && t2 > 0)
                    {
                        signedDist = Mathf.Abs(t1) < Mathf.Abs(t2) ? t1 : t2;
                        return true;
                    }
                }

                signedDist = float.PositiveInfinity;
                return false;
            }

            public static bool PlaneVsRay(Plane plane, Ray ray, out float dist)
            {
                float denom = Vector3.Dot(ray.direction, plane.Normal);

                if (!Mathf.Approximately(denom, 0))
                {
                    float nom = Vector3.Dot(plane.RefPoint - ray.origin, plane.Normal);
                    dist = nom / denom;

                    return dist > 0;
                }

                dist = 0;
                return false;
            }

            public static bool PointIn2DAABB(Vector2 pt, Vector2[] aabb)
            {
                return 
                    aabb[0].x <= pt.x && pt.x <= aabb[2].x &&
                    aabb[0].y <= pt.y && pt.y <= aabb[2].y;
            }

            public static bool PointInAABB(Vector3 pt, Vector3 aabbMin, Vector3 aabbMax)
            {
                return 
                    aabbMin.x <= pt.x && pt.x <= aabbMax.x &&
                    aabbMin.y <= pt.y && pt.y <= aabbMax.y &&
                    aabbMin.z <= pt.z && pt.z <= aabbMax.z;
            }

            /// <summary>
            /// An efficient triangle-triangle intersection test, based on Tomas Möller's article:
            /// https://fileadmin.cs.lth.se/cs/Personal/Tomas_Akenine-Moller/pubs/tritri.pdf
            /// </summary>
            /// <param name="T1"></param>
            /// <param name="T2"></param>
            /// <returns></returns>
            public static bool MollerTriangleVsTriangle(Triangle T1, Triangle T2, out Vector3 w1, out Vector3 w2, out int contactCount)
            {
                w1 = Vector3.zero;
                w2 = Vector3.zero;
                contactCount = 0;

                // Are vertices of T2 on the same side of T1's plane?
                float s20 = Mathf.Sign(Vector3.Dot(T1.Normal, T2[0] - T1.RefPoint /*T1[0] - T2[0]*/));
                float s21 = Mathf.Sign(Vector3.Dot(T1.Normal, T2[1] - T1.RefPoint /*T1[0] - T2[1]*/));
                float s22 = Mathf.Sign(Vector3.Dot(T1.Normal, T2[2] - T1.RefPoint /*T1[0] - T2[2]*/));

                if (s20 * s21 > 0 && s21 * s22 > 0) return false;

                // Are vertices of T1 on the same side of T2's plane?
                float s10 = Mathf.Sign(Vector3.Dot(T2.Normal, T1[0] - T2.RefPoint /*T2[0] - T1[0]*/));
                float s11 = Mathf.Sign(Vector3.Dot(T2.Normal, T1[1] - T2.RefPoint /*T2[0] - T1[1]*/));
                float s12 = Mathf.Sign(Vector3.Dot(T2.Normal, T1[2] - T2.RefPoint /*T2[0] - T1[2]*/));

                if (s10 * s11 > 0 && s11 * s12 > 0) return false;

                // direction vector of the plane-plane intersection line
                Vector3 d = Vector3.Cross(T1.Normal, T2.Normal);

                // Are the two planes parallel?
                if (Mathf.Approximately(d.magnitude, 0)) return false;

                // Which principal coordinate axis is the most parallel to d?
                int principalAxis;
                if (Mathf.Abs(d.x) >= Mathf.Abs(d.y) && Mathf.Abs(d.x) >= Mathf.Abs(d.z)) principalAxis = 0;
                else if (Mathf.Abs(d.y) >= Mathf.Abs(d.x) && Mathf.Abs(d.y) >= Mathf.Abs(d.z)) principalAxis = 1;
                else principalAxis = 2;

                // Find T1's intersection with T2's plane
                float a1, b1;
                Vector3 u11, u12;
                if(s10 * s11 < 0 && s10 * s12 < 0)
                {
                    GetInterval(T2,T1[0], T1[1], T1[2], principalAxis, out a1, out b1, out u11,out u12);
                    if (s10 < 0) { w1 = T1[0]; contactCount = 1; }
                    else { w1 = T1[1]; w2 = T1[2]; contactCount = 2; }
                }
                else if(s11 * s10 < 0 && s11 * s12 < 0)
                {
                    GetInterval(T2, T1[1], T1[2], T1[0], principalAxis, out a1, out b1, out u11, out u12);
                    if (s11 < 0) { w1 = T1[1]; contactCount = 1; }
                    else { w1 = T1[2]; w2 = T1[0]; contactCount = 2; }
                }
                else /*if (s12 * s10 < 0 && s12 * s11 < 0)*/
                {
                    GetInterval(T2, T1[2], T1[0], T1[1], principalAxis, out a1, out b1, out u11, out u12);
                    if (s12 < 0) { w1 = T1[2]; contactCount = 1; }
                    else { w1 = T1[0]; w2 = T1[1]; contactCount = 2; }
                }
                

                // Find T2's intersection with T1's plane
                float a2, b2;
                Vector3 u21, u22;
                if(s20 * s21 < 0 && s20 * s22 < 0)
                {                    
                    GetInterval(T1, T2[0], T2[1], T2[2], principalAxis, out a2, out b2, out u21, out u22);
                }
                else if (s21 * s20 < 0 && s21 * s22 < 0)
                {                   
                    GetInterval(T1, T2[1], T2[2], T2[0], principalAxis, out a2, out b2, out u21, out u22);
                }
                else /*if (s22 * s20 < 0 && s22 * s21 < 0)*/
                {
                    GetInterval(T1, T2[2], T2[0], T2[1], principalAxis, out a2, out b2, out u21, out u22);
                }

                // perform the interval overlap test
                if (a2 < b1 && a1 < b2)
                {
                    // set the contact points as the intersected segment between the two triangle
                    //if (b2 < b1) w2 = u22;
                    //else w2 = u12;

                    //if (a1 < a2) w1 = u21;
                    //else w1 = u11;

                    return true;
                }

                return false;
            }



            /// <summary>
            /// Performing two plane-segment test, then returns the two intersected point
            /// with the interval projected onto a principal axis.
            /// </summary>
            /// <param name="T"> ... </param>
            /// <param name="v1"> the vertex which is on the other side of the plane than v2 and v3</param>
            /// <param name="v2"> ... </param>
            /// <param name="v3"> ... </param>
            /// <param name="principalAxis"> where to project the interval </param>
            /// <param name="a">begin of the interval</param>
            /// <param name="b">end of the interval</param>
            /// <param name="u1"></param>
            /// <param name="u2"></param>
            private static void GetInterval(
                IPlane T, Vector3 v1, Vector3 v2, Vector3 v3,
                int principalAxis, out float a,out float b,
                out Vector3 u1, out Vector3 u2
                )
            {         
                u1 = v1 + Vector3.Dot(/*T[0]*/ T.RefPoint - v1, T.Normal) / Vector3.Dot(v2 - v1, T.Normal) * (v2 - v1);
                u2 = v1 + Vector3.Dot(/*T[0]*/ T.RefPoint - v1, T.Normal) / Vector3.Dot(v3 - v1, T.Normal) * (v3 - v1);

                if(u1[principalAxis] > u2[principalAxis])
                {
                    Vector3 tmp = u1;
                    u1 = u2;
                    u2 = tmp;          
                }

                a = u1[principalAxis];
                b = u2[principalAxis];
            }

            private static void GetInterval(
                IPlane T, Vector3 v1, Vector3 v2, Vector3 v3,
                int principalAxis, out float a, out float b
                )
            {
                GetInterval(T, v1, v2, v3, principalAxis, out a, out b, out Vector3 u1, out Vector3 u2);
            }


            public static bool TriangleVsRay(Triangle T, Line line)
            {
                return TriangleVsLine(T, line, out Vector3 intersection, out float lambda);
            }

            public static bool TriangleVsLine(Triangle T, Line line, out Vector3 intersection, out float lambda)
            {
                Vector3 d1 = T[1] - T[0];
                Vector3 d2 = T[2] - T[0];
                Vector3 n = Vector3.Cross(d1, d2);
                float det = Vector3.Dot(-line.direction, n);
                if (!Mathf.Approximately(det, 0))
                {
                    Vector3 b = line.origin -T[0];
                    Vector3 u = Vector3.Cross(b, line.direction);

                    lambda = Vector3.Dot(b, n)/det;
                    float mu1 = Vector3.Dot(d2, u)/det;
                    float mu2 = Vector3.Dot(-d1, u)/det;

                    if(0 <= mu1 && 0 <= mu2 && mu1 + mu2 <= 1)
                    {
                        intersection = line.origin + lambda * line.direction; /*(1 - mu1 - mu2) * T[0] + mu1 * T[1] + mu2 * T[2]*/;
                        return true;
                    }
                }

                intersection = Vector3.positiveInfinity;
                lambda = float.PositiveInfinity;
                return false;
            }

            public static bool IsVertexUnderTriangle(Triangle T, Vector3 vert, out Vector3 projection, out float dist)
            {
                // Step 0°: is the vertex under the triangle's plane?
                if(Vector3.Dot(vert - T.RefPoint,T.Normal) < 0)
                {
                    Line line = new Line(vert, -T.Normal);
                    if(TriangleVsLine(T, line, out projection, out float lambda))
                    {
                        dist = Mathf.Abs(lambda);
                        return true;
                    }
                }

                projection = Vector3.positiveInfinity;
                dist = float.PositiveInfinity;
                return false;
            }


            /// <summary>
            /// Tests if a triangle and a convex polyhedron can be separated by a plane.
            /// See: https://citeseerx.ist.psu.edu/viewdoc/summary?doi=10.1.1.121.9217
            /// </summary>
            /// <returns> Returns true if a separation axis is found, false otherwise. </returns>    
            public static bool ChungWangTest(
                IConvexShape A,
                IConvexShape B,
                Vector3 startVec,
                int iterations,
                out Vector3 p,
                out Vector3 q,
                out Vector3 S)
            {
                S = startVec;
                p = q = Vector3.zero; // Make C# happy...

                for (int k = 0; k < iterations; k++)
                {
                    p = A.SupportingVertex(S);
                    q = B.SupportingVertex(-S);

                    if (Vector3.Dot(S, q - p) > epsTol) return true; // SAT passed
                    else
                    {
                        Vector3 r = (q - p).normalized;
                        S = (S - 2 * Vector3.Dot(r, S) * r).normalized;
                    }
                }

                return false;
            }

            /// <summary>
            /// Implementation of separating axis GJK algorithm
            /// </summary>
            /// <param name="A"></param>
            /// <param name="B"></param>
            /// <param name="startVec"></param>
            /// <param name="maxIt"></param>
            /// <param name="W"></param>
            /// <param name="AW"></param>
            /// <param name="BW"></param>
            /// <returns> 
            /// true if the distance between two convex shape is 0,
            /// false if separating axis exists between A and B.
            /// </returns>
            public static bool SAGJK(
                IConvexShape A,
                IConvexShape B,
                Vector3 startVec,
                int maxIt,
                out Vector3[] W,
                out Vector3[] AW,
                out Vector3[] BW)
            {
                Vector3 v = startVec;
                W = new Vector3[4];
                AW = new Vector3[4];
                BW = new Vector3[4];
                uint y = 0x0;
                float[,] Delta = new float[16, 4];
                float[] lambda;

                for (int i = 0; i < maxIt; i++)
                {
                    Vector3 aw = A.SupportingVertex(-v);
                    Vector3 bw = B.SupportingVertex(v);
                    Vector3 w = aw - bw;

                    // check if v is considerable as separating axis
                    if (gjkIsIn(W, w) && Vector3.Dot(v, w) > -10f*Mathf.Epsilon)
                        return false;

                    gjkPrepareSimplex(W, AW, BW, ref y, w, aw, bw, out uint wbit);

                    v = JohnsonsDistance(W, ref y, wbit, Delta, out lambda);

                    // check if v is considerable as 0
                    if (y == 0xf || v.magnitude <= Mathf.Epsilon)
                    {
                        //Debug.Log("y: " + y.ToString());
                        //Debug.Log("v: " + v.ToString());
                        Debug.Log(W[0] + "\n" + W[1] + "\n" + W[2] + "\n" + W[3] + "\n\n");
                        return true;
                    }
                }

                return false;
            }

            public static bool GJK(
                IConvexShape A,
                IConvexShape B,
                Vector3 intA,
                Vector3 intB,
                int maxIt,
                out uint y,
                out Vector3[] W,
                out Vector3[] AW,
                out Vector3[] BW,
                out Vector3 v,
                out Vector3 a,
                out Vector3 b)
            {
                v = intA - intB;
                W = new Vector3[4];
                AW = new Vector3[4];
                BW = new Vector3[4];
                y = 0x0;
                float[,] Delta = new float[16, 4];
                float[] lambda = new float[] { 0, 0, 0 };

                for (int i = 0; i < maxIt; i++)
                {
                    Vector3 aw = A.SupportingVertex(-v);
                    Vector3 bw = B.SupportingVertex(v);
                    Vector3 w = aw - bw;

                    // check if v is considerable as separating axis
                    float vd2 = Vector3.Dot(v, v);
                    if (gjkIsIn(W, w) && vd2 - Vector3.Dot(v,w) <= epsTol * vd2)
                        break;

                    gjkPrepareSimplex(W, AW, BW, ref y, w, aw, bw, out uint wbit);

                    v = JohnsonsDistance(W, ref y, wbit, Delta, out lambda);

                    // check if v is considerable as 0
                    if (y == 0xf || v.magnitude <= epsTol)
                        break;
                }

                a = lambda[0] * AW[0] + lambda[1] * AW[1] + lambda[2] * AW[2];
                b = lambda[0] * BW[0] + lambda[1] * BW[1] + lambda[2] * BW[2];

                return y == 0xf || v.magnitude < epsTol;
            }



            private static bool gjkIsIn(Vector3[] W, Vector3 w)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Vector3.Distance(W[i], w) <= epsTol)
                        return true;
                }
                return false;
            }

            private static void gjkPrepareSimplex(
                Vector3[] W, Vector3[] AW, Vector3[] BW,
                ref uint y, Vector3 w, Vector3 aw, Vector3 bw,
                out uint wbit)
            {
                wbit = 0x1;
                for (int i = 0; i < 4; i++)
                {
                    if((wbit & y) == 0x0)
                    {
                        W[i] = w;
                        AW[i] = aw;
                        BW[i] = bw;
                        return;
                    }

                    wbit = wbit << 1;
                }
            }

            private static Vector3 JohnsonsDistance(Vector3[] W, ref uint y, uint w, float[,] Delta, out float[] lambda)
            {
                // TODO: implementation

                uint yw = y | w;

                // Compute determinants for all subsets containing w
                for (uint x = 0; x <= y; x++)
                {
                    uint xw = x | w;

                    // check if xw is a subset of yw
                    if ((xw & yw) != xw) continue;

                    uint b = 0x1;
                    for (int i = 0; i < 4; i++)
                    {
                        if ((xw & b) == 0)
                        {
                            //Delta[xw, i] = 0f;
                            b = b << 1;
                            continue;
                        }

                        Delta[xw, i] = johnsonsCalcDelta(xw, i, Delta, yw, W);

                        b = b << 1;
                    }
                }

                // Find valid subset
                for (uint x = 0; x <= y; x++)
                {
                    if ((x & y) != x) continue;
            
                    if (johnsonsIsValid(x | w, Delta, yw))
                    {
                        y = x | w;
                        break;
                    }
                }

                float denom = Delta[y, 0] + Delta[y, 1] + Delta[y, 2] + Delta[y, 3];
                lambda = new float[]{ Delta[y,0]/denom, Delta[y,1]/denom, Delta[y,2]/denom, Delta[y,3]/denom};

                return W[0] * lambda[0] + W[1] * lambda[1] + W[2] * lambda[2] + W[3] * lambda[3];
            }

            private static float johnsonsCalcDelta(uint x, int i, float[,] Delta, uint bits, Vector3[] Y)
            {
                if (x > 0 && (x & x - 1) == 0 && (x & bits) == x)
                    return 1f;

                float deltaXi = 0f;

                uint z = x & (~(0x1u << i));

                int k = -1;
                uint b = 0x1;
                for (int j = 0; j < 4; j++)
                {
                    if((z & b) != 0)
                    {
                        if (k == -1) k = j;

                        deltaXi += Delta[z, j] * Vector3.Dot(Y[k] - Y[i], Y[j]);
                    }

                    b = b << 1;
                }

                return deltaXi;
            }

            private static bool johnsonsIsValid(uint x, float[,] Delta, uint bits)
            {
                uint b = 0x1;
                for (int j = 0; j < 4; j++)
                {
                    if ((bits & b) == 0x0)
                    {
                        b = b << 1;
                        continue;
                    }

                    if((x & b) != 0x0)
                    {
                        if (Delta[x, j] < 0) return false;
                    }
                    else
                    {
                        if (Delta[x | b, j] > 0) return false;
                    }

                    b = b << 1;
                }

                return true;
            }

            /// <summary>
            /// Implementation of the Minkowsky Portal Refinement algorithm (Gary Snethen)
            /// http://xenocollide.snethen.com/
            /// </summary>
            /// <param name="A"> Convex object A </param>
            /// <param name="B"> Convex object B </param>
            /// <param name="aw0"> A deep internal point from A (preferably the center of mass) </param>
            /// <param name="bw0"> A deep internal point from B (preferably the center of mass) </param>
            /// <param name="v"> v = a - b, the penetration vector. </param>
            /// <param name="a"> Contact point of A </param>
            /// <param name="b"> Contact point of B </param>
            /// <returns> true if there's intersection between the two object </returns>
            public static bool MPR(
                IConvexShape A,
                IConvexShape B,
                Vector3 aw0,
                Vector3 bw0,
                out Vector3 v,
                out Vector3 a,
                out Vector3 b)
            {
                bool hit = false;

                // --- Phase 1: POrtal discovery ---

                Vector3 w0 = aw0 - bw0;

                Vector3[] AW = new Vector3[3];
                Vector3[] BW = new Vector3[3];
                Vector3[] W = new Vector3[3];

                AW[0] = A.SupportingVertex(-w0);
                BW[0] = B.SupportingVertex(w0);
                W[0] = AW[0] - BW[0];

                Vector3 normal = Vector3.Cross(W[0], w0);
                AW[1] = A.SupportingVertex(normal);
                BW[1] = B.SupportingVertex(-normal);
                W[1] = AW[1] - BW[1];

                normal = Vector3.Cross(W[2] - W[0], W[1] - W[0]);
                AW[2] = A.SupportingVertex(normal);
                BW[2] = B.SupportingVertex(-normal);
                W[2] = AW[2] - BW[2];

                int wrong = -1;
                int max_it = 10;

                while(max_it-- > 0)
                {
                    wrong = OriginVsPortalCandidate(w0, W, out normal);

                    if (wrong < 0) break;

                    AW[wrong] = A.SupportingVertex(normal);
                    BW[wrong] = B.SupportingVertex(-normal);
                    W[wrong] = AW[wrong] - BW[wrong];
                }

                // --- Phase 2: Portal refinement ---
                max_it = 14;
                
                while(max_it-- > 0)
                {
                    normal = Vector3.Cross(W[2] - W[0], W[1] - W[0]);
                    float sgnO = Mathf.Sign(Vector3.Dot(normal, -W[0]));
                    float sgnw0 = Mathf.Sign(Vector3.Dot(normal, w0 - W[0]));

                    if(sgnO * sgnw0 > 0)
                    {
                        hit = true;

                        normal *= -sgnO;
                    }
                    else
                    {
                        normal *= sgnO;
                    }

                    Vector3 aw4 = A.SupportingVertex(normal);
                    Vector3 bw4 = B.SupportingVertex(-normal);
                    Vector3 w4 = aw4 - bw4;

                    // check if origin lies outside of the support plane
                    if(Vector3.Dot(normal,-w4) > 0)
                    {
                        break;
                    }

                    // check if support plane is close enough to the portal
                    if (Vector3.Dot(normal,w4 - W[0])/normal.magnitude < Mathf.Epsilon)
                    {
                        break;
                    }

                    // Choose new portal

                    int ind = -1;

                    if (!hit)
                    {
                        ind = NewPortalVertexIndex(w0, W, w4, Vector3.zero, false);
                    }
                    else
                    {
                        float t = -Vector3.Dot(W[0], normal) / Vector3.Dot(w0, normal);
                        ind = NewPortalVertexIndex(w0, W, w4, -t * w0, true);
                    }
              
                    if (ind < 0) break;

                    AW[ind] = aw4;
                    BW[ind] = bw4;
                    W[ind] = w4;
                }

                ClosestPtToOrigin(W, out v, out float[] lambda);
                a = lambda[0] * AW[0] + lambda[1] * AW[1] + lambda[2] * AW[2];
                b = lambda[0] * BW[0] + lambda[1] * BW[1] + lambda[2] * BW[2];

                return hit;
            }

            private static bool ClosestPtToOrigin(Vector3[] W, out Vector3 v, out float[] lambda)
            {
                lambda = null;
                v = new Vector3();

                Vector3 ab = W[1] - W[0];
                Vector3 ac = W[2] - W[0];
                

                float alpha = Vector3.SqrMagnitude(ab);
                float beta = Vector3.Dot(ab, ac);
                float gamma = Vector3.SqrMagnitude(ac);

                float det = alpha * gamma - beta * beta;

                if (Mathf.Abs(det) < 0) return false;

                float delta = Vector3.Dot(ab, W[0]);
                float theta = Vector3.Dot(ac, W[0]);

                lambda = new float[3];
                lambda[1] = (-gamma * delta + beta * theta) / det;
                lambda[2] = (+beta * delta - alpha * theta) / det;
                lambda[0] = 1 - lambda[1] - lambda[2];
                v = lambda[0] * W[0] + lambda[1] * W[1] + lambda[2] * W[2];

                return true;
            }

            private static int NewPortalVertexIndex(Vector3 w0, Vector3[] W, Vector3 w4, Vector3 O, bool checkPortal)
            {
                Vector3[] normals = new Vector3[]
                {
                    Vector3.Cross(W[0] -w0, w4 - w0),
                    Vector3.Cross(W[1] -w0, w4 - w0),
                    Vector3.Cross(W[2] -w0, w4 - w0),
                };

                float[] signs = new float[]
                {
                    Mathf.Sign(Vector3.Dot(normals[0],O - w4)),
                    Mathf.Sign(Vector3.Dot(normals[1],O - w4)),
                    Mathf.Sign(Vector3.Dot(normals[2],O - w4))
                };
                
                for (int i = 0; i < 3; i++)
                {
                    int k = (i + 1) % 3;
                    int l = (k + 1) % 3;
                    float sgnWik = Mathf.Sign(Vector3.Dot(normals[i], W[k] - w4));
                    float sgnWki = Mathf.Sign(Vector3.Dot(normals[k], W[i] - w4));

                    if(sgnWik * signs[i] > 0 && sgnWki * signs[k] > 0)
                    {
                        if(!checkPortal)
                            return l;
                        else
                        {
                            Vector3[] candW = new Vector3[] { W[i], W[k], w4 };
                            int wrong = OriginVsPortalCandidate(w0, candW, out Vector3 n);

                            if (wrong < 0) return l;
                        }
                    }
                }

                return -1;
            }

            private static int OriginVsPortalCandidate(Vector3 w0,Vector3[] W, out Vector3 n)
            {
                int wrong = -1;
                n = Vector3.zero;

                for (int i = 0; i < 3; i++)
                {
                    int k = (i + 1) % 3;
                    int l = (k + 1) % 3;
                    n = Vector3.Cross(W[i] - w0, W[k] - w0);
                    float sgnO = Mathf.Sign(Vector3.Dot(n, -w0));
                    float sgnWl = Mathf.Sign(Vector3.Dot(n, W[l] - w0));

                    if(sgnO * sgnWl < 0)
                    {
                        wrong = l;
                        n *= sgnO;
                        break;
                    }
                }

                return wrong;
            }

            private static bool ContainsOrigin(Vector3[] W)
            {
                for (int i = 0; i < 4; i++)
                {
                    int j = (i + 1) % 4;
                    int k = (i + 2) % 4;
                    int l = (i + 3) % 4;

                    Vector3 n = Vector3.Cross(W[j] - W[i], W[k] - W[i]);

                    float sgnWl = Mathf.Sign(Vector3.Dot(n, W[l] - W[i]));
                    float sgnO = Mathf.Sign(Vector3.Dot(n, -W[i]));

                    if (sgnWl * sgnO < 0) return false;
                }

                return true;
            }

            private class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
            {
                public int Compare(TKey x, TKey y)
                {
                    int result = x.CompareTo(y);

                    if (result == 0)
                        return 1;   // Handle equality as beeing greater
                    else
                        return result;
                }
            }

            /// <summary>
            /// Implementation of the Expanding Polytope Algorithm (Gino van den Bergen)
            /// </summary>
            /// <param name="A"> Convex object A </param>
            /// <param name="B"> Convex object B </param>
            /// <param name="W"> Initial polytope: point, segment, triangle, or tetrahedron </param>
            /// <param name="AW"> W = AW - BW </param>
            /// <param name="BW"> </param>
            /// <param name="v"> v = a - b, the penetration vector </param>
            /// <param name="a"> Contact point of object A </param>
            /// <param name="b"> Contact point of object B </param>
            /// <returns> true if the algorithm succeeded, false if didn't </returns>
            public static bool EPA(
                IConvexShape A,
                IConvexShape B,
                Vector3[] W,
                Vector3[] AW,
                Vector3[] BW,
                out Vector3 v,
                out Vector3 a,
                out Vector3 b)
            {
                // out parameters must be initialized before return
                v = a = b = Vector3.zero;

                SortedList<float, EpaEntry> pQueue = new SortedList<float, EpaEntry>(new DuplicateKeyComparer<float>());

                ConstructInitTetrahedron(W, AW, BW, out int[] FW, out int[] AdjFW, out int[] IndFW);

                if (!ContainsOrigin(W))
                    return false;

                EpaEntry[] initEntries = new EpaEntry[4];
                for (int i = 0; i < 4; i++)
                {
                    initEntries[i] = ConstructEpaEntry(
                        W[FW[i * 3]], W[FW[i * 3 + 1]], W[FW[i * 3 + 2]],
                        AW[FW[i * 3]], AW[FW[i * 3 + 1]], AW[FW[i * 3 + 2]],
                        BW[FW[i * 3]], BW[FW[i * 3 + 1]], BW[FW[i * 3 + 2]]
                        );
                }

                for (int i = 0; i < 4; i++)
                {
                    initEntries[i].adj = new EpaEntry[]
                    {
                        initEntries[AdjFW[i * 3]],
                        initEntries[AdjFW[i*3 + 1]],
                        initEntries[AdjFW[i*3 + 2]]
                    };

                    initEntries[i].ind = new int[]
                    {
                        IndFW[i * 3],
                        IndFW[i * 3 + 1],
                        IndFW[i * 3 + 2]
                    };

                    if(ClosestIsInternal(initEntries[i]))
                    {
                        pQueue.Add(initEntries[i].sqDistance, initEntries[i]);
                    }
                }

                int maxIt = 20;
                float mu = float.PositiveInfinity;
                EpaEntry entry = null;

                while (maxIt-- > 0)
                {
                    //Debug.Log(pQueue.Count);

                    if (pQueue.Count == 0) break;

                    entry = pQueue.First().Value;
                    pQueue.RemoveAt(0);

                    if(!entry.obsolete)
                    {
                        v = entry.closestPt;
                        float d2 = entry.sqDistance;
                        Vector3 aw = A.SupportingVertex(v);
                        Vector3 bw = B.SupportingVertex(-v);
                        Vector3 w = aw - bw;
                        mu = Mathf.Min(mu, Mathf.Pow(Vector3.Dot(w, v), 2) / d2);

                        // check if v is close enough
                        if (mu <= Mathf.Pow((1 + Mathf.Epsilon), 2) * d2)
                            break;

                        entry.obsolete = true;
                        List<EpaEntry> E = new List<EpaEntry>();
                        List<int> I = new List<int>();

                        for (int i = 0; i < 3; i++)
                        {
                            Silhouette(entry.adj[i], entry.ind[i], w, E, I);
                        }

                        EpaEntry[] newEntries = new EpaEntry[E.Count];
                        bool entryFailed = false;

                        for (int i = 0; i < E.Count; i++)
                        {
                            int j = I[i];
                            int next = (j + 1) % 3;
                            EpaEntry newEntry = ConstructEpaEntry(
                                E[i].vertices[j],  w,  E[i].vertices[next],
                                E[i].verticesA[j], aw, E[i].verticesA[next],
                                E[i].verticesB[j], bw, E[i].verticesB[next]
                                );

                            if(newEntry == null)
                            {
                                entryFailed = true;
                                break;
                            }

                            newEntries[i] = newEntry;
                        }

                        if (entryFailed) break;

                        for (int i = 0; i < E.Count; i++)
                        {
                            try
                            {
                                newEntries[i].adj = new EpaEntry[]
                                {
                                    newEntries[(i + 1) % E.Count],
                                    newEntries[(i - 1 + E.Count) % E.Count],
                                    E[i]
                                };
                            }
                            catch(Exception exception)
                            {

                                Debug.Log(exception.ToString());
                                break;
                            }

                            newEntries[i].ind = new int[] { 1, 0, I[i] };

                            E[i].adj[I[i]] = newEntries[i];
                            E[i].ind[I[i]] = 2;

                            if(ClosestIsInternal(newEntries[i]) &&
                                d2 <= newEntries[i].sqDistance &&
                                newEntries[i].sqDistance <= mu)
                            {
                                pQueue.Add(newEntries[i].sqDistance,newEntries[i]);
                            }
                        }
                    }
                }

                if(entry != null)
                {
                    v = entry.closestPt;

                    a = entry.lambda[0] * entry.verticesA[0] +
                        entry.lambda[1] * entry.verticesA[1] +
                        entry.lambda[2] * entry.verticesA[2];

                    b = entry.lambda[0] * entry.verticesB[0] +
                        entry.lambda[1] * entry.verticesB[1] +
                        entry.lambda[2] * entry.verticesB[2];
                    return true;
                }

                return false;
            }

            private static void Swap<T>(ref T lhs, ref T rhs)
            {
                T temp;
                temp = lhs;
                lhs = rhs;
                rhs = temp;
            }

            private static void ConstructInitTetrahedron(
                Vector3[] W,
                Vector3[] AW,
                Vector3[] BW,
                out int[] FW,
                out int[] AdjFW,
                out int[] IndFW)
            {

                Vector3 normal = Vector3.Cross(W[1] - W[0], W[2] - W[0]);
                if (Vector3.Dot(W[3] - W[0], normal) > 0)
                {
                    Swap(ref W[1], ref W[2]);
                    Swap(ref AW[1], ref AW[2]);
                    Swap(ref BW[1], ref BW[2]);
                }

                FW = new int[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    2, 1, 3,
                    0, 3, 1
                };

                AdjFW = new int[]
                {
                    3, 2, 1,
                    0, 2, 3,
                    0, 3, 1,
                    1, 2, 0
                };

                IndFW = new int[]
                {
                    2, 0, 0,
                    2, 2, 0,
                    1, 1, 1,
                    2, 1, 0
                };
            }

            private class EpaEntry
            {
                public EpaEntry(
                    Vector3[] vertices,
                    Vector3[] verticesA,
                    Vector3[] verticesB,
                    Vector3 closestPt,
                    float sqDistance,
                    float[] lambda)
                {
                    this.vertices = vertices;
                    this.verticesA = verticesA;
                    this.verticesB = verticesB;
                    this.closestPt = closestPt;
                    this.sqDistance = sqDistance;
                    this.lambda = lambda;
                    adj = null;
                    obsolete = false;
                }

                public Vector3[] vertices;
                public Vector3[] verticesA;
                public Vector3[] verticesB;
                public Vector3 closestPt;
                public float sqDistance;
                public float[] lambda;
                public EpaEntry[] adj;
                public int[] ind;
                public bool obsolete;
            }

            private static EpaEntry ConstructEpaEntry(
                Vector3 y1,
                Vector3 y2,
                Vector3 y3,
                Vector3 p1,
                Vector3 p2,
                Vector3 p3,
                Vector3 q1,
                Vector3 q2,
                Vector3 q3)
            {
                Vector3[] Y = new Vector3[] { y1, y2, y3 };
                if(ClosestPtToOrigin(Y, out Vector3 v, out float[] lambda))
                {
                    return new EpaEntry(
                        Y, new Vector3[] { p1, p2, p3 },
                        new Vector3[] { q1, q2, q3 }, v,
                        v.sqrMagnitude, lambda);
                }

                return null;
            }

            private static bool ClosestIsInternal(EpaEntry entry)
            {
                return
                    0 <= entry.lambda[0] &&
                    0 <= entry.lambda[1] &&
                    0 <= entry.lambda[2];
            }

            private static void Silhouette(EpaEntry entry, int i, Vector3 w, List<EpaEntry> E, List<int> I)
            {
                if (entry.obsolete) return;

                if(Vector3.Dot(entry.closestPt,w) < entry.sqDistance)
                {
                    E.Add(entry);
                    I.Add(i);
                }
                else
                {
                    entry.obsolete = true;
                    Silhouette(entry.adj[(i + 1) % 3], entry.ind[(i + 1) % 3], w, E, I);
                    Silhouette(entry.adj[(i + 2) % 3], entry.ind[(i + 2) % 3], w, E, I);
                }
            }

            static float epsTol = 1e3f * Mathf.Epsilon;
            static float epsSquared = Mathf.Epsilon * Mathf.Epsilon;
            const float safetyMargin = 0.1f;
        }
    }
}

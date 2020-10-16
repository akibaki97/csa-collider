using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        public struct Matrix3x3 : IEquatable<Matrix3x3>
        {

            public Matrix3x3(Vector3 c1, Vector3 c2, Vector3 c3)
            {
                cols = new Vector3[] { c1, c2, c3 };
            }

            public float this[int i, int j]
            {
                get { return cols[j][i]; }
                set { cols[j][i] = value; }
            }

            public Vector3 this[int i]
            {
                get { return cols[i]; }
                set { cols[i] = value; }
            }

            public float Det()
            {
                return Vector3.Dot(cols[0], Vector3.Cross(cols[1], cols[2]));
            }

            public Matrix3x3 GetTransponant()
            {
                Vector3 c1 = new Vector3(this[0, 0], this[0, 1], this[0, 2]);
                Vector3 c2 = new Vector3(this[1, 0], this[1, 1], this[1, 2]);
                Vector3 c3 = new Vector3(this[2, 0], this[2, 1], this[2, 2]);

                return new Matrix3x3(c1, c2, c3);
            }

            public bool Equals(Matrix3x3 other)
            {
                return cols[0] == other[0] &&
                       cols[1] == other[1] &&
                       cols[2] == other[2];
            }

            public Matrix3x3 Inverse(out float det)
            {
                Vector3 d = Vector3.Cross(cols[1], cols[2]);
                det = Vector3.Dot(cols[0], d);

                if (Mathf.Approximately(det, 0)) return new Matrix3x3();

                Vector3 a = d / det;
                Vector3 b = Vector3.Cross(cols[2], cols[0]) / det;
                Vector3 c = Vector3.Cross(cols[0], cols[1]) / det;

                Vector3 c1 = new Vector3(a.x, b.x, c.x);
                Vector3 c2 = new Vector3(a.y, b.y, c.y);
                Vector3 c3 = new Vector3(a.z, b.z, c.z);

                return new Matrix3x3(c1, c2, c3);
            }

            public Matrix3x3 Transposed()
            {
                Vector3 c1 = new Vector3(cols[0].x, cols[1].x, cols[2].x);
                Vector3 c2 = new Vector3(cols[0].y, cols[1].y, cols[2].y);
                Vector3 c3 = new Vector3(cols[0].z, cols[1].z, cols[2].z);

                return new Matrix3x3(c1, c2, c3);
            }

            public static Matrix3x3 Rotate(Quaternion q)
            {
                float xx = q.x * q.x;
                float yy = q.y * q.y;
                float zz = q.z * q.z;
                float xy = q.x * q.y;
                float xz = q.x * q.z;
                float yz = q.y * q.z;
                float wx = q.w * q.x;
                float wy = q.w * q.y;
                float wz = q.w * q.z;

                return new Matrix3x3(
                    new Vector3(1 - 2 * yy - 2 * zz, 2 * xy + 2 * wz, 2 * xz - 2 * wy),
                    new Vector3(2 * xy - 2 * wz, 1 - 2 * xx - 2 * zz, 2 * yz + wx),
                    new Vector3(2 * xz + 2 * wy, 2 * yz - 2 * wx, 1 - 2 * xx - 2 * yy)
                    );
            }

            public static Matrix3x3 SkewSymmetric(Vector3 vec)
            {
                return new Matrix3x3(
                    new Vector3(0, vec.z, -vec.y),
                    new Vector3(-vec.z, 0, vec.x),
                    new Vector3(vec.y, -vec.x, 0)
                    );
            }

            public static Matrix3x3 Diag(float a, float b, float c)
            {
                return new Matrix3x3(
                    new Vector3(a, 0, 0),
                    new Vector3(0, b, 0),
                    new Vector3(0, 0, c));
            }

            public Matrix3x3 Inverse()
            {
                return Inverse(out float det);
            }

            public static Matrix3x3 operator +(Matrix3x3 lhs, Matrix3x3 rhs)
            {
                return new Matrix3x3(lhs[0] + rhs[0], lhs[1] + rhs[1], lhs[2] + rhs[2]);
            }

            public static Matrix3x3 operator *(Matrix3x3 lhs, Matrix3x3 rhs)
            {

                Vector3 c1 = new Vector3(
                    lhs[0, 0] * rhs[0, 0] + lhs[0, 1] * rhs[1, 0] + lhs[0, 2] * rhs[2, 0],
                    lhs[1, 0] * rhs[0, 0] + lhs[1, 1] * rhs[1, 0] + lhs[1, 2] * rhs[2, 0],
                    lhs[2, 0] * rhs[0, 0] + lhs[2, 1] * rhs[1, 0] + lhs[2, 2] * rhs[2, 0]
                    );

                Vector3 c2 = new Vector3(
                    lhs[0, 0] * rhs[0, 1] + lhs[0, 1] * rhs[1, 1] + lhs[0, 2] * rhs[2, 1],
                    lhs[1, 0] * rhs[0, 1] + lhs[1, 1] * rhs[1, 1] + lhs[1, 2] * rhs[2, 1],
                    lhs[2, 0] * rhs[0, 1] + lhs[2, 1] * rhs[1, 1] + lhs[2, 2] * rhs[2, 1]
                    );

                Vector3 c3 = new Vector3(
                    lhs[0, 0] * rhs[0, 2] + lhs[0, 1] * rhs[1, 2] + lhs[0, 2] * rhs[2, 2],
                    lhs[1, 0] * rhs[0, 2] + lhs[1, 1] * rhs[1, 2] + lhs[1, 2] * rhs[2, 2],
                    lhs[2, 0] * rhs[0, 2] + lhs[2, 1] * rhs[1, 2] + lhs[2, 2] * rhs[2, 2]
                    );

                return new Matrix3x3(c1, c2, c3);
            }

            public static Vector3 operator *(Matrix3x3 lhs, Vector3 rhs)
            {
                return rhs.x * lhs[0] + rhs.y * lhs[1] + rhs.z * lhs[2];
            }

            public override string ToString()
            {
                string str =
                    cols[0].x.ToString() + ", " + cols[1].x.ToString() + ", " + cols[2].x.ToString() + '\n' +
                    cols[0].y.ToString() + ", " + cols[1].y.ToString() + ", " + cols[2].y.ToString() + '\n' +
                    cols[0].z.ToString() + ", " + cols[1].z.ToString() + ", " + cols[2].z.ToString();

                return str;
            }

            Vector3[] cols;
        }
    }
}

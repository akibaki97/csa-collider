using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// Triangle represented with three vertices in order.
        /// </summary>
        public struct Triangle : IPlane, IConvexShape
        {
            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                vertices = new Vector3[] { v1, v2, v3 };
                Normal = Vector3.Cross(v2 - v1, v3 - v1);
                Normal = Normal / Normal.magnitude;
            }

            public void Translate(Vector3 T)
            {
                vertices[0] += T;
                vertices[1] += T;
                vertices[2] += T;
            }

            public void TransformWith(Transform transform)
            {
                vertices[0] = transform.rotation * vertices[0] + transform.position;
                vertices[1] = transform.rotation * vertices[1] + transform.position;
                vertices[2] = transform.rotation * vertices[2] + transform.position;
            }

            public Vector3 SupportingVertex(Vector3 dir)
            {
                float x = Vector3.Dot(vertices[0], dir);
                float y = Vector3.Dot(vertices[1], dir);
                float z = Vector3.Dot(vertices[2], dir);

                return x > y ? (x > z ? vertices[0] : vertices[2]) : (y > z ? vertices[1] : vertices[2]);
            }

            public Vector3 this[int index]
            {
                get { return vertices[index]; }
            }

            public Vector3 Center
            {
                get
                {
                    return (vertices[0] + vertices[1] + vertices[2]) / 3f;
                }
            }

            public Vector3[] Vertices { get => vertices; }

            private readonly Vector3[] vertices;
            public Vector3 Normal { get; private set; }
            public Vector3 RefPoint { get { return vertices[0]; } }

            // IList<Vector3> IConvexShape.Vertices { get => vertices; }

            //public static const List<List<int>> TriAdj = new List<List<int>>
        }
    }
}

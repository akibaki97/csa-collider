using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        public interface IPlane
        {
            Vector3 Normal { get; }
            Vector3 RefPoint { get; }
        }

        /// <summary>
        /// Plane represented with it's normal vector and a point on the plane.
        /// </summary>
        public class Plane : IPlane
        {
            public Plane(Vector3 normal, Vector3 refPoint, Transform transform)
            {
                this.normal = new Vector3(normal.x, normal.y, normal.z);
                this.normal.Normalize();
                this.refPoint = refPoint;
                this.transform = transform;

                if (Mathf.Approximately(Vector3.Distance(normal, Vector3.right), 0))
                {
                    localX = new Vector3(0, 1, 0);
                    localY = new Vector3(0, 0, 1);
                }
                else if (Mathf.Approximately(Vector3.Distance(normal, Vector3.up), 0))
                {
                    localX = new Vector3(1, 0, 0);
                    localY = new Vector3(0, 0, 1);
                }
                else if (Mathf.Approximately(Vector3.Distance(normal, Vector3.forward), 0))
                {
                    localX = new Vector3(1, 0, 0);
                    localY = new Vector3(0, 1, 0);
                }
            }

            public Plane(Vector3 vert1, Vector3 vert2, Vector3 vert3, Transform trans)
            {
                normal = Vector3.Cross(vert1, vert2);
                normal.Normalize();
                refPoint = vert3;
                transform = trans;
            }

            public bool PointIsOnPlane(Vector3 vert)
            {
                return Mathf.Abs(Vector3.Dot(Normal, vert) - PlaneConst) < Mathf.Epsilon;
            }

            public Vector2 TransformToLocalBase(Vector3 vec)
            {
                Vector2 localVec = new Vector2(
                    Vector3.Dot(vec - RefPoint, LocalX),
                    Vector3.Dot(vec - RefPoint, LocalY)
                    );

                return localVec;
            }

            public Vector3 TransformFromLocalBase(Vector2 vec)
            {
                return LocalX * vec.x + LocalY * vec.y + RefPoint;
            }

            public Vector3 TransformFromLocalBase(float x, float y)
            {
                return LocalX * x + LocalY * y + RefPoint;
            }

            /// <value> The normal vector in world coordinates </value>
            public Vector3 Normal { get { return transform.rotation * normal; } }

            /// <value> The local X basis vector relative to the transform </value>
            public Vector3 LocalXL { get { return localX; } }

            /// <value> The local Y basis vector relative to the transform </value>
            public Vector3 LocalYL { get { return localY; } }

            /// <value> The local X basis vector in world coordinates </value>
            public Vector3 LocalX { get { return transform.rotation * localX; } }

            /// <summary> The local Y basis vector in world coordinates </value>
            public Vector3 LocalY { get { return transform.rotation * localY; } }

            /// <value> The reference point (a point on the plane) of the plane in world coordinates </value>
            public Vector3 RefPoint { get { return transform.position + transform.rotation * refPoint; } }

            public float PlaneConst { get { return Vector3.Dot(Normal, RefPoint); } }

            /// <value> The transform of the parent object </value>
            private Transform transform;

            /// <value> The normal relative to the transform </valuey>
            private Vector3 normal;

            /// <value> The reference point relative to the transform </value>
            private Vector3 refPoint;

            /// <value> The local basis relative to the tranform </value>
            private Vector3 localX;
            private Vector3 localY;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// Struct for representing spheres with center and radius
        /// </summary>
        public struct Sphere
        {
            public Sphere(Transform transform, Vector3 center, float radius)
            {
                this.transform = transform;
                mCenter = center;
                this.mRadius = radius;
            }

            private Vector3 mCenter;
            private float mRadius;

            public Vector3 CenterW { get => transform != null ? transform.position + transform.rotation * mCenter : Vector3.zero; }
            public Vector3 Center { get => mCenter; }
            public float Radius { get => mRadius; }
            private Transform transform;

            /// <summary>
            /// Static function for computing bounding sphere through Mesh AABB
            /// </summary>
            /// <param name="mesh"></param>
            /// <param name="transform"></param>
            /// <returns></returns>
            public static Sphere BoundingSphereFromAABB(Mesh mesh, Transform transform)
            {
                Vector3 aabbMin = mesh.vertices[0];
                Vector3 aabbMax = mesh.vertices[0];

                foreach (Vector3 vert in mesh.vertices)
                {
                    if (aabbMin.x > vert.x) aabbMin.x = vert.x;
                    if (aabbMax.x < vert.x) aabbMax.x = vert.x;
                    if (aabbMin.y > vert.y) aabbMin.y = vert.y;
                    if (aabbMax.y < vert.y) aabbMax.y = vert.y;
                    if (aabbMin.z > vert.z) aabbMin.z = vert.z;
                    if (aabbMax.z < vert.z) aabbMax.z = vert.z;
                }

                Vector3 center = 0.5f * (aabbMax + aabbMin);
                float radius = 0;
                foreach (Vector3 vert in mesh.vertices)
                {
                    float d = Vector3.Distance(center, vert);
                    if (d > radius) radius = d;
                }

                Sphere boundingSphere = new Sphere(transform, center, radius);

                return boundingSphere;
            }
        }
    }
}

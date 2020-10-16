using Model.MathAndGeometry;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        class CylinderShape : IConvexShape
        {
            public CylinderShape(float radius, float halfHeight)
            {
                mRadius = radius;
                mHalfHeight = halfHeight;
            }

            public Vector3 SupportingVertex(Vector3 dir)
            {
                float uv = Vector3.Dot(mCentralAxis, dir);
                Vector3 ortho = dir - uv * mCentralAxis;

                return
                    ortho.magnitude > Mathf.Epsilon ?
                    Mathf.Sign(uv) * mHalfHeight * mCentralAxis + mRadius * ortho.normalized :
                    Mathf.Sign(uv) * mHalfHeight * mCentralAxis;
            }

            Vector3 IConvexShape.Center { get => mCenter; }

            Vector3 mCenter = Vector3.zero;
            Vector3 mCentralAxis = Vector3.up;
            float mRadius;
            float mHalfHeight;


        }
    }
}

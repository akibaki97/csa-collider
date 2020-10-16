using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        public struct Segment
        {
            public Segment(Vector3 start, Vector3 end)
            {
                this.start = start;
                this.end = end;
            }

            public static Segment operator*(Quaternion lhs, Segment rhs)
            {
                return new Segment(lhs * rhs.start, lhs * rhs.end);
            }

            public static Segment operator+(Vector3 lhs, Segment rhs)
            {
                return new Segment(lhs + rhs.start, lhs + rhs.end);
            }

            public static Segment operator+(Segment lhs, Vector3 rhs)
            {
                return new Segment(lhs.start + rhs, lhs.end + rhs);
            }

            public Vector3 start;
            public Vector3 end;
        }
    }
}

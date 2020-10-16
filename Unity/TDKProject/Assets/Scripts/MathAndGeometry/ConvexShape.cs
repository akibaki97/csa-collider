using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        public interface IConvexShape
        {
            Vector3 SupportingVertex(Vector3 dir);
            Vector3 Center { get; }
        }
    }
}

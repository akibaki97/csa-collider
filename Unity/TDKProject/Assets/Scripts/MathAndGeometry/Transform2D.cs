using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// Holds the data for transforming 2D objects.
        /// </summary>
        public class Transform2D
        {
            public Transform2D() { }
            public Transform2D(Vector2 translation, float cosTheta, float sinTheta)
            {
                position = translation;

                rotation = new Complex(cosTheta, sinTheta);
            }

            public Transform2D(Vector2 position, Complex rotation)
            {
                this.position = position;
                this.rotation = rotation;
            }

            /// <value> The position of the object. </value>
            public Vector2 position;
            public Complex rotation;
        }
    }
}
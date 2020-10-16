using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// A 2D polygon, represented with vertices in order.
        /// </summary>
        [System.Serializable]
        public class Polygon : IEnumerable, IEnumerator
        {
            /// <summary>
            /// Constructs a polygon from given vertices in order.
            /// </summary>
            /// <param name="vertices"></param>
            public Polygon(List<Vector2> vertices)
            {
                index = 0;
                this.vertices = vertices;
            }

            public List<Vector2> vertices;
            public int Count { get { return vertices.Count; } }

            public Vector3 this[int index]
            {
                get { return vertices[index]; }
            }

            public IEnumerator GetEnumerator()
            {
                return (IEnumerator)this;
            }

            public object Current
            {
                get { return vertices[index]; }
            }

            public bool MoveNext()
            {
                if (index++ < vertices.Count) return true;
                else return false;
            }

            public void Reset()
            {
                index = 0;
            }

            public void UpdateAABB()
            {
                if (vertices != null)
                {
                    AABB = new Vector2[4];

                    AABB[0] = new Vector2(vertices[0].x, vertices[0].y); // aabbMin
                    AABB[2] = new Vector2(vertices[0].x, vertices[0].y); // aabbMax

                    for (int i = 0; i < vertices.Count; i++)
                    {
                        if (AABB[0].x > vertices[i].x) AABB[0].x = vertices[i].x;
                        if (AABB[0].y > vertices[i].y) AABB[0].y = vertices[i].y;
                        if (AABB[2].x < vertices[i].x) AABB[2].x = vertices[i].x;
                        if (AABB[2].y < vertices[i].y) AABB[2].y = vertices[i].y;
                    }
                }

                AABB[1] = new Vector2(AABB[2].x, AABB[0].y);
                AABB[3] = new Vector2(AABB[0].x, AABB[2].y);
            }

            public Vector2[] AABB { get; private set; }

            private int index;
        }
    }
}

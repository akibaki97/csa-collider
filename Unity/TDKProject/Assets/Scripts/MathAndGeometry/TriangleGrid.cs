using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// An array to hold triangles in a grid structure.
        /// </summary>
        public class TriangleGrid : IEnumerable, IEnumerator
        {
            public TriangleGrid(
                Vector3[] vertices,
                int[] indices,
                int width,
                int height,
                Vector2 cellSize,
                Vector3 scale,
                Vector3 translate)
            {
                this.width = width;
                this.height = height;
                triangles = new Triangle[width, height, 2];
                this.indices = new Vector3Int[width, height, 2];
                this.cellSize = new Vector2(scale.x * cellSize.x, scale.z * cellSize.y);
                this.translate = translate;

                int k = 0;
                for (int j = 0; j < height; j++)
                {
                    for (int i = 0; i < width; i++)
                    {
                        triangles[i, j, 0] = new Triangle(
                            scale.x * vertices[indices[k]] + translate,
                            scale.y * vertices[indices[k + 1]] + translate,
                            scale.z * vertices[indices[k + 2]] + translate
                            );

                        this.indices[i, j, 0] = new Vector3Int(indices[k], indices[k + 1], indices[k + 2]);

                        triangles[i, j, 1] = new Triangle(
                            scale.x * vertices[indices[k + 3]] + translate,
                            scale.y * vertices[indices[k + 4]] + translate,
                            scale.z * vertices[indices[k + 5]] + translate
                            );

                        this.indices[i, j, 1] = new Vector3Int(indices[k + 3], indices[k + 4], indices[k + 5]);

                        k += 6;
                    }
                }

                pos = new Vector3Int(-1, -1, -1);
                firstPos = new Vector3Int(0, 0, 0);
                lastPos = new Vector3Int(width - 1, height - 1, 1);
                isEmpty = false;
            }

            public IEnumerator GetEnumerator()
            {
                return (IEnumerator)this;
            }

            public bool MoveNext()
            {
                if (isEmpty) return false;

                pos.z = (pos.z + 1) % 2;
                if (pos.z == 0)
                {
                    pos.x = (pos.x + 1) % (lastPos.x + 1);

                    if (pos.x == 0)
                    {
                        pos.x = firstPos.x;
                        ++pos.y;
                    }
                }

                return pos.x <= lastPos.x && pos.y <= lastPos.y;
            }

            public void Reset()
            {
                pos.x = -1;
                pos.y = firstPos.y - 1;
                pos.z = -1;
                isEmpty = false;
            }

            public object Current
            {
                get { return triangles[pos.x, pos.y, pos.z]; }
            }

            public Vector3Int CurrentIndices
            {
                get { return indices[pos.x, pos.y, pos.z]; }
            }

            public Vector3Int CurrentPos
            {
                get { return pos; }
            }

            /// <summary>
            /// Filtering the enumerator with a sphere projection circle.
            /// </summary>
            /// <param name="sphereCenter"></param>
            /// <param name="sphereRadius"></param>
            public void FilterBySphereProjection(Vector3 sphereCenter, float sphereRadius)
            {
                firstPos.x = Mathf.FloorToInt((sphereCenter.x - translate.x - sphereRadius) / cellSize.x);
                firstPos.y = Mathf.FloorToInt((sphereCenter.z - translate.z - sphereRadius) / cellSize.y);
                lastPos.x = Mathf.FloorToInt((sphereCenter.x - translate.x + sphereRadius) / cellSize.x);
                lastPos.y = Mathf.FloorToInt((sphereCenter.z - translate.z + sphereRadius) / cellSize.y);


                if (firstPos.x > width - 1 || firstPos.y > height - 1 || lastPos.x < 0 || lastPos.y < 0)
                    isEmpty = true;
                else
                {
                    isEmpty = false;

                    if (firstPos.x < 0) firstPos.x = 0;
                    if (firstPos.y < 0) firstPos.y = 0;
                    if (lastPos.x > width - 1) lastPos.x = width - 1;
                    if (lastPos.y > height - 1) lastPos.y = height - 1;
                }

                pos.y = firstPos.y - 1;
            }

            public void DeleteFilter()
            {
                firstPos = new Vector3Int(0, 0, 0);
                lastPos = new Vector3Int(width - 1, height - 1, 1);
            }

            private Triangle[,,] triangles;
            private Vector3Int[,,] indices;
            private Vector3Int pos;
            private Vector3Int firstPos;
            private Vector3Int lastPos;
            private int width, height;
            private Vector2 cellSize;
            private Vector3 translate;
            private bool isEmpty;
        }
    }
}
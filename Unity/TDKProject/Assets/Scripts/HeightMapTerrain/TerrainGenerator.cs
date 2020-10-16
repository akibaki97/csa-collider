using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    /// <summary>
    /// Class responsible for generating terrains from a light map.
    /// </summary>
    public static class TerrainGenerator
    {
        /// <summary>
        /// Generate the Mesh file of the terrain from the appropriate data.
        /// </summary>
        /// <param name="heightMap"> The heightmap as a 2D array. </param>
        /// <param name="heightCurve"> The height curve mapping from [0,1]. </param>
        /// <param name="heightMultiplier"></param>
        /// <param name="cellSize"></param>
        /// <returns>
        /// The <c>Mesh</c> object created from the heightMap.
        /// </returns>
        public static Mesh GenerateMeshFromHeightmap(float[,] heightMap, AnimationCurve heightCurve, float heightMultiplier, Vector2 cellSize, bool flatShading)
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Vector3[] vertices = new Vector3[width * height];
            int[] triangles = new int[6 * (width - 1) * (height - 1)];

            for (int i = 0, j = 0, y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    i = x + y * width;
                    vertices[i] = new Vector3(
                        cellSize.x * x,
                        heightMultiplier * heightCurve.Evaluate(heightMap[x, y]),
                        cellSize.y * y
                        );

                    if (x < width - 1 && y < height - 1)
                    {
                        triangles[j++] = i;
                        triangles[j++] = i + width;
                        triangles[j++] = i + 1;

                        triangles[j++] = i + 1;
                        triangles[j++] = i + width;
                        triangles[j++] = i + width + 1;
                    }
                }
            }

            Mesh mesh = new Mesh();

            if (flatShading)
            {
                FlatShading(vertices, triangles, out Vector3[] flatVertices, out int[] flatTriangles);
                mesh.vertices = flatVertices;
                mesh.triangles = flatTriangles;
            }
            else
            {

                mesh.vertices = vertices;
                mesh.triangles = triangles;
            }
            mesh.RecalculateNormals();
            return mesh;
        }

        /// <summary>
        /// Creates unique vertices for each triangles
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="triangles"></param>
        /// <param name="flatVertices"></param>
        /// <param name="flatTriangles"></param>
        public static void FlatShading(Vector3[] vertices, int[] triangles, out Vector3[] flatVertices, out int[] flatTriangles)
        {
            flatVertices = new Vector3[triangles.Length];
            flatTriangles = new int[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                flatVertices[i] = vertices[triangles[i]];
                flatTriangles[i] = i;
            }
        }
    }
}

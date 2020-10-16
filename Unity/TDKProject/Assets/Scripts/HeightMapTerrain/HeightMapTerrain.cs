using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    /// <summary>
    /// Class for representing terrains as a heightmap.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class HeightMapTerrain : MonoBehaviour
    {
        /// <summary>
        /// The class that contains the first pixels ...
        /// </summary>
        [System.Serializable]
        public struct IntRect
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }

        [System.Serializable]
        public struct SceneViewParameters
        {
            public bool drawMeshWire;
        }

        /// <value> The lightMap picture represented as a Texture2D </value>
        public Texture2D lightMap;

        /// <value> </value>
        public IntRect sourceRect;

        /// <value> The curve for mapping the [0,1] interval of the heights. </value>
        public AnimationCurve heightCurve;

        public float heightMultiplier;

        public Vector2 cellSize;
        
        public bool useFlatShading;

        /// <value> </value>
        public SceneViewParameters sceneViewParameters;

        

        /// <summary>
        /// Call the routine which generates the HeightMap from the <c>lightMap</c> picture.
        /// </summary>
        private void Awake()
        {
            GenerateHightMapTerrain();
        }

        /// <summary>
        /// Generates the HeightMapTerrain
        /// </summary>
        public void GenerateHightMapTerrain()
        {
            if (lightMap != null)
            {
                int startX = Mathf.FloorToInt(sourceRect.x);
                int startY = Mathf.FloorToInt(sourceRect.y);
                int width = Mathf.FloorToInt(sourceRect.width);
                int height = Mathf.FloorToInt(sourceRect.height);

                Color[] pixels = lightMap.GetPixels(startX, startY, width, height);

                float[,] heightMap = new float[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        heightMap[x, y] = pixels[x + (height - 1 - y) * width].grayscale;
                    }
                }

                DrawMesh(TerrainGenerator.GenerateMeshFromHeightmap(heightMap, heightCurve, heightMultiplier, cellSize, useFlatShading));
            }
        }

        void DrawMesh(Mesh mesh)
        {
            GetComponent<MeshFilter>().sharedMesh = mesh;
        }

        private void OnValidate()
        {
            if (sourceRect.width < 1) sourceRect.width = 1;
            else if (sourceRect.width > lightMap.width) sourceRect.width = lightMap.width;

            if (sourceRect.height < 1) sourceRect.height = 1;
            else if (sourceRect.height > lightMap.height) sourceRect.height = lightMap.height;

            if (sourceRect.x < 0) sourceRect.x = 0;
            else if (sourceRect.x + sourceRect.width > lightMap.width) sourceRect.x = lightMap.width - sourceRect.width;

            if (sourceRect.y < 0) sourceRect.y = 0;
            else if (sourceRect.y + sourceRect.height > lightMap.height) sourceRect.y = lightMap.height - sourceRect.height;
        }

        private void OnDrawGizmos()
        {
            if (sceneViewParameters.drawMeshWire)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireMesh(GetComponent<MeshFilter>().sharedMesh, GetComponent<Transform>().position);
            }
        }
    }
}

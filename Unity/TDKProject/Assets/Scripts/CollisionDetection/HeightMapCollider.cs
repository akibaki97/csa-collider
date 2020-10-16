using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        /// <summary>
        /// The collider class of <c>HeightMap</c> objects.
        /// </summary>
        /// <remarks>
        /// Contains the triangle grid as a representation of the terrain.
        /// </remarks>
        [RequireComponent(typeof(HeightMapTerrain))]
        public class HeightMapCollider : MonoBehaviour
        {
            /// <value> The array which contains the triangles of the terrain as a grid structure. </value>
            public TriangleGrid TriGrid { get; private set; }

            private HeightMapTerrain heightMapTerrain;

            /// <value> The widht of the triangle grid cells (in 2D) </value>
            public int Width { get; private set; }

            /// <value> The height of the triangle grid cells (in 2D) </value>
            public int Height { get; private set; }


            public Vector2 CellSize { get; private set; }


            public Mesh TriMesh { get; private set; }

            /// <summary>
            /// Initialise the data needed for colliding with height maps and fires the 
            /// <c>HeightMapTerrainColliderCreation</c> event.
            /// </summary>
            void Start()
            {
                heightMapTerrain = GetComponent<HeightMapTerrain>();
                Width = heightMapTerrain.sourceRect.width;
                Height = heightMapTerrain.sourceRect.height;
                CellSize = heightMapTerrain.cellSize;
                TriMesh = GetComponent<MeshFilter>().sharedMesh;

                TriGrid = new TriangleGrid(
                    TriMesh.vertices,
                    TriMesh.triangles,
                    Width - 1,
                    Height - 1,
                    CellSize,
                    transform.localScale,
                    transform.position
                    );

                var args = new CollisionEvents.HeightMapColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.HeightMapTerrainColliderCreation, gameObject, args);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {

        /// <summary>
        /// The collider class for concave objects, using cross section approximation of meshes.
        /// </summary>
        [RequireComponent(typeof(MeshFilter))]
        public class CSACollider : DynamicCollider
        {
            public enum RootFinder
            {
                SimulatedAnnealing,
                AberthEhrlich
            }

            /// <value> The slices defining the "net". </value>
            public List<Slice> Slices { get; private set; }

            /// <value> Number of coefficients used.</value>
            [Range(1, 50)]
            public int fourierCoefficients;

            public RootFinder rootFinder;

            public ViewParameters viewParameters;

            /// <value> The file which contains the slices in json format. </value>
            public TextAsset slicesJsonFile;
            
            [System.Serializable]
            public class ViewParameters
            {
                public bool drawSlices;
                public bool drawBoundingSphere;
                public bool drawSliceAABB;
            }
            
            /// <summary>
            /// Initialise the data needed for collision detection of <c>CSACollider</c>s.
            /// </summary>
            /// <remarks>
            /// This function calculates the bounding sphere, and reads the slices from a json file,
            /// then fire the <c>DynamicColliderCreation</c> event.
            /// </remarks>
            new void Start()
            {
                base.Start();

                if (slicesJsonFile != null) InitSlicesFromJson(slicesJsonFile.ToString());

                CollisionEvents.DynamicColliderCreationEventArgs args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }

            public override void EditorStart()
            {
                base.EditorStart();

                if (slicesJsonFile != null) InitSlicesFromJson(slicesJsonFile.ToString());
            }

            /// <summary>
            /// Reads the consisting slices from a <c>.json</c> file.
            /// </summary>
            /// <param name="json"> The name of the json file containing the slices data. </param>
            public void InitSlicesFromJson(string json)
            {
                SliceDataArray sliceDataArray = JsonUtility.FromJson<SliceDataArray>(json);

                Slices = new List<Slice>();

                foreach (SliceData slice in sliceDataArray.slices)
                {
                    foreach (Polygon polygon in slice.polygons)
                        polygon.UpdateAABB();

                    Slice newSlice = new Slice(
                        gameObject.transform,
                        slice.normal,
                        slice.refPoint,
                        slice.polygons,
                        fourierCoefficients
                        );

                    Slices.Add(newSlice);
                }
            }

            private void OnDrawGizmos()
            {
                if (Slices != null && (viewParameters.drawSlices || viewParameters.drawSliceAABB))
                {
                    foreach (Slice slice in Slices)
                    {
                        foreach (FourierCurve curve in slice.curves)
                        {
                            if (viewParameters.drawSlices)
                                GizmosExtension.DrawCurve(curve, slice.LocalX, slice.LocalY, slice.RefPoint, GizmosExtension.lightGreen);

                            if (viewParameters.drawSliceAABB)
                            {
                                GizmosExtension.DrawRectangle(curve.AABB, slice.LocalX, slice.LocalY, slice.RefPoint, GizmosExtension.lightRed);
                            }
                        }
                    }
                }

                if (viewParameters.drawBoundingSphere && BoundingSphere.CenterW != null)
                {
                    Gizmos.color = GizmosExtension.lightBlue;
                    Gizmos.DrawWireSphere(BoundingSphere.CenterW, BoundingSphere.Radius);
                    //Gizmos.DrawSphere(BoundingSphere.Center, BoundingSphere.Radius);
                }
            }
        }
    }
}

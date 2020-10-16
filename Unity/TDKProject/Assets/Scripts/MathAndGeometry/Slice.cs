using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        /// <summary>
        /// Represents a 2D curve set on a plane in 3D.
        /// </summary>
        public class Slice : Plane
        {
            /// <summary>
            /// Creates the slice from the given data
            /// </summary>
            /// <param name="transform"> Reference to the transform of the holder plane. </param>
            /// <param name="normal"> Normal vector of the holder plane in local coordinates. </param>
            /// <param name="refPoint"> A point on the holder plane. </param>
            /// <param name="polygons"> The polygons, which came from the mesh slicer program. </param>
            /// <param name="lastInd"> The last index of the trigonometric polynomials. </param>
            public Slice(Transform transform, Vector3 normal, Vector3 refPoint, List<Polygon> polygons, int lastInd)
                : base(normal, refPoint, transform)
            {
                curves = new List<FourierCurve>();
                saCache = new Dictionary<FourierCurve,float>();

                foreach (Polygon polygon in polygons)
                {
                    FourierCurve curve = new FourierCurve(polygon, lastInd);
                    curves.Add(curve);
                    saCache.Add(curve, 2 * Mathf.PI * Random.value);
                }
            }

            public List<FourierCurve> curves;
            public Dictionary<FourierCurve, float> saCache;
        }


        /// <summary>
        /// Contains the data for one slice.
        /// </summary>
        [System.Serializable]
        class SliceData
        {
            public SliceData() { }

            public SliceData(Vector3 normal, Vector3 refPoint, List<Polygon> polygons)
            {
                this.normal = normal;
                this.refPoint = refPoint;
                this.polygons = polygons;
            }

            public Vector3 normal;
            public Vector3 refPoint;
            public List<Polygon> polygons;
        }

        /// <summary>
        /// An array holding <c>SliceData</c> objects.
        /// </summary>
        [System.Serializable]
        class SliceDataArray
        {
            public SliceDataArray() { }
            public SliceDataArray(string meshFile, int sliceCount, List<SliceData> slices)
            {
                this.meshFile = meshFile;
                this.sliceCount = sliceCount;
                this.slices = slices;
            }
            public string meshFile;
            public int sliceCount;
            public List<SliceData> slices;
        }
    }
}

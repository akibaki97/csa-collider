using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        public class ConvDecompCollider : DynamicCollider
        {
            public enum CollisionSolver
            {
                GJK,
                MPR
            }

            public List<ConvexPolyhShape> CollisionShapes { get => mShapes; }
            public IList<Vector3> ShapeCenters { get => mShapeCenters as IList<Vector3>; }

            public ViewParameters viewParameters;
            public TextAsset convDecomMeshObj;
            public CollisionSolver collisionSolver;
            public bool ChungWangEnabled;

            [Range(1, 50)]
            public int ChungWangIt;

            [Range(1, 50)]
            public int iterations;

            protected new void Start()
            {
                base.Start();

                ObjParser.ParseObj(convDecomMeshObj.ToString(), out Mesh mesh, out mDecompMeshes);
                mShapeCenters = new List<Vector3>();

                mShapes = new List<ConvexPolyhShape>();
                foreach (var decompMesh in mDecompMeshes)
                {
                    mShapes.Add(new ConvexPolyhShape(decompMesh));
                    mShapeCenters.Add(MathUtils.CenterOfMass(decompMesh.vertices));
                }

                var args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }

            public override void EditorStart()
            {
                base.EditorStart();

                ObjParser.ParseObj(convDecomMeshObj.ToString(), out Mesh mesh, out mDecompMeshes);
                mShapeCenters = new List<Vector3>();

                mShapes = new List<ConvexPolyhShape>();
                foreach (var decompMesh in mDecompMeshes)
                {
                    mShapes.Add(new ConvexPolyhShape(decompMesh));
                    mShapeCenters.Add(MathUtils.CenterOfMass(decompMesh.vertices));
                }
            }

            [System.Serializable]
            public class ViewParameters
            {
                public bool drawBoundingSphere;
                public bool drawTriangles;
            }

            private void OnDrawGizmos()
            {
                if(viewParameters.drawTriangles && mDecompMeshes != null)
                {
                    Gizmos.color = GizmosExtension.lightGreen;
                    foreach (Mesh decompMesh in mDecompMeshes)
                    {
                        Gizmos.DrawWireMesh(decompMesh, transform.position, transform.rotation);
                    }
                }

                if (viewParameters.drawBoundingSphere && BoundingSphere.CenterW != null)
                {
                    Gizmos.color = GizmosExtension.lightBlue;
                    Gizmos.DrawWireSphere(BoundingSphere.CenterW, BoundingSphere.Radius);
                }
            }

            private List<ConvexPolyhShape> mShapes;
            private List<Mesh> mDecompMeshes;
            private List<Vector3> mShapeCenters;
        }
    }
}
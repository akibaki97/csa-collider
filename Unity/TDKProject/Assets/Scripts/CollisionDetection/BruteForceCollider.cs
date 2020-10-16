using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        public class BruteForceCollider : DynamicCollider
        {
            private List<Triangle> mTriangles;

            public List<Triangle> Triangles { get { return mTriangles; } }

            protected new void Start()
            {
                base.Start();

                mTriangles = new List<Triangle>();
                for (int i = 0; i < mMesh.triangles.Length; i += 3)
                {
                    mTriangles.Add(new Triangle(
                        mMesh.vertices[mMesh.triangles[i]],
                        mMesh.vertices[mMesh.triangles[i + 1]],
                        mMesh.vertices[mMesh.triangles[i + 2]]
                        ));
                }

                CollisionEvents.DynamicColliderCreationEventArgs args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }

            public override void EditorStart()
            {
                base.EditorStart();
            }

            public ViewParameters viewParameters;

            [System.Serializable]
            public class ViewParameters
            {
                public bool drawBoundingSphere;
                public bool drawTriangles;
            }

            private void OnDrawGizmos()
            {
                if(viewParameters.drawTriangles && mMesh != null)
                {
                    Gizmos.color = GizmosExtension.lightGreen;
                    Gizmos.DrawWireMesh(mMesh, transform.position, transform.rotation);
                }

                if (viewParameters.drawBoundingSphere && BoundingSphere.CenterW != null)
                {
                    Gizmos.color = GizmosExtension.lightBlue;
                    Gizmos.DrawWireSphere(BoundingSphere.CenterW, BoundingSphere.Radius);
                }
            }
        }
    }
    
}

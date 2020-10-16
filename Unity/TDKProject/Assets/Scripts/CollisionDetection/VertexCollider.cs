using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        public class VertexCollider : DynamicCollider
        {
            // public variables, properties

            public ViewParameters viewParameters;
            public List<Vector3> Vertices { get => mVertices; }

            protected new void Start()
            {
                base.Start();

                mVertices = new List<Vector3>(mMesh.vertices);

                CollisionEvents.DynamicColliderCreationEventArgs args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }

            public override void EditorStart()
            {
                base.EditorStart();

                mVertices = new List<Vector3>(mMesh.vertices);
            }


            [System.Serializable]
            public class ViewParameters
            {
                public bool drawBoundingSphere;
                public bool drawVertices;
            }

            private void OnDrawGizmos()
            {
                if (viewParameters.drawVertices && mMesh != null)
                {
                    Gizmos.color = GizmosExtension.lightGreen;
                    foreach (Vector3 vert in mVertices)
                    {
                        Gizmos.DrawSphere((transform.rotation * vert) + transform.position, 0.05f);
                    }
                }

                if (viewParameters.drawBoundingSphere && BoundingSphere.CenterW != null)
                {
                    Gizmos.color = GizmosExtension.lightBlue;
                    Gizmos.DrawWireSphere(BoundingSphere.CenterW, BoundingSphere.Radius);
                }
            }

            // private variables
            private List<Vector3> mVertices;
        }
    }
}

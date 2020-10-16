using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        public class CylinderCollider : DynamicCollider
        {
            //public ViewParameters viewParameters;
            public float radius;
            public float halfHeight;

            protected new void Start()
            {
                base.Start();

                mShape = new CylinderShape(radius, halfHeight);

                var args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }

            public override void EditorStart()
            {
                base.EditorStart();

                mShape = new CylinderShape(radius, halfHeight);
            }

            [System.Serializable]
            public class ViewParameters
            {
                public bool drawBoundingSphere;
                public bool drawCylinder;
            }

            private CylinderShape mShape;
        }
    }
}

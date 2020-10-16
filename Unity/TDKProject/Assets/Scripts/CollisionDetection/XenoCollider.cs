using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        public class XenoCollider : ConvexCollider
        {   
            protected new void Start()
            {
                base.Start();
              
                var args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }
    
        }
    }
}
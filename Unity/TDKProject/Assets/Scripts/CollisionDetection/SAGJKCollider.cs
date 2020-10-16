using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Model
{
    namespace CollisionDetection
    {
        public class SAGJKCollider : ConvexCollider
        {

            [Range(1, 50)]
            public int ChungWangIt;

            [Range(1, 50)]
            public int gjkMaxIt;

            // Start is called before the first frame update
            protected new void Start()
            {
                base.Start();

                var args = new CollisionEvents.DynamicColliderCreationEventArgs(this);
                CollisionEvents.FireEvent(CollisionEvents.EventType.DynamicColliderCreation, gameObject, args);
            }
        }
    }
}

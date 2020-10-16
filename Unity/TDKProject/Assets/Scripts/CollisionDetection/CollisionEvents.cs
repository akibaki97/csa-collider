using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;
using Model.PhysicsSimulation;

namespace Model
{
    namespace CollisionDetection
    {
        /// <summary>
        /// The class which is responsible to manage the events.
        /// </summary>
        /// <remarks>
        /// This class contains the global events, thus it is accesible
        /// for every object in the program.
        /// </remarks>
        public static class CollisionEvents
        {
            /// <summary>
            /// Enumerator to hold the different types of events.
            /// </summary>
            public enum EventType
            {
                HeightMapTerrainColliderCreation,
                DynamicColliderCreation,
                CoarseCollision,
                Collision,            
            }

            // HeightMapCollider events

            public class HeightMapColliderCreationEventArgs : EventArgs
            {
                public HeightMapColliderCreationEventArgs(HeightMapCollider collider)
                {
                    Collider = collider;
                }

                public HeightMapCollider Collider { get; private set; }
            }

            public delegate void HeightMapColliderCreationEventHandler(object source, HeightMapColliderCreationEventArgs args);
            public static event HeightMapColliderCreationEventHandler HeightMapColliderCreation;

            // Dynamic collider creation event

            public class DynamicColliderCreationEventArgs : EventArgs
            {
                public DynamicColliderCreationEventArgs(DynamicCollider collider)
                {
                    Collider = collider;
                }

                public DynamicCollider Collider { get; private set; }
            }

            public delegate void DynamicColliderCreationEventHandler(object source, DynamicColliderCreationEventArgs collider);
            public static event DynamicColliderCreationEventHandler DynamicColliderCreation;


            // CoarseCollision events

            public class CoarseCollisionEventArgs : EventArgs
            {
                public CoarseCollisionEventArgs(RigidBody rb, List<Segment> segments)
                {
                    Rb = rb;
                    Segments = segments;
                }

                public RigidBody Rb { get; private set; }
                public List<Segment> Segments { get; private set; }
            }

            public delegate void CoarseCollisionEventHandler(object source, CoarseCollisionEventArgs args);
            public static event CoarseCollisionEventHandler CoarseCollision;

            // Collision events

            public class CollisionEventArgs : EventArgs
            {
                public CollisionEventArgs(RigidBody rb, List<Contact> contacts)
                {
                    Rb = rb;
                    Contacts = contacts;
                }

                public RigidBody Rb { get; private set; }
                public List<Contact> Contacts { get; private set; }
            }

            public delegate void CollisionEventHandler(object source, CollisionEventArgs args);
            public static event CollisionEventHandler Collision;

            /// <summary>
            /// The function which offers an interface for fireing events.
            /// </summary>
            /// <param name="eventType"> The type of the event. </param>
            /// <param name="source"> Source of the object which fired the event. </param>
            /// <param name="args"> The arguments that contain all the data needed to handle the event. </param>
            /// <returns>
            /// True if the event fires succesfully, otherwise false.
            /// </returns>
            public static bool FireEvent(EventType eventType, object source, EventArgs args)
            {
                switch (eventType)
                {
                    case EventType.HeightMapTerrainColliderCreation:
                        if (args is HeightMapColliderCreationEventArgs && HeightMapColliderCreation != null)
                        {
                            HeightMapColliderCreation(source, args as HeightMapColliderCreationEventArgs);
                        }
                        break;
                    case EventType.DynamicColliderCreation:
                        if (args is DynamicColliderCreationEventArgs && DynamicColliderCreation != null)
                        {
                            DynamicColliderCreation(source, args as DynamicColliderCreationEventArgs);
                            return true;
                        }
                        break;
                    case EventType.Collision:
                        if (args is CollisionEventArgs && Collision != null)
                        {
                            Collision(source, args as CollisionEventArgs);
                            return true;
                        }
                        break;
                    case EventType.CoarseCollision:
                        if (args is CoarseCollisionEventArgs && CoarseCollision != null)
                        {
                            CoarseCollision(source, args as CoarseCollisionEventArgs);
                            return true;
                        }
                        break;
                }
                return false;
            }
        }
    }
}
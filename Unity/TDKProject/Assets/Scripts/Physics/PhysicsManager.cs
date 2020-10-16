using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.CollisionDetection;

namespace Model
{
    namespace PhysicsSimulation
    {
        /// <summary>
        /// Responsible for managing the collision responses for
        /// the contacts.
        /// </summary>
        public class PhysicsManager : MonoBehaviour
        {
            public static Vector3 gravityAcceleration = new Vector3(0, -15f, 0);

            /// <value>
            /// Maximum number of iteration for resolving interpenetration
            /// </value>
            public const int positionIterations = 100;

            /// <summary>
            /// Maximum number of iteration for resolving the collision
            /// </summary>
            public const int impulseIterations = 100;

            void Awake()
            {
                CollisionEvents.Collision += OnCollision;
            }          

            void OnCollision(object source, CollisionEvents.CollisionEventArgs args)
            {
                // Prepare contacts for processing
                PrepareContacts(args.Contacts, args.Rb);

                // Resolve interpenetration
                ResolveInterpenetration(args.Contacts, args.Rb);

                // Apply impulses
                ApplyImpulses(args.Contacts, args.Rb);
            }

            /// <summary>
            /// Calculates the data which is required for both
            /// the interpenetration resolving and collision response
            /// </summary>
            /// <param name="contacts"></param>
            /// <param name="body"></param>
            void PrepareContacts(List<Contact> contacts, RigidBody body)
            {
                foreach (Contact contact in contacts)
                {
                    contact.CalculateInternals(body);
                }
            }

            /// <summary>
            /// Responsible for resolving the interpenetration of
            /// the given contact list.
            /// </summary>
            /// <param name="contacts"></param>
            /// <param name="body"></param>
            void ResolveInterpenetration(List<Contact> contacts, RigidBody body)
            {
                for (int i = 0; i < positionIterations; i++)
                {
                    Contact deepestContact = null;
                    float maxPenetration = 0;

                    // find the deepest contact
                    foreach (Contact contact in contacts)
                    {
                        if (contact.penetration < maxPenetration)
                        {
                            deepestContact = contact;
                            maxPenetration = contact.penetration;
                        }
                    }

                    // resolve the deepest contact
                    if (deepestContact != null)
                    {
                        //Debug.Log("max penetration:" + maxPenetration);
                        deepestContact.ApplyPositionChange(body, out Vector3 translation, out Vector3 rotation);

                        // updating penetrations
                        foreach (Contact contact in contacts)
                        {
                            Vector3 deltaPosition = translation + Vector3.Cross(rotation, contact.relativeContactPosition);
                            contact.penetration -= Vector3.Dot(deltaPosition, contact.contactBase[0]);
                        }
                    }
                    else break;
                }
            }

            /// <summary>
            /// Calculates the collision response for a list a contacts
            /// </summary>
            /// <param name="contacts"></param>
            /// <param name="body"></param>
            void ApplyImpulses(List<Contact> contacts, RigidBody body)
            {
                for (int i = 0; i < impulseIterations; i++)
                {
                    Contact fastestContact = null;
                    float maxVelocity = 0;

                    foreach (Contact contact in contacts)
                    {
                        if (contact.contactVelocity.magnitude > maxVelocity)
                        {
                            fastestContact = contact;
                            maxVelocity = contact.contactVelocity.magnitude;
                        }
                    }

                    if (fastestContact != null && fastestContact.desiredDeltaVelocity < 0)
                    {
                        fastestContact.ApplyImpulse(body);

                        // updating velocities
                        foreach (Contact contact in contacts)
                        {
                            contact.CalculateInternals(body);
                        }
                    }
                }
            }
        }
    }
}

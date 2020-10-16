using Model.MathAndGeometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace PhysicsSimulation
    {
        [RequireComponent(typeof(MeshFilter))]
        public class RigidBody : MonoBehaviour
        {
            public bool gravity = true;
            public bool isStatic;
            public float mass = 1.0f;
            [Range(0.0f, 1.0f)]
            public float damping = 0.95f;
            [Range(0.0f, 2.0f)]
            public float restitution = 0.2f;
            [Range(0.0f,1.2f)]
            public float friction = 0.3f;

            public Vector3 Position { get; private set; }
            public Vector3 Velocity { get; set; }
            public Vector3 Acceleration { get; private set; }

            public Quaternion Orientation { get; private set; }                
            public Vector3 AngularVelocity { get; set; }

            // Mass properties
            public float InverseMass { get; private set; }
            public float Mass
            {
                get { return mass; }
                set
                {
                    mass = value;
                    InverseMass = 1 / mass;
                    ComputeInvInertiaTensor();
                }
            }
            public Matrix3x3 InvInertiaTensor { get; private set; }
            public Matrix3x3 InvInertiaTensorWorld { get; private set; }
            private Vector3 centerOfMass;

            private float lnDamp;
            private bool motionLock;

            // Start is called before the first frame update
            void Awake()
            {
                ComputeCenterOfMass();
                TransformMeshToCoM();
            }

            void Start()
            {
                Position = gameObject.transform.position;
                Orientation = gameObject.transform.rotation;

                InverseMass = Mathf.Approximately(mass, 0) ? 0 : 1 / mass; ;
                ComputeInvInertiaTensor();

                lnDamp = Mathf.Log(damping);

                Velocity = new Vector3(0, 0, 0);
                Acceleration = (gravity ? PhysicsManager.gravityAcceleration : Vector3.zero);
                AngularVelocity = new Vector3(0, 0, 0);
            }

            private void ComputeCenterOfMass()
            {
                Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
                centerOfMass = new Vector3(0, 0, 0);
                foreach (var vert in mesh.vertices)
                {
                    centerOfMass += vert;
                }
                centerOfMass /= mesh.vertexCount;
            }

            private void TransformMeshToCoM()
            {
                Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
                for (int i = 0; i < mesh.vertexCount; i++)
                {
                    mesh.vertices[i] -= centerOfMass;
                }
            }

            private void ComputeInvInertiaTensor()
            {
                ComputeAABB(GetComponent<MeshFilter>().sharedMesh, out Vector3 aabbMin, out Vector3 aabbMax);
                Vector3 dims = aabbMax - aabbMin;
                float c = 1f;
                float Ix = c / 12f * Mass * (dims.y * dims.y + dims.z * dims.z);
                float Iy = c / 12f * Mass * (dims.x * dims.x + dims.z * dims.z);
                float Iz = c / 12f * Mass * (dims.x * dims.y + dims.y * dims.y);

                InvInertiaTensor = new Matrix3x3(
                    new Vector3(Ix != 0 ? 1 / Ix : 0, 0, 0),
                    new Vector3(0, Iy != 0 ? 1 / Iy : 0, 0),
                    new Vector3(0, 0, Iz != 0 ? 1 / Iz : 0)
                    );

                Matrix3x3 rotMx = Matrix3x3.Rotate(Orientation);
                InvInertiaTensorWorld = rotMx * InvInertiaTensor * rotMx.Transposed();           
            }

            private void ComputeAABB(Mesh mesh, out Vector3 aabbMin, out Vector3 aabbMax)
            {
                aabbMin = new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, mesh.vertices[0].z);
                aabbMax = new Vector3(mesh.vertices[0].x, mesh.vertices[0].y, mesh.vertices[0].z);
                foreach (Vector3 vert in mesh.vertices)
                {
                    if (aabbMin.x > vert.x) aabbMin.x = vert.x;
                    if (aabbMax.x < vert.x) aabbMax.x = vert.x;
                    if (aabbMin.y > vert.y) aabbMin.y = vert.y;
                    if (aabbMax.y < vert.y) aabbMax.y = vert.y;
                    if (aabbMin.z > vert.z) aabbMin.z = vert.z;
                    if (aabbMax.z < vert.z) aabbMax.z = vert.z;
                }
            }
           
            public void LockMotion()
            {
                if (motionLock) return;

                motionLock = true;
                Acceleration = new Vector3(0, 0, 0);
                Velocity = new Vector3(0, 0, 0);
                AngularVelocity = new Vector3(0, 0, 0);
            }

            public void UnLockMotion(Vector3 velocity)
            {
                if (!motionLock) return;

                Position = gameObject.transform.position;
                Orientation = gameObject.transform.rotation;
             
                Velocity = velocity;
                motionLock = false;
            }

            void FixedUpdate()
            {
                float dTime = Time.deltaTime;

                // Integrating
                if (!motionLock && !isStatic)
                {
                    // Update linear motion
                    
                    Position = Position + dTime * Velocity + dTime * dTime * 0.5f * Acceleration;                   
                    Velocity = Mathf.Exp(dTime * lnDamp) * (Velocity + dTime * Acceleration);
                    Acceleration = gravity ? PhysicsManager.gravityAcceleration : Vector3.zero;

                    // Update angular motion
                    Quaternion dOrientation = VectorToQuaternion(dTime / 2f * AngularVelocity) * Orientation;
                    Orientation = AddQuaternions(Orientation, dOrientation);
                    Orientation.Normalize();

                    AngularVelocity = Mathf.Exp(dTime * lnDamp) * AngularVelocity;


                    // Set new position and orientation
                    gameObject.transform.position = Position;
                    gameObject.transform.rotation = Orientation;
                }
                else
                {
                    Position = gameObject.transform.position;
                    Orientation = gameObject.transform.rotation;
                }

                // Update inv inertia tensior in world coordinates
                Matrix3x3 rotMx = Matrix3x3.Rotate(Orientation);
                InvInertiaTensorWorld = rotMx * InvInertiaTensor * rotMx.Transposed();
            }

            public void Translate(Vector3 trans)
            {
                Position += trans;
            }

            public void Rotate(Vector3 rotation)
            {
                Quaternion dOrientation = VectorToQuaternion(0.5f * rotation) * Orientation;
                Orientation = AddQuaternions(Orientation, dOrientation);
                Orientation.Normalize();
            }

            private Quaternion VectorToQuaternion(Vector3 vec)
            {
                return new Quaternion(vec.x, vec.y, vec.z, 0);
            }

            private Quaternion AddQuaternions(Quaternion q1, Quaternion q2)
            {
                Quaternion result = new Quaternion(q1.x + q2.x, q1.y + q2.y, q1.z + q2.z, q1.w + q2.w);
                return result;
            }

            public void AddImpulse(Vector3 impulse, Vector3 impulsePosition)
            {
                Velocity += InverseMass * impulse;

                Vector3 angMomentum = Vector3.Cross(impulsePosition, impulse);

                AngularVelocity += InvInertiaTensorWorld * angMomentum;
            }

            public void AddCentralImpulse(Vector3 impulse)
            {
                Velocity += InverseMass * impulse;
            }

            public void OnValidate()
            {
                if (mass <= 0)
                {
                    mass = 0.1f;
                }
            }
        }
    }
}

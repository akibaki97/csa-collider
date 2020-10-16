using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Model.MathAndGeometry;
using Model.PhysicsSimulation;

namespace Model
{
    namespace CollisionDetection
    {
        /// <summary>
        /// The dispatcher class between intersecting algorithms and the physics simulator.
        /// </summary>
        /// <remarks>
        /// This class is responsible for creating the intersected points between <c>CSACollider</c>,
        /// and the <c>HeightMapCollider</c>.
        /// </remarks>
        public class CollisionDispatcher : MonoBehaviour
        {
            private List<HeightMapCollider> heightMapColliders;
            private List<DynamicCollider> dynamicColliderList;

            private List<Segment> debugSegments;
            private List<Segment> debugVectors;

            private List<Color> segmentColor;
            private List<Vector3> debugPoints;

            [Range(10, 100)]
            public int annealingTemperature;

            [Range(0.1f, 5f)]
            public float penetrationLimit;

            public bool debugColors;

            // debug
            Color[] colors;

            /// <summary>
            /// Initialise the member lists, and subscribes for the creation events
            /// of <c> CSACollider </c> and <c> HeightMap </c>
            /// </summary>
            public void Awake()
            {
                heightMapColliders = new List<HeightMapCollider>();
                dynamicColliderList = new List<DynamicCollider>();

                debugSegments = new List<Segment>();
                debugPoints = new List<Vector3>();
                debugVectors = new List<Segment>();
                segmentColor = new List<Color>();

                CollisionEvents.HeightMapColliderCreation += new CollisionEvents.HeightMapColliderCreationEventHandler(OnHeightMapColliderCreation);
                CollisionEvents.DynamicColliderCreation += new CollisionEvents.DynamicColliderCreationEventHandler(OnDynamicColliderCreation);
            }

            void OnHeightMapColliderCreation(object source, CollisionEvents.HeightMapColliderCreationEventArgs args)
            {
                heightMapColliders.Add(args.Collider);
            }

            void OnDynamicColliderCreation(object source, CollisionEvents.DynamicColliderCreationEventArgs args)
            {
                dynamicColliderList.Add(args.Collider);
            }

            /// <summary>
            /// Iterates throught the list of <c>HeightMap</c> and <c>CSACollider</c>
            /// to find the intersected points.
            /// </summary>
            /// <remarks>
            /// This function performs the Sphere-Triangle, Plane-Triangle, Curve-Triangle
            /// intersection tests. After found any contact points it sends toward the
            /// <c>PhysicsManager</c>.
            /// </remarks>
            void FixedUpdate()
            {
                debugSegments.Clear();
                debugPoints.Clear();
                debugVectors.Clear();
                segmentColor.Clear();
                foreach (HeightMapCollider hMapCollider in heightMapColliders)
                {
                    if (!hMapCollider.enabled) continue;

                    colors = new Color[hMapCollider.TriMesh.vertexCount];
                    Color origCol = new Color(200f / 255f, 200f / 255f, 200f / 255f);
                    for (int i = 0; i < colors.Length; i++)
                        colors[i] = origCol;


                    foreach (DynamicCollider dynCollider in dynamicColliderList)
                    {
                        if (!dynCollider.enabled) continue;

                        // creating a list of contacts for the physics manager
                        List<Contact> contacts = new List<Contact>();
                        List<Segment> coarseContacts = new List<Segment>();

                        // position of bounding sphere in world coordinates
                        Vector3 spherePos = dynCollider.BoundingSphere.CenterW;
                        float radius = dynCollider.BoundingSphere.Radius;

                        // filtering the triangle grid with the sphere projection
                        hMapCollider.TriGrid.FilterBySphereProjection(spherePos, radius);

                        if (dynCollider is CSACollider)
                        {
                            HandleCSAVsHeightMapCollision(
                                dynCollider as CSACollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);                  
                        }
                        else if(dynCollider is BruteForceCollider)
                        {
                            HandleBruteForceVsHeightMapCollision(
                                dynCollider as BruteForceCollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);
                        }
                        else if(dynCollider is VertexCollider)
                        {
                            HandleVertexVsHeightMapCollision(
                                dynCollider as VertexCollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);
                        }
                        else if(dynCollider is SAGJKCollider)
                        {
                            HandleSAGJKVsHeightMapCollision(
                                dynCollider as SAGJKCollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);
                        }
                        else if(dynCollider is GJKCollider)
                        {
                            HandleGJKVsHeightMapCollision(
                                dynCollider as GJKCollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);
                        }
                        else if(dynCollider is XenoCollider)
                        {
                            HandleXenoVsHeighMapCollision(
                                dynCollider as XenoCollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);
                        }
                        else if(dynCollider is ConvDecompCollider)
                        {
                            HandleConvDecompVsHeighMapCollision(
                                dynCollider as ConvDecompCollider,
                                hMapCollider,
                                contacts,
                                coarseContacts);
                        }

                        // reset the filtering of the triangle grid
                        hMapCollider.TriGrid.Reset();


                        if (coarseContacts.Count > 0 && dynCollider.GetComponent<Rigidbody>() != null)
                        {
                            var args = new CollisionEvents.CoarseCollisionEventArgs(
                                dynCollider.GetComponent<RigidBody>(),
                                coarseContacts
                                );

                            CollisionEvents.FireEvent(CollisionEvents.EventType.CoarseCollision, this, args);
                        }

                        // if any contacts got caught, we send them to the physics manager by the collision event
                        if (contacts.Count > 0 && dynCollider.GetComponent<RigidBody>() != null)
                        {
                            var args = new CollisionEvents.CollisionEventArgs(
                                dynCollider.GetComponent<RigidBody>(),
                                contacts
                                );

                            CollisionEvents.FireEvent(CollisionEvents.EventType.Collision, this, args);
                        }
                    }

                    if(debugColors)
                        hMapCollider.TriMesh.colors = colors;
                }
            }

            private void HandleDynamicVsHeightMapCollision(
                DynamicCollider dynCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts)
            {
                // position of bounding sphere in world coordinates
                Vector3 spherePos = dynCollider.BoundingSphere.CenterW;
                float radius = dynCollider.BoundingSphere.Radius;

                // filtering the triangle grid with the sphere projection
                hMapCollider.TriGrid.FilterBySphereProjection(spherePos, radius);

                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        if(dynCollider is CSACollider)
                        {
                            // curves vs triangle
                        }
                        else if(dynCollider is BruteForceCollider)
                        {
                            // triangles vs triangle
                        }
                        else if(dynCollider is VertexCollider)
                        {
                            // vertices vs triangle
                        }
                    }
                }
            }

            private void HandleCSAVsHeightMapCollision(
                CSACollider csaCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {               
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    // position of bounding sphere in world coordinates
                    Vector3 spherePos = csaCollider.BoundingSphere.CenterW;
                    float radius = csaCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        Diagnostics.Tic();
                        foreach (Slice slice in csaCollider.Slices)
                        {
                            if (IntersectionTest.PlaneVsTriangle(slice, tri, out Vector3 vec1, out Vector3 vec2))
                            {
                                // endpoints of the plane-triangle intersection in plane local coordinates
                                Vector2 vecA = slice.TransformToLocalBase(vec1);
                                Vector2 vecB = slice.TransformToLocalBase(vec2);

                                // calculating the data for transforming the curve
                                float theta = Mathf.Atan2(vecB.y - vecA.y, vecB.x - vecA.x);
                                Complex segRotation = new Complex(theta);
                                Transform2D segTrf = new Transform2D(-vecA, segRotation);
                                float segLength = Vector2.Distance(vecA, vecB);

                                Vector2 upInLocal = slice.TransformToLocalBase(vec1 + tri.Normal);

                                foreach (FourierCurve curve in slice.curves)
                                {
                                    if (IntersectionTest.SegmentVsRectangle(curve.AABB, segTrf, segLength))
                                    {
                                        // add the segments to render on the scene
                                        debugSegments.Add(new Segment(vec1, vec2));
                                        coarseContacts.Add(new Segment(vec1, vec2));

                                        if (csaCollider.rootFinder == CSACollider.RootFinder.SimulatedAnnealing)
                                        {
                                            if (IntersectionTest.SegmentVsCurveSA(
                                                curve,
                                                vecA,
                                                vecB,
                                                segLength,
                                                segRotation,
                                                upInLocal,
                                                out Vector2 contactInPlaneLocal,
                                                out float penetration,
                                                annealingTemperature,
                                                penetrationLimit))
                                            {
                                                // rotate back the contact point into world coordinates
                                                Vector3 contactInWorld = contactInPlaneLocal.x * slice.LocalX + contactInPlaneLocal.y * slice.LocalY + slice.RefPoint;

                                                debugPoints.Add(contactInWorld);
                                                segmentColor.Add(Color.green);

                                                // the contact point relative to the object's transform
                                                Vector3 contactInLocal = contactInPlaneLocal.x * slice.LocalXL + contactInPlaneLocal.y * slice.LocalYL;

                                                RigidBody body = csaCollider.GetComponent<RigidBody>();
                                                if (!csaCollider.isTrigger && body != null && body.enabled)
                                                {
                                                    contacts.Add(new Contact(tri, contactInWorld, body));
                                                }
                                            }
                                            else
                                            {
                                                segmentColor.Add(Color.magenta);
                                            }
                                        }
                                        else if (csaCollider.rootFinder == CSACollider.RootFinder.AberthEhrlich)
                                        {
                                            List<Vector2> intersections = new List<Vector2>();
                                            if (IntersectionTest.SegmentVsCurveAE(
                                                curve,
                                                vecA,
                                                vecB,
                                                segLength,
                                                segRotation,
                                                upInLocal,
                                                intersections))
                                            {
                                                foreach (Vector2 intPlane in intersections)
                                                {
                                                    Vector3 intInWorld = intPlane.x * slice.LocalX + intPlane.y * slice.LocalY + slice.RefPoint;
                                                    debugPoints.Add(intInWorld);
                                                    segmentColor.Add(Color.green);

                                                    RigidBody body = csaCollider.GetComponent<RigidBody>();
                                                    if (!csaCollider.isTrigger && body != null && body.enabled)
                                                    {
                                                        contacts.Add(new Contact(tri, intInWorld, body));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                segmentColor.Add(Color.magenta);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Diagnostics.Toc2Log("slice time");
                    }
                }              
            }

            private void HandleBruteForceVsHeightMapCollision(
                BruteForceCollider bfCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    // position of bounding sphere in world coordinates
                    Vector3 spherePos = bfCollider.BoundingSphere.CenterW;
                    float radius = bfCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        // transform the hMap's triangle into the dynamic object's local coordinate system
                        Triangle hMapTri = new Triangle(
                            Quaternion.Inverse(bfCollider.transform.rotation) * (tri[0] - bfCollider.transform.position),
                            Quaternion.Inverse(bfCollider.transform.rotation) * (tri[1] - bfCollider.transform.position),
                            Quaternion.Inverse(bfCollider.transform.rotation) * (tri[2] - bfCollider.transform.position)
                            );

                        foreach (Triangle bfTri in bfCollider.Triangles)
                        {
                            if(IntersectionTest.MollerTriangleVsTriangle(bfTri,hMapTri, out Vector3 w1, out Vector3 w2, out int contactCount))
                            {
                                RigidBody body = bfCollider.GetComponent<RigidBody>();

                                if (body)
                                {
                                    Vector3 contact1InWorld = bfCollider.transform.rotation * w1 + bfCollider.transform.position;
                                    contacts.Add(new Contact(tri, contact1InWorld, body));


                                    if (contactCount > 1)
                                    {
                                        Vector3 contact2InWorld = bfCollider.transform.rotation * w1 + bfCollider.transform.position;
                                        contacts.Add(new Contact(tri, contact2InWorld, body));
                                    }
                                }

                                debugPoints.Add(bfCollider.transform.rotation * w1 + bfCollider.transform.position);
                                if(contactCount > 1) debugPoints.Add(bfCollider.transform.rotation * w2 + bfCollider.transform.position);

                                //intersectedSegments.Add(new Segment(
                                //        bfCollider.transform.rotation * w1 + bfCollider.transform.position,
                                //        bfCollider.transform.rotation * w2 + bfCollider.transform.position
                                //        ));


                                //segmentColor.Add(Color.green);

                                //intersectedPoints.Add(bfCollider.transform.rotation * w1 + bfCollider.transform.position);
                                //intersectedPoints.Add(bfCollider.transform.rotation * w2 + bfCollider.transform.position);
                            }
                        }
                    }
                }
            }

            private void HandleVertexVsHeightMapCollision(
                VertexCollider vertCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    // position of bounding sphere in world coordinates
                    Vector3 spherePos = vertCollider.BoundingSphere.CenterW;
                    float radius = vertCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        // transform the hMap's triangle into the dynamic object's local coordinate system
                        Triangle hMapTri = new Triangle(
                            Quaternion.Inverse(vertCollider.transform.rotation) * (tri[0] - vertCollider.transform.position),
                            Quaternion.Inverse(vertCollider.transform.rotation) * (tri[1] - vertCollider.transform.position),
                            Quaternion.Inverse(vertCollider.transform.rotation) * (tri[2] - vertCollider.transform.position)
                            );

                        Vector3 aabbMinTri = new Vector3(
                                Mathf.Min(hMapTri[0].x, hMapTri[1].x, hMapTri[2].x),
                                Mathf.Min(hMapTri[0].y, hMapTri[1].y, hMapTri[2].y),
                                Mathf.Min(hMapTri[0].z, hMapTri[1].z, hMapTri[2].z)
                                );

                        Vector3 aabbMaxTri = new Vector3(
                                Mathf.Max(hMapTri[0].x, hMapTri[1].x, hMapTri[2].x),
                                Mathf.Max(hMapTri[0].y, hMapTri[1].y, hMapTri[2].y),
                                Mathf.Max(hMapTri[0].z, hMapTri[1].z, hMapTri[2].z)
                            );

                        foreach (Vector3 vert in vertCollider.Vertices)
                        {
                            if( IntersectionTest.PointInAABB(vert,aabbMinTri,aabbMaxTri) &&
                                IntersectionTest.IsVertexUnderTriangle(hMapTri, vert, out Vector3 projection, out float dist))
                            {
                                Vector3 contactInWorl = vertCollider.transform.rotation * vert + vertCollider.transform.position;
                                RigidBody body = vertCollider.GetComponent<RigidBody>();
                                //if (dist > 1) Debug.Log("dist: "+dist);

                                if (body != null)
                                {
                                    contacts.Add(new Contact(tri, contactInWorl, body));
                                }


                                debugPoints.Add(contactInWorl);
                            }
                        }
                    }
                }
            }

            private void HandleSAGJKVsHeightMapCollision(
                SAGJKCollider sagjkCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    Vector3 spherePos = sagjkCollider.BoundingSphere.CenterW;
                    float radius = sagjkCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if(true || IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        // transform the hMap's triangle into the dynamic object's local coordinate system
                        Triangle hMapTri = new Triangle(
                            Quaternion.Inverse(sagjkCollider.transform.rotation) * (tri[0] - sagjkCollider.transform.position),
                            Quaternion.Inverse(sagjkCollider.transform.rotation) * (tri[1] - sagjkCollider.transform.position),
                            Quaternion.Inverse(sagjkCollider.transform.rotation) * (tri[2] - sagjkCollider.transform.position)
                            );

                        Vector3 p, q, S;
                        //if (IntersectionTest.ChungWangTest(
                        //    hMapTri,
                        //    cvxCollider.Vertices,
                        //    cvxCollider.Adjacencies,
                        //    cvxCollider.BoundingSphere.Center,
                        //    cvxCollider.ChungWangIt,
                        //    out p,
                        //    out q,
                        //    out S
                        //    ))

                        if(IntersectionTest.ChungWangTest(
                            sagjkCollider.CollisionShape,
                            hMapTri as IConvexShape,
                            hMapTri.Normal,
                            sagjkCollider.ChungWangIt,
                            out p, out q, out S))
                        {
                            // separating axis exists => no intersection

                            debugPoints.Add(sagjkCollider.transform.rotation * p + sagjkCollider.transform.position);
                            debugPoints.Add(sagjkCollider.transform.rotation * q + sagjkCollider.transform.position);

                            AddDebugVector(new Segment((p + q) / 2, (p + q) / 2 + S), sagjkCollider.transform);
                            //Debug.Log("Separating axis found.");
                        }
                        else
                        {
                            // separating axis probably not exists => further examination is needed.
                            colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.green;

                            if(IntersectionTest.SAGJK(
                                sagjkCollider.CollisionShape,
                                hMapTri as IConvexShape,
                                sagjkCollider.CenterOfMass - hMapTri.Center,
                                sagjkCollider.gjkMaxIt,
                                out Vector3[] W,
                                out Vector3[] AW,
                                out Vector3[] BW))
                            {
                                colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.cyan;
                            }

                            debugPoints.Add(sagjkCollider.transform.rotation * p + sagjkCollider.transform.position);
                            debugPoints.Add(sagjkCollider.transform.rotation * q + sagjkCollider.transform.position);

                            AddDebugVector(new Segment((p + q) / 2, (p + q) / 2 + 5 * S), sagjkCollider.transform);
                            //Debug.Log(p);
                            //Debug.Log("CW didn't find separating axis");
                        }


                    }
                }
            }

            private void HandleGJKVsHeightMapCollision(
                GJKCollider gjkCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    Vector3 spherePos = gjkCollider.BoundingSphere.CenterW;
                    float radius = gjkCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        // transform the hMap's triangle into the dynamic object's local coordinate system
                        Triangle hMapTri = new Triangle(
                            Quaternion.Inverse(gjkCollider.transform.rotation) * (tri[0] - gjkCollider.transform.position),
                            Quaternion.Inverse(gjkCollider.transform.rotation) * (tri[1] - gjkCollider.transform.position),
                            Quaternion.Inverse(gjkCollider.transform.rotation) * (tri[2] - gjkCollider.transform.position)
                            );

                        Vector3 p, q, S;

                        if (!IntersectionTest.ChungWangTest(
                            gjkCollider.CollisionShape,
                            hMapTri as IConvexShape,
                            hMapTri.Normal,
                            gjkCollider.ChungWangIt,
                            out p, out q, out S))                      
                        {
                            // separating axis probably not exists => further examination is needed.
                            colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.green;

                            bool hit = IntersectionTest.GJK(
                                gjkCollider.CollisionShape,
                                hMapTri as IConvexShape,
                                gjkCollider.CenterOfMass,
                                hMapTri.Center,
                                gjkCollider.gjkMaxIt,
                                out uint y,
                                out Vector3[] W,
                                out Vector3[] AW,
                                out Vector3[] BW,
                                out Vector3 v,
                                out Vector3 a,
                                out Vector3 b);

                            if (hit && y == 0xf)
                            {
                                colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.cyan;

                                if(IntersectionTest.EPA(
                                    gjkCollider.CollisionShape,
                                    hMapTri as IConvexShape,
                                    W, AW, BW,
                                    out v, out a, out b
                                ))
                                {
                                    Vector3 contactInWorl = gjkCollider.transform.rotation * a + gjkCollider.transform.position;
                                    RigidBody body = gjkCollider.GetComponent<RigidBody>();

                                    if (body && body.enabled)
                                        contacts.Add(new Contact(tri, contactInWorl, body));

                                    debugPoints.Add(contactInWorl);
                                }
                            }
                            else if(y != 0xf)
                            {
                                uint bit = 0x0u;
                                int count = 0;
                                for (int i = 0; i < 4; i++)
                                {
                                    if ((bit & y) != 0x0u)
                                        count++;

                                    bit <<= 1;
                                }
                            }

                            //debugPoints.Add(gjkCollider.transform.rotation * a + gjkCollider.transform.position);
                            //AddDebugVector(new Segment(a, b), gjkCollider.transform);
                        }

                        
                    }
                }
            }

            private void HandleXenoVsHeighMapCollision(
                XenoCollider xenoCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    Vector3 spherePos = xenoCollider.BoundingSphere.CenterW;
                    float radius = xenoCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (true || IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        // transform the hMap's triangle into the dynamic object's local coordinate system
                        Triangle hMapTri = new Triangle(
                            Quaternion.Inverse(xenoCollider.transform.rotation) * (tri[0] - xenoCollider.transform.position),
                            Quaternion.Inverse(xenoCollider.transform.rotation) * (tri[1] - xenoCollider.transform.position),
                            Quaternion.Inverse(xenoCollider.transform.rotation) * (tri[2] - xenoCollider.transform.position)
                            );

                        if(IntersectionTest.MPR(
                            xenoCollider.CollisionShape,
                            hMapTri,
                            xenoCollider.CenterOfMass,
                            hMapTri.Center,
                            out Vector3 v,
                            out Vector3 a,
                            out Vector3 b))
                        {
                            colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.cyan;

                            Vector3 contactInWorl = xenoCollider.transform.rotation * a + xenoCollider.transform.position;
                            RigidBody body = xenoCollider.GetComponent<RigidBody>();

                            if (body && body.enabled)
                                contacts.Add(new Contact(tri, contactInWorl, body));

                            debugPoints.Add(contactInWorl);
                        }
                    }
                }
            }

            private void HandleConvDecompVsHeighMapCollision(
                ConvDecompCollider cdCollider,
                HeightMapCollider hMapCollider,
                List<Contact> contacts,
                List<Segment> coarseContacts
                )
            {
                foreach (Triangle tri in hMapCollider.TriGrid)
                {
                    Vector3Int ind = hMapCollider.TriGrid.CurrentIndices;

                    Vector3 spherePos = cdCollider.BoundingSphere.CenterW;
                    float radius = cdCollider.BoundingSphere.Radius;

                    colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                    if (IntersectionTest.SphereVsTriangle(spherePos, radius, tri))
                    {
                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.red;

                        // transform the hMap's triangle into the dynamic object's local coordinate system
                        Triangle hMapTri = new Triangle(
                            Quaternion.Inverse(cdCollider.transform.rotation) * (tri[0] - cdCollider.transform.position),
                            Quaternion.Inverse(cdCollider.transform.rotation) * (tri[1] - cdCollider.transform.position),
                            Quaternion.Inverse(cdCollider.transform.rotation) * (tri[2] - cdCollider.transform.position)
                            );

                        Diagnostics.Tic();

                        foreach (IConvexShape convShape in cdCollider.CollisionShapes)
                        {
                            if(cdCollider.ChungWangEnabled &&
                                IntersectionTest.ChungWangTest(
                                    convShape,
                                    hMapTri as IConvexShape,
                                    hMapTri.Normal,
                                    cdCollider.ChungWangIt,
                                    out var p, out var q, out var S))
                            {
                                continue;
                            }

                            if(colors[ind.x] != Color.cyan)
                                colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.blue;

                            switch (cdCollider.collisionSolver)
                            {
                                case ConvDecompCollider.CollisionSolver.GJK:

                                    bool hit = IntersectionTest.GJK(
                                    convShape,
                                    hMapTri as IConvexShape,
                                    convShape.Center,
                                    hMapTri.Center,
                                    cdCollider.iterations,
                                    out uint y,
                                    out Vector3[] W,
                                    out Vector3[] AW,
                                    out Vector3[] BW,
                                    out Vector3 v,
                                    out Vector3 a,
                                    out Vector3 b);

                                    if(hit && y == 0xf)
                                    {
                                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.cyan;

                                        if (IntersectionTest.EPA(
                                            convShape,
                                            hMapTri as IConvexShape,
                                            W, AW, BW,
                                            out v, out a, out b
                                        ))
                                        {
                                            Vector3 contactInWorl = cdCollider.transform.rotation * a + cdCollider.transform.position;
                                            RigidBody body = cdCollider.GetComponent<RigidBody>();

                                            if (body && body.enabled)
                                                contacts.Add(new Contact(tri, contactInWorl, body));

                                            debugPoints.Add(contactInWorl);
                                        }
                                    }


                                    break;
                                case ConvDecompCollider.CollisionSolver.MPR:

                                    if (IntersectionTest.MPR(
                                        convShape,
                                        hMapTri,
                                        convShape.Center,
                                        hMapTri.Center,
                                        out v,
                                        out a,
                                        out b))
                                    {
                                        colors[ind.x] = colors[ind.y] = colors[ind.z] = Color.cyan;

                                        Vector3 contactInWorl = cdCollider.transform.rotation * a + cdCollider.transform.position;
                                        RigidBody body = cdCollider.GetComponent<RigidBody>();

                                        if (body && body.enabled)
                                            contacts.Add(new Contact(tri, contactInWorl, body));

                                        debugPoints.Add(contactInWorl);
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        Diagnostics.Toc2Log("GJK time");
                    }

                }
            }

            /// <summary>
            /// Casts a ray to the world and finds the hitted <c>CSACollider</c>
            /// </summary>
            /// <param name="ray"> The <c>Ray</c> which is needed cast.</param>
            /// <param name="hittedCollider"> The <c>CSACollider</c> hitted. </param>
            /// <param name="maxDistance"> Maximumal distance of the hitted point. </param>
            /// <returns>
            /// True if any collider hitted, false otherwise.
            /// </returns>
            public bool Raycast(Ray ray, out DynamicCollider hittedCollider, float maxDistance = 100f)
            {
                hittedCollider = null;
                float minDist = maxDistance;

                foreach (DynamicCollider dynCollider in dynamicColliderList)
                {
                    if (IntersectionTest.SphereVsLine(dynCollider.BoundingSphere, ray, out float dSphere))
                    {
                        if (dynCollider is CSACollider)
                        {
                            CSACollider csaCollider = dynCollider as CSACollider;                         
                            float minDistPlane = maxDistance;

                            foreach (Slice slice in csaCollider.Slices)
                            {
                                if (IntersectionTest.PlaneVsRay(slice, ray, out float dPlane))
                                {
                                    Vector3 hitPt = ray.origin + dPlane * ray.direction;
                                    Vector2 intersectionOnPlane = slice.TransformToLocalBase(hitPt);
                                    foreach (FourierCurve curve in slice.curves)
                                    {
                                        if (minDist > dPlane && IntersectionTest.PointIn2DAABB(intersectionOnPlane, curve.AABB))
                                        {
                                            hittedCollider = csaCollider;
                                            minDist = dPlane;
                                        }
                                    }
                                }
                            }                         
                        }
                        else if(dynCollider is BruteForceCollider)
                        {
                            BruteForceCollider bfCollider = dynCollider as BruteForceCollider;

                            Ray transformedRay = new Ray(
                                Quaternion.Inverse(dynCollider.transform.rotation) * (ray.origin - dynCollider.transform.position),
                                Quaternion.Inverse(dynCollider.transform.rotation) * ray.direction
                                );

                            foreach(Triangle triangle in bfCollider.Triangles)
                            {
                                if(IntersectionTest.TriangleVsLine(triangle, transformedRay, out Vector3 hitPt, out float signedDist))
                                {
                                    float dist = Mathf.Abs(signedDist); /*Vector2.Distance(transformedRay.origin, hitPt);*/
                                    if(minDist > dist)
                                    {
                                        hittedCollider = bfCollider;
                                        minDist = dist;
                                    }
                                }
                            }
                        }
                        else if(dynCollider is VertexCollider || dynCollider is ConvexCollider || dynCollider is ConvDecompCollider)
                        {
                            if(IntersectionTest.SphereVsRay(dynCollider.BoundingSphere,ray,out float dist))
                            {
                                hittedCollider = dynCollider;
                                minDist = dist;
                            }                          
                        }
                        //else if(dynCollider is ConvexCollider)
                        //{
                        //    if(IntersectionTest.SphereVsRay(dynCollider.BoundingSphere,ray,out float dist))
                        //    {
                        //        debugPoints.Clear();
                        //        ConvexCollider cvCollider = dynCollider as ConvexCollider;
                        //        for (int i = 0; i < cvCollider.Vertices.Count; i++)
                        //        {
                        //            Sphere sp = new Sphere(cvCollider.transform, cvCollider.Vertices[i], 0.1f);
                        //            if(IntersectionTest.SphereVsRay(sp,ray,out float sgndist))
                        //            {
                        //                AddDebugPoint(cvCollider.Vertices[i], cvCollider.transform);
                        //                foreach (int adj in cvCollider.Adjacencies[i])
                        //                {
                        //                    AddDebugPoint(cvCollider.Vertices[adj], cvCollider.transform);
                        //                }
                        //            }
                        //        }

                        //        hittedCollider = dynCollider;
                        //        minDist = dist;
                        //    }
                        //}
                    }
                }

                return hittedCollider != null;
            }

            private void AddDebugVector(Segment vector, Transform trans)
            {
                debugVectors.Add((trans.rotation * vector) + trans.position);
            }

            private void AddDebugPoint(Vector3 point, Transform trans)
            {
                debugPoints.Add((trans.rotation * point) + trans.position);
            }

            private void OnDrawGizmos()
            {
                if (debugSegments != null)
                {
                    int i = 0;
                    foreach (var segment in debugSegments)
                    {
                        Gizmos.color = segmentColor[i++];
                        Gizmos.DrawSphere(segment.start, 0.03f);
                        Gizmos.DrawSphere(segment.end, 0.03f);
                        Gizmos.DrawLine(segment.start, segment.end);
                    }                   
                }

                if(debugPoints != null)
                {                    
                    foreach (var point in debugPoints)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(point, 0.1f);
                    }
                }

                if (debugVectors != null)
                {
                    foreach (var vec in debugVectors)
                    {
                        Gizmos.color = Color.magenta;
                        Gizmos.DrawCube(vec.start, new Vector3(0.3f, 0.3f, 0.3f));
                        Gizmos.DrawSphere(vec.end, 0.3f);
                        Gizmos.DrawLine(vec.start, vec.end);
                    }
                }
            }
        }
    }
}

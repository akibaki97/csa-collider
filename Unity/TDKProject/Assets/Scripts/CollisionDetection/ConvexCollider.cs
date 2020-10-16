using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

namespace Model
{
    namespace CollisionDetection
    {
        public class ConvexCollider : DynamicCollider
        {
            public ViewParameters viewParameters;

            public IList<Vector3> Vertices { get => mShape.Vertices; }
            public IList<IList<int>> Adjacencies { get => mShape.Adj; }
            public IConvexShape CollisionShape { get => mShape as IConvexShape; }
            public Vector3 CenterOfMass { get => mCenterOfMass; }

            public TextAsset meshObj;

            protected new void Start()
            {
                base.Start();

                //InitShapeFromJson(adjacenciesJsonFile.ToString(), out List<Vector3> vertices, out List<List<int>> adj);
                ObjParser.ParseSimpleObj(meshObj.ToString(), out List<Vector3> vertices, out List<int> indices);

                mMesh = new Mesh
                {
                    vertices = vertices.ToArray(),
                    triangles = indices.ToArray()
                };
                mMesh.RecalculateNormals();

                mShape = new ConvexPolyhShape(vertices, indices);
                computeCenterOfMass(vertices);
            }

            public override void EditorStart()
            {
                base.EditorStart();

                ObjParser.ParseSimpleObj(meshObj.ToString(), out List<Vector3> vertices, out List<int> indices);

                mMesh = new Mesh
                {
                    vertices = vertices.ToArray(),
                    triangles = indices.ToArray()
                };
                mMesh.RecalculateNormals();

                mShape = new ConvexPolyhShape(vertices, indices);
                computeCenterOfMass(vertices);
            }

            private void InitShapeFromJson(string json, out List<Vector3> vertices, out List<List<int>> adj)
            {
                VertexDataArray adjDataArray = JsonUtility.FromJson<VertexDataArray>(json);


                //Vector3 translate = new Vector3(
                //    adjDataArray.vertices[0].x,
                //    adjDataArray.vertices[0].y,
                //    adjDataArray.vertices[0].z);
                //translate -= mMesh.vertices[0];

                vertices = new List<Vector3>();

                adj = new List<List<int>>();

                foreach (VertexData vertexData in adjDataArray.vertices)
                {
                    vertices.Add(new Vector3(
                        -vertexData.x, // TODO: ...
                        vertexData.y,
                        vertexData.z
                        ));
                    adj.Add(vertexData.adj);
                }

            }

            private void computeCenterOfMass(List<Vector3> vertices)
            {
                mCenterOfMass = new Vector3(0, 0, 0);
                foreach (Vector3 v in vertices)
                    mCenterOfMass += v;

                mCenterOfMass /= vertices.Count;
            }

            private List<List<int>> BuildAdjacencies(List<Vector3> vertices, List<int> indices)
            {
                List<List<int>> adj = new List<List<int>>(vertices.Count);
                int[,] adjMx = new int[vertices.Count, vertices.Count];

                for (int i = 0; i < mMesh.triangles.Length; i += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int currInd = mMesh.triangles[i + j];

                        adjMx[currInd, mMesh.triangles[i + ((j + 1)) % 3]] = 1;
                        adjMx[currInd, mMesh.triangles[i + ((j + 2)) % 3]] = 1;
                    }
                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    adj.Add(new List<int>());
                    for (int j = 0; j < vertices.Count; j++)
                        if (adjMx[i, j] == 1) adj[i].Add(j);
                }

                return adj;
            }


            [System.Serializable]
            public class ViewParameters
            {
                public bool drawBoundingSphere;
                public bool drawTriangles;
                public bool drawVertices;
            }
            
            [System.Serializable]
            public class VertexData
            {
                public float x;
                public float y;
                public float z;
                public List<int> adj;
            }

            [System.Serializable]
            public class VertexDataArray
            {
                public VertexDataArray() { }

                public List<VertexData> vertices;
            }

            private void OnDrawGizmos()
            {
                if (viewParameters.drawTriangles && mMesh != null)
                {
                    Gizmos.color = GizmosExtension.lightGreen;
                    Gizmos.DrawWireMesh(mMesh, transform.position, transform.rotation);
                }

                if (viewParameters.drawBoundingSphere && BoundingSphere.CenterW != null)
                {
                    Gizmos.color = GizmosExtension.lightBlue;
                    Gizmos.DrawWireSphere(BoundingSphere.CenterW, BoundingSphere.Radius);
                }

                if(viewParameters.drawVertices && Vertices != null)
                {
                    Gizmos.color = GizmosExtension.lightGreen;
                    foreach (Vector3 vert in Vertices)
                    {                     
                        Gizmos.DrawSphere(transform.rotation * vert + transform.position, 0.1f);
                    }
                }
            }

            private ConvexPolyhShape mShape;
            private Vector3 mCenterOfMass;
        }
    }
}

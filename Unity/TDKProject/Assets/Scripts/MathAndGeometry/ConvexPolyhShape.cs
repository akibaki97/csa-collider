using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    namespace MathAndGeometry
    {
        public class ConvexPolyhShape : IConvexShape
        {
            public ConvexPolyhShape(Mesh mesh)
            {
                mVertices = new List<Vector3>(mesh.vertices);
                mAdj = BuildAdjacencies(mVertices, mesh.triangles);
                mCenter = MathUtils.CenterOfMass(mesh.vertices);
            }

            public ConvexPolyhShape(List<Vector3> vertices, List<int> indices)
            {
                mVertices = vertices;
                mAdj = BuildAdjacencies(vertices, indices);
                mCenter = MathUtils.CenterOfMass(vertices);
            }

            public ConvexPolyhShape(List<Vector3> vertices, List<List<int>> adj)
            {
                mVertices = vertices;
                mAdj = adj as List<List<int>>;
                mCenter = MathUtils.CenterOfMass(vertices);
            }

            public IList<Vector3> Vertices { get => mVertices as IList<Vector3>; }
            public IList<IList<int>> Adj { get => mAdj as IList<IList<int>>; }
            public Vector3 Center {get => mCenter;}

            /// <summary>
            /// Hill climbing algorithm to find the supporting vertex of a convex polyhedron in a given direction.
            /// </summary>
            /// <returns>Support vertex in the direction of dir.</returns>
            public Vector3 SupportingVertex(Vector3 dir)
            {
                int curr = 0;
                float maxProj = Vector3.Dot(mVertices[curr], dir);
                bool supportFound = false;
                bool[] marked = new bool[mVertices.Count];
                marked[0] = true;

                while (!supportFound)
                {
                    int maxAdj = 0;
                    supportFound = true;
                    foreach (int adjOfInd in mAdj[curr])
                    {
                        if (marked[adjOfInd]) continue;

                        float proj = Vector3.Dot(mVertices[adjOfInd], dir);
                        if (proj >= maxProj)
                        {
                            maxProj = proj;
                            maxAdj = adjOfInd;
                            supportFound = false;
                        }

                        marked[adjOfInd] = true;
                    }

                    if (!supportFound)
                        curr = maxAdj;
                }

                return mVertices[curr];
            }
            public static List<List<int>> BuildAdjacencies(IList<Vector3> vertices, IList<int> indices)
            {
                List<List<int>> adj = new List<List<int>>(vertices.Count);
                int[,] adjMx = new int[vertices.Count, vertices.Count];

                for (int i = 0; i < indices.Count; i += 3)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int currInd = indices[i + j];

                        adjMx[currInd, indices[i + ((j + 1)) % 3]] = 1;
                        adjMx[currInd, indices[i + ((j + 2)) % 3]] = 1;
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


            public List<Vector3> mVertices;
            public List<List<int>> mAdj;
            public Vector3 mCenter;
        }
    }
}

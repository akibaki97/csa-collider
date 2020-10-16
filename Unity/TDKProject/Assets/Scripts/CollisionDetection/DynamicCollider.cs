using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Model.MathAndGeometry;

[RequireComponent(typeof(MeshFilter))]
public abstract class DynamicCollider : MonoBehaviour
{
    protected Sphere mBoundingSphere;
    protected Mesh mMesh;

    public bool isTrigger;
    public Sphere BoundingSphere { get { return mBoundingSphere; } }

    public PhysicMaterial material;

    protected void Start()
    {
        mMesh = GetComponent<MeshFilter>().mesh;
        mBoundingSphere = Sphere.BoundingSphereFromAABB(mMesh, gameObject.transform);
    }

    public virtual void EditorStart()
    {
        mMesh = GetComponent<MeshFilter>().sharedMesh;
        mBoundingSphere = Sphere.BoundingSphereFromAABB(mMesh, gameObject.transform);
    }

    protected Vector3 CenterOfMassFromVertices()
    {
        Vector3 com = new Vector3(0, 0, 0);
        foreach (Vector3 vert in mMesh.vertices)
            com += vert;

        com /= mMesh.vertexCount;

        return com;
    }
}

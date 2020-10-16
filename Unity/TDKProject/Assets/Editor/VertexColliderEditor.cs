using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Model.CollisionDetection;

[CustomEditor(typeof(VertexCollider))]
public class VertexColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VertexCollider vertCollider = (VertexCollider)target;
        if (DrawDefaultInspector())
        {
            vertCollider.EditorStart();
        }
    }
}

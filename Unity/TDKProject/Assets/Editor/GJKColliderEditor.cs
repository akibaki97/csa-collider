using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Model.CollisionDetection;

[CustomEditor(typeof(GJKCollider))]
public class GJKColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GJKCollider gjkCollider = (GJKCollider)target;
        if (DrawDefaultInspector())
        {
            gjkCollider.EditorStart();
        }
    }
}

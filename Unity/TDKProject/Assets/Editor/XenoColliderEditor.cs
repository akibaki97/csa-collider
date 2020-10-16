using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Model.CollisionDetection;

[CustomEditor(typeof(XenoCollider))]
public class XenoColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        XenoCollider gjkCollider = (XenoCollider)target;
        if (DrawDefaultInspector())
        {
            gjkCollider.EditorStart();
        }
    }
}

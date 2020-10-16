using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Model.CollisionDetection;

[CustomEditor(typeof(CSACollider))]
public class CSAColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CSACollider csaCollider = (CSACollider)target;
        if (DrawDefaultInspector())
        {
            csaCollider.EditorStart();
        }
    }
}
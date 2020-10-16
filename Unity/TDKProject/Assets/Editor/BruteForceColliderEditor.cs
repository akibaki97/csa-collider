using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Model.CollisionDetection;

[CustomEditor(typeof(BruteForceCollider))]
public class BruteForceColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BruteForceCollider bfCollider = (BruteForceCollider)target;
        if (DrawDefaultInspector())
        {
            bfCollider.EditorStart();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Model.CollisionDetection;

[CustomEditor(typeof(ConvDecompCollider))]
public class ConvDecompColliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ConvDecompCollider cdCollider = (ConvDecompCollider)target;
        if(DrawDefaultInspector())
        {
            cdCollider.EditorStart();
        }
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(Model.HeightMapTerrain))]
public class HeightMapTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Model.HeightMapTerrain heightMapTerrain = (Model.HeightMapTerrain)target;
        if (DrawDefaultInspector())
        {
            heightMapTerrain.GenerateHightMapTerrain();
        }
    }
}

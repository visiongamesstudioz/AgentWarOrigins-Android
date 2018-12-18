using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TerrainGenerator))]
public class GenerateTerrainEditor : Editor
{

    public override void OnInspectorGUI()
    {
        TerrainGenerator terrianGen = (TerrainGenerator)target;
        if (DrawDefaultInspector()) {

            terrianGen.GenerateTerrain();

        }
        if (GUILayout.Button("Generate")) {
            terrianGen.GenerateTerrain();

        }
    }


}

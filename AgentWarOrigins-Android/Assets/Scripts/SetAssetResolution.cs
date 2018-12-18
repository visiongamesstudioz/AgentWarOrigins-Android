using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
public class SetAssetResolution : MonoBehaviour
{

    public static string LIGHTMAP_RESOURCE_PATH = "Assets/Resources/Lightmaps/Modified/CityFlyOver/";
    public static string filename= "Assets/Scenes/city_fly_over";

    [MenuItem("Assets/SetAssetResolutionInFolder")]
    static void SetAssetResolutions()
    {
        string[] guids2 = AssetDatabase.FindAssets("Lightmap", new[] {filename});
        Debug.Log(guids2.Length + " Assets");

        foreach (string guid2 in guids2)
        {
            Debug.Log(AssetDatabase.GUIDToAssetPath(guid2));
            string assetpath = AssetDatabase.GUIDToAssetPath(guid2);
            string[] assetSplit = assetpath.Split('-');
            string lightmapIndexString = assetSplit[1];
            string resultString = Regex.Match(lightmapIndexString, @"\d+").Value;
            Debug.Log(Int32.Parse(resultString));
            int lightmapIndex = Int32.Parse(resultString);
            AssetDatabase.ImportAsset(assetpath, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(assetpath) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                //   importer.textureFormat = tf;
                importer.maxTextureSize = 512;
                AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceUpdate);
                var assetLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(assetpath);            
                var lightmappath = LIGHTMAP_RESOURCE_PATH + "city_fly_over" + "_light" + "-" + lightmapIndex + ".asset";
                var newLightmap = Instantiate(assetLightmap) as Texture2D;
                AssetDatabase.CreateAsset(newLightmap, lightmappath);
                newLightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(lightmappath);
                AssetDatabase.ImportAsset(filename, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
#endif

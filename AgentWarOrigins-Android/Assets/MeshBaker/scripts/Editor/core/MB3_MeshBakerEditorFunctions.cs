

using DigitalOpus.MB.Core;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MB3_MeshBakerEditorFunctions
{
    public static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] colorArray = new Color[width * height];
        for (int index = 0; index < colorArray.Length; ++index)
            colorArray[index] = col;
        Texture2D texture2D = new Texture2D(width, height);
        texture2D.SetPixels(colorArray);
        texture2D.Apply();
        return texture2D;
    }

    public static bool BakeIntoCombined(MB3_MeshBakerCommon mom, out bool createdDummyTextureBakeResults)
    {
        MB2_OutputOptions outputOption = mom.meshCombiner.outputOption;
        createdDummyTextureBakeResults = false;
        if (MB3_MeshCombiner.EVAL_VERSION && outputOption == MB2_OutputOptions.bakeIntoPrefab)
        {
            Debug.LogError("Cannot BakeIntoPrefab with evaluation version.");
            return false;
        }
        if (outputOption != MB2_OutputOptions.bakeIntoPrefab && (uint)outputOption > 0U)
        {
            Debug.LogError("Paramater prefabOrSceneObject must be bakeIntoPrefab or bakeIntoSceneObject");
            return false;
        }
        MB3_TextureBaker componentInParent = mom.GetComponentInParent<MB3_TextureBaker>();
        if ((mom.textureBakeResults== null) && (componentInParent!= null))
            mom.textureBakeResults=(componentInParent.textureBakeResults);
        if ((mom.textureBakeResults== null) && _OkToCreateDummyTextureBakeResult(mom))
        {
            createdDummyTextureBakeResults = true;
            List<GameObject> objectsToCombine = ((MB3_MeshBakerRoot)mom).GetObjectsToCombine();
            if (mom.GetNumObjectsInCombined() > 0)
            {
                if ((bool)mom.clearBuffersAfterBake)
                {
                    mom.ClearMesh();
                }
                else
                {
                    Debug.LogError("'Texture Bake Result' must be set to add more objects to a combined mesh that already contains objects. Try enabling 'clear buffers after bake'");
                    return false;
                }
            }
          ((MB3_MeshBakerRoot)mom).textureBakeResults=(MB2_TextureBakeResults.CreateForMaterialsOnRenderer(objectsToCombine.ToArray(), mom.meshCombiner.GetMaterialsOnTargetRenderer()));

            if (mom.meshCombiner.LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log("'Texture Bake Result' was not set. Creating a temporary one. Each material will be mapped to a separate submesh.");
        }
        MB2_ValidationLevel mb2ValidationLevel = Application.isPlaying ? MB2_ValidationLevel.quick : MB2_ValidationLevel.robust;
        if (!MB3_MeshBakerRoot.DoCombinedValidate((MB3_MeshBakerRoot)mom, MB_ObjsToCombineTypes.sceneObjOnly, (MB2_EditorMethodsInterface)new MB3_EditorMethods(), mb2ValidationLevel))
            return false;
        if (outputOption == MB2_OutputOptions.bakeIntoPrefab && (mom.resultPrefab== null))
        {
            Debug.LogError("Need to set the Combined Mesh Prefab field. Create a prefab asset, drag an empty game object into it, and drag it to the 'Combined Mesh Prefab' field.");
            return false;
        }
        if ((mom.meshCombiner.resultSceneObject!= null) && (PrefabUtility.GetPrefabType(mom.meshCombiner.resultSceneObject) == (PrefabType) 2 || PrefabUtility.GetPrefabType(mom.meshCombiner.resultSceneObject) == (PrefabType) 1))
        {
            Debug.LogWarning("Result Game Object was a project asset not a scene object instance. Clearing this field.");
            mom.meshCombiner.resultSceneObject = null;
        }
        mom.ClearMesh();
        if (mom.AddDeleteGameObjects(((MB3_MeshBakerRoot)mom).GetObjectsToCombine().ToArray(), null, false))
        {
            mom.Apply(new MB3_MeshCombiner.GenerateUV2Delegate(UnwrapUV2));
            if (createdDummyTextureBakeResults)
                Debug.Log(string.Format("Successfully baked {0} meshes each material is mapped to its own submesh.", ((MB3_MeshBakerRoot)mom).GetObjectsToCombine().Count));
            else
                Debug.Log(string.Format("Successfully baked {0} meshes", ((MB3_MeshBakerRoot)mom).GetObjectsToCombine().Count));
            switch (outputOption)
            {
                case MB2_OutputOptions.bakeIntoSceneObject:
                    PrefabType prefabType = PrefabUtility.GetPrefabType(mom.meshCombiner.resultSceneObject);
                    if (prefabType == (PrefabType) 1 || prefabType == (PrefabType) 2)
                    {
                        Debug.LogError("Combined Mesh Object is a prefab asset. If output option bakeIntoSceneObject then this must be an instance in the scene.");
                        return false;
                    }
                    break;
                case MB2_OutputOptions.bakeIntoPrefab:
                    string assetPath = AssetDatabase.GetAssetPath(mom.resultPrefab);
                    if (assetPath == null || assetPath.Length == 0)
                    {
                        Debug.LogError("Could not save result to prefab. Result Prefab value is not an Asset.");
                        return false;
                    }
                    string withoutExtension = Path.GetFileNameWithoutExtension(assetPath);
                    string folderPath = assetPath.Substring(0, assetPath.Length - withoutExtension.Length - 7);
                    string newFileNameBase = folderPath + withoutExtension + "-mesh";
                    SaveMeshsToAssetDatabase(mom, folderPath, newFileNameBase);
                    if (mom.meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer)
                        Debug.LogWarning(("Render type is skinned mesh renderer. Can't create prefab until all bones have been added to the combined mesh object " + mom.resultPrefab + " Add the bones then drag the combined mesh object to the prefab."));
                    RebuildPrefab(mom);
                    MB_Utility.Destroy(mom.meshCombiner.resultSceneObject);
                    break;
                default:
                    Debug.LogError("Unknown parameter");
                    return false;
            }
            if ((bool)mom.clearBuffersAfterBake)
                mom.meshCombiner.ClearBuffers();
            if (createdDummyTextureBakeResults)
                MB_Utility.Destroy(((MB3_MeshBakerRoot)mom).textureBakeResults);
            return true;
        }
        if ((bool)mom.clearBuffersAfterBake)
            mom.meshCombiner.ClearBuffers();
        if (createdDummyTextureBakeResults)
            MB_Utility.Destroy(((MB3_MeshBakerRoot)mom).textureBakeResults);
        return false;
    }

    public static void SaveMeshsToAssetDatabase(MB3_MeshBakerCommon mom, string folderPath, string newFileNameBase)
    {
        if (MB3_MeshCombiner.EVAL_VERSION)
            return;
        if (mom is MB3_MeshBaker)
        {
            MB3_MeshBaker mb3MeshBaker = (MB3_MeshBaker)mom;
            string str = newFileNameBase + ".asset";
            string assetPath = AssetDatabase.GetAssetPath(((MB3_MeshCombinerSingle)((MB3_MeshBakerCommon)mb3MeshBaker).meshCombiner).GetMesh());
            if (assetPath == null || assetPath.Equals(""))
            {
                Debug.Log(("Saving mesh asset to " + str));
                AssetDatabase.CreateAsset(((MB3_MeshCombinerSingle)((MB3_MeshBakerCommon)mb3MeshBaker).meshCombiner).GetMesh(), str);
            }
            else
                Debug.Log(("Mesh is an asset at " + assetPath));
        }
        else if (mom is MB3_MultiMeshBaker)
        {
            List<MB3_MultiMeshCombiner.CombinedMesh> meshCombiners = ((MB3_MultiMeshCombiner)mom.meshCombiner).meshCombiners;
            for (int index = 0; index < meshCombiners.Count; ++index)
            {
                string str = newFileNameBase + index + ".asset";
                Mesh mesh = meshCombiners[index].combinedMesh.GetMesh();
                string assetPath = AssetDatabase.GetAssetPath(mesh);
                if (assetPath == null || assetPath.Equals(""))
                {
                    Debug.Log(("Saving mesh asset to " + str));
                    AssetDatabase.CreateAsset(mesh, str);
                }
                else
                    Debug.Log(("Mesh is an asset at " + assetPath));
            }
        }
        else
            Debug.LogError("Argument was not a MB3_MeshBaker or an MB3_MultiMeshBaker.");
    }

    public static void RebuildPrefab(MB3_MeshBakerCommon mom)
    {
        if (MB3_MeshCombiner.EVAL_VERSION)
            return;
        GameObject resultPrefab = (GameObject)mom.resultPrefab;
        GameObject instantiatedPrefabRoot = (GameObject)PrefabUtility.InstantiatePrefab(resultPrefab);
        Renderer[] componentsInChildren = (Renderer[])instantiatedPrefabRoot.GetComponentsInChildren<Renderer>();
        for (int index = 0; index < componentsInChildren.Length; ++index)
        {
            if ((componentsInChildren[index]!= null) && ((componentsInChildren[index]).transform.parent== instantiatedPrefabRoot.transform))
                MB_Utility.Destroy(componentsInChildren[index].gameObject);
        }
        if (mom is MB3_MeshBaker)
        {
            MB3_MeshCombinerSingle meshCombiner = (MB3_MeshCombinerSingle)mom.meshCombiner;
            MB3_MeshCombinerSingle.BuildPrefabHierarchy(meshCombiner, instantiatedPrefabRoot, meshCombiner.GetMesh(), false, (GameObject[])null);
        }
        else if (mom is MB3_MultiMeshBaker)
        {
            MB3_MultiMeshCombiner meshCombiner = (MB3_MultiMeshCombiner)mom.meshCombiner;
            for (int index = 0; index < meshCombiner.meshCombiners.Count; ++index)
                MB3_MeshCombinerSingle.BuildPrefabHierarchy(meshCombiner.meshCombiners[index].combinedMesh, instantiatedPrefabRoot, meshCombiner.meshCombiners[index].combinedMesh.GetMesh(), true, (GameObject[])null);
        }
        else
            Debug.LogError("Argument was not a MB3_MeshBaker or an MB3_MultiMeshBaker.");
        string assetPath = AssetDatabase.GetAssetPath(resultPrefab);
        PrefabUtility.ReplacePrefab(instantiatedPrefabRoot, AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)), (ReplacePrefabOptions)1);
        if (mom.meshCombiner.renderType == MB_RenderType.skinnedMeshRenderer)
            return;
        Object.DestroyImmediate(instantiatedPrefabRoot);
    }

    public static void UnwrapUV2(Mesh mesh, float hardAngle, float packingMargin)
    {
        UnwrapParam unwrapParam;
        UnwrapParam.SetDefaults(out unwrapParam);
        unwrapParam.hardAngle = hardAngle;
        unwrapParam.packMargin = packingMargin;
        Unwrapping.GenerateSecondaryUVSet(mesh, unwrapParam);
    }

    public static bool _OkToCreateDummyTextureBakeResult(MB3_MeshBakerCommon mom)
    {
        return ((MB3_MeshBakerRoot)mom).GetObjectsToCombine().Count != 0;
    }
}

using DigitalOpus.MB.Core;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MB2_TextureBakeResults : ScriptableObject
{
    private const int VERSION = 3230;
    public int version;
    public MB_MaterialAndUVRect[] materialsAndUVRects;
    public MB_MultiMaterial[] resultMaterials;
    public bool doMultiMaterial;
    public Material[] materials;
    public bool fixOutOfBoundsUVs;
    public Material resultMaterial;

    private void OnEnable()
    {
        if (version < 3230 && resultMaterials != null)
        {
            for (int index = 0; index < resultMaterials.Length; ++index)
                resultMaterials[index].considerMeshUVs = fixOutOfBoundsUVs;
        }
        version = 3230;
    }

    public static MB2_TextureBakeResults CreateForMaterialsOnRenderer(GameObject[] gos, List<Material> matsOnTargetRenderer)
    {
        HashSet<Material> materialSet = new HashSet<Material>(matsOnTargetRenderer);
        for (int index1 = 0; index1 < gos.Length; ++index1)
        {
            if ((gos[index1]== null))
            {
                Debug.LogError(string.Format("Game object {0} in list of objects to add was null", index1));
                return null;
            }
            Material[] goMaterials = MB_Utility.GetGOMaterials(gos[index1]);
                 
         
            Debug.Log("material count" + goMaterials.Length);
            
            if (goMaterials.Length == 0)
            {
                Debug.LogError(string.Format("Game object {0} in list of objects to add no renderer", index1));
                return null;
            }
      
            for (int index2 = 0; index2 < goMaterials.Length; ++index2)
            {
                if (!materialSet.Contains(goMaterials[index2]))
                    materialSet.Add(goMaterials[index2]);
            }
        }
        Material[] array = new Material[materialSet.Count];
        materialSet.CopyTo(array);
        MB2_TextureBakeResults instance = (MB2_TextureBakeResults)CreateInstance(typeof(MB2_TextureBakeResults));
        List<MB_MaterialAndUVRect> materialAndUvRectList = new List<MB_MaterialAndUVRect>();
        for (int index = 0; index < array.Length; ++index)
        {
            if ((array[index]!= null))
            {
                MB_MaterialAndUVRect materialAndUvRect = new MB_MaterialAndUVRect(array[index], new Rect(0.0f, 0.0f, 1f, 1f), new Rect(0.0f, 0.0f, 1f, 1f), new Rect(0.0f, 0.0f, 1f, 1f), new Rect(0.0f, 0.0f, 1f, 1f), "");
                if (!materialAndUvRectList.Contains(materialAndUvRect))
                    materialAndUvRectList.Add(materialAndUvRect);
            }
        }
        Material[] materialArray;
        instance.materials = materialArray = new Material[materialAndUvRectList.Count];
        instance.resultMaterials = new MB_MultiMaterial[materialAndUvRectList.Count];
        for (int index = 0; index < materialAndUvRectList.Count; ++index)
        {
            materialArray[index] = materialAndUvRectList[index].material;
            instance.resultMaterials[index] = new MB_MultiMaterial();
            List<Material> materialList = new List<Material>();
            materialList.Add(materialAndUvRectList[index].material);
            instance.resultMaterials[index].sourceMaterials = materialList;
            instance.resultMaterials[index].combinedMaterial = materialArray[index];
            instance.resultMaterials[index].considerMeshUVs = false;
        }
        instance.doMultiMaterial = array.Length != 1;
        instance.materialsAndUVRects = materialAndUvRectList.ToArray();
        return instance;
    }

    public bool DoAnyResultMatsUseConsiderMeshUVs()
    {
        if (resultMaterials == null)
            return false;
        for (int index = 0; index < resultMaterials.Length; ++index)
        {
            if (resultMaterials[index].considerMeshUVs)
                return true;
        }
        return false;
    }

    public bool ContainsMaterial(Material m)
    {
        for (int index = 0; index < materialsAndUVRects.Length; ++index)
        {
            if ((materialsAndUVRects[index].material== m))
                return true;
        }
        return false;
    }

    public string GetDescription()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("Shaders:\n");
        HashSet<Shader> shaderSet = new HashSet<Shader>();
        if (materialsAndUVRects != null)
        {
            for (int index = 0; index < materialsAndUVRects.Length; ++index)
            {
                if ((materialsAndUVRects[index].material!= null))
                    shaderSet.Add(materialsAndUVRects[index].material.shader);
            }
        }
        using (HashSet<Shader>.Enumerator enumerator = shaderSet.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Shader current = enumerator.Current;
                stringBuilder.Append("  ").Append((current).name).AppendLine();
            }
        }
        stringBuilder.Append("Materials:\n");
        if (materialsAndUVRects != null)
        {
            for (int index = 0; index < materialsAndUVRects.Length; ++index)
            {
                if ((materialsAndUVRects[index].material!= null))
                    stringBuilder.Append("  ").Append((materialsAndUVRects[index].material).name).AppendLine();
            }
        }
        return stringBuilder.ToString();
    }

    public static bool IsMeshAndMaterialRectEnclosedByAtlasRect(Rect uvR, Rect sourceMaterialTiling, Rect samplingEncapsulatinRect, MB2_LogLevel logLevel)
    {
        Rect r2 = sourceMaterialTiling;
        Rect rect1 = samplingEncapsulatinRect;
        MB3_UVTransformUtility.Canonicalize(ref rect1, 0.0f, 0.0f);
        Rect rect2 = MB3_UVTransformUtility.CombineTransforms(ref uvR, ref r2);
        if (logLevel >= MB2_LogLevel.trace)
        {
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            Debug.Log(("uvR=" + uvR.ToString("f5") + " matR=" + r2.ToString("f5") + "Potential Rect " + rect2.ToString("f5") + " encapsulating=" + rect1.ToString("f5")));
        }
        // ISSUE: explicit reference operation
        // ISSUE: explicit reference operation
        MB3_UVTransformUtility.Canonicalize(ref rect2, (rect1).x, (rect1).y);
        if (logLevel >= MB2_LogLevel.trace)
        {
            // ISSUE: explicit reference operation
            // ISSUE: explicit reference operation
            Debug.Log(("Potential Rect Cannonical " + rect2.ToString("f5") + " encapsulating=" + rect1.ToString("f5")));
        }
        return MB3_UVTransformUtility.RectContains(ref rect1, ref rect2);
    }

    public MB2_TextureBakeResults()
    {
     //   base.;
    }

    public class Material2AtlasRectangleMapper
    {
        private MB2_TextureBakeResults tbr;
        private int[] numTimesMatAppearsInAtlas;
        private MB_MaterialAndUVRect[] matsAndSrcUVRect;

        public Material2AtlasRectangleMapper(MB2_TextureBakeResults res)
        {
            tbr = res;
            matsAndSrcUVRect = res.materialsAndUVRects;
            numTimesMatAppearsInAtlas = new int[matsAndSrcUVRect.Length];
            for (int index1 = 0; index1 < matsAndSrcUVRect.Length; ++index1)
            {
                if (numTimesMatAppearsInAtlas[index1] <= 1)
                {
                    int num = 1;
                    for (int index2 = index1 + 1; index2 < matsAndSrcUVRect.Length; ++index2)
                    {
                        if ((matsAndSrcUVRect[index1].material== matsAndSrcUVRect[index2].material))
                            ++num;
                    }
                    numTimesMatAppearsInAtlas[index1] = num;
                    if (num > 1)
                    {
                        for (int index2 = index1 + 1; index2 < matsAndSrcUVRect.Length; ++index2)
                        {
                            if ((matsAndSrcUVRect[index1].material== matsAndSrcUVRect[index2].material))
                                numTimesMatAppearsInAtlas[index2] = num;
                        }
                    }
                }
            }
        }

        public bool TryMapMaterialToUVRect(Material mat, Mesh m, int submeshIdx, int idxInResultMats, MB3_MeshCombinerSingle.MeshChannelsCache meshChannelCache, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisCache, out Rect rectInAtlas, out Rect encapsulatingRect, out Rect sourceMaterialTilingOut, ref string errorMsg, MB2_LogLevel logLevel)
        {
            if (tbr.materialsAndUVRects.Length == 0 && (uint)tbr.materials.Length > 0U)
            {
                errorMsg = "The 'Texture Bake Result' needs to be re-baked to be compatible with this version of Mesh Baker. Please re-bake using the MB3_TextureBaker.";

                rectInAtlas = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            if ((mat== null))
            {
                
                errorMsg = string.Format("Mesh {0} Had no material on submesh {1} cannot map to a material in the atlas", (m).name, submeshIdx);
                rectInAtlas = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            if (submeshIdx >= m.subMeshCount)
            {
                errorMsg = "Submesh index is greater than the number of submeshes";

                rectInAtlas = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            int index1 = -1;
            for (int index2 = 0; index2 < matsAndSrcUVRect.Length; ++index2)
            {
                if ((mat== matsAndSrcUVRect[index2].material))
                {
                    index1 = index2;
                    break;
                }
            }
            if (index1 == -1)
            {
               
                errorMsg = string.Format("Material {0} could not be found in the Texture Bake Result", (mat).name);
                rectInAtlas = new Rect();
                encapsulatingRect = new Rect();
                sourceMaterialTilingOut = new Rect();
                return false;
            }
            if (!tbr.resultMaterials[idxInResultMats].considerMeshUVs)
            {
                if (numTimesMatAppearsInAtlas[index1] != 1)
                    Debug.LogError("There is a problem with this TextureBakeResults. FixOutOfBoundsUVs is false and a material appears more than once.");
                rectInAtlas = matsAndSrcUVRect[index1].atlasRect;
                encapsulatingRect = matsAndSrcUVRect[index1].samplingEncapsulatinRect;
                sourceMaterialTilingOut = matsAndSrcUVRect[index1].sourceMaterialTiling;
                return true;
            }
            MB_Utility.MeshAnalysisResult[] meshAnalysisResultArray;
            if (!meshAnalysisCache.TryGetValue((m).GetInstanceID(), out meshAnalysisResultArray))
            {
                meshAnalysisResultArray = new MB_Utility.MeshAnalysisResult[m.subMeshCount];
                for (int submeshIndex = 0; submeshIndex < m.subMeshCount; ++submeshIndex)
                    MB_Utility.hasOutOfBoundsUVs(meshChannelCache.GetUv0Raw(m), m, ref meshAnalysisResultArray[submeshIndex], submeshIndex);
                meshAnalysisCache.Add((m).GetInstanceID(), meshAnalysisResultArray);
            }
            bool flag = false;
            if (logLevel >= MB2_LogLevel.trace)
                Debug.Log(string.Format("Trying to find a rectangle in atlas capable of holding tiled sampling rect for mesh {0} using material {1}", m, mat));
            for (int index2 = index1; index2 < matsAndSrcUVRect.Length; ++index2)
            {
                if ((matsAndSrcUVRect[index2].material== mat) && IsMeshAndMaterialRectEnclosedByAtlasRect(meshAnalysisResultArray[submeshIdx].uvRect, matsAndSrcUVRect[index2].sourceMaterialTiling, matsAndSrcUVRect[index2].samplingEncapsulatinRect, logLevel))
                {
                    if (logLevel >= MB2_LogLevel.trace)
                        Debug.Log(("Found rect in atlas capable of containing tiled sampling rect for mesh " + m + " at idx=" + index2));
                    index1 = index2;
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                rectInAtlas = matsAndSrcUVRect[index1].atlasRect;
                encapsulatingRect = matsAndSrcUVRect[index1].samplingEncapsulatinRect;
                sourceMaterialTilingOut = matsAndSrcUVRect[index1].sourceMaterialTiling;
                return true;
            }
      
            errorMsg = string.Format("Could not find a tiled rectangle in the atlas capable of containing the uv and material tiling on mesh {0} for material {1}", (m).name, mat);
            rectInAtlas = new Rect();
            encapsulatingRect = new Rect();
            sourceMaterialTilingOut = new Rect();
            return false;
        }
    }
}

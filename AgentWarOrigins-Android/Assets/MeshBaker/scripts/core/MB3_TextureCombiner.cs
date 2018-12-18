// Decompiled with JetBrains decompiler
// Type: DigitalOpus.MB.Core.MB3_TextureCombiner
// Assembly: MeshBakerCore, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D590286C-1214-465B-A384-78BAAD755E88
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\MeshBakerCore.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public class MB3_TextureCombiner
    {
        public static ShaderTextureProperty[] shaderTexPropertyNames = new ShaderTextureProperty[19]
        {
      new ShaderTextureProperty("_MainTex", false),
      new ShaderTextureProperty("_BumpMap", true),
      new ShaderTextureProperty("_Normal", true),
      new ShaderTextureProperty("_BumpSpecMap", false),
      new ShaderTextureProperty("_DecalTex", false),
      new ShaderTextureProperty("_Detail", false),
      new ShaderTextureProperty("_GlossMap", false),
      new ShaderTextureProperty("_Illum", false),
      new ShaderTextureProperty("_LightTextureB0", false),
      new ShaderTextureProperty("_ParallaxMap", false),
      new ShaderTextureProperty("_ShadowOffset", false),
      new ShaderTextureProperty("_TranslucencyMap", false),
      new ShaderTextureProperty("_SpecMap", false),
      new ShaderTextureProperty("_SpecGlossMap", false),
      new ShaderTextureProperty("_TranspMap", false),
      new ShaderTextureProperty("_MetallicGlossMap", false),
      new ShaderTextureProperty("_OcclusionMap", false),
      new ShaderTextureProperty("_EmissionMap", false),
      new ShaderTextureProperty("_DetailMask", false)
        };
        public static bool _RunCorutineWithoutPauseIsRunning = false;
        private static bool LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS = true;
        public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;
        [SerializeField]
        protected int _atlasPadding = 1;
        [SerializeField]
        protected int _maxAtlasSize = 1;
        [SerializeField]
        protected bool _resizePowerOfTwoTextures = false;
        [SerializeField]
        protected bool _fixOutOfBoundsUVs = false;
        [SerializeField]
        protected int _maxTilingBakeSize = 1024;
        [SerializeField]
        protected bool _saveAtlasesAsAssets = false;
        [SerializeField]
        protected MB2_PackingAlgorithmEnum _packingAlgorithm = MB2_PackingAlgorithmEnum.UnitysPackTextures;
        [SerializeField]
        protected bool _meshBakerTexturePackerForcePowerOfTwo = true;
        [SerializeField]
        protected List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();
        [SerializeField]
        protected bool _normalizeTexelDensity = false;
        [SerializeField]
        protected bool _considerNonTextureProperties = false;
        protected TextureBlender[] textureBlenders = new TextureBlender[0];
        protected List<Texture2D> _temporaryTextures = new List<Texture2D>();
        [SerializeField]
        protected MB2_TextureBakeResults _textureBakeResults;
        protected TextureBlender resultMaterialTextureBlender;
        private int __step2_CalculateIdealSizesForTexturesInAtlasAndPadding;
        private Rect[] __createAtlasesMBTexturePacker;

        public MB2_TextureBakeResults textureBakeResults
        {
            get
            {
                return _textureBakeResults;
            }
            set
            {
                _textureBakeResults = value;
            }
        }

        public int atlasPadding
        {
            get
            {
                return _atlasPadding;
            }
            set
            {
                _atlasPadding = value;
            }
        }

        public int maxAtlasSize
        {
            get
            {
                return _maxAtlasSize;
            }
            set
            {
                _maxAtlasSize = value;
            }
        }

        public bool resizePowerOfTwoTextures
        {
            get
            {
                return _resizePowerOfTwoTextures;
            }
            set
            {
                _resizePowerOfTwoTextures = value;
            }
        }

        public bool fixOutOfBoundsUVs
        {
            get
            {
                return _fixOutOfBoundsUVs;
            }
            set
            {
                _fixOutOfBoundsUVs = value;
            }
        }

        public int maxTilingBakeSize
        {
            get
            {
                return _maxTilingBakeSize;
            }
            set
            {
                _maxTilingBakeSize = value;
            }
        }

        public bool saveAtlasesAsAssets
        {
            get
            {
                return _saveAtlasesAsAssets;
            }
            set
            {
                _saveAtlasesAsAssets = value;
            }
        }

        public MB2_PackingAlgorithmEnum packingAlgorithm
        {
            get
            {
                return _packingAlgorithm;
            }
            set
            {
                _packingAlgorithm = value;
            }
        }

        public bool meshBakerTexturePackerForcePowerOfTwo
        {
            get
            {
                return _meshBakerTexturePackerForcePowerOfTwo;
            }
            set
            {
                _meshBakerTexturePackerForcePowerOfTwo = value;
            }
        }

        public List<ShaderTextureProperty> customShaderPropNames
        {
            get
            {
                return _customShaderPropNames;
            }
            set
            {
                _customShaderPropNames = value;
            }
        }

        public bool considerNonTextureProperties
        {
            get
            {
                return _considerNonTextureProperties;
            }
            set
            {
                _considerNonTextureProperties = value;
            }
        }

        public static void RunCorutineWithoutPause(IEnumerator cor, int recursionDepth)
        {
            if (recursionDepth == 0)
                _RunCorutineWithoutPauseIsRunning = true;
            if (recursionDepth > 20)
            {
                Debug.LogError("Recursion Depth Exceeded.");
            }
            else
            {
                while (cor.MoveNext())
                {
                    object current = cor.Current;
                    if (!(current is YieldInstruction) && current != null && current is IEnumerator)
                        RunCorutineWithoutPause((IEnumerator)cor.Current, recursionDepth + 1);
                }
                if (recursionDepth != 0)
                    return;
                _RunCorutineWithoutPauseIsRunning = false;
            }
        }

        public bool CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods = null, List<AtlasPackingResult> packingResults = null, bool onlyPackRects = false)
        {
            CombineTexturesIntoAtlasesCoroutineResult result = new CombineTexturesIntoAtlasesCoroutineResult();
            RunCorutineWithoutPause(_CombineTexturesIntoAtlases(progressInfo, result, resultAtlasesAndRects, resultMaterial, objsToMesh, allowedMaterialsFilter, textureEditorMethods, packingResults, onlyPackRects), 0);
            return result.success;
        }

        public IEnumerator CombineTexturesIntoAtlasesCoroutine(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods = null, CombineTexturesIntoAtlasesCoroutineResult coroutineResult = null, float maxTimePerFrame = 0.01f, List<AtlasPackingResult> packingResults = null, bool onlyPackRects = false)
        {
            if (!_RunCorutineWithoutPauseIsRunning && (MBVersion.GetMajorVersion() < 5 || MBVersion.GetMajorVersion() == 5 && MBVersion.GetMinorVersion() < 3))
            {
                Debug.LogError("Running the texture combiner as a coroutine only works in Unity 5.3 and higher");
                yield return null;
            }
            coroutineResult.success = true;
            coroutineResult.isFinished = false;
            if ((double)maxTimePerFrame <= 0.0)
            {
                Debug.LogError("maxTimePerFrame must be a value greater than zero");
                coroutineResult.isFinished = true;
            }
            else
            {
                yield return _CombineTexturesIntoAtlases(progressInfo, coroutineResult, resultAtlasesAndRects, resultMaterial, objsToMesh, allowedMaterialsFilter, textureEditorMethods, packingResults, onlyPackRects);
                coroutineResult.isFinished = true;
            }
        }

        private static bool InterfaceFilter(Type typeObj, object criteriaObj)
        {
            return typeObj.ToString() == criteriaObj.ToString();
        }

        private void _LoadTextureBlenders()
        {
            string str = "DigitalOpus.MB.Core.TextureBlender";
            TypeFilter filter = new TypeFilter(InterfaceFilter);
            List<Type> typeList = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                IEnumerable enumerable = (IEnumerable)null;
                try
                {
                    enumerable = (IEnumerable)assembly.GetTypes();
                }
                catch (Exception ex)
                {
                    ex.Equals(null);
                }
                if (enumerable != null)
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if ((uint)type.FindInterfaces(filter, str).Length > 0U)
                            typeList.Add(type);
                    }
                }
            }
            TextureBlender textureBlender = (TextureBlender)null;
            List<TextureBlender> textureBlenderList = new List<TextureBlender>();
            foreach (Type type in typeList)
            {
                if (!type.IsAbstract && !type.IsInterface)
                {
                    TextureBlender instance = (TextureBlender)Activator.CreateInstance(type);
                    if (instance is TextureBlenderFallback)
                        textureBlender = instance;
                    else
                        textureBlenderList.Add(instance);
                }
            }
            if (textureBlender != null)
                textureBlenderList.Add(textureBlender);
            textureBlenders = textureBlenderList.ToArray();
            if (LOG_LEVEL < MB2_LogLevel.debug)
                return;
            Debug.Log(string.Format("Loaded {0} TextureBlenders.", textureBlenders.Length));
        }

        private bool _CollectPropertyNames(Material resultMaterial, List<ShaderTextureProperty> texPropertyNames)
        {
            for (int i = 0; i < texPropertyNames.Count; i++)
            {
                ShaderTextureProperty shaderTextureProperty = _customShaderPropNames.Find((Predicate<ShaderTextureProperty>)(x => x.name.Equals(texPropertyNames[i].name)));
                if (shaderTextureProperty != null)
                    _customShaderPropNames.Remove(shaderTextureProperty);
            }
            Material material = resultMaterial;
            if (material== null)
            {
                Debug.LogError("Please assign a result material. The combined mesh will use this material.");
                return false;
            }
            string str = "";
            for (int index = 0; index < shaderTexPropertyNames.Length; ++index)
            {
                if (material.HasProperty(shaderTexPropertyNames[index].name))
                {
                    str = str + ", " + shaderTexPropertyNames[index].name;
                    if (!texPropertyNames.Contains(shaderTexPropertyNames[index]))
                        texPropertyNames.Add(shaderTexPropertyNames[index]);
                    if ((material.GetTextureOffset(shaderTexPropertyNames[index].name)!= new Vector2(0.0f, 0.0f)) && LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning("Result material has non-zero offset. This is may be incorrect.");
                    if ((material.GetTextureScale(shaderTexPropertyNames[index].name)!= new Vector2(1f, 1f)) && LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning("Result material should have tiling of 1,1");
                }
            }
            for (int index = 0; index < _customShaderPropNames.Count; ++index)
            {
                if (material.HasProperty(_customShaderPropNames[index].name))
                {
                    str = str + ", " + _customShaderPropNames[index].name;
                    texPropertyNames.Add(_customShaderPropNames[index]);
                    if ((material.GetTextureOffset(_customShaderPropNames[index].name)!= new Vector2(0.0f, 0.0f)) && LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");
                    if ((material.GetTextureScale(_customShaderPropNames[index].name)!= new Vector2(1f, 1f)) && LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning("Result material should probably have tiling of 1,1.");
                }
                else if (LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning(("Result material shader does not use property " + _customShaderPropNames[index].name + " in the list of custom shader property names"));
            }
            return true;
        }

        private IEnumerator _CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, CombineTexturesIntoAtlasesCoroutineResult result, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods, List<AtlasPackingResult> atlasPackingResult, bool onlyPackRects)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool flag;
            try
            {
                _temporaryTextures.Clear();
                if (textureEditorMethods != null)
                {
                    textureEditorMethods.Clear();
                    textureEditorMethods.OnPreTextureBake();
                }
                if (objsToMesh == null || objsToMesh.Count == 0)
                {
                    Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
                    result.success = false;
                    flag = false;
                }
                else if (_atlasPadding < 0)
                {
                    Debug.LogError("Atlas padding must be zero or greater.");
                    result.success = false;
                    flag = false;
                }
                else if (_maxTilingBakeSize < 2 || _maxTilingBakeSize > 4096)
                {
                    Debug.LogError("Invalid value for max tiling bake size.");
                    result.success = false;
                    flag = false;
                }
                else
                {
                    for (int i = 0; i < objsToMesh.Count; ++i)
                    {
                        Material[] ms = MB_Utility.GetGOMaterials(objsToMesh[i]);
                        for (int j = 0; j < ms.Length; ++j)
                        {
                            Material m = ms[j];
                            if ((m== null))
                            {
                                Debug.LogError(("Game object " + objsToMesh[i] + " has a null material"));
                                result.success = false;
                                flag = false;
                                goto label_35;
                            }
                            else
                                m = (Material)null;
                        }
                        ms = (Material[])null;
                    }
                    if (progressInfo != null)
                        progressInfo("Collecting textures for " + objsToMesh.Count + " meshes.", 0.01f);
                    List<ShaderTextureProperty> texPropertyNames = new List<ShaderTextureProperty>();
                    if (!_CollectPropertyNames(resultMaterial, texPropertyNames))
                    {
                        result.success = false;
                        flag = false;
                    }
                    else
                    {
                        if (_considerNonTextureProperties)
                        {
                            _LoadTextureBlenders();
                            resultMaterialTextureBlender = FindMatchingTextureBlender((resultMaterial.shader).name);
                            if (resultMaterialTextureBlender != null)
                            {
                                if (LOG_LEVEL >= MB2_LogLevel.debug)
                                    Debug.Log(("Using _considerNonTextureProperties found a TextureBlender for result material. Using: " + resultMaterialTextureBlender));
                            }
                            else
                            {
                                if (LOG_LEVEL >= MB2_LogLevel.error)
                                    Debug.LogWarning("Using _considerNonTextureProperties could not find a TextureBlender that matches the shader on the result material. Using the Fallback Texture Blender.");
                                resultMaterialTextureBlender = (TextureBlender)new TextureBlenderFallback();
                            }
                        }
                        if (onlyPackRects)
                            yield return __RunTexturePacker(result, texPropertyNames, objsToMesh, allowedMaterialsFilter, textureEditorMethods, atlasPackingResult);
                        else
                            yield return __CombineTexturesIntoAtlases(progressInfo, result, resultAtlasesAndRects, resultMaterial, texPropertyNames, objsToMesh, allowedMaterialsFilter, textureEditorMethods);
                        texPropertyNames = (List<ShaderTextureProperty>)null;
                        yield break;
                    }
                }
                label_35:;
            }
            finally
            {
                _destroyTemporaryTextures();
                if (textureEditorMethods != null)
                {
                    textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);
                    textureEditorMethods.OnPostTextureBake();
                }
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log(("===== Done creating atlases for " + resultMaterial + " Total time to create atlases " + sw.Elapsed.ToString()));
            }
            yield return flag;
        }

        private IEnumerator __CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, CombineTexturesIntoAtlasesCoroutineResult result, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<ShaderTextureProperty> texPropertyNames, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods)
        {
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("__CombineTexturesIntoAtlases texture properties in shader:" + texPropertyNames.Count + " objsToMesh:" + objsToMesh.Count + " _fixOutOfBoundsUVs:" + _fixOutOfBoundsUVs.ToString()));
            if (progressInfo != null)
                progressInfo("Collecting textures ", 0.01f);
            List<MB_TexSet> distinctMaterialTextures = new List<MB_TexSet>();
            List<GameObject> usedObjsToMesh = new List<GameObject>();
            yield return __Step1_CollectDistinctMatTexturesAndUsedObjects(progressInfo, result, objsToMesh, allowedMaterialsFilter, texPropertyNames, textureEditorMethods, distinctMaterialTextures, usedObjsToMesh);
            if (result.success)
            {
                if (MB3_MeshCombiner.EVAL_VERSION)
                {
                    bool usesAllowedShaders = true;
                    for (int i = 0; i < distinctMaterialTextures.Count; ++i)
                    {
                        for (int j = 0; j < distinctMaterialTextures[i].matsAndGOs.mats.Count; ++j)
                        {
                            if (!(distinctMaterialTextures[i].matsAndGOs.mats[j].mat.shader.name.EndsWith("Diffuse") && !(distinctMaterialTextures[i].matsAndGOs.mats[j].mat.shader).name.EndsWith("Bumped Diffuse")))
                            {
                                Debug.LogError(("The free version of Mesh Baker only works with Diffuse and Bumped Diffuse Shaders. The full version can be used with any shader. Material " + (distinctMaterialTextures[i].matsAndGOs.mats[j].mat).name + " uses shader " + (distinctMaterialTextures[i].matsAndGOs.mats[j].mat.shader).name));
                                usesAllowedShaders = false;
                            }
                        }
                    }
                    if (!usesAllowedShaders)
                    {
                        result.success = false;
                        yield break;
                    }
                }
                bool[] allTexturesAreNullAndSameColor = new bool[texPropertyNames.Count];
                yield return __Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(progressInfo, result, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, textureEditorMethods);
                if (result.success)
                {
                    int _padding = __step2_CalculateIdealSizesForTexturesInAtlasAndPadding;
                    yield return __Step3_BuildAndSaveAtlasesAndStoreResults(result, progressInfo, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, _padding, textureEditorMethods, resultAtlasesAndRects, resultMaterial);
                }
            }
        }

        private IEnumerator __RunTexturePacker(CombineTexturesIntoAtlasesCoroutineResult result, List<ShaderTextureProperty> texPropertyNames, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods, List<AtlasPackingResult> packingResult)
        {
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("__RunTexturePacker texture properties in shader:" + texPropertyNames.Count + " objsToMesh:" + objsToMesh.Count + " _fixOutOfBoundsUVs:" + _fixOutOfBoundsUVs.ToString()));
            List<MB_TexSet> distinctMaterialTextures = new List<MB_TexSet>();
            List<GameObject> usedObjsToMesh = new List<GameObject>();
            yield return __Step1_CollectDistinctMatTexturesAndUsedObjects((ProgressUpdateDelegate)null, result, objsToMesh, allowedMaterialsFilter, texPropertyNames, textureEditorMethods, distinctMaterialTextures, usedObjsToMesh);
            if (result.success)
            {
                bool[] allTexturesAreNullAndSameColor = new bool[texPropertyNames.Count];
                yield return __Step2_CalculateIdealSizesForTexturesInAtlasAndPadding((ProgressUpdateDelegate)null, result, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, textureEditorMethods);
                if (result.success)
                {
                    int _padding = __step2_CalculateIdealSizesForTexturesInAtlasAndPadding;
                    foreach (AtlasPackingResult atlasPackingResult in __Step3_RunTexturePacker(distinctMaterialTextures, _padding))
                        packingResult.Add(atlasPackingResult);
                }
            }
        }

        private IEnumerator __Step1_CollectDistinctMatTexturesAndUsedObjects(ProgressUpdateDelegate progressInfo, CombineTexturesIntoAtlasesCoroutineResult result, List<GameObject> allObjsToMesh, List<Material> allowedMaterialsFilter, List<ShaderTextureProperty> texPropertyNames, MB2_EditorMethodsInterface textureEditorMethods, List<MB_TexSet> distinctMaterialTextures, List<GameObject> usedObjsToMesh)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool outOfBoundsUVs = false;
            Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResultsCache = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
            for (int i = 0; i < allObjsToMesh.Count; ++i)
            {
                GameObject obj = allObjsToMesh[i];
                if (progressInfo != null)
                    progressInfo("Collecting textures for " + obj, (float)((double)i / (double)allObjsToMesh.Count / 2.0));
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log(("Collecting textures for object " + obj));
                if (obj== null)
                {
                    Debug.LogError("The list of objects to mesh contained nulls.");
                    result.success = false;
                    yield break;
                }
                else
                {
                    Mesh sharedMesh = MB_Utility.GetMesh(obj);
                    if (sharedMesh== null)
                    {
                        Debug.LogError(("Object " + (obj).name + " in the list of objects to mesh has no mesh."));
                        result.success = false;
                        yield break;
                    }
                    else
                    {
                        Material[] sharedMaterials = MB_Utility.GetGOMaterials(obj);
                        if (sharedMaterials.Length == 0)
                        {
                            Debug.LogError(("Object " + (obj).name + " in the list of objects has no materials."));
                            result.success = false;
                            yield break;
                        }
                        else
                        {
                            MB_Utility.MeshAnalysisResult[] mar;
                            if (!meshAnalysisResultsCache.TryGetValue((sharedMesh).GetInstanceID(), out mar))
                            {
                                mar = new MB_Utility.MeshAnalysisResult[sharedMesh.subMeshCount];
                                for (int j = 0; j < sharedMesh.subMeshCount; ++j)
                                {
                                    MB_Utility.hasOutOfBoundsUVs(sharedMesh, ref mar[j], j, 0);
                                    if (_normalizeTexelDensity)
                                        mar[j].submeshArea = GetSubmeshArea(sharedMesh, j);
                                    if (_fixOutOfBoundsUVs && !mar[j].hasUVs)
                                    {
                                        mar[j].uvRect = new Rect(0.0f, 0.0f, 1f, 1f);
                                        Debug.LogWarning(("Mesh for object " + obj + " has no UV channel but 'consider UVs' is enabled. Assuming UVs will be generated filling 0,0,1,1 rectangle."));
                                    }
                                }
                                meshAnalysisResultsCache.Add((sharedMesh).GetInstanceID(), mar);
                            }
                            if (_fixOutOfBoundsUVs && LOG_LEVEL >= MB2_LogLevel.trace)
                                Debug.Log(("Mesh Analysis for object " + obj + " numSubmesh=" + mar.Length + " HasOBUV=" + mar[0].hasOutOfBoundsUVs.ToString() + " UVrectSubmesh0=" + mar[0].uvRect));
                            for (int matIdx = 0; matIdx < sharedMaterials.Length; ++matIdx)
                            {
                                if (progressInfo != null)
                                    progressInfo(string.Format("Collecting textures for {0} submesh {1}", obj, matIdx), (float)((double)i / (double)allObjsToMesh.Count / 2.0));
                                Material mat = sharedMaterials[matIdx];
                                if (allowedMaterialsFilter == null || allowedMaterialsFilter.Contains(mat))
                                {
                                    outOfBoundsUVs = outOfBoundsUVs || mar[matIdx].hasOutOfBoundsUVs;
                                    if ((mat).name.Contains("(Instance)"))
                                    {
                                        Debug.LogError(("The sharedMaterial on object " + (obj).name + " has been 'Instanced'. This was probably caused by a script accessing the meshRender.material property in the editor.  The material to UV Rectangle mapping will be incorrect. To fix this recreate the object from its prefab or re-assign its material from the correct asset."));
                                        result.success = false;
                                        yield break;
                                    }
                                    else
                                    {
                                        if (_fixOutOfBoundsUVs && !MB_Utility.AreAllSharedMaterialsDistinct(sharedMaterials) && LOG_LEVEL >= MB2_LogLevel.warn)
                                            Debug.LogWarning(("Object " + (obj).name + " uses the same material on multiple submeshes. This may generate strange resultAtlasesAndRects especially when used with fix out of bounds uvs. Try duplicating the material."));
                                        MeshBakerMaterialTexture[] mts = new MeshBakerMaterialTexture[texPropertyNames.Count];
                                        for (int j = 0; j < texPropertyNames.Count; j++)
                                        {
                                            Texture2D tx = null;
                                            Vector2 scale = Vector2.one;
                                            Vector2 offset = Vector2.zero;
                                            float texelDensity = 0f;
                                            if (mat.HasProperty(texPropertyNames[j].name))
                                            {
                                                Texture txx = mat.GetTexture(texPropertyNames[j].name);
                                                if (txx != null)
                                                {
                                                    if (txx is Texture2D)
                                                    {
                                                        tx = (Texture2D)txx;
                                                        TextureFormat f = tx.format;
                                                        bool isNormalMap = false;
                                                        if (!Application.isPlaying && textureEditorMethods != null) isNormalMap = textureEditorMethods.IsNormalMap(tx);
                                                        if ((f == TextureFormat.ARGB32 ||
                                                            f == TextureFormat.RGBA32 ||
                                                            f == TextureFormat.BGRA32 ||
                                                            f == TextureFormat.RGB24 ||
                                                            f == TextureFormat.Alpha8) && !isNormalMap) //DXT5 does not work
                                                        {
                                                            //good
                                                        }
                                                        else
                                                        {
                                                            //TRIED to copy texture using tex2.SetPixels(tex1.GetPixels()) but bug in 3.5 means DTX1 and 5 compressed textures come out skewed
                                                            //MB2_Log.Log(MB2_LogLevel.warn,obj.name + " in the list of objects to mesh uses Texture "+tx.name+" uses format " + f + " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These formats cannot be resized. MeshBaker will create duplicates.");
                                                            //tx = createTextureCopy(tx);
                                                            if (Application.isPlaying && _packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
                                                            {
                                                                Debug.LogError("Object " + obj.name + " in the list of objects to mesh uses Texture " + tx.name + " uses format " + f + " that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized at runtime. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'");
                                                                result.success = false;
                                                                yield break;
                                                            }
                                                            else
                                                            {
                                                                tx = (Texture2D)mat.GetTexture(texPropertyNames[j].name);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Debug.LogError("Object " + obj.name + " in the list of objects to mesh uses a Texture that is not a Texture2D. Cannot build atlases.");
                                                        result.success = false;
                                                        yield break;
                                                    }

                                                }

                                                if (tx != null && _normalizeTexelDensity)
                                                {
                                                    //todo this doesn't take into account tiling and out of bounds UV sampling
                                                    if (mar[j].submeshArea == 0)
                                                    {
                                                        texelDensity = 0f;
                                                    }
                                                    else
                                                    {
                                                        texelDensity = (tx.width * tx.height) / (mar[j].submeshArea);
                                                    }
                                                }
                                                scale = mat.GetTextureScale(texPropertyNames[j].name);
                                                offset = mat.GetTextureOffset(texPropertyNames[j].name);
                                            }
                                            mts[j] = new MeshBakerMaterialTexture(tx, offset, scale, texelDensity);
                                        }
                              
                                        Vector2 obUVscale = new Vector2((mar[matIdx].uvRect).width, (mar[matIdx].uvRect).height);
                         
                                        Vector2 obUVoffset = new Vector2((mar[matIdx].uvRect).x, ((mar[matIdx].uvRect).y));
                                        MB_TexSet setOfTexs = new MB_TexSet(mts, obUVoffset, obUVscale);
                                        MatAndTransformToMerged matt = new MatAndTransformToMerged(mat);
                                        setOfTexs.matsAndGOs.mats.Add(matt);
                                        MB_TexSet setOfTexs2 = distinctMaterialTextures.Find((Predicate<MB_TexSet>)(x => x.IsEqual(setOfTexs, _fixOutOfBoundsUVs, _considerNonTextureProperties, resultMaterialTextureBlender)));
                                        if (setOfTexs2 != null)
                                            setOfTexs = setOfTexs2;
                                        else
                                            distinctMaterialTextures.Add(setOfTexs);
                                        if (!setOfTexs.matsAndGOs.mats.Contains(matt))
                                            setOfTexs.matsAndGOs.mats.Add(matt);
                                        if (!setOfTexs.matsAndGOs.gos.Contains(obj))
                                        {
                                            setOfTexs.matsAndGOs.gos.Add(obj);
                                            if (!usedObjsToMesh.Contains(obj))
                                                usedObjsToMesh.Add(obj);
                                        }
                                        mat = null;
                                        mts = null;
                                        obUVscale = Vector2.zero;
                                        obUVoffset = Vector2.zero;
                                        matt = null;
                                        setOfTexs2 = null;
                                    }
                                }
                            }
                            obj = null;
                            sharedMesh = null;
                            sharedMaterials = null;
                            mar = null;
                        }
                    }
                }
            }
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(string.Format("Step1_CollectDistinctTextures collected {0} sets of textures fixOutOfBoundsUV={1} considerNonTextureProperties={2}", distinctMaterialTextures.Count, _fixOutOfBoundsUVs, _considerNonTextureProperties));
            if (distinctMaterialTextures.Count == 0)
            {
                Debug.LogError("None of the source object materials matched any of the allowed materials for this submesh.");
                result.success = false;
            }
            else
            {
                MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(distinctMaterialTextures, fixOutOfBoundsUVs);
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log(("Total time Step1_CollectDistinctTextures " + sw.ElapsedMilliseconds.ToString("f5")));
            }
        }

        private IEnumerator __Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(ProgressUpdateDelegate progressInfo, CombineTexturesIntoAtlasesCoroutineResult result, List<MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, MB2_EditorMethodsInterface textureEditorMethods)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < texPropertyNames.Count; ++i)
            {
                bool allTexturesAreNull = true;
                bool allNonTexturePropertiesAreSame = true;
                for (int j = 0; j < distinctMaterialTextures.Count; ++j)
                {
                    if (distinctMaterialTextures[j].ts[i].t!= null)
                    {
                        allTexturesAreNull = false;
                        break;
                    }
                    if (_considerNonTextureProperties)
                    {
                        for (int k = j + 1; k < distinctMaterialTextures.Count; ++k)
                        {
                            Color? colJ = resultMaterialTextureBlender.GetColorIfNoTexture(distinctMaterialTextures[j].matsAndGOs.mats[0].mat, texPropertyNames[i]);
                            Color colK = resultMaterialTextureBlender.GetColorIfNoTexture(distinctMaterialTextures[k].matsAndGOs.mats[0].mat, texPropertyNames[i]);
                            if ((colJ!= colK))
                            {
                                allNonTexturePropertiesAreSame = false;
                                break;
                            }
                            colJ = null;
                        }
                    }
                }
                allTexturesAreNullAndSameColor[i] = allTexturesAreNull & allNonTexturePropertiesAreSame;
                if (LOG_LEVEL >= MB2_LogLevel.trace)
                    Debug.Log(string.Format("AllTexturesAreNullAndSameColor prop: {0} val:{1}", texPropertyNames[i].name, allTexturesAreNullAndSameColor[i]));
            }
            int _padding = _atlasPadding;
            if (distinctMaterialTextures.Count == 1 && !_fixOutOfBoundsUVs)
            {
                if (LOG_LEVEL >= MB2_LogLevel.info)
                    Debug.Log("All objects use the same textures in this set of atlases. Original textures will be reused instead of creating atlases.");
                _padding = 0;
            }
            else
            {
                if (allTexturesAreNullAndSameColor.Length != texPropertyNames.Count)
                    Debug.LogError("allTexturesAreNullAndSameColor array must be the same length of texPropertyNames.");
                for (int i = 0; i < distinctMaterialTextures.Count; ++i)
                {
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        Debug.Log(("Calculating ideal sizes for texSet TexSet " + i + " of " + distinctMaterialTextures.Count));
                    MB_TexSet txs = distinctMaterialTextures[i];
                    txs.idealWidth = 1;
                    txs.idealHeight = 1;
                    int tWidth = 1;
                    int tHeight = 1;
                    if (txs.ts.Length != texPropertyNames.Count)
                        Debug.LogError("length of arrays in each element of distinctMaterialTextures must be texPropertyNames.Count");
                    for (int propIdx = 0; propIdx < texPropertyNames.Count; ++propIdx)
                    {
                        MeshBakerMaterialTexture matTex = txs.ts[propIdx];
                        if (!matTex.matTilingRect.size.Equals(Vector2.one) && distinctMaterialTextures.Count > 1 && LOG_LEVEL >= MB2_LogLevel.warn)
                            Debug.LogWarning(("Texture " + matTex.t + "is tiled by " + matTex.matTilingRect.size + " tiling will be baked into a texture with maxSize:" + _maxTilingBakeSize));
                        if (!txs.obUVscale.Equals(Vector2.one) && distinctMaterialTextures.Count > 1 && _fixOutOfBoundsUVs && LOG_LEVEL >= MB2_LogLevel.warn)
                            Debug.LogWarning(("Texture " + matTex.t + "has out of bounds UVs that effectively tile by " + txs.obUVscale + " tiling will be baked into a texture with maxSize:" + _maxTilingBakeSize));
                        if (!allTexturesAreNullAndSameColor[propIdx] && (matTex.t== null))
                        {
                            if (LOG_LEVEL >= MB2_LogLevel.trace)
                                Debug.Log("No source texture creating a 16x16 texture.");
                            matTex.t = _createTemporaryTexture(16, 16, (TextureFormat)5, true);
                            if (_considerNonTextureProperties && resultMaterialTextureBlender != null)
                            {
                                Color col = resultMaterialTextureBlender.GetColorIfNoTexture(txs.matsAndGOs.mats[0].mat, texPropertyNames[propIdx]);
                                if (LOG_LEVEL >= MB2_LogLevel.trace)
                                    Debug.Log(("Setting texture to solid color " + col));
                                MB_Utility.setSolidColor(matTex.t, col);
                            }
                            else
                            {
                                Color col = GetColorIfNoTexture(texPropertyNames[propIdx]);
                                MB_Utility.setSolidColor(matTex.t, col);
                            }
                            matTex.encapsulatingSamplingRect = !fixOutOfBoundsUVs ? new DRect(0.0f, 0.0f, 1f, 1f) : txs.obUVrect;
                        }
                        if ((matTex.t!= null))
                        {
                            Vector2 dim = GetAdjustedForScaleAndOffset2Dimensions(matTex, txs.obUVoffset, txs.obUVscale);
                            if ((int)(dim.x * dim.y) > tWidth * tHeight)
                            {
                                if (LOG_LEVEL >= MB2_LogLevel.trace)
                                    Debug.Log(("    matTex " + matTex.t + " " + dim + " has a bigger size than " + tWidth + " " + tHeight));
                                tWidth = (int)dim.x;
                                tHeight = (int)dim.y;
                            }
                        }
                    }
                    if (_resizePowerOfTwoTextures)
                    {
                        if (tWidth <= _padding * 5)
                            Debug.LogWarning(string.Format("Some of the textures have widths close to the size of the padding. It is not recommended to use _resizePowerOfTwoTextures with widths this small.", txs.ToString()));
                        if (tHeight <= _padding * 5)
                            Debug.LogWarning(string.Format("Some of the textures have heights close to the size of the padding. It is not recommended to use _resizePowerOfTwoTextures with heights this small.", txs.ToString()));
                        if (IsPowerOfTwo(tWidth))
                            tWidth -= _padding * 2;
                        if (IsPowerOfTwo(tHeight))
                            tHeight -= _padding * 2;
                        if (tWidth < 1)
                            tWidth = 1;
                        if (tHeight < 1)
                            tHeight = 1;
                    }
                    if (LOG_LEVEL >= MB2_LogLevel.trace)
                        Debug.Log(("    Ideal size is " + tWidth + " " + tHeight));
                    txs.idealWidth = tWidth;
                    txs.idealHeight = tHeight;
                    txs = (MB_TexSet)null;
                }
            }
            TimeSpan elapsed;
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                string str1 = "Total time Step2 Calculate Ideal Sizes part1: ";
                elapsed = sw.Elapsed;
                string str2 = elapsed.ToString();
                Debug.Log((str1 + str2));
            }
            if (distinctMaterialTextures.Count > 1 && _packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
            {
                for (int i = 0; i < distinctMaterialTextures.Count; ++i)
                {
                    for (int j = 0; j < texPropertyNames.Count; ++j)
                    {
                        Texture2D tx = distinctMaterialTextures[i].ts[j].t;
                        if ((tx!= null) && textureEditorMethods != null)
                        {
                            if (progressInfo != null)
                                progressInfo(string.Format("Convert texture {0} to readable format ", tx), 0.5f);
                            textureEditorMethods.AddTextureFormat(tx, texPropertyNames[j].isNormalMap);
                        }
                        tx = (Texture2D)null;
                    }
                }
            }
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                string str1 = "Total time Step2 Calculate Ideal Sizes part2: ";
                elapsed = sw.Elapsed;
                string str2 = elapsed.ToString();
                Debug.Log((str1 + str2));
            }
            __step2_CalculateIdealSizesForTexturesInAtlasAndPadding = _padding;
            yield break;
        }

        private AtlasPackingResult[] __Step3_RunTexturePacker(List<MB_TexSet> distinctMaterialTextures, int _padding)
        {
            AtlasPackingResult[] atlasPackingResultArray = __RuntTexturePackerOnly(distinctMaterialTextures, _padding);
            for (int index1 = 0; index1 < atlasPackingResultArray.Length; ++index1)
            {
                List<MatsAndGOs> matsAndGosList = new List<MatsAndGOs>();
                atlasPackingResultArray[index1].data = matsAndGosList;
                for (int index2 = 0; index2 < atlasPackingResultArray[index1].srcImgIdxs.Length; ++index2)
                {
                    MB_TexSet distinctMaterialTexture = distinctMaterialTextures[atlasPackingResultArray[index1].srcImgIdxs[index2]];
                    matsAndGosList.Add(distinctMaterialTexture.matsAndGOs);
                }
            }
            return atlasPackingResultArray;
        }

        private IEnumerator __Step3_BuildAndSaveAtlasesAndStoreResults(CombineTexturesIntoAtlasesCoroutineResult result, ProgressUpdateDelegate progressInfo, List<MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, int _padding, MB2_EditorMethodsInterface textureEditorMethods, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int numAtlases = texPropertyNames.Count;
            StringBuilder report = new StringBuilder();
            if (numAtlases > 0)
            {
                report = new StringBuilder();
                report.AppendLine("Report");
                for (int i = 0; i < distinctMaterialTextures.Count; ++i)
                {
                    MB_TexSet txs = distinctMaterialTextures[i];
                    report.AppendLine("----------");
                    report.Append("This set of textures will be resized to:" + txs.idealWidth + "x" + txs.idealHeight + "\n");
                    for (int j = 0; j < txs.ts.Length; ++j)
                    {
                        if ((txs.ts[j].t!= null))
                        {
                            report.Append("   [" + texPropertyNames[j].name + " " + (txs.ts[j].t).name + " " + ((Texture)txs.ts[j].t).width + "x" + ((Texture)txs.ts[j].t).height + "]");
                            if ((txs.ts[j].matTilingRect.size!= Vector2.one) || (txs.ts[j].matTilingRect.min!= Vector2.zero))
                            {
                                StringBuilder stringBuilder = report;
                                string format = " material scale {0} offset{1} ";
                                Vector2 vector2 = txs.ts[j].matTilingRect.size;
                                // ISSUE: explicit reference operation
                                string str1 = ((Vector2)@vector2).ToString("G4");
                                vector2 = txs.ts[j].matTilingRect.min;
                                // ISSUE: explicit reference operation
                                string str2 = ((Vector2)@vector2).ToString("G4");
                                stringBuilder.AppendFormat(format, str1, str2);
                            }
                            if ((txs.obUVscale!= Vector2.one) || txs.obUVoffset!= Vector2.zero)
                            {
                                report.AppendFormat(" obUV scale {0} offset{1} ", (txs.obUVscale).ToString("G4"), (txs.obUVoffset).ToString("G4"));
                            }
                            report.AppendLine("");
                        }
                        else
                        {
                            report.Append("   [" + texPropertyNames[j].name + " null ");
                            if (allTexturesAreNullAndSameColor[j])
                                report.Append("no atlas will be created all textures null]\n");
                            else
                                report.AppendFormat("a 16x16 texture will be created]\n");
                        }
                    }
                    report.AppendLine("");
                    report.Append("Materials using:");
                    for (int j = 0; j < txs.matsAndGOs.mats.Count; ++j)
                        report.Append((txs.matsAndGOs.mats[j].mat).name + ", ");
                    report.AppendLine("");
                    txs = (MB_TexSet)null;
                }
            }
            GC.Collect();
            Texture2D[] atlases = new Texture2D[numAtlases];
            TimeSpan elapsed;
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                string str1 = "time Step 3 Create And Save Atlases part 1 ";
                elapsed = sw.Elapsed;
                string str2 = elapsed.ToString();
                Debug.Log((str1 + str2));
            }
            Rect[] rectsInAtlas;
            if (_packingAlgorithm == MB2_PackingAlgorithmEnum.UnitysPackTextures)
                rectsInAtlas = __CreateAtlasesUnityTexturePacker(progressInfo, numAtlases, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, resultMaterial, atlases, textureEditorMethods, _padding);
            else if (_packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker)
            {
                yield return __CreateAtlasesMBTexturePacker(progressInfo, numAtlases, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, resultMaterial, atlases, textureEditorMethods, _padding);
                rectsInAtlas = __createAtlasesMBTexturePacker;
            }
            else
                rectsInAtlas = __CreateAtlasesMBTexturePackerFast(progressInfo, numAtlases, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, resultMaterial, atlases, textureEditorMethods, _padding);
            float t3 = (float)sw.ElapsedMilliseconds;
            AdjustNonTextureProperties(resultMaterial, texPropertyNames, distinctMaterialTextures, _considerNonTextureProperties, textureEditorMethods);
            if (progressInfo != null)
                progressInfo("Building Report", 0.7f);
            StringBuilder atlasMessage = new StringBuilder();
            atlasMessage.AppendLine("---- Atlases ------");
            for (int i = 0; i < numAtlases; ++i)
            {
                if (atlases[i]!= null)
                    atlasMessage.AppendLine("Created Atlas For: " + texPropertyNames[i].name + " h=" + ((Texture)atlases[i]).height + " w=" + ((Texture)atlases[i]).width);
                else if (allTexturesAreNullAndSameColor[i])
                    atlasMessage.AppendLine("Did not create atlas for " + texPropertyNames[i].name + " because all source textures were null.");
            }
            report.Append(atlasMessage.ToString());
            List<MB_MaterialAndUVRect> mat2rect_map = new List<MB_MaterialAndUVRect>();
            for (int i = 0; i < distinctMaterialTextures.Count; ++i)
            {
                List<MatAndTransformToMerged> mats = distinctMaterialTextures[i].matsAndGOs.mats;
                Rect fullSamplingRect = new Rect(0.0f, 0.0f, 1f, 1f);
                if ((uint)distinctMaterialTextures[i].ts.Length > 0U)
                    fullSamplingRect = !distinctMaterialTextures[i].allTexturesUseSameMatTiling ? distinctMaterialTextures[i].obUVrect.GetRect() : distinctMaterialTextures[i].ts[0].encapsulatingSamplingRect.GetRect();
                for (int j = 0; j < mats.Count; ++j)
                {
                    MB_MaterialAndUVRect key = new MB_MaterialAndUVRect(mats[j].mat, rectsInAtlas[i], mats[j].samplingRectMatAndUVTiling.GetRect(), mats[j].materialTiling.GetRect(), fullSamplingRect, mats[j].objName);
                    if (!mat2rect_map.Contains(key))
                        mat2rect_map.Add(key);
                }

            }
            resultAtlasesAndRects.atlases = atlases;
            resultAtlasesAndRects.texPropertyNames = ShaderTextureProperty.GetNames(texPropertyNames);
            resultAtlasesAndRects.mat2rect_map = mat2rect_map;
            if (progressInfo != null)
                progressInfo("Restoring Texture Formats & Read Flags", 0.8f);
            _destroyTemporaryTextures();
            if (textureEditorMethods != null)
                textureEditorMethods.RestoreReadFlagsAndFormats(progressInfo);
            if (report != null && LOG_LEVEL >= MB2_LogLevel.info)
                Debug.Log(report.ToString());
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("Time Step 3 Create And Save Atlases part 3 " + ((float)sw.ElapsedMilliseconds - t3).ToString("f5")));
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                string str1 = "Total time Step 3 Create And Save Atlases ";
                elapsed = sw.Elapsed;
                string str2 = elapsed.ToString();
                Debug.Log((str1 + str2));
            }
        }

        private AtlasPackingResult[] __RuntTexturePackerOnly(List<MB_TexSet> distinctMaterialTextures, int _padding)
        {
            AtlasPackingResult[] atlasPackingResultArray;
            if (distinctMaterialTextures.Count == 1 && !_fixOutOfBoundsUVs)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log("Only one image per atlas. Will re-use original texture");
                atlasPackingResultArray = new AtlasPackingResult[1]
                {
          new AtlasPackingResult()
                };
                atlasPackingResultArray[0].rects = new Rect[1];
                atlasPackingResultArray[0].srcImgIdxs = new int[1];
                atlasPackingResultArray[0].rects[0] = new Rect(0.0f, 0.0f, 1f, 1f);
                Texture2D texture2D = (Texture2D)null;
                MeshBakerMaterialTexture bakerMaterialTexture = (MeshBakerMaterialTexture)null;
                if ((uint)distinctMaterialTextures[0].ts.Length > 0U)
                {
                    bakerMaterialTexture = distinctMaterialTextures[0].ts[0];
                    texture2D = bakerMaterialTexture.t;
                }
                atlasPackingResultArray[0].atlasX = (texture2D== null) ? 16 : bakerMaterialTexture.t.width;
                atlasPackingResultArray[0].atlasY = (texture2D== null) ? 16 : bakerMaterialTexture.t.height;
                atlasPackingResultArray[0].usedW = (texture2D== null) ? 16 : bakerMaterialTexture.t.width;
                atlasPackingResultArray[0].usedH = (texture2D== null) ? 16 : bakerMaterialTexture.t.height;
            }
            else
            {
                List<Vector2> imgWidthHeights = new List<Vector2>();
                for (int index = 0; index < distinctMaterialTextures.Count; ++index)
                    imgWidthHeights.Add(new Vector2((float)distinctMaterialTextures[index].idealWidth, (float)distinctMaterialTextures[index].idealHeight));
                atlasPackingResultArray = new MB2_TexturePacker()
                {
                    doPowerOfTwoTextures = _meshBakerTexturePackerForcePowerOfTwo
                }.GetRects(imgWidthHeights, _maxAtlasSize, _padding, true);
            }
            return atlasPackingResultArray;
        }

        private IEnumerator __CreateAtlasesMBTexturePacker(ProgressUpdateDelegate progressInfo, int numAtlases, List<MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, Material resultMaterial, Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods, int _padding)
        {
            Rect[] uvRects;
            if (distinctMaterialTextures.Count == 1 && !_fixOutOfBoundsUVs)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log("Only one image per atlas. Will re-use original texture");
                uvRects = new Rect[1]
                {
          new Rect(0.0f, 0.0f, 1f, 1f)
                };
                for (int i = 0; i < numAtlases; ++i)
                {
                    MeshBakerMaterialTexture dmt = distinctMaterialTextures[0].ts[i];
                    atlases[i] = dmt.t;
                    resultMaterial.SetTexture(texPropertyNames[i].name, (Texture)atlases[i]);
                    resultMaterial.SetTextureScale(texPropertyNames[i].name, dmt.matTilingRect.size);
                    resultMaterial.SetTextureOffset(texPropertyNames[i].name, dmt.matTilingRect.min);
                }
            }
            else
            {
                List<Vector2> imageSizes = new List<Vector2>();
                for (int i = 0; i < distinctMaterialTextures.Count; ++i)
                    imageSizes.Add(new Vector2((float)distinctMaterialTextures[i].idealWidth, (float)distinctMaterialTextures[i].idealHeight));
                MB2_TexturePacker tp = new MB2_TexturePacker();
                tp.doPowerOfTwoTextures = _meshBakerTexturePackerForcePowerOfTwo;
                int atlasSizeX = 1;
                int atlasSizeY = 1;
                int atlasMaxDimension = _maxAtlasSize;
                AtlasPackingResult[] packerRects = tp.GetRects(imageSizes, atlasMaxDimension, _padding);
                atlasSizeX = packerRects[0].atlasX;
                atlasSizeY = packerRects[0].atlasY;
                uvRects = packerRects[0].rects;
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log(("Generated atlas will be " + atlasSizeX + "x" + atlasSizeY + " (Max atlas size for platform: " + atlasMaxDimension + ")"));
                for (int propIdx = 0; propIdx < numAtlases; ++propIdx)
                {
                    Texture2D atlas;
                    if (allTexturesAreNullAndSameColor[propIdx])
                    {
                        atlas = null;
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.Log(("=== Not creating atlas for " + texPropertyNames[propIdx].name + " because textures are null and default value parameters are the same."));
                    }
                    else
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.Log(("=== Creating atlas for " + texPropertyNames[propIdx].name));
                        GC.Collect();
                        Color[][] atlasPixels = new Color[atlasSizeY][];
                        for (int j = 0; j < atlasPixels.Length; ++j)
                            atlasPixels[j] = new Color[atlasSizeX];
                        bool isNormalMap = false;
                        if (texPropertyNames[propIdx].isNormalMap)
                            isNormalMap = true;
                        for (int texSetIdx = 0; texSetIdx < distinctMaterialTextures.Count; ++texSetIdx)
                        {
                            string s = "Creating Atlas '" + texPropertyNames[propIdx].name + "' texture " + distinctMaterialTextures[texSetIdx];
                            if (progressInfo != null)
                                progressInfo(s, 0.01f);
                            MB_TexSet texSet = distinctMaterialTextures[texSetIdx];
                            if (LOG_LEVEL >= MB2_LogLevel.trace)
                                Debug.Log(string.Format("Adding texture {0} to atlas {1}", (texSet.ts[propIdx].t== null) ? "null" : (texSet.ts[propIdx].t).ToString(), texPropertyNames[propIdx]));
                            Rect r = uvRects[texSetIdx];
                            Texture2D t = texSet.ts[propIdx].t;
                            // ISSUE: explicit reference operation
                            int x = Mathf.RoundToInt((r).x * (float)atlasSizeX);
                            // ISSUE: explicit reference operation
                            int y = Mathf.RoundToInt((r).y * (float)atlasSizeY);
                            // ISSUE: explicit reference operation
                            int ww = Mathf.RoundToInt(r.width * (float)atlasSizeX);
                            // ISSUE: explicit reference operation
                            int hh = Mathf.RoundToInt((r).height * (float)atlasSizeY);
                            if (ww == 0 || hh == 0)
                                Debug.LogError("Image in atlas has no height or width");
                            if (progressInfo != null)
                                progressInfo(s + " set ReadWrite flag", 0.01f);
                            if (textureEditorMethods != null)
                                textureEditorMethods.SetReadWriteFlag(t, true, true);
                            if (progressInfo != null)
                                progressInfo(s + "Copying to atlas: '" + texSet.ts[propIdx].t + "'", 0.02f);
                            DRect samplingRect = texSet.ts[propIdx].encapsulatingSamplingRect;
                            yield return CopyScaledAndTiledToAtlas(texSet.ts[propIdx], texSet, texPropertyNames[propIdx], samplingRect, x, y, ww, hh, _fixOutOfBoundsUVs, _maxTilingBakeSize, atlasPixels, atlasSizeX, isNormalMap, progressInfo);

                        }
                        yield return numAtlases;
                        if (progressInfo != null)
                            progressInfo("Applying changes to atlas: '" + texPropertyNames[propIdx].name + "'", 0.03f);
                        atlas = new Texture2D(atlasSizeX, atlasSizeY, (TextureFormat)5, true);
                        for (int j = 0; j < atlasPixels.Length; ++j)
                            atlas.SetPixels(0, j, atlasSizeX, 1, atlasPixels[j]);
                        atlas.Apply();
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.Log(("Saving atlas " + texPropertyNames[propIdx].name + " w=" + ((Texture)atlas).width + " h=" + ((Texture)atlas).height));
                    }
                    atlases[propIdx] = atlas;
                    if (progressInfo != null)
                        progressInfo("Saving atlas: '" + texPropertyNames[propIdx].name + "'", 0.04f);
                    if (_saveAtlasesAsAssets && textureEditorMethods != null)
                        textureEditorMethods.SaveAtlasToAssetDatabase(atlases[propIdx], texPropertyNames[propIdx], propIdx, resultMaterial);
                    else
                        resultMaterial.SetTexture(texPropertyNames[propIdx].name, (Texture)atlases[propIdx]);
                    resultMaterial.SetTextureOffset(texPropertyNames[propIdx].name, Vector2.zero);
                    resultMaterial.SetTextureScale(texPropertyNames[propIdx].name, Vector2.one);
                    _destroyTemporaryTextures();
                    atlas = (Texture2D)null;
                }
                imageSizes = (List<Vector2>)null;
                tp = (MB2_TexturePacker)null;
                packerRects = (AtlasPackingResult[])null;
            }
            __createAtlasesMBTexturePacker = uvRects;
        }

        private Rect[] __CreateAtlasesMBTexturePackerFast(ProgressUpdateDelegate progressInfo, int numAtlases, List<MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, Material resultMaterial, Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods, int _padding)
        {
            Rect[] rectArray;
            if (distinctMaterialTextures.Count == 1 && !_fixOutOfBoundsUVs)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log("Only one image per atlas. Will re-use original texture");
                rectArray = new Rect[1]
                {
          new Rect(0.0f, 0.0f, 1f, 1f)
                };
                for (int index = 0; index < numAtlases; ++index)
                {
                    MeshBakerMaterialTexture t = distinctMaterialTextures[0].ts[index];
                    atlases[index] = t.t;
                    resultMaterial.SetTexture(texPropertyNames[index].name, (Texture)atlases[index]);
                    resultMaterial.SetTextureScale(texPropertyNames[index].name, t.matTilingRect.size);
                    resultMaterial.SetTextureOffset(texPropertyNames[index].name, t.matTilingRect.min);
                }
            }
            else
            {
                List<Vector2> imgWidthHeights = new List<Vector2>();
                for (int index = 0; index < distinctMaterialTextures.Count; ++index)
                    imgWidthHeights.Add(new Vector2((float)distinctMaterialTextures[index].idealWidth, (float)distinctMaterialTextures[index].idealHeight));
                MB2_TexturePacker mb2TexturePacker = new MB2_TexturePacker();
                mb2TexturePacker.doPowerOfTwoTextures = _meshBakerTexturePackerForcePowerOfTwo;
                int maxAtlasSize = _maxAtlasSize;
                AtlasPackingResult[] rects = mb2TexturePacker.GetRects(imgWidthHeights, maxAtlasSize, _padding);
                int atlasX = rects[0].atlasX;
                int atlasY = rects[0].atlasY;
                rectArray = rects[0].rects;
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log(("Generated atlas will be " + atlasX + "x" + atlasY + " (Max atlas size for platform: " + maxAtlasSize + ")"));
                GameObject gameObject = (GameObject)null;
                try
                {
                    gameObject = new GameObject("MBrenderAtlasesGO");
                    MB3_AtlasPackerRenderTexture packerRenderTexture = (MB3_AtlasPackerRenderTexture)gameObject.AddComponent<MB3_AtlasPackerRenderTexture>();
                    gameObject.AddComponent<Camera>();
                    if (_considerNonTextureProperties && LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning("Blend Non-Texture Properties has limited functionality when used with Mesh Baker Texture Packer Fast.");
                    for (int atlasNum = 0; atlasNum < numAtlases; ++atlasNum)
                    {
                        Texture2D texture2D;
                        if (allTexturesAreNullAndSameColor[atlasNum])
                        {
                            texture2D = (Texture2D)null;
                            if (LOG_LEVEL >= MB2_LogLevel.debug)
                                Debug.Log(("Not creating atlas for " + texPropertyNames[atlasNum].name + " because textures are null and default value parameters are the same."));
                        }
                        else
                        {
                            GC.Collect();
                            if (progressInfo != null)
                                progressInfo("Creating Atlas '" + texPropertyNames[atlasNum].name + "'", 0.01f);
                            if (LOG_LEVEL >= MB2_LogLevel.debug)
                                Debug.Log(("About to render " + texPropertyNames[atlasNum].name + " isNormal=" + texPropertyNames[atlasNum].isNormalMap.ToString()));
                            packerRenderTexture.LOG_LEVEL = LOG_LEVEL;
                            packerRenderTexture.width = atlasX;
                            packerRenderTexture.height = atlasY;
                            packerRenderTexture.padding = _padding;
                            packerRenderTexture.rects = rectArray;
                            packerRenderTexture.textureSets = distinctMaterialTextures;
                            packerRenderTexture.indexOfTexSetToRender = atlasNum;
                            packerRenderTexture.texPropertyName = texPropertyNames[atlasNum];
                            packerRenderTexture.isNormalMap = texPropertyNames[atlasNum].isNormalMap;
                            packerRenderTexture.fixOutOfBoundsUVs = _fixOutOfBoundsUVs;
                            packerRenderTexture.considerNonTextureProperties = _considerNonTextureProperties;
                            packerRenderTexture.resultMaterialTextureBlender = resultMaterialTextureBlender;
                            texture2D = packerRenderTexture.OnRenderAtlas(this);
                            if (LOG_LEVEL >= MB2_LogLevel.debug)
                                Debug.Log(("Saving atlas " + texPropertyNames[atlasNum].name + " w=" + ((Texture)texture2D).width + " h=" + ((Texture)texture2D).height+ " id=" + (texture2D).GetInstanceID()));
                        }
                        atlases[atlasNum] = texture2D;
                        if (progressInfo != null)
                            progressInfo("Saving atlas: '" + texPropertyNames[atlasNum].name + "'", 0.04f);
                        if (_saveAtlasesAsAssets && textureEditorMethods != null)
                            textureEditorMethods.SaveAtlasToAssetDatabase(atlases[atlasNum], texPropertyNames[atlasNum], atlasNum, resultMaterial);
                        else
                            resultMaterial.SetTexture(texPropertyNames[atlasNum].name, (Texture)atlases[atlasNum]);
                        resultMaterial.SetTextureOffset(texPropertyNames[atlasNum].name, Vector2.zero);
                        resultMaterial.SetTextureScale(texPropertyNames[atlasNum].name, Vector2.one);
                        _destroyTemporaryTextures();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                finally
                {
                    if (gameObject!= null)
                        MB_Utility.Destroy(gameObject);
                }
            }
            return rectArray;
        }

        private Rect[] __CreateAtlasesUnityTexturePacker(ProgressUpdateDelegate progressInfo, int numAtlases, List<MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, Material resultMaterial, Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods, int _padding)
        {
            Rect[] rs;
            if (distinctMaterialTextures.Count == 1 && !_fixOutOfBoundsUVs)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log("Only one image per atlas. Will re-use original texture");
                rs = new Rect[1] { new Rect(0.0f, 0.0f, 1f, 1f) };
                for (int index = 0; index < numAtlases; ++index)
                {
                    MeshBakerMaterialTexture t = distinctMaterialTextures[0].ts[index];
                    atlases[index] = t.t;
                    resultMaterial.SetTexture(texPropertyNames[index].name, (Texture)atlases[index]);
                    resultMaterial.SetTextureScale(texPropertyNames[index].name, t.matTilingRect.size);
                    resultMaterial.SetTextureOffset(texPropertyNames[index].name, t.matTilingRect.min);
                }
            }
            else
            {
                long estimatedAtlasSize = 0;
                int w = 1;
                int h = 1;
                rs = (Rect[])null;
                for (int atlasNum = 0; atlasNum < numAtlases; ++atlasNum)
                {
                    Texture2D texture2D1;
                    if (allTexturesAreNullAndSameColor[atlasNum])
                    {
                        texture2D1 = (Texture2D)null;
                    }
                    else
                    {
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.LogWarning(("Beginning loop " + atlasNum + " num temporary textures " + _temporaryTextures.Count));
                        for (int index = 0; index < distinctMaterialTextures.Count; ++index)
                        {
                            MB_TexSet distinctMaterialTexture = distinctMaterialTextures[index];
                            int idealWidth = distinctMaterialTexture.idealWidth;
                            int idealHeight = distinctMaterialTexture.idealHeight;
                            Texture2D texture2D2 = distinctMaterialTexture.ts[atlasNum].t;
                            if ((texture2D2== null))
                            {
                                texture2D2 = distinctMaterialTexture.ts[atlasNum].t = _createTemporaryTexture(idealWidth, idealHeight, (TextureFormat)5, true);
                                if (_considerNonTextureProperties && resultMaterialTextureBlender != null)
                                {
                                    Color colorIfNoTexture = resultMaterialTextureBlender.GetColorIfNoTexture(distinctMaterialTexture.matsAndGOs.mats[0].mat, texPropertyNames[atlasNum]);
                                    if (LOG_LEVEL >= MB2_LogLevel.trace)
                                        Debug.Log(("Setting texture to solid color " + colorIfNoTexture));
                                    MB_Utility.setSolidColor(texture2D2, colorIfNoTexture);
                                }
                                else
                                {
                                    Color colorIfNoTexture = GetColorIfNoTexture(texPropertyNames[atlasNum]);
                                    MB_Utility.setSolidColor(texture2D2, colorIfNoTexture);
                                }
                            }
                            if (progressInfo != null)
                                progressInfo("Adjusting for scale and offset " + texture2D2, 0.01f);
                            if (textureEditorMethods != null)
                                textureEditorMethods.SetReadWriteFlag(texture2D2, true, true);
                            Texture2D t = GetAdjustedForScaleAndOffset2(distinctMaterialTexture.ts[atlasNum], distinctMaterialTexture.obUVoffset, distinctMaterialTexture.obUVscale);
                            if (t.width!= idealWidth || t.height != idealHeight)
                            {
                                if (progressInfo != null)
                                    progressInfo("Resizing texture '" + t + "'", 0.01f);
                                if (LOG_LEVEL >= MB2_LogLevel.debug)
                                    Debug.LogWarning(("Copying and resizing texture " + texPropertyNames[atlasNum].name + " from " + ((Texture)t).width + "x" + ((Texture)t).height + " to " + idealWidth + "x" + idealHeight));
                                t = _resizeTexture(t, idealWidth, idealHeight);
                            }
                            distinctMaterialTexture.ts[atlasNum].t = t;
                        }
                        Texture2D[] texToPack = new Texture2D[distinctMaterialTextures.Count];
                        for (int index = 0; index < distinctMaterialTextures.Count; ++index)
                        {
                            Texture2D t = distinctMaterialTextures[index].ts[atlasNum].t;
                            estimatedAtlasSize += (long)(((Texture)t).width* ((Texture)t).height);
                            if (_considerNonTextureProperties)
                                t = TintTextureWithTextureCombiner(t, distinctMaterialTextures[index], texPropertyNames[atlasNum]);
                            texToPack[index] = t;
                        }
                        if (textureEditorMethods != null)
                            textureEditorMethods.CheckBuildSettings(estimatedAtlasSize);
                        if (Math.Sqrt((double)estimatedAtlasSize) > 3500.0 && LOG_LEVEL >= MB2_LogLevel.warn)
                            Debug.LogWarning("The maximum possible atlas size is 4096. Textures may be shrunk");
                        texture2D1 = new Texture2D(1, 1, (TextureFormat)5, true);
                        if (progressInfo != null)
                            progressInfo("Packing texture atlas " + texPropertyNames[atlasNum].name, 0.25f);
                        if (atlasNum == 0)
                        {
                            double num1;
                            if (progressInfo != null)
                            {
                                ProgressUpdateDelegate progressUpdateDelegate = progressInfo;
                                string str1 = "Estimated min size of atlases: ";
                                num1 = Math.Sqrt((double)estimatedAtlasSize);
                                string str2 = num1.ToString("F0");
                                string msg = str1 + str2;
                                double num2 = 0.100000001490116;
                                progressUpdateDelegate(msg, (float)num2);
                            }
                            if (LOG_LEVEL >= MB2_LogLevel.info)
                            {
                                string str1 = "Estimated atlas minimum size:";
                                num1 = Math.Sqrt((double)estimatedAtlasSize);
                                string str2 = num1.ToString("F0");
                                Debug.Log((str1 + str2));
                            }
                            _addWatermark(texToPack);
                            if (distinctMaterialTextures.Count == 1 && !_fixOutOfBoundsUVs)
                            {
                                rs = new Rect[1]
                                {
                  new Rect(0.0f, 0.0f, 1f, 1f)
                                };
                                texture2D1 = _copyTexturesIntoAtlas(texToPack, _padding, rs, ((Texture)texToPack[0]).width, ((Texture)texToPack[0]).height);
                            }
                            else
                            {
                                int num2 = 4096;
                                rs = texture2D1.PackTextures(texToPack, _padding, num2, false);
                            }
                            if (LOG_LEVEL >= MB2_LogLevel.info)
                                Debug.Log(("After pack textures atlas size " + ((Texture)texture2D1).width) + " " + ((Texture)texture2D1).height);
                            w = ((Texture)texture2D1).width;
                            h = ((Texture)texture2D1).height;
                            texture2D1.Apply();
                        }
                        else
                        {
                            if (progressInfo != null)
                                progressInfo("Copying Textures Into: " + texPropertyNames[atlasNum].name, 0.1f);
                            texture2D1 = _copyTexturesIntoAtlas(texToPack, _padding, rs, w, h);
                        }
                    }
                    atlases[atlasNum] = texture2D1;
                    if (_saveAtlasesAsAssets && textureEditorMethods != null)
                        textureEditorMethods.SaveAtlasToAssetDatabase(atlases[atlasNum], texPropertyNames[atlasNum], atlasNum, resultMaterial);
                    resultMaterial.SetTextureOffset(texPropertyNames[atlasNum].name, Vector2.zero);
                    resultMaterial.SetTextureScale(texPropertyNames[atlasNum].name, Vector2.one);
                    _destroyTemporaryTextures();
                    GC.Collect();
                }
            }
            return rs;
        }

        private void _addWatermark(Texture2D[] texToPack)
        {
        }

        private Texture2D _addWatermark(Texture2D texToPack)
        {
            return texToPack;
        }

        private Texture2D _copyTexturesIntoAtlas(Texture2D[] texToPack, int padding, Rect[] rs, int w, int h)
        {
            Texture2D t = new Texture2D(w, h, (TextureFormat)5, true);
            MB_Utility.setSolidColor(t, Color.clear);
            for (int index = 0; index < rs.Length; ++index)
            {
                Rect r = rs[index];
                Texture2D source = texToPack[index];
                int num1 = Mathf.RoundToInt((r).x * (float)w);
                int num2 = Mathf.RoundToInt((r).y * (float)h);
                int newWidth = (int) (Mathf.RoundToInt(r.width) * (float)w);
                int newHeight = Mathf.RoundToInt(r.height * (float)h);
                if (source.width != newWidth && source.height != newHeight)
                {
                    source = MB_Utility.resampleTexture(source, newWidth, newHeight);
                    _temporaryTextures.Add(source);
                }
                t.SetPixels(num1, num2, newWidth, newHeight, source.GetPixels());
            }
            t.Apply();
            return t;
        }

        private bool IsPowerOfTwo(int x)
        {
            return (x & x - 1) == 0;
        }

        private void MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects(List<MB_TexSet> distinctMaterialTextures, bool fixOutOfBoundsUVs)
        {
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log("MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects");
            int num1 = 0;
            for (int index1 = 0; index1 < distinctMaterialTextures.Count; ++index1)
            {
                MB_TexSet distinctMaterialTexture = distinctMaterialTextures[index1];
                int num2 = -1;
                bool flag = true;
                DRect drect = new DRect();
                for (int index2 = 0; index2 < distinctMaterialTexture.ts.Length; ++index2)
                {
                    if (num2 != -1)
                    {
                        if ((distinctMaterialTexture.ts[index2].t!= null) && drect != distinctMaterialTexture.ts[index2].matTilingRect)
                            flag = false;
                    }
                    else if ((distinctMaterialTexture.ts[index2].t!= null))
                    {
                        num2 = index2;
                        drect = distinctMaterialTexture.ts[index2].matTilingRect;
                    }
                }
                if (LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS)
                    Debug.LogFormat("TextureSet {0} allTexturesUseSameMatTiling = {1}", new object[2]
                    {
             index1,
             flag
                    });
                if (flag)
                {
                    distinctMaterialTexture.allTexturesUseSameMatTiling = true;
                }
                else
                {
                    if (LOG_LEVEL <= MB2_LogLevel.info || LOG_LEVEL_TRACE_MERGE_MAT_SUBRECTS)
                        Debug.Log(string.Format("Textures in material(s) do not all use the same material tiling. This set of textures will not be considered for merge: {0} ", distinctMaterialTexture.GetDescription()));
                    distinctMaterialTexture.allTexturesUseSameMatTiling = false;
                }
            }
            for (int index1 = 0; index1 < distinctMaterialTextures.Count; ++index1)
            {
                MB_TexSet distinctMaterialTexture = distinctMaterialTextures[index1];
                DRect drect = !fixOutOfBoundsUVs ? new DRect(0.0, 0.0, 1.0, 1.0) : new DRect(distinctMaterialTexture.obUVoffset, distinctMaterialTexture.obUVscale);
                for (int index2 = 0; index2 < distinctMaterialTexture.matsAndGOs.mats.Count; ++index2)
                {
                    distinctMaterialTexture.matsAndGOs.mats[index2].obUVRectIfTilingSame = drect;
                    distinctMaterialTexture.matsAndGOs.mats[index2].objName = (distinctMaterialTextures[index1].matsAndGOs.gos[0]).name;
                }
                distinctMaterialTexture.CalcInitialFullSamplingRects(fixOutOfBoundsUVs);
                distinctMaterialTexture.CalcMatAndUVSamplingRects();
            }
            List<int> intList = new List<int>();
            for (int index1 = 0; index1 < distinctMaterialTextures.Count; ++index1)
            {
                MB_TexSet distinctMaterialTexture1 = distinctMaterialTextures[index1];
                for (int index2 = index1 + 1; index2 < distinctMaterialTextures.Count; ++index2)
                {
                    MB_TexSet distinctMaterialTexture2 = distinctMaterialTextures[index2];
                    if (distinctMaterialTexture2.AllTexturesAreSameForMerge(distinctMaterialTexture1, _considerNonTextureProperties, resultMaterialTextureBlender))
                    {
                        double num2 = 0.0;
                        double num3 = 0.0;
                        DRect drect1 = new DRect();
                        int num4 = -1;
                        for (int index3 = 0; index3 < distinctMaterialTexture1.ts.Length; ++index3)
                        {
                            if ((distinctMaterialTexture1.ts[index3].t!= null) && num4 == -1)
                                num4 = index3;
                        }
                        if (num4 != -1)
                        {
                            DRect uvRect1 = distinctMaterialTexture2.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
                            for (int index3 = 1; index3 < distinctMaterialTexture2.matsAndGOs.mats.Count; ++index3)
                                uvRect1 = MB3_UVTransformUtility.GetEncapsulatingRect(ref uvRect1, ref distinctMaterialTexture2.matsAndGOs.mats[index3].samplingRectMatAndUVTiling);
                            DRect drect2 = distinctMaterialTexture1.matsAndGOs.mats[0].samplingRectMatAndUVTiling;
                            for (int index3 = 1; index3 < distinctMaterialTexture1.matsAndGOs.mats.Count; ++index3)
                                drect2 = MB3_UVTransformUtility.GetEncapsulatingRect(ref drect2, ref distinctMaterialTexture1.matsAndGOs.mats[index3].samplingRectMatAndUVTiling);
                            drect1 = MB3_UVTransformUtility.GetEncapsulatingRect(ref uvRect1, ref drect2);
                            num2 += drect1.width * drect1.height;
                            num3 += uvRect1.width * uvRect1.height + drect2.width * drect2.height;
                        }
                        else
                            drect1 = new DRect(0.0f, 0.0f, 1f, 1f);
                        if (num2 < num3)
                        {
                            ++num1;
                            StringBuilder stringBuilder = (StringBuilder)null;
                            if (LOG_LEVEL >= MB2_LogLevel.info)
                            {
                                stringBuilder = new StringBuilder();
                                stringBuilder.AppendFormat("About To Merge:\n   TextureSet1 {0}\n   TextureSet2 {1}\n", distinctMaterialTexture2.GetDescription(), distinctMaterialTexture1.GetDescription());
                                if (LOG_LEVEL >= MB2_LogLevel.trace)
                                {
                                    for (int index3 = 0; index3 < distinctMaterialTexture2.matsAndGOs.mats.Count; ++index3)
                                        stringBuilder.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", distinctMaterialTexture2.matsAndGOs.mats[index3].mat, distinctMaterialTexture2.matsAndGOs.mats[index3].samplingRectMatAndUVTiling, distinctMaterialTexture2.ts[0].encapsulatingSamplingRect);
                                    for (int index3 = 0; index3 < distinctMaterialTexture1.matsAndGOs.mats.Count; ++index3)
                                        stringBuilder.AppendFormat("tx2 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", distinctMaterialTexture1.matsAndGOs.mats[index3].mat, distinctMaterialTexture1.matsAndGOs.mats[index3].samplingRectMatAndUVTiling, distinctMaterialTexture1.ts[0].encapsulatingSamplingRect);
                                }
                            }
                            for (int index3 = 0; index3 < distinctMaterialTexture1.matsAndGOs.gos.Count; ++index3)
                            {
                                if (!distinctMaterialTexture2.matsAndGOs.gos.Contains(distinctMaterialTexture1.matsAndGOs.gos[index3]))
                                    distinctMaterialTexture2.matsAndGOs.gos.Add(distinctMaterialTexture1.matsAndGOs.gos[index3]);
                            }
                            for (int index3 = 0; index3 < distinctMaterialTexture1.matsAndGOs.mats.Count; ++index3)
                                distinctMaterialTexture2.matsAndGOs.mats.Add(distinctMaterialTexture1.matsAndGOs.mats[index3]);
                            distinctMaterialTexture2.matsAndGOs.mats.Sort((IComparer<MatAndTransformToMerged>)new SamplingRectEnclosesComparer());
                            for (int index3 = 0; index3 < distinctMaterialTexture2.ts.Length; ++index3)
                                distinctMaterialTexture2.ts[index3].encapsulatingSamplingRect = drect1;
                            if (!intList.Contains(index1))
                                intList.Add(index1);
                            if (LOG_LEVEL >= MB2_LogLevel.debug)
                            {
                                if (LOG_LEVEL >= MB2_LogLevel.trace)
                                {
                                    stringBuilder.AppendFormat("=== After Merge TextureSet {0}\n", distinctMaterialTexture2.GetDescription());
                                    for (int index3 = 0; index3 < distinctMaterialTexture2.matsAndGOs.mats.Count; ++index3)
                                        stringBuilder.AppendFormat("tx1 Mat {0} matAndMeshUVRect {1} fullSamplingRect {2}\n", distinctMaterialTexture2.matsAndGOs.mats[index3].mat, distinctMaterialTexture2.matsAndGOs.mats[index3].samplingRectMatAndUVTiling, distinctMaterialTexture2.ts[0].encapsulatingSamplingRect);
                                }
                                Debug.Log(stringBuilder.ToString());
                                break;
                            }
                            break;
                        }
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                            Debug.Log(string.Format("Considered merging {0} and {1} but there was not enough overlap. It is more efficient to bake these to separate rectangles.", distinctMaterialTexture2.GetDescription(), distinctMaterialTexture1.GetDescription()));
                    }
                }
            }
            for (int index = intList.Count - 1; index >= 0; --index)
                distinctMaterialTextures.RemoveAt(intList[index]);
            intList.Clear();
            if (LOG_LEVEL < MB2_LogLevel.info)
                return;
            Debug.Log(string.Format("MergeOverlappingDistinctMaterialTexturesAndCalcMaterialSubrects complete merged {0}", num1));
        }

        private void DoIntegrityCheckMergedAtlasRects(List<MB_TexSet> distinctMaterialTextures)
        {
        }

        private Vector2 GetAdjustedForScaleAndOffset2Dimensions(MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale)
        {
            if (source.matTilingRect.x == 0.0 && source.matTilingRect.y == 0.0 && source.matTilingRect.width == 1.0 && source.matTilingRect.height == 1.0)
            {
                if (!_fixOutOfBoundsUVs)
                    return new Vector2((float)((Texture)source.t).width, (float)((Texture)source.t).height);
                if (obUVoffset.x == 0.0 && obUVoffset.y == 0.0 && obUVscale.x == 1.0 && obUVscale.y == 1.0)
                    return new Vector2((float)((Texture)source.t).width, (float)((Texture)source.t).height);
            }
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("GetAdjustedForScaleAndOffset2Dimensions: " + source.t + " " + obUVoffset + " " + obUVscale));
            float num1 = (float)source.encapsulatingSamplingRect.width * (float)((Texture)source.t).width;
            float num2 = (float)source.encapsulatingSamplingRect.height * (float)((Texture)source.t).height;
            if ((double)num1 > (double)_maxTilingBakeSize)
                num1 = (float)_maxTilingBakeSize;
            if ((double)num2 > (double)_maxTilingBakeSize)
                num2 = (float)_maxTilingBakeSize;
            if ((double)num1 < 1.0)
                num1 = 1f;
            if ((double)num2 < 1.0)
                num2 = 1f;
            return new Vector2(num1, num2);
        }

        public Texture2D GetAdjustedForScaleAndOffset2(MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale)
        {
            if (source.matTilingRect.x == 0.0 && source.matTilingRect.y == 0.0 && source.matTilingRect.width == 1.0 && source.matTilingRect.height == 1.0 && (!_fixOutOfBoundsUVs || obUVoffset.x == 0.0 && obUVoffset.y == 0.0 && obUVscale.x == 1.0 && obUVscale.y == 1.0))
                return source.t;
            Vector2 offset2Dimensions = GetAdjustedForScaleAndOffset2Dimensions(source, obUVoffset, obUVscale);
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.LogWarning(("GetAdjustedForScaleAndOffset2: " + source.t + " " + obUVoffset + " " + obUVscale));
            float x = (float)offset2Dimensions.x;
            float y = (float)offset2Dimensions.y;
            float width = (float)source.matTilingRect.width;
            float height = (float)source.matTilingRect.height;
            float num1 = (float)source.matTilingRect.x;
            float num2 = (float)source.matTilingRect.y;
            if (_fixOutOfBoundsUVs)
            {
                width *= (float)obUVscale.x;
                height *= (float)obUVscale.y;
                num1 = (float)source.matTilingRect.x * (float)(double)obUVscale.x + (float)(double)obUVoffset.x;
                num2 = (float)source.matTilingRect.y * (float)(double)obUVscale.y + (float)(double)obUVoffset.y;
            }
            Texture2D temporaryTexture = _createTemporaryTexture((int)x, (int)y, (TextureFormat)5, true);
            for (int index1 = 0; index1 < ((Texture)temporaryTexture).width; ++index1)
            {
                for (int index2 = 0; index2 < ((Texture)temporaryTexture).height; ++index2)
                {
                    float num3 = (float)index1 / x * width + num1;
                    float num4 = (float)index2 / y * height + num2;
                    temporaryTexture.SetPixel(index1, index2, source.t.GetPixelBilinear(num3, num4));
                }
            }
            temporaryTexture.Apply();
            return temporaryTexture;
        }

        internal static DRect GetSourceSamplingRect(MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale)
        {
            DRect matTilingRect = source.matTilingRect;
            DRect r2 = new DRect(obUVoffset, obUVscale);
            return MB3_UVTransformUtility.CombineTransforms(ref matTilingRect, ref r2);
        }

        private Texture2D TintTextureWithTextureCombiner(Texture2D t, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName)
        {
            if (LOG_LEVEL >= MB2_LogLevel.trace)
                Debug.Log(string.Format("Blending texture {0} mat {1} with non-texture properties using TextureBlender {2}", (t).name, sourceMaterial.matsAndGOs.mats[0].mat, resultMaterialTextureBlender));
            resultMaterialTextureBlender.OnBeforeTintTexture(sourceMaterial.matsAndGOs.mats[0].mat, shaderPropertyName.name);
            t = _createTextureCopy(t);
            for (int index1 = 0; index1 < ((Texture)t).height; ++index1)
            {
                Color[] pixels = t.GetPixels(0, index1, ((Texture)t).width, 1);
                for (int index2 = 0; index2 < pixels.Length; ++index2)
                    pixels[index2] = resultMaterialTextureBlender.OnBlendTexturePixel(shaderPropertyName.name, pixels[index2]);
                t.SetPixels(0, index1, ((Texture)t).width, 1, pixels);
            }
            t.Apply();
            return t;
        }

        public IEnumerator CopyScaledAndTiledToAtlas(MeshBakerMaterialTexture source, MB_TexSet sourceMaterial, ShaderTextureProperty shaderPropertyName, DRect srcSamplingRect, int targX, int targY, int targW, int targH, bool _fixOutOfBoundsUVs, int maxSize, Color[][] atlasPixels, int atlasWidth, bool isNormalMap, ProgressUpdateDelegate progressInfo = null)
        {
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("CopyScaledAndTiledToAtlas: " + source.t + " inAtlasX=" + targX + " inAtlasY=" + targY + " inAtlasW=" + targW + " inAtlasH=" + targH));
            float newWidth = (float)targW;
            float newHeight = (float)targH;
            float scx = (float)srcSamplingRect.width;
            float scy = (float)srcSamplingRect.height;
            float ox = (float)srcSamplingRect.x;
            float oy = (float)srcSamplingRect.y;
            int w = (int)newWidth;
            int h = (int)newHeight;
            Texture2D t = source.t;
            if ((t==null))
            {
                if (LOG_LEVEL >= MB2_LogLevel.trace)
                    Debug.Log("No source texture creating a 16x16 texture.");
                t = _createTemporaryTexture(16, 16, (TextureFormat)5, true);
                scx = 1f;
                scy = 1f;
                if (_considerNonTextureProperties && resultMaterialTextureBlender != null)
                {
                    Color col = resultMaterialTextureBlender.GetColorIfNoTexture(sourceMaterial.matsAndGOs.mats[0].mat, shaderPropertyName);
                    if (LOG_LEVEL >= MB2_LogLevel.trace)
                        Debug.Log(("Setting texture to solid color " + col));
                    MB_Utility.setSolidColor(t, col);
                }
                else
                {
                    Color col = GetColorIfNoTexture(shaderPropertyName);
                    MB_Utility.setSolidColor(t, col);
                }
            }
            if (_considerNonTextureProperties && resultMaterialTextureBlender != null)
                t = TintTextureWithTextureCombiner(t, sourceMaterial, shaderPropertyName);
            t = _addWatermark(t);
            for (int i = 0; i < w; ++i)
            {
                if (progressInfo != null && w > 0)
                    progressInfo("CopyScaledAndTiledToAtlas " + ((float)((double)i / (double)w * 100.0)).ToString("F0"), 0.2f);
                for (int j = 0; j < h; ++j)
                {
                    float u = (float)i / newWidth * scx + ox;
                    float v = (float)j / newHeight * scy + oy;
                    atlasPixels[targY + j][targX + i] = t.GetPixelBilinear(u, v);
                }
            }
            for (int i = 0; i < w; ++i)
            {
                for (int j = 1; j <= atlasPadding; ++j)
                {
                    atlasPixels[targY - j][targX + i] = atlasPixels[targY][targX + i];
                    atlasPixels[targY + h - 1 + j][targX + i] = atlasPixels[targY + h - 1][targX + i];
                }
            }
            for (int j = 0; j < h; ++j)
            {
                for (int i = 1; i <= _atlasPadding; ++i)
                {
                    atlasPixels[targY + j][targX - i] = atlasPixels[targY + j][targX];
                    atlasPixels[targY + j][targX + w + i - 1] = atlasPixels[targY + j][targX + w - 1];
                }
            }
            for (int i = 1; i <= _atlasPadding; ++i)
            {
                for (int j = 1; j <= _atlasPadding; ++j)
                {
                    atlasPixels[targY - j][targX - i] = atlasPixels[targY][targX];
                    atlasPixels[targY + h - 1 + j][targX - i] = atlasPixels[targY + h - 1][targX];
                    atlasPixels[targY + h - 1 + j][targX + w + i - 1] = atlasPixels[targY + h - 1][targX + w - 1];
                    atlasPixels[targY - j][targX + w + i - 1] = atlasPixels[targY][targX + w - 1];
                    yield return null;
                }
                yield return null;
            }
        }

        public Texture2D _createTemporaryTexture(int w, int h, TextureFormat texFormat, bool mipMaps)
        {
            Texture2D t = new Texture2D(w, h, texFormat, mipMaps);
            MB_Utility.setSolidColor(t, Color.clear);
            _temporaryTextures.Add(t);
            return t;
        }

        internal Texture2D _createTextureCopy(Texture2D t)
        {
            Texture2D textureCopy = MB_Utility.createTextureCopy(t);
            _temporaryTextures.Add(textureCopy);
            return textureCopy;
        }

        private Texture2D _resizeTexture(Texture2D t, int w, int h)
        {
            Texture2D texture2D = MB_Utility.resampleTexture(t, w, h);
            _temporaryTextures.Add(texture2D);
            return texture2D;
        }

        private void _destroyTemporaryTextures()
        {
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("Destroying " + _temporaryTextures.Count + " temporary textures"));
            for (int index = 0; index < _temporaryTextures.Count; ++index)
                MB_Utility.Destroy(_temporaryTextures[index]);
            _temporaryTextures.Clear();
        }

        public void SuggestTreatment(List<GameObject> objsToMesh, Material[] resultMaterials, List<ShaderTextureProperty> _customShaderPropNames)
        {
            _customShaderPropNames = _customShaderPropNames;
            StringBuilder stringBuilder1 = new StringBuilder();
            Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary1 = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
            for (int index1 = 0; index1 < objsToMesh.Count; ++index1)
            {
                GameObject go = objsToMesh[index1];
                if (!(go== null))
                {
                    Material[] goMaterials = MB_Utility.GetGOMaterials(objsToMesh[index1]);
                    if (goMaterials.Length > 1)
                    {
                        stringBuilder1.AppendFormat("\nObject {0} uses {1} materials. Possible treatments:\n", (objsToMesh[index1]).name, goMaterials.Length);
                        stringBuilder1.AppendFormat("  1) Collapse the submeshes together into one submesh in the combined mesh. Each of the original submesh materials will map to a different UV rectangle in the atlas(es) used by the combined material.\n");
                        stringBuilder1.AppendFormat("  2) Use the multiple materials feature to map submeshes in the source mesh to submeshes in the combined mesh.\n");
                    }
                    Mesh mesh = MB_Utility.GetMesh(go);
                    MB_Utility.MeshAnalysisResult[] meshAnalysisResultArray;
                    if (!dictionary1.TryGetValue((mesh).GetInstanceID(), out meshAnalysisResultArray))
                    {
                        meshAnalysisResultArray = new MB_Utility.MeshAnalysisResult[mesh.subMeshCount];
                        MB_Utility.doSubmeshesShareVertsOrTris(mesh, ref meshAnalysisResultArray[0]);
                        for (int submeshIndex = 0; submeshIndex < mesh.subMeshCount; ++submeshIndex)
                        {
                            MB_Utility.hasOutOfBoundsUVs(mesh, ref meshAnalysisResultArray[submeshIndex], submeshIndex, 0);
                            meshAnalysisResultArray[submeshIndex].hasOverlappingSubmeshTris = meshAnalysisResultArray[0].hasOverlappingSubmeshTris;
                            meshAnalysisResultArray[submeshIndex].hasOverlappingSubmeshVerts = meshAnalysisResultArray[0].hasOverlappingSubmeshVerts;
                        }
                        dictionary1.Add((mesh).GetInstanceID(), meshAnalysisResultArray);
                    }
                    for (int index2 = 0; index2 < goMaterials.Length; ++index2)
                    {
                        if (meshAnalysisResultArray[index2].hasOutOfBoundsUVs)
                        {
                            DRect drect = new DRect(meshAnalysisResultArray[index2].uvRect);
                            StringBuilder stringBuilder2 = stringBuilder1;
                            string format = "\nObject {0} submesh={1} material={2} uses UVs outside the range 0,0 .. 1,1 to create tiling that tiles the box {3},{4} .. {5},{6}. This is a problem because the UVs outside the 0,0 .. 1,1 rectangle will pick up neighboring textures in the atlas. Possible Treatments:\n";
                            object[] objArray = new object[7]
                            {
                 go,
                 index2,
                 goMaterials[index2],
                 drect.x.ToString("G4"),
                 drect.y.ToString("G4"),
                null,
                null
                            };
                            int index3 = 5;
                            double num = drect.x + drect.width;
                            string str1 = num.ToString("G4");
                            objArray[index3] = str1;
                            int index4 = 6;
                            num = drect.y + drect.height;
                            string str2 = num.ToString("G4");
                            objArray[index4] = str2;
                            stringBuilder2.AppendFormat(format, objArray);
                            stringBuilder1.AppendFormat("    1) Ignore the problem. The tiling may not affect result significantly.\n");
                            stringBuilder1.AppendFormat("    2) Use the 'fix out of bounds UVs' feature to bake the tiling and scale the UVs to fit in the 0,0 .. 1,1 rectangle.\n");
                            stringBuilder1.AppendFormat("    3) Use the Multiple Materials feature to map the material on this submesh to its own submesh in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n");
                            stringBuilder1.AppendFormat("    4) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n");
                        }
                    }
                    if (meshAnalysisResultArray[0].hasOverlappingSubmeshVerts)
                    {
                        stringBuilder1.AppendFormat("\nObject {0} has submeshes that share vertices. This is a problem because each vertex can have only one UV coordinate and may be required to map to different positions in the various atlases that are generated. Possible treatments:\n", objsToMesh[index1]);
                        stringBuilder1.AppendFormat(" 1) Ignore the problem. The vertices may not affect the result.\n");
                        stringBuilder1.AppendFormat(" 2) Use the Multiple Materials feature to map the submeshs that overlap to their own submeshs in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n");
                        stringBuilder1.AppendFormat(" 3) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n");
                    }
                }
            }
            Dictionary<Material, List<GameObject>> dictionary2 = new Dictionary<Material, List<GameObject>>();
            for (int index1 = 0; index1 < objsToMesh.Count; ++index1)
            {
                if ((objsToMesh[index1]!= null))
                {
                    Material[] goMaterials = MB_Utility.GetGOMaterials(objsToMesh[index1]);
                    for (int index2 = 0; index2 < goMaterials.Length; ++index2)
                    {
                        if (goMaterials[index2]!= null)
                        {
                            List<GameObject> gameObjectList;
                            if (!dictionary2.TryGetValue(goMaterials[index2], out gameObjectList))
                            {
                                gameObjectList = new List<GameObject>();
                                dictionary2.Add(goMaterials[index2], gameObjectList);
                            }
                            if (!gameObjectList.Contains(objsToMesh[index1]))
                                gameObjectList.Add(objsToMesh[index1]);
                        }
                    }
                }
            }
            List<ShaderTextureProperty> texPropertyNames = new List<ShaderTextureProperty>();
            for (int index1 = 0; index1 < resultMaterials.Length; ++index1)
            {
                _CollectPropertyNames(resultMaterials[index1], texPropertyNames);
                using (Dictionary<Material, List<GameObject>>.KeyCollection.Enumerator enumerator = dictionary2.Keys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Material current = enumerator.Current;
                        for (int index2 = 0; index2 < texPropertyNames.Count; ++index2)
                        {
                            if (current.HasProperty(texPropertyNames[index2].name))
                            {
                                Texture texture = current.GetTexture(texPropertyNames[index2].name);
                                if ((texture!= null))
                                {
                                    Vector2 textureOffset = current.GetTextureOffset(texPropertyNames[index2].name);
                                    Vector3 vector3 = current.GetTextureScale(texPropertyNames[index2].name);
                                    if (textureOffset.x < 0.0 || textureOffset.x + vector3.x > 1.0 || textureOffset.y < 0.0 || textureOffset.y + vector3.y > 1.0)
                                    {
                                        stringBuilder1.AppendFormat("\nMaterial {0} used by objects {1} uses texture {2} that is tiled (scale={3} offset={4}). If there is more than one texture in the atlas  then Mesh Baker will bake the tiling into the atlas. If the baked tiling is large then quality can be lost. Possible treatments:\n", current, PrintList(dictionary2[current]), texture, vector3, textureOffset);
                                        stringBuilder1.AppendFormat("  1) Use the baked tiling.\n");
                                        stringBuilder1.AppendFormat("  2) Use the Multiple Materials feature to map the material on this object/submesh to its own submesh in the combined mesh. No other materials should map to this submesh. The original material can be applied to this submesh.\n");
                                        stringBuilder1.AppendFormat("  3) Combine only meshes that use the same (or subset of) the set of textures on this mesh. The original material can be applied to the result.\n");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Debug.Log(stringBuilder1.Length != 0 ? ("====== There are possible problems with these meshes that may prevent them from combining well. TREATMENT SUGGESTIONS (copy and paste to text editor if too big) =====\n" + stringBuilder1.ToString()) : "====== No problems detected. These meshes should combine well ====\n  If there are problems with the combined meshes please report the problem to digitalOpus.ca so we can improve Mesh Baker.");
        }

        private TextureBlender FindMatchingTextureBlender(string shaderName)
        {
            for (int index = 0; index < textureBlenders.Length; ++index)
            {
                if (textureBlenders[index].DoesShaderNameMatch(shaderName))
                    return textureBlenders[index];
            }
            return (TextureBlender)null;
        }

        private void AdjustNonTextureProperties(Material mat, List<ShaderTextureProperty> texPropertyNames, List<MB_TexSet> distinctMaterialTextures, bool considerTintColor, MB2_EditorMethodsInterface editorMethods)
        {
            if ((mat== null) || texPropertyNames == null)
                return;
            if (_considerNonTextureProperties)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log(("Adjusting non texture properties using TextureBlender for shader: " + (mat.shader).name));
                resultMaterialTextureBlender.SetNonTexturePropertyValuesOnResultMaterial(mat);
            }
            else
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log("Adjusting non texture properties on result material");
                for (int index = 0; index < texPropertyNames.Count; ++index)
                {
                    string name = texPropertyNames[index].name;
                    if (name.Equals("_MainTex"))
                    {
                        if (mat.HasProperty("_Color"))
                        {
                            try
                            {
                                if (considerTintColor)
                                    mat.SetColor("_Color", Color.white);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    if (name.Equals("_BumpMap"))
                    {
                        if (mat.HasProperty("_BumpScale"))
                        {
                            try
                            {
                                mat.SetFloat("_BumpScale", 1f);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    if (name.Equals("_ParallaxMap"))
                    {
                        if (mat.HasProperty("_Parallax"))
                        {
                            try
                            {
                                mat.SetFloat("_Parallax", 0.02f);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    if (name.Equals("_OcclusionMap"))
                    {
                        if (mat.HasProperty("_OcclusionStrength"))
                        {
                            try
                            {
                                mat.SetFloat("_OcclusionStrength", 1f);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    if (name.Equals("_EmissionMap"))
                    {
                        if (mat.HasProperty("_EmissionColor"))
                        {
                            try
                            {
                                mat.SetColor("_EmissionColor", new Color(0.0f, 0.0f, 0.0f, 0.0f));
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        if (mat.HasProperty("_EmissionScaleUI"))
                        {
                            try
                            {
                                mat.SetFloat("_EmissionScaleUI", 1f);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                if (editorMethods == null)
                    return;
                editorMethods.CommitChangesToAssets();
            }
        }

        public static Color GetColorIfNoTexture(ShaderTextureProperty texProperty)
        {
            if (texProperty.isNormalMap)
                return new Color(0.5f, 0.5f, 1f);
            if (texProperty.name.Equals("_MetallicGlossMap"))
                return new Color(0.0f, 0.0f, 0.0f, 1f);
            if (texProperty.name.Equals("_ParallaxMap"))
                return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            if (texProperty.name.Equals("_OcclusionMap"))
                return new Color(1f, 1f, 1f, 1f);
            if (texProperty.name.Equals("_EmissionMap"))
                return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            if (texProperty.name.Equals("_DetailMask"))
                return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            return new Color(1f, 1f, 1f, 0.0f);
        }

        private Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
        {
            Vector3 zero = Vector3.zero;
            zero.x = (float) (c.a * 2.0 - 1.0);
            zero.y = (float) (c.g * 2.0 - 1.0);
            zero.z = Mathf.Sqrt((float)(1.0 - zero.x * zero.x - zero.y * zero.y));
            Color32 color32=Color.white;
            color32.a = 1;
            color32.r = (byte)((zero.x + 1.0) * 0.5);
            color32.g = (byte)((zero.y + 1.0) * 0.5);
            color32.b = (byte)((zero.z + 1.0) * 0.5);
            return color32;
        }

        private float GetSubmeshArea(Mesh m, int submeshIdx)
        {
            if (submeshIdx >= m.subMeshCount || submeshIdx < 0)
                return 0.0f;
            Vector3[] vertices = m.vertices;
            int[] indices = m.GetIndices(submeshIdx);
            float num = 0.0f;
            int index = 0;
            while (index < indices.Length)
            {
                Vector3 vector3_1 = vertices[indices[index]];
                Vector3 vector3_2 = vertices[indices[index + 1]];
                Vector3 vector3_3 = vertices[indices[index + 2]];
                Vector3 vector3_4 = Vector3.Cross((vector3_2- vector3_1),(vector3_3- vector3_1));
                // ISSUE: explicit reference operation
                num += (vector3_4).magnitude / 2f;
                index += 3;
            }
            return num;
        }

        private string PrintList(List<GameObject> gos)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < gos.Count; ++index)
                stringBuilder.Append(gos[index].ToString() + ",");
            return stringBuilder.ToString();
        }

        public class MeshBakerMaterialTexture
        {
            public Texture2D t;
            public float texelDensity;
            public DRect encapsulatingSamplingRect;
            public DRect matTilingRect;

            public MeshBakerMaterialTexture()
            {
            }

            public MeshBakerMaterialTexture(Texture2D tx)
            {
                t = tx;
            }

            public MeshBakerMaterialTexture(Texture2D tx, Vector2 o, Vector2 s, float texelDens)
            {
                t = tx;
                matTilingRect = new DRect(o, s);
                texelDensity = texelDens;
            }
        }

        public class MatAndTransformToMerged
        {
            public DRect obUVRectIfTilingSame = new DRect(0.0f, 0.0f, 1f, 1f);
            public DRect samplingRectMatAndUVTiling = new DRect();
            public DRect materialTiling = new DRect();
            public Material mat;
            public string objName;

            public MatAndTransformToMerged(Material m)
            {
                mat = m;
            }

            public override bool Equals(object obj)
            {
                if (obj is MatAndTransformToMerged)
                {
                    MatAndTransformToMerged transformToMerged = (MatAndTransformToMerged)obj;
                    if ((transformToMerged.mat== mat) && transformToMerged.obUVRectIfTilingSame == obUVRectIfTilingSame)
                        return true;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (mat).GetHashCode() ^ obUVRectIfTilingSame.GetHashCode() ^ samplingRectMatAndUVTiling.GetHashCode();
            }
        }

        public class SamplingRectEnclosesComparer : IComparer<MatAndTransformToMerged>
        {
            public int Compare(MatAndTransformToMerged x, MatAndTransformToMerged y)
            {
                if (x.samplingRectMatAndUVTiling.Equals(y.samplingRectMatAndUVTiling))
                    return 0;
                return x.samplingRectMatAndUVTiling.Encloses(y.samplingRectMatAndUVTiling) ? -1 : 1;
            }
        }

        public class MatsAndGOs
        {
            public List<MatAndTransformToMerged> mats;
            public List<GameObject> gos;
        }

        public class MB_TexSet
        {
            public bool allTexturesUseSameMatTiling = false;
            public Vector2 obUVoffset = new Vector2(0.0f, 0.0f);
            public Vector2 obUVscale = new Vector2(1f, 1f);
            public MeshBakerMaterialTexture[] ts;
            public MatsAndGOs matsAndGOs;
            public int idealWidth;
            public int idealHeight;

            public DRect obUVrect
            {
                get
                {
                    return new DRect(obUVoffset, obUVscale);
                }
            }

            public MB_TexSet(MeshBakerMaterialTexture[] tss, Vector2 uvOffset, Vector2 uvScale)
            {
                ts = tss;
                obUVoffset = uvOffset;
                obUVscale = uvScale;
                allTexturesUseSameMatTiling = false;
                matsAndGOs = new MatsAndGOs();
                matsAndGOs.mats = new List<MatAndTransformToMerged>();
                matsAndGOs.gos = new List<GameObject>();
            }

            public bool IsEqual(object obj, bool fixOutOfBoundsUVs, bool considerNonTextureProperties, TextureBlender resultMaterialTextureBlender)
            {
                if (!(obj is MB_TexSet))
                    return false;
                MB_TexSet mbTexSet = (MB_TexSet)obj;
                if (mbTexSet.ts.Length != ts.Length)
                    return false;
                for (int index = 0; index < ts.Length; ++index)
                {
                    if (ts[index].matTilingRect != mbTexSet.ts[index].matTilingRect || (ts[index].t!=mbTexSet.ts[index].t) || considerNonTextureProperties && resultMaterialTextureBlender != null && !resultMaterialTextureBlender.NonTexturePropertiesAreEqual(matsAndGOs.mats[0].mat, mbTexSet.matsAndGOs.mats[0].mat))
                        return false;
                }
                return (!fixOutOfBoundsUVs || obUVoffset.x == mbTexSet.obUVoffset.x && obUVoffset.y == mbTexSet.obUVoffset.y) && (!fixOutOfBoundsUVs || obUVscale.x == mbTexSet.obUVscale.x && obUVscale.y == mbTexSet.obUVscale.y);
            }

            public void CalcInitialFullSamplingRects(bool fixOutOfBoundsUVs)
            {
                DRect drect = new DRect(0.0f, 0.0f, 1f, 1f);
                if (fixOutOfBoundsUVs)
                    drect = obUVrect;
                for (int index = 0; index < ts.Length; ++index)
                {
                    if ((ts[index].t!= null))
                    {
                        DRect matTilingRect = ts[index].matTilingRect;
                        DRect r1 = !fixOutOfBoundsUVs ? new DRect(0.0, 0.0, 1.0, 1.0) : obUVrect;
                        ts[index].encapsulatingSamplingRect = MB3_UVTransformUtility.CombineTransforms(ref r1, ref matTilingRect);
                        drect = ts[index].encapsulatingSamplingRect;
                    }
                }
                for (int index = 0; index < ts.Length; ++index)
                {
                    if ((ts[index].t== null))
                        ts[index].encapsulatingSamplingRect = drect;
                }
            }

            public void CalcMatAndUVSamplingRects()
            {
                if (allTexturesUseSameMatTiling)
                {
                    DRect r2 = new DRect(0.0f, 0.0f, 1f, 1f);
                    for (int index = 0; index < ts.Length; ++index)
                    {
                        if ((ts[index].t!= null))
                            r2 = ts[index].matTilingRect;
                    }
                    for (int index = 0; index < matsAndGOs.mats.Count; ++index)
                    {
                        matsAndGOs.mats[index].materialTiling = r2;
                        matsAndGOs.mats[index].samplingRectMatAndUVTiling = MB3_UVTransformUtility.CombineTransforms(ref matsAndGOs.mats[index].obUVRectIfTilingSame, ref r2);
                    }
                }
                else
                {
                    for (int index = 0; index < matsAndGOs.mats.Count; ++index)
                    {
                        DRect r2 = new DRect(0.0f, 0.0f, 1f, 1f);
                        matsAndGOs.mats[index].materialTiling = r2;
                        matsAndGOs.mats[index].samplingRectMatAndUVTiling = MB3_UVTransformUtility.CombineTransforms(ref matsAndGOs.mats[index].obUVRectIfTilingSame, ref r2);
                    }
                }
            }

            public bool AllTexturesAreSameForMerge(MB_TexSet other, bool considerNonTextureProperties, TextureBlender resultMaterialTextureBlender)
            {
                if (other.ts.Length != ts.Length || (!other.allTexturesUseSameMatTiling || !allTexturesUseSameMatTiling))
                    return false;
                int num = -1;
                for (int index = 0; index < ts.Length; ++index)
                {
                    if ((ts[index].t!= other.ts[index].t))
                        return false;
                    if (num == -1 && (ts[index].t!= null))
                        num = index;
                    if (considerNonTextureProperties && resultMaterialTextureBlender != null && !resultMaterialTextureBlender.NonTexturePropertiesAreEqual(matsAndGOs.mats[0].mat, other.matsAndGOs.mats[0].mat))
                        return false;
                }
                if (num != -1)
                {
                    for (int index = 0; index < ts.Length; ++index)
                    {
                        if ((ts[index].t!= other.ts[index].t))
                            return false;
                    }
                }
                return true;
            }

            internal string GetDescription()
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("[GAME_OBJS=");
                for (int index = 0; index < matsAndGOs.gos.Count; ++index)
                    stringBuilder.AppendFormat("{0},", (matsAndGOs.gos[index]).name);
                stringBuilder.AppendFormat("MATS=");
                for (int index = 0; index < matsAndGOs.mats.Count; ++index)
                    stringBuilder.AppendFormat("{0},", (matsAndGOs.mats[index].mat).name);
                stringBuilder.Append("]");
                return stringBuilder.ToString();
            }

            internal string GetMatSubrectDescriptions()
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int index = 0; index < matsAndGOs.mats.Count; ++index)
                    stringBuilder.AppendFormat("\n    {0}={1},", (matsAndGOs.mats[index].mat).name, matsAndGOs.mats[index].samplingRectMatAndUVTiling);
                return stringBuilder.ToString();
            }
        }

        public class CombineTexturesIntoAtlasesCoroutineResult
        {
            public bool success = true;
            public bool isFinished = false;
        }
    }
}

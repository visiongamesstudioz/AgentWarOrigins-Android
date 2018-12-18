using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public class MB3_MeshCombinerSingle : MB3_MeshCombiner
    {
        [SerializeField]
        protected List<GameObject> objectsInCombinedMesh = new List<GameObject>();
        [SerializeField]
        private int lightmapIndex = -1;
        [SerializeField]
        private List<MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = new List<MB_DynamicGameObject>();
        private Dictionary<int, MB_DynamicGameObject> _instance2combined_map = new Dictionary<int, MB_DynamicGameObject>();
        [SerializeField]
        private Vector3[] verts = new Vector3[0];
        [SerializeField]
        private Vector3[] normals = new Vector3[0];
        [SerializeField]
        private Vector4[] tangents = new Vector4[0];
        [SerializeField]
        private Vector2[] uvs = new Vector2[0];
        [SerializeField]
        private Vector2[] uv2s = new Vector2[0];
        [SerializeField]
        private Vector2[] uv3s = new Vector2[0];
        [SerializeField]
        private Vector2[] uv4s = new Vector2[0];
        [SerializeField]
        private Color[] colors = new Color[0];
        [SerializeField]
        private Matrix4x4[] bindPoses = new Matrix4x4[0];
        [SerializeField]
        private Transform[] bones = new Transform[0];
        [SerializeField]
        internal MBBlendShape[] blendShapes = new MBBlendShape[0];
        [SerializeField]
        internal MBBlendShape[] blendShapesInCombined = new MBBlendShape[0];
        [SerializeField]
        private SerializableIntArray[] submeshTris = new SerializableIntArray[0];
        private BoneWeight[] boneWeights = new BoneWeight[0];
        private GameObject[] empty = new GameObject[0];
        private int[] emptyIDs = new int[0];
        [SerializeField]
        private Mesh _mesh;

        public override MB2_TextureBakeResults textureBakeResults
        {
            set
            {
                if (mbDynamicObjectsInCombinedMesh.Count > 0 && _textureBakeResults!=value && _textureBakeResults!=null && LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning("If Texture Bake Result is changed then objects currently in combined mesh may be invalid.");
                _textureBakeResults = value;
            }
        }

        public override MB_RenderType renderType
        {
            set
            {
                if (value == MB_RenderType.skinnedMeshRenderer && _renderType == MB_RenderType.meshRenderer && boneWeights.Length != verts.Length)
                    Debug.LogError("Can't set the render type to SkinnedMeshRenderer without clearing the mesh first. Try deleteing the CombinedMesh scene object.");
                _renderType = value;
            }
        }

        public override GameObject resultSceneObject
        {
            set
            {
                if (_resultSceneObject!=value)
                {
                    _targetRenderer = null;
                    if (_mesh!= null && _LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning("Result Scene Object was changed when this mesh baker component had a reference to a mesh. If mesh is being used by another object make sure to reset the mesh to none before baking to avoid overwriting the other mesh.");
                }
                _resultSceneObject = value;
            }
        }

        private MB_DynamicGameObject instance2Combined_MapGet(int gameObjectID)
        {
            return _instance2combined_map[gameObjectID];
        }

        private void instance2Combined_MapAdd(int gameObjectID, MB_DynamicGameObject dgo)
        {
            _instance2combined_map.Add(gameObjectID, dgo);
        }

        private void instance2Combined_MapRemove(int gameObjectID)
        {
            _instance2combined_map.Remove(gameObjectID);
        }

        private bool instance2Combined_MapTryGetValue(int gameObjectID, out MB_DynamicGameObject dgo)
        {
            return _instance2combined_map.TryGetValue(gameObjectID, out dgo);
        }

        private int instance2Combined_MapCount()
        {
            return _instance2combined_map.Count;
        }

        private void instance2Combined_MapClear()
        {
            _instance2combined_map.Clear();
        }

        private bool instance2Combined_MapContainsKey(int gameObjectID)
        {
            return _instance2combined_map.ContainsKey(gameObjectID);
        }

        public override int GetNumObjectsInCombined()
        {
            return mbDynamicObjectsInCombinedMesh.Count;
        }

        public override List<GameObject> GetObjectsInCombined()
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            gameObjectList.AddRange(objectsInCombinedMesh);
            return gameObjectList;
        }

        public Mesh GetMesh()
        {
            if (_mesh== null)
                _mesh = new Mesh();
            return _mesh;
        }

        public Transform[] GetBones()
        {
            return bones;
        }

        public override int GetLightmapIndex()
        {
            if (lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout || lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
                return lightmapIndex;
            return -1;
        }

        public override int GetNumVerticesFor(GameObject go)
        {
            return GetNumVerticesFor((go).GetInstanceID());
        }

        public override int GetNumVerticesFor(int instanceID)
        {
            MB_DynamicGameObject dgo;
            if (instance2Combined_MapTryGetValue(instanceID, out dgo))
                return dgo.numVerts;
            return -1;
        }

        public override Dictionary<MBBlendShapeKey, MBBlendShapeValue> BuildSourceBlendShapeToCombinedIndexMap()
        {
            Dictionary<MBBlendShapeKey, MBBlendShapeValue> dictionary = new Dictionary<MBBlendShapeKey, MBBlendShapeValue>();
            for (int index = 0; index < blendShapesInCombined.Length; ++index)
                dictionary.Add(new MBBlendShapeKey(blendShapesInCombined[index].gameObjectID, blendShapesInCombined[index].indexInSource), new MBBlendShapeValue()
                {
                    combinedMeshGameObject = _targetRenderer.gameObject,
                    blendShapeIndex = index
                });
            return dictionary;
        }

        private void _initialize(int numResultMats)
        {
            if (mbDynamicObjectsInCombinedMesh.Count == 0)
                lightmapIndex = -1;
            if (_mesh== null)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("_initialize Creating new Mesh");
                _mesh = GetMesh();
            }
            if (instance2Combined_MapCount() != mbDynamicObjectsInCombinedMesh.Count)
            {
                instance2Combined_MapClear();
                for (int index = 0; index < mbDynamicObjectsInCombinedMesh.Count; ++index)
                {
                    if (mbDynamicObjectsInCombinedMesh[index] != null)
                        instance2Combined_MapAdd(mbDynamicObjectsInCombinedMesh[index].instanceID, mbDynamicObjectsInCombinedMesh[index]);
                }
                boneWeights = _mesh.boneWeights;
            }
            if (objectsInCombinedMesh.Count == 0 && submeshTris.Length != numResultMats)
            {
                submeshTris = new SerializableIntArray[numResultMats];
                for (int index = 0; index < submeshTris.Length; ++index)
                    submeshTris[index] = new SerializableIntArray(0);
            }
            if (mbDynamicObjectsInCombinedMesh.Count > 0 && mbDynamicObjectsInCombinedMesh[0].indexesOfBonesUsed.Length == 0 && renderType == MB_RenderType.skinnedMeshRenderer && boneWeights.Length > 0U)
            {
                for (int index = 0; index < mbDynamicObjectsInCombinedMesh.Count; ++index)
                {
                    MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index];
                    HashSet<int> idxsOfBonesUsed = new HashSet<int>();
                    for (int vertIdx = dynamicGameObject.vertIdx; vertIdx < dynamicGameObject.vertIdx + dynamicGameObject.numVerts; ++vertIdx)
                    {
                        if ((boneWeights[vertIdx]).weight0 > 0.0)
                        {
                            idxsOfBonesUsed.Add((boneWeights[vertIdx]).boneIndex0);
                        }
                        if (boneWeights[vertIdx].weight1> 0.0)
                        {
                            idxsOfBonesUsed.Add(boneWeights[vertIdx].boneIndex1);
                        }
                        if ((boneWeights[vertIdx]).weight2 > 0.0)
                        {
                            idxsOfBonesUsed.Add(boneWeights[vertIdx].boneIndex2);
                        }
                        if (boneWeights[vertIdx].weight3 > 0.0)
                        {
                            idxsOfBonesUsed.Add(@boneWeights[vertIdx].boneIndex3);
                        }
                    }
                    dynamicGameObject.indexesOfBonesUsed = new int[idxsOfBonesUsed.Count];
                    idxsOfBonesUsed.CopyTo(dynamicGameObject.indexesOfBonesUsed);
                }
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    Debug.Log("Baker used old systems that duplicated bones. Upgrading to new system by building indexesOfBonesUsed");
            }
            if (LOG_LEVEL < MB2_LogLevel.trace)
                return;
            Debug.Log(string.Format("_initialize numObjsInCombined={0}", mbDynamicObjectsInCombinedMesh.Count));
        }

        private bool _collectMaterialTriangles(Mesh m, MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map)
        {
            int length = m.subMeshCount;
            if (sharedMaterials.Length < length)
                length = sharedMaterials.Length;
            dgo._tmpSubmeshTris = new SerializableIntArray[length];
            dgo.targetSubmeshIdxs = new int[length];
            for (int index = 0; index < length; ++index)
            {
                if (_textureBakeResults.doMultiMaterial)
                {
                    if (!sourceMats2submeshIdx_map.Contains(sharedMaterials[index]))
                    {
                        Debug.LogError(("Object " + dgo.name + " has a material that was not found in the result materials maping. " + sharedMaterials[index]));
                        return false;
                    }
                    dgo.targetSubmeshIdxs[index] = (int)sourceMats2submeshIdx_map[sharedMaterials[index]];
                }
                else
                    dgo.targetSubmeshIdxs[index] = 0;
                dgo._tmpSubmeshTris[index] = new SerializableIntArray();
                dgo._tmpSubmeshTris[index].data = m.GetTriangles(index);
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Collecting triangles for: " + dgo.name + " submesh:" + index + " maps to submesh:" + dgo.targetSubmeshIdxs[index] + " added:" + dgo._tmpSubmeshTris[index].data.Length, LOG_LEVEL);
            }
            return true;
        }

        private bool _collectOutOfBoundsUVRects2(Mesh m, MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResults, MeshChannelsCache meshChannelCache)
        {
            if (_textureBakeResults== null)
            {
                Debug.LogError("Need to bake textures into combined material");
                return false;
            }
            MB_Utility.MeshAnalysisResult[] meshAnalysisResultArray1;
            if (meshAnalysisResults.TryGetValue((m).GetInstanceID(), out meshAnalysisResultArray1))
            {
                dgo.obUVRects = new Rect[sharedMaterials.Length];
                for (int index = 0; index < dgo.obUVRects.Length; ++index)
                    dgo.obUVRects[index] = meshAnalysisResultArray1[index].uvRect;
            }
            else
            {
                int subMeshCount = m.subMeshCount;
                int length = subMeshCount;
                if (sharedMaterials.Length < subMeshCount)
                    length = sharedMaterials.Length;
                dgo.obUVRects = new Rect[length];
                MB_Utility.MeshAnalysisResult[] meshAnalysisResultArray2 = new MB_Utility.MeshAnalysisResult[subMeshCount];
                for (int submeshIndex = 0; submeshIndex < subMeshCount; ++submeshIndex)
                {
                    if (_textureBakeResults.resultMaterials[dgo.targetSubmeshIdxs[submeshIndex]].considerMeshUVs)
                    {
                        MB_Utility.hasOutOfBoundsUVs(meshChannelCache.GetUv0Raw(m), m, ref meshAnalysisResultArray2[submeshIndex], submeshIndex);
                        Rect uvRect = meshAnalysisResultArray2[submeshIndex].uvRect;
                        if (submeshIndex < length)
                            dgo.obUVRects[submeshIndex] = uvRect;
                    }
                }
                meshAnalysisResults.Add((m).GetInstanceID(), meshAnalysisResultArray2);
            }
            return true;
        }

        private bool _validateTextureBakeResults()
        {
            if (_textureBakeResults==null)
            {
                Debug.LogError("Texture Bake Results is null. Can't combine meshes.");
                return false;
            }
            if (_textureBakeResults.materialsAndUVRects == null || _textureBakeResults.materialsAndUVRects.Length == 0)
            {
                Debug.LogError("Texture Bake Results has no materials in material to sourceUVRect map. Try baking materials. Can't combine meshes.");
                return false;
            }
            if (_textureBakeResults.resultMaterials == null || _textureBakeResults.resultMaterials.Length == 0)
            {
                if (_textureBakeResults.materialsAndUVRects != null && _textureBakeResults.materialsAndUVRects.Length != 0 && !_textureBakeResults.doMultiMaterial && _textureBakeResults.resultMaterial!=null)
                {
                    MB_MultiMaterial[] mbMultiMaterialArray = _textureBakeResults.resultMaterials = new MB_MultiMaterial[1];
                    mbMultiMaterialArray[0] = new MB_MultiMaterial();
                    mbMultiMaterialArray[0].combinedMaterial = _textureBakeResults.resultMaterial;
                    mbMultiMaterialArray[0].considerMeshUVs = _textureBakeResults.fixOutOfBoundsUVs;
                    List<Material> materialList = mbMultiMaterialArray[0].sourceMaterials = new List<Material>();
                    for (int index = 0; index < _textureBakeResults.materialsAndUVRects.Length; ++index)
                    {
                        if (!materialList.Contains(_textureBakeResults.materialsAndUVRects[index].material))
                            materialList.Add(_textureBakeResults.materialsAndUVRects[index].material);
                    }
                }
                else
                {
                    Debug.LogError("Texture Bake Results has no result materials. Try baking materials. Can't combine meshes.");
                    return false;
                }
            }
            return true;
        }

        private bool _validateMeshFlags()
        {
            if (mbDynamicObjectsInCombinedMesh.Count > 0 && (!_doNorm && doNorm || !_doTan && doTan || (!_doCol && doCol || !_doUV && doUV) || !_doUV3 && doUV3 || !_doUV4 && doUV4))
            {
                Debug.LogError("The channels have changed. There are already objects in the combined mesh that were added with a different set of channels.");
                return false;
            }
            _doNorm = doNorm;
            _doTan = doTan;
            _doCol = doCol;
            _doUV = doUV;
            _doUV3 = doUV3;
            _doUV4 = doUV4;
            return true;
        }

        private bool _showHide(GameObject[] goToShow, GameObject[] goToHide)
        {
            if (goToShow == null)
                goToShow = empty;
            if (goToHide == null)
                goToHide = empty;
            _initialize(_textureBakeResults.resultMaterials.Length);
            for (int index = 0; index < goToHide.Length; ++index)
            {
                if (!instance2Combined_MapContainsKey((goToHide[index]).GetInstanceID()))
                {
                    if (LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning(("Trying to hide an object " + goToHide[index] + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?"));
                    return false;
                }
            }
            for (int index = 0; index < goToShow.Length; ++index)
            {
                if (!instance2Combined_MapContainsKey((goToShow[index]).GetInstanceID()))
                {
                    if (LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning(("Trying to show an object " + goToShow[index] + " that is not in combined mesh. Did you initially bake with 'clear buffers after bake' enabled?"));
                    return false;
                }
            }
            for (int index = 0; index < goToHide.Length; ++index)
                _instance2combined_map[(goToHide[index]).GetInstanceID()].show = false;
            for (int index = 0; index < goToShow.Length; ++index)
                _instance2combined_map[(goToShow[index]).GetInstanceID()].show = true;
            return true;
        }

        private bool _addToCombined(GameObject[] goToAdd, int[] goToDelete, bool disableRendererInSource)
        {
            if (!_validateTextureBakeResults() || !_validateMeshFlags() || !ValidateTargRendererAndMeshAndResultSceneObj())
                return false;
            if (outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace && renderType == MB_RenderType.skinnedMeshRenderer)
            {
                if (_targetRenderer==null || !(_targetRenderer is SkinnedMeshRenderer))
                {
                    Debug.LogError("Target renderer must be set and must be a SkinnedMeshRenderer");
                    return false;
                }
                SkinnedMeshRenderer smr = (SkinnedMeshRenderer)targetRenderer;
                if (smr.sharedMesh != _mesh)
                {
                    Debug.LogError("The combined mesh was not assigned to the targetRenderer. Try using buildSceneMeshObject to set up the combined mesh correctly");
                }
            }
            if (_doBlendShapes && renderType != MB_RenderType.skinnedMeshRenderer)
            {
                Debug.LogError("If doBlendShapes is set then RenderType must be skinnedMeshRenderer.");
                return false;
            }
            GameObject[] _goToAdd = goToAdd != null ? (GameObject[])goToAdd.Clone() : empty;
            int[] array = goToDelete != null ? (int[])goToDelete.Clone() : emptyIDs;
            if (_mesh == null) DestroyMesh(); //cleanup maps and arrays
            MB2_TextureBakeResults.Material2AtlasRectangleMapper atlasRectangleMapper = new MB2_TextureBakeResults.Material2AtlasRectangleMapper(textureBakeResults);
            int length1 = _textureBakeResults.resultMaterials.Length;
            _initialize(length1);
            if (submeshTris.Length != length1)
            {
                Debug.LogError(("The number of submeshes " + submeshTris.Length + " in the combined mesh was not equal to the number of result materials " + length1 + " in the Texture Bake Result"));
                return false;
            }
            if (_mesh.vertexCount > 0 && _instance2combined_map.Count == 0)
                Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                MB2_Log.LogDebug("==== Calling _addToCombined objs adding:" + _goToAdd.Length + " objs deleting:" + array.Length + " fixOutOfBounds:" + textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs().ToString() + " doMultiMaterial:" + textureBakeResults.doMultiMaterial.ToString() + " disableRenderersInSource:" + disableRendererInSource.ToString(), LOG_LEVEL);
            if (_textureBakeResults.resultMaterials == null || _textureBakeResults.resultMaterials.Length == 0)
            {
                _textureBakeResults.resultMaterials = new MB_MultiMaterial[1];
                _textureBakeResults.resultMaterials[0] = new MB_MultiMaterial();
                _textureBakeResults.resultMaterials[0].combinedMaterial = _textureBakeResults.resultMaterial;
                _textureBakeResults.resultMaterials[0].considerMeshUVs = false;
                List<Material> materialList = _textureBakeResults.resultMaterials[0].sourceMaterials = new List<Material>();
                for (int index = 0; index < _textureBakeResults.materialsAndUVRects.Length; ++index)
                    materialList.Add(_textureBakeResults.materialsAndUVRects[index].material);
            }
            OrderedDictionary sourceMats2submeshIdx_map = new OrderedDictionary();
            for (int index1 = 0; index1 < length1; ++index1)
            {
                MB_MultiMaterial resultMaterial = _textureBakeResults.resultMaterials[index1];
                for (int index2 = 0; index2 < resultMaterial.sourceMaterials.Count; ++index2)
                {
                    if (resultMaterial.sourceMaterials[index2]==null)
                    {
                        Debug.LogError(("Found null material in source materials for combined mesh materials " + index1));
                        return false;
                    }
                    if (!sourceMats2submeshIdx_map.Contains(resultMaterial.sourceMaterials[index2]))
                        sourceMats2submeshIdx_map.Add(resultMaterial.sourceMaterials[index2], index1);
                }
            }
            int totalDeleteVerts = 0;
            int[] numArray1 = new int[length1];
            int num1 = 0;
            List<MB_DynamicGameObject>[] dynamicGameObjectListArray = null;
            HashSet<int> intSet = new HashSet<int>();
            HashSet<BoneAndBindpose> bonesToAdd = new HashSet<BoneAndBindpose>();
            if (renderType == MB_RenderType.skinnedMeshRenderer && (uint)array.Length > 0U)
                dynamicGameObjectListArray = _buildBoneIdx2dgoMap();
            for (int index1 = 0; index1 < array.Length; ++index1)
            {
                MB_DynamicGameObject dgo;
                if (instance2Combined_MapTryGetValue(array[index1], out dgo))
                {
                    totalDeleteVerts += dgo.numVerts;
                    num1 += dgo.numBlendShapes;
                    if (renderType == MB_RenderType.skinnedMeshRenderer)
                    {
                        for (int index2 = 0; index2 < dgo.indexesOfBonesUsed.Length; ++index2)
                        {
                            if (dynamicGameObjectListArray[dgo.indexesOfBonesUsed[index2]].Contains(dgo))
                            {
                                dynamicGameObjectListArray[dgo.indexesOfBonesUsed[index2]].Remove(dgo);
                                if (dynamicGameObjectListArray[dgo.indexesOfBonesUsed[index2]].Count == 0)
                                    intSet.Add(dgo.indexesOfBonesUsed[index2]);
                            }
                        }
                    }
                    for (int index2 = 0; index2 < dgo.submeshNumTris.Length; ++index2)
                        numArray1[index2] += dgo.submeshNumTris[index2];
                }
                else if (LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning("Trying to delete an object that is not in combined mesh");
            }
            List<MB_DynamicGameObject> dynamicGameObjectList = new List<MB_DynamicGameObject>();
            Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
            MeshChannelsCache meshChannelsCache = new MeshChannelsCache(this);
            int num2 = 0;
            int[] numArray2 = new int[length1];
            int num3 = 0;
            Dictionary<Transform, int> bone2idx = new Dictionary<Transform, int>();
            for (int index = 0; index < bones.Length; ++index)
                bone2idx.Add(bones[index], index);
            for (int i = 0; i < _goToAdd.Length; i++)
            {
                if (!instance2Combined_MapContainsKey((_goToAdd[i]).GetInstanceID()) || Array.FindIndex<int>(array, o => o == (_goToAdd[i]).GetInstanceID()) != -1)
                {
                    MB_DynamicGameObject dgo = new MB_DynamicGameObject();
                    GameObject go = _goToAdd[i];
                    Material[] goMaterials = MB_Utility.GetGOMaterials(go);
                    if (LOG_LEVEL >= MB2_LogLevel.trace)
                        Debug.Log(string.Format("Getting {0} shared materials for {1}", goMaterials.Length, go));
                    if (goMaterials == null)
                    {
                        Debug.LogError(("Object " + (go).name + " does not have a Renderer"));
                        _goToAdd[i] = null;
                        return false;
                    }
                    Mesh mesh = MB_Utility.GetMesh(go);
                    if (mesh== null)
                    {
                        Debug.LogError(("Object " + (go).name + " MeshFilter or SkinedMeshRenderer had no mesh"));
                        _goToAdd[i] = null;
                        return false;
                    }
                    if (MBVersion.IsRunningAndMeshNotReadWriteable(mesh))
                    {
                        Debug.LogError(("Object " + (go).name + " Mesh Importer has read/write flag set to 'false'. This needs to be set to 'true' in order to read data from this mesh."));
                        _goToAdd[i] = null;
                        return false;
                    }
                    Rect[] rectArray1 = new Rect[goMaterials.Length];
                    Rect[] rectArray2 = new Rect[goMaterials.Length];
                    Rect[] rectArray3 = new Rect[goMaterials.Length];
                    string errorMsg = "";
                    for (int submeshIdx = 0; submeshIdx < goMaterials.Length; ++submeshIdx)
                    {
                        object obj = sourceMats2submeshIdx_map[goMaterials[submeshIdx]];
                        if (obj == null)
                        {
                            Debug.LogError(("Source object " + (go).name + " used a material " + goMaterials[submeshIdx] + " that was not in the baked materials."));
                            return false;
                        }
                        int idxInResultMats = (int)obj;
                        if (!atlasRectangleMapper.TryMapMaterialToUVRect(goMaterials[submeshIdx], mesh, submeshIdx, idxInResultMats, meshChannelsCache, dictionary, out rectArray1[submeshIdx], out rectArray2[submeshIdx], out rectArray3[submeshIdx], ref errorMsg, LOG_LEVEL))
                        {
                            Debug.LogError(errorMsg);
                            _goToAdd[i] = null;
                            return false;
                        }
                    }
                    if (_goToAdd[i]!= null)
                    {
                        dynamicGameObjectList.Add(dgo);
                        dgo.name = string.Format("{0} {1}", (_goToAdd[i]).ToString(), (_goToAdd[i]).GetInstanceID());
                        dgo.instanceID = (_goToAdd[i]).GetInstanceID();
                        dgo.uvRects = rectArray1;
                        dgo.encapsulatingRect = rectArray2;
                        dgo.sourceMaterialTiling = rectArray3;
                        dgo.numVerts = mesh.vertexCount;
                        if (_doBlendShapes)
                            dgo.numBlendShapes = mesh.blendShapeCount;
                        Renderer renderer = MB_Utility.GetRenderer(go);
                        if (renderType == MB_RenderType.skinnedMeshRenderer)
                            _CollectBonesToAddForDGO(dgo, bone2idx, intSet, bonesToAdd, renderer, meshChannelsCache);
                        if (lightmapIndex == -1)
                            lightmapIndex = renderer.lightmapIndex;
                        if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
                        {
                            if (lightmapIndex != renderer.lightmapIndex && LOG_LEVEL >= MB2_LogLevel.warn)
                                Debug.LogWarning(("Object " + (go).name + " has a different lightmap index. Lightmapping will not work."));
                            if (!MBVersion.GetActive(go) && LOG_LEVEL >= MB2_LogLevel.warn)
                                Debug.LogWarning(("Object " + (go).name + " is inactive. Can only get lightmap index of active objects."));
                            if (renderer.lightmapIndex == -1 && LOG_LEVEL >= MB2_LogLevel.warn)
                                Debug.LogWarning(("Object " + (go).name + " does not have an index to a lightmap."));
                        }
                        dgo.lightmapIndex = renderer.lightmapIndex;
                        dgo.lightmapTilingOffset = MBVersion.GetLightmapTilingOffset(renderer);
                        if (!_collectMaterialTriangles(mesh, dgo, goMaterials, sourceMats2submeshIdx_map))
                            return false;
                        MB_DynamicGameObject dynamicGameObject = dgo;
                        Bounds bounds = renderer.bounds;
                        // ISSUE: explicit reference operation
                        Vector3 size = bounds.size;
                        dynamicGameObject.meshSize = size;
                        dgo.submeshNumTris = new int[length1];
                        dgo.submeshTriIdxs = new int[length1];
                        if (textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs() && !_collectOutOfBoundsUVRects2(mesh, dgo, goMaterials, sourceMats2submeshIdx_map, dictionary, meshChannelsCache))
                            return false;
                        num2 += dgo.numVerts;
                        num3 += dgo.numBlendShapes;
                        for (int index = 0; index < dgo._tmpSubmeshTris.Length; ++index)
                            numArray2[dgo.targetSubmeshIdxs[index]] += dgo._tmpSubmeshTris[index].data.Length;
                        dgo.invertTriangles = IsMirrored(go.transform.localToWorldMatrix);
                    }
                }
                else
                {
                    if (LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning(("Object " + (_goToAdd[i]).name + " has already been added"));
                    _goToAdd[i] = null;
                }
            }
            for (int index = 0; index < _goToAdd.Length; ++index)
            {
                if ((_goToAdd[index]!= null) & disableRendererInSource)
                {
                    MB_Utility.DisableRendererInSource(_goToAdd[index]);
                    if (LOG_LEVEL == MB2_LogLevel.trace)
                        Debug.Log(("Disabling renderer on " + (_goToAdd[index]).name + " id=" + (_goToAdd[index]).GetInstanceID()));
                }
            }
            int length2 = verts.Length + num2 - totalDeleteVerts;
            int length3 = bindPoses.Length + bonesToAdd.Count - intSet.Count;
            int[] numArray3 = new int[length1];
            int length4 = blendShapes.Length + num3 - num1;
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("Verts adding:" + num2 + " deleting:" + totalDeleteVerts + " submeshes:" + numArray3.Length + " bones:" + length3 + " blendShapes:" + length4));
            for (int index = 0; index < numArray3.Length; ++index)
            {
                numArray3[index] = submeshTris[index].data.Length + numArray2[index] - numArray1[index];
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("    submesh :" + index + " already contains:" + submeshTris[index].data.Length + " tris to be Added:" + numArray2[index] + " tris to be Deleted:" + numArray1[index]);
            }
            if (length2 > 65534)
            {
                Debug.LogError("Cannot add objects. Resulting mesh will have more than 64k vertices. Try using a Multi-MeshBaker component. This will split the combined mesh into several meshes. You don't have to re-configure the MB2_TextureBaker. Just remove the MB2_MeshBaker component and add a MB2_MultiMeshBaker component.");
                return false;
            }
            Vector3[] vector3Array1 = null;
            Vector4[] vector4Array1 = null;
            Vector2[] vector2Array1 = null;
            Vector2[] vector2Array2 = null;
            Vector2[] vector2Array3 = null;
            Vector2[] vector2Array4 = null;
            Color[] colorArray = null;
            MBBlendShape[] mbBlendShapeArray = null;
            Vector3[] vector3Array2 = new Vector3[length2];
            if (_doNorm)
                vector3Array1 = new Vector3[length2];
            if (_doTan)
                vector4Array1 = new Vector4[length2];
            if (_doUV)
                vector2Array1 = new Vector2[length2];
            if (_doUV3)
                vector2Array3 = new Vector2[length2];
            if (_doUV4)
                vector2Array4 = new Vector2[length2];
            if (doUV2())
                vector2Array2 = new Vector2[length2];
            if (_doCol)
                colorArray = new Color[length2];
            if (_doBlendShapes)
                mbBlendShapeArray = new MBBlendShape[length4];
            BoneWeight[] nboneWeights = new BoneWeight[length2];
            Matrix4x4[] nbindPoses = new Matrix4x4[length3];
            Transform[] nbones = new Transform[length3];
            SerializableIntArray[] serializableIntArrayArray = new SerializableIntArray[length1];
            for (int index = 0; index < serializableIntArrayArray.Length; ++index)
                serializableIntArrayArray[index] = new SerializableIntArray(numArray3[index]);
            for (int index = 0; index < array.Length; ++index)
            {
                MB_DynamicGameObject dgo = null;
                if (instance2Combined_MapTryGetValue(array[index], out dgo))
                    dgo._beingDeleted = true;
            }
            mbDynamicObjectsInCombinedMesh.Sort();
            int destinationIndex1 = 0;
            int destinationIndex2 = 0;
            int[] numArray4 = new int[length1];
            int num4 = 0;
            for (int index1 = 0; index1 < mbDynamicObjectsInCombinedMesh.Count; ++index1)
            {
                MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index1];
                if (!dynamicGameObject._beingDeleted)
                {
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("Copying obj in combined arrays idx:" + index1, LOG_LEVEL);
                    Array.Copy(verts, dynamicGameObject.vertIdx, vector3Array2, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doNorm)
                        Array.Copy(normals, dynamicGameObject.vertIdx, vector3Array1, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doTan)
                        Array.Copy(tangents, dynamicGameObject.vertIdx, vector4Array1, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doUV)
                        Array.Copy(uvs, dynamicGameObject.vertIdx, vector2Array1, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doUV3)
                        Array.Copy(uv3s, dynamicGameObject.vertIdx, vector2Array3, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doUV4)
                        Array.Copy(uv4s, dynamicGameObject.vertIdx, vector2Array4, destinationIndex1, dynamicGameObject.numVerts);
                    if (doUV2())
                        Array.Copy(uv2s, dynamicGameObject.vertIdx, vector2Array2, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doCol)
                        Array.Copy(colors, dynamicGameObject.vertIdx, colorArray, destinationIndex1, dynamicGameObject.numVerts);
                    if (_doBlendShapes)
                        Array.Copy(blendShapes, dynamicGameObject.blendShapeIdx, mbBlendShapeArray, destinationIndex2, dynamicGameObject.numBlendShapes);
                    if (renderType == MB_RenderType.skinnedMeshRenderer)
                        Array.Copy(boneWeights, dynamicGameObject.vertIdx, nboneWeights, destinationIndex1, dynamicGameObject.numVerts);
                    for (int index2 = 0; index2 < length1; ++index2)
                    {
                        int[] data = submeshTris[index2].data;
                        int sourceIndex = dynamicGameObject.submeshTriIdxs[index2];
                        int submeshNumTri = dynamicGameObject.submeshNumTris[index2];
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                            MB2_Log.LogDebug("    Adjusting submesh triangles submesh:" + index2 + " startIdx:" + sourceIndex + " num:" + submeshNumTri + " nsubmeshTris:" + serializableIntArrayArray.Length + " targSubmeshTidx:" + numArray4.Length, LOG_LEVEL);
                        for (int index3 = sourceIndex; index3 < sourceIndex + submeshNumTri; ++index3)
                            data[index3] = data[index3] - num4;
                        Array.Copy(data, sourceIndex, serializableIntArrayArray[index2].data, numArray4[index2], submeshNumTri);
                    }
                    dynamicGameObject.vertIdx = destinationIndex1;
                    dynamicGameObject.blendShapeIdx = destinationIndex2;
                    for (int index2 = 0; index2 < numArray4.Length; ++index2)
                    {
                        dynamicGameObject.submeshTriIdxs[index2] = numArray4[index2];
                        numArray4[index2] += dynamicGameObject.submeshNumTris[index2];
                    }
                    destinationIndex2 += dynamicGameObject.numBlendShapes;
                    destinationIndex1 += dynamicGameObject.numVerts;
                }
                else
                {
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("Not copying obj: " + index1, LOG_LEVEL);
                    num4 += dynamicGameObject.numVerts;
                }
            }
            if (renderType == MB_RenderType.skinnedMeshRenderer)
                _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(intSet, bonesToAdd, nbones, nbindPoses, nboneWeights, totalDeleteVerts);
            for (int index = mbDynamicObjectsInCombinedMesh.Count - 1; index >= 0; --index)
            {
                if (mbDynamicObjectsInCombinedMesh[index]._beingDeleted)
                {
                    instance2Combined_MapRemove(mbDynamicObjectsInCombinedMesh[index].instanceID);
                    objectsInCombinedMesh.RemoveAt(index);
                    mbDynamicObjectsInCombinedMesh.RemoveAt(index);
                }
            }
            verts = vector3Array2;
            if (_doNorm)
                normals = vector3Array1;
            if (_doTan)
                tangents = vector4Array1;
            if (_doUV)
                uvs = vector2Array1;
            if (_doUV3)
                uv3s = vector2Array3;
            if (_doUV4)
                uv4s = vector2Array4;
            if (doUV2())
                uv2s = vector2Array2;
            if (_doCol)
                colors = colorArray;
            if (_doBlendShapes)
                blendShapes = mbBlendShapeArray;
            if (renderType == MB_RenderType.skinnedMeshRenderer)
                boneWeights = nboneWeights;
            int num5 = bones.Length - intSet.Count;
            bindPoses = nbindPoses;
            bones = nbones;
            submeshTris = serializableIntArrayArray;
            int num6 = 0;
            foreach (BoneAndBindpose boneAndBindpose in bonesToAdd)
            {
                nbones[num5 + num6] = boneAndBindpose.bone;
                nbindPoses[num5 + num6] = boneAndBindpose.bindPose;
                ++num6;
            }
            for (int index1 = 0; index1 < dynamicGameObjectList.Count; ++index1)
            {
                MB_DynamicGameObject dgo = dynamicGameObjectList[index1];
                GameObject go = _goToAdd[index1];
                int vertsIdx = destinationIndex1;
                int index2 = destinationIndex2;
                Mesh mesh = MB_Utility.GetMesh(go);
                Matrix4x4 l2wMat = go.transform.localToWorldMatrix;
                Matrix4x4 matrix4x4 = l2wMat;
                //same as l2w with translation removed
                Matrix4x4 l2wRotScale = l2wMat;
                l2wRotScale[0, 3] = l2wRotScale[1, 3] = l2wRotScale[2, 3] = 0f;
                Vector3[] nverts = meshChannelsCache.GetVertices(mesh);
                Vector3[] nnorms = null;
                Vector4[] ntangs = null;
                if (_doNorm)
                    nnorms = meshChannelsCache.GetNormals(mesh);
                if (_doTan)
                    ntangs = meshChannelsCache.GetTangents(mesh);
                if (renderType != MB_RenderType.skinnedMeshRenderer)
                {
                    for (int j = 0; j < nverts.Length; ++j)
                    {
                        int vIdx = vertsIdx + j;
                        // ISSUE: explicit reference operation
                        verts[vertsIdx + j] = l2wMat.MultiplyPoint3x4(nverts[j]);
                        if (_doNorm)
                        {
                            normals[vIdx] = l2wRotScale.MultiplyPoint3x4(nnorms[j]);
                            normals[vIdx] = normals[vIdx].normalized;
                        }
                        if (_doTan)
                        {
                            float w = ntangs[j].w; //need to preserve the w value
                            Vector3 tn = l2wRotScale.MultiplyPoint3x4(ntangs[j]);
                            tn.Normalize();
                            tangents[vIdx] = tn;
                            tangents[vIdx].w = w;
                        }
                    }
                }
                else
                {
                    if (_doNorm)
                        nnorms.CopyTo(normals, vertsIdx);
                    if (_doTan)
                        ntangs.CopyTo(tangents, vertsIdx);
                    nverts.CopyTo(verts, vertsIdx);
                }
                int subMeshCount = mesh.subMeshCount;
                if (dgo.uvRects.Length < subMeshCount)
                {
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("Mesh " + dgo.name + " has more submeshes than materials");
                    int length5 = dgo.uvRects.Length;
                }
                else if (dgo.uvRects.Length > subMeshCount && LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning(("Mesh " + dgo.name + " has fewer submeshes than materials"));
                if (_doUV)
                    _copyAndAdjustUVsFromMesh(dgo, mesh, vertsIdx, meshChannelsCache);
                if (doUV2())
                    _copyAndAdjustUV2FromMesh(dgo, mesh, vertsIdx, meshChannelsCache);
                if (_doUV3)
                    meshChannelsCache.GetUv3(mesh).CopyTo(uv3s, vertsIdx);
                if (_doUV4)
                    meshChannelsCache.GetUv4(mesh).CopyTo(uv4s, vertsIdx);
                if (_doCol)
                    meshChannelsCache.GetColors(mesh).CopyTo(colors, vertsIdx);
                if (_doBlendShapes)
                {
                    mbBlendShapeArray = meshChannelsCache.GetBlendShapes(mesh, dgo.instanceID);
                    mbBlendShapeArray.CopyTo(blendShapes, index2);
                }
                if (renderType == MB_RenderType.skinnedMeshRenderer)
                {
                    Renderer renderer = MB_Utility.GetRenderer(go);
                    _AddBonesToNewBonesArrayAndAdjustBWIndexes(dgo, renderer, vertsIdx, nbones, nboneWeights, meshChannelsCache);
                }
                for (int index3 = 0; index3 < numArray4.Length; ++index3)
                    dgo.submeshTriIdxs[index3] = numArray4[index3];
                for (int index3 = 0; index3 < dgo._tmpSubmeshTris.Length; ++index3)
                {
                    int[] data = dgo._tmpSubmeshTris[index3].data;
                    for (int index4 = 0; index4 < data.Length; ++index4)
                        data[index4] = data[index4] + vertsIdx;
                    if (dgo.invertTriangles)
                    {
                        int index4 = 0;
                        while (index4 < data.Length)
                        {
                            int num16 = data[index4];
                            data[index4] = data[index4 + 1];
                            data[index4 + 1] = num16;
                            index4 += 3;
                        }
                    }
                    int index5 = dgo.targetSubmeshIdxs[index3];
                    data.CopyTo(submeshTris[index5].data, numArray4[index5]);
                    dgo.submeshNumTris[index5] += data.Length;
                    numArray4[index5] += data.Length;
                }
                dgo.vertIdx = destinationIndex1;
                dgo.blendShapeIdx = destinationIndex2;
                instance2Combined_MapAdd((go).GetInstanceID(), dgo);
                objectsInCombinedMesh.Add(go);
                mbDynamicObjectsInCombinedMesh.Add(dgo);
                destinationIndex1 += nverts.Length;
                if (_doBlendShapes)
                    destinationIndex2 += mbBlendShapeArray.Length;
                for (int index3 = 0; index3 < dgo._tmpSubmeshTris.Length; ++index3)
                    dgo._tmpSubmeshTris[index3] = null;
                dgo._tmpSubmeshTris = null;
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Added to combined:" + dgo.name + " verts:" + nverts.Length + " bindPoses:" + nbindPoses.Length, LOG_LEVEL);
            }
            if (lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects)
                _copyUV2unchangedToSeparateRects();
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                MB2_Log.LogDebug("===== _addToCombined completed. Verts in buffer: " + verts.Length, LOG_LEVEL);
            return true;
        }

        private void _copyAndAdjustUVsFromMesh(MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx, MeshChannelsCache meshChannelsCache)
        {
            Vector2[] nuvs = meshChannelsCache.GetUv0Raw(mesh);
            bool needToModfyUVs = true;
            if (!_textureBakeResults.DoAnyResultMatsUseConsiderMeshUVs())
            {
                Rect ident = new Rect(0.0f, 0.0f, 1f, 1f);

                bool allAreIdent = true;
                for (int index = 0; index < _textureBakeResults.materialsAndUVRects.Length; ++index)
                {
                    if (_textureBakeResults.materialsAndUVRects[index].atlasRect!=ident)
                    {
                        allAreIdent = false;
                        break;
                    }
                }
                if (allAreIdent)
                {
                    needToModfyUVs = false;
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        Debug.Log("All atlases have only one texture in atlas UVs will be copied without adjusting");
                }
            }
            if (needToModfyUVs)
            {
                int[] done = new int[nuvs.Length];
                for (int index = 0; index < done.Length; ++index)
                    done[index] = -1;
                bool triangleArraysOverlap = false;
                for (int k = 0; k < dgo.targetSubmeshIdxs.Length; k++)
                {
                    int[] subTris = dgo._tmpSubmeshTris == null ? mesh.GetTriangles(k) : dgo._tmpSubmeshTris[k].data;
                    DRect atlasRect = new DRect(dgo.uvRects[k]);
                    DRect obUVRect = !textureBakeResults.resultMaterials[dgo.targetSubmeshIdxs[k]].considerMeshUVs ? new DRect(0.0, 0.0, 1.0, 1.0) : new DRect(dgo.obUVRects[k]);
                    DRect sourceMaterialTiling = new DRect(dgo.sourceMaterialTiling[k]);
                    DRect encapsulatingRectMatAndUVTiling = new DRect(dgo.encapsulatingRect[k]);
                    DRect encapsulatingRectMatAndUVTilingInverse = MB3_UVTransformUtility.InverseTransform(ref encapsulatingRectMatAndUVTiling);
                    DRect toNormalizedUVs = MB3_UVTransformUtility.InverseTransform(ref obUVRect);
                    DRect meshFullSamplingRect = MB3_UVTransformUtility.CombineTransforms(ref obUVRect, ref sourceMaterialTiling);
                    DRect relativeTrans = MB3_UVTransformUtility.CombineTransforms(ref meshFullSamplingRect, ref encapsulatingRectMatAndUVTilingInverse);
                    DRect trans = MB3_UVTransformUtility.CombineTransforms(ref toNormalizedUVs, ref relativeTrans);
                    trans = MB3_UVTransformUtility.CombineTransforms(ref trans, ref atlasRect);
                    Rect rr = trans.GetRect();
                    for (int l = 0; l < subTris.Length; l++)
                    {
                        int vidx = subTris[l];
                        if (done[vidx] == -1)
                        {
                            done[vidx] = k; //prevents a uv from being adjusted twice. Same vert can be on more than one submesh.
                            Vector2 nuv = nuvs[vidx]; //don't modify nuvs directly because it is cached and we might be re-using
                                                      //if (textureBakeResults.fixOutOfBoundsUVs) {
                                                      //uvRectInSrc can be larger than (out of bounds uvs) or smaller than 0..1
                                                      //this transforms the uvs so they fit inside the uvRectInSrc sample box 

                            //string s = dgo.name + " " + nuv.ToString("f3") + " to ";
                            // scale, shift to fit in atlas rect
                            nuv.x = rr.x + nuv.x * rr.width;
                            nuv.y = rr.y + nuv.y * rr.height;
                            //Debug.Log(s + nuv.ToString("f3"));
                            uvs[vertsIdx + vidx] = nuv;
                        }
                        if (done[vidx] != k)
                        {
                            triangleArraysOverlap = true;
                        }
                    }
                }
                if (triangleArraysOverlap && LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning((dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas."));
            }
            else
                nuvs.CopyTo(uvs, vertsIdx);
            if (LOG_LEVEL < MB2_LogLevel.trace)
                return;
            Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", nuvs.Length));
        }

        private void _copyAndAdjustUV2FromMesh(MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx, MeshChannelsCache meshChannelsCache)
        {
            Vector2[] nuv2s = meshChannelsCache.GetUv2(mesh);
            if (lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
            { //has a lightmap
                //this does not work in Unity 5. the lightmapTilingOffset is always 1,1,0,0 for all objects
                //lightMap index is always 1
                Vector2 uvscale2;
                Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
                Vector2 uvscale = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
                Vector2 uvoffset = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
                for (int j = 0; j < nuv2s.Length; j++)
                {
                    uvscale2.x = uvscale.x * nuv2s[j].x;
                    uvscale2.y = uvscale.y * nuv2s[j].y;
                    uv2s[vertsIdx + j] = uvoffset + uvscale2;
                }
                if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + nuv2s.Length);
            }
            else
            {
                nuv2s.CopyTo(uv2s, vertsIdx);
                if (LOG_LEVEL >= MB2_LogLevel.trace)
                {
                    Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + nuv2s.Length);
                }
            }
        }

        public override void UpdateSkinnedMeshApproximateBounds()
        {
            UpdateSkinnedMeshApproximateBoundsFromBounds();
        }

        public override void UpdateSkinnedMeshApproximateBoundsFromBones()
        {
            if (outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
            {
                if (LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
            }
            else if (bones.Length == 0)
            {
                if ((uint)verts.Length <= 0U || LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
            }
            else if (_targetRenderer== null)
            {
                if (LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
            }
            else if (!(_targetRenderer).GetType().Equals(typeof(SkinnedMeshRenderer)))
            {
                if (LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
            }
            else
                UpdateSkinnedMeshApproximateBoundsFromBonesStatic(bones, (SkinnedMeshRenderer)targetRenderer);
        }

        public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
        {
            if (outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
            {
                if (LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBoundsFromBounds when output type is bakeMeshAssetsInPlace");
            }
            else if (verts.Length == 0 || mbDynamicObjectsInCombinedMesh.Count == 0)
            {
                if ((uint)verts.Length <= 0U || LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Nothing in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBoundsFromBounds.");
            }
            else if (_targetRenderer== null)
            {
                if (LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
            }
            else if (!(_targetRenderer).GetType().Equals(typeof(SkinnedMeshRenderer)))
            {
                if (LOG_LEVEL < MB2_LogLevel.warn)
                    return;
                Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
            }
            else
                UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(objectsInCombinedMesh, (SkinnedMeshRenderer)targetRenderer);
        }

        private int _getNumBones(Renderer r)
        {
            if (renderType != MB_RenderType.skinnedMeshRenderer)
                return 0;
            if (r is SkinnedMeshRenderer)
                return ((SkinnedMeshRenderer)r).bones.Length;
            if (r is MeshRenderer)
                return 1;
            Debug.LogError("Could not _getNumBones. Object does not have a renderer");
            return 0;
        }

        private Transform[] _getBones(Renderer r)
        {
            return MBVersion.GetBones(r);
        }

        public override void Apply(GenerateUV2Delegate uv2GenerationMethod)
        {
            bool bones = false;
            if (renderType == MB_RenderType.skinnedMeshRenderer)
                bones = true;
            Apply(true, true, _doNorm, _doTan, _doUV, doUV2(), _doUV3, _doUV4, doCol, bones, doBlendShapes, uv2GenerationMethod);
        }

        public virtual void ApplyShowHide()
        {
            if (_validationLevel >= MB2_ValidationLevel.quick && !ValidateTargRendererAndMeshAndResultSceneObj())
                return;
            if (_mesh!=null)
            {
                if (renderType == MB_RenderType.meshRenderer)
                {
                    MBVersion.MeshClear(_mesh, true);
                    _mesh.vertices=verts;
                }
                SerializableIntArray[] withShowHideApplied = GetSubmeshTrisWithShowHideApplied();
                if (textureBakeResults.doMultiMaterial)
                {
                    int num1;
                    _mesh.subMeshCount= num1 = _numNonZeroLengthSubmeshTris(withShowHideApplied);
                    int numNonZeroLengthSubmeshTris = num1;
                    int num2 = 0;
                    for (int index = 0; index < withShowHideApplied.Length; ++index)
                    {
                        if ((uint)withShowHideApplied[index].data.Length > 0U)
                        {
                            _mesh.SetTriangles(withShowHideApplied[index].data, num2);
                            ++num2;
                        }
                    }
                    _updateMaterialsOnTargetRenderer(withShowHideApplied, numNonZeroLengthSubmeshTris);
                }
                else
                    _mesh.triangles= withShowHideApplied[0].data;
                if (renderType == MB_RenderType.skinnedMeshRenderer)
                {
                    if (verts.Length == 0)
                        targetRenderer.enabled=false ;
                    else
                        targetRenderer.enabled = true;
                    bool updateWhenOffscreen = ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen= true;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen=updateWhenOffscreen;
                }
                if (LOG_LEVEL < MB2_LogLevel.trace)
                    return;
                Debug.Log("ApplyShowHide");
            }
            else
                Debug.LogError("Need to add objects to this meshbaker before calling ApplyShowHide");
        }

        public override void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapesFlag = false, GenerateUV2Delegate uv2GenerationMethod = null)
        {
            if (_validationLevel >= MB2_ValidationLevel.quick && !ValidateTargRendererAndMeshAndResultSceneObj())
                return;
            if (_mesh!= null)
            {
                if (LOG_LEVEL >= MB2_LogLevel.trace)
                    Debug.Log(string.Format("Apply called tri={0} vert={1} norm={2} tan={3} uv={4} col={5} uv3={6} uv4={7} uv2={8} bone={9} blendShape{10} meshID={11}", triangles, vertices, normals, tangents, uvs, colors, uv3, uv4, uv2, bones, blendShapes, (_mesh).GetInstanceID()));
                if (triangles || _mesh.vertexCount != verts.Length)
                {
                    if (triangles && !vertices && (!normals && !tangents) && (!uvs && !colors && (!uv3 && !uv4)) && !uv2 && !bones)
                        MBVersion.MeshClear(_mesh, true);
                    else
                        MBVersion.MeshClear(_mesh, false);
                }
                if (vertices)
                {
                    Vector3[] verts2Write = verts;
                    if ((uint)verts.Length > 0U)
                    {
                        if (_recenterVertsToBoundsCenter && _renderType == MB_RenderType.meshRenderer)
                        {
                            verts2Write = new Vector3[verts.Length];
                            Vector3 max = verts[0];
                            Vector3 min = verts[0];
                            for (int index = 1; index < verts.Length; ++index)
                            {
                                Vector3 vert3 = verts[index];
                                if (max.x < vert3.x)
                                    max.x = vert3.x;
                                if (max.y < vert3.y)
                                    max.y = vert3.y;
                                if (max.z < vert3.z)
                                    max.z = vert3.z;
                                if (min.x > vert3.x)
                                    min.x = vert3.x;
                                if (min.y > vert3.y)
                                    min.y = vert3.y;
                                if (min.z > vert3.z)
                                    min.z = vert3.z;
                            }
                            Vector3 center = max+ min/ 2f;
                            for (int index = 0; index < verts.Length; ++index)
                                verts2Write[index] = (verts[index]- center);
                            targetRenderer.transform.position=center;
                        }
                        else
                            targetRenderer .transform.position= Vector3.zero;
                    }
                    _mesh.vertices= verts2Write;
                }
                if (triangles && _textureBakeResults)
                {
                    if (_textureBakeResults== null)
                    {
                        Debug.LogError("Texture Bake Result was not set.");
                    }
                    else
                    {
                        SerializableIntArray[] submeshTrisToUse = GetSubmeshTrisWithShowHideApplied();
                        int num1;
                        _mesh.subMeshCount= num1 = _numNonZeroLengthSubmeshTris(submeshTrisToUse);
                        int numNonZeroLengthSubmeshTris = num1;
                        int num2 = 0;
                        for (int index = 0; index < submeshTrisToUse.Length; ++index)
                        {
                            if ((uint)submeshTrisToUse[index].data.Length > 0U)
                            {
                                _mesh.SetTriangles(submeshTrisToUse[index].data, num2);
                                ++num2;
                            }
                        }
                        _updateMaterialsOnTargetRenderer(submeshTrisToUse, numNonZeroLengthSubmeshTris);
                    }
                }
                if (normals)
                {
                    if (_doNorm) { _mesh.normals = this.normals; }
                    else
                        Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals");
                }
                if (tangents)
                {
                    if (_doTan) { _mesh.tangents = this.tangents; }
                    else { Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents"); }
                }
                if (uvs)
                {
                    if (_doUV) { _mesh.uv = this.uvs; }
                    else { Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs"); }
                }
                if (colors)
                {
                    if (_doCol) { _mesh.colors = this.colors; }
                    else { Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors"); }
                }
                if (uv3)
                {
                    if (_doUV3)
                        MBVersion.MeshAssignUV3(_mesh, uv3s);
                    else
                        Debug.LogError("uv3 flag was set in Apply but MeshBaker didn't generate uv3s");
                }
                if (uv4)
                {
                    if (_doUV4)
                        MBVersion.MeshAssignUV4(_mesh, uv4s);
                    else
                        Debug.LogError("uv4 flag was set in Apply but MeshBaker didn't generate uv4s");
                }
                if (uv2)
                {
                    if (doUV2()) { _mesh.uv2 = this.uv2s; }
                    else { Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + lightmapOption); }
                }
                bool flag = false;
                if (renderType != MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
                {
                    if (uv2GenerationMethod != null)
                    {
                        uv2GenerationMethod(_mesh, uv2UnwrappingParamsHardAngle, uv2UnwrappingParamsPackMargin);
                        if (LOG_LEVEL >= MB2_LogLevel.trace)
                            Debug.Log("generating new UV2 layout for the combined mesh ");
                    }
                    else
                        Debug.LogError("No GenerateUV2Delegate method was supplied. UV2 cannot be generated.");
                    flag = true;
                }
                else if (renderType == MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && LOG_LEVEL >= MB2_LogLevel.warn)
                    Debug.LogWarning("UV2 cannot be generated for SkinnedMeshRenderer objects.");
                if (renderType != MB_RenderType.skinnedMeshRenderer && lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && !flag)
                    Debug.LogError("Failed to generate new UV2 layout. Only works in editor.");
                if (renderType == MB_RenderType.skinnedMeshRenderer)
                {
                    if (verts.Length == 0)
                        targetRenderer.enabled= false;
                    else
                        targetRenderer.enabled= true;
                    bool updateWhenOffscreen = ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen= true;
                    ((SkinnedMeshRenderer)targetRenderer).updateWhenOffscreen=updateWhenOffscreen;
                }
                if (bones)
                {
                    _mesh.bindposes=bindPoses;
                    _mesh.boneWeights= boneWeights;
                }
                if (blendShapesFlag && (MBVersion.GetMajorVersion() > 5 || MBVersion.GetMajorVersion() == 5 && MBVersion.GetMinorVersion() >= 3))
                {
                    if (blendShapesInCombined.Length != blendShapes.Length)
                        blendShapesInCombined = new MBBlendShape[blendShapes.Length];
                    Vector3[] vs = new UnityEngine.Vector3[verts.Length];
                    Vector3[] ns = new UnityEngine.Vector3[verts.Length];
                    Vector3[] ts = new UnityEngine.Vector3[verts.Length];
                    _mesh.ClearBlendShapes();
                    for (int i = 0; i < blendShapes.Length; i++)
                    {
                        MB_DynamicGameObject dgo = instance2Combined_MapGet(blendShapes[i].gameObjectID);
                        if (dgo != null)
                        {
                            for (int j = 0; j < blendShapes[i].frames.Length; j++)
                            {
                                MBBlendShapeFrame frame = blendShapes[i].frames[j];
                                int destIdx = dgo.vertIdx;
                                Array.Copy(frame.vertices, 0, vs, destIdx, blendShapes[i].frames[j].vertices.Length);
                                Array.Copy(frame.normals, 0, ns, destIdx, blendShapes[i].frames[j].normals.Length);
                                Array.Copy(frame.tangents, 0, ts, destIdx, blendShapes[i].frames[j].tangents.Length);
                                _mesh.AddBlendShapeFrame(blendShapes[i].name + blendShapes[i].gameObjectID, frame.frameWeight, vs, ns, ts);
                                _ZeroArray(vs, destIdx, blendShapes[i].frames[j].vertices.Length);
                                _ZeroArray(ns, destIdx, blendShapes[i].frames[j].normals.Length);
                                _ZeroArray(ts, destIdx, blendShapes[i].frames[j].tangents.Length);
                            }
                        }
                        else
                        {
                            Debug.LogError("InstanceID in blend shape that was not in instance2combinedMap");
                        }
                    }
                    //this is necessary to get the renderer to refresh its data about the blendshapes.
                    ((SkinnedMeshRenderer)_targetRenderer).sharedMesh = null;
                    ((SkinnedMeshRenderer)_targetRenderer).sharedMesh = _mesh;
                }
                if (triangles | vertices)
                {
                    if (LOG_LEVEL >= MB2_LogLevel.trace)
                        Debug.Log("recalculating bounds on mesh.");
                    _mesh.RecalculateBounds();
                }
                if (!_optimizeAfterBake || Application.isPlaying)
                    return;
                MBVersion.OptimizeMesh(_mesh);
            }
            else
                Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");
        }

        private int _numNonZeroLengthSubmeshTris(SerializableIntArray[] subTris)
        {
            int num = 0;
            for (int index = 0; index < subTris.Length; ++index)
            {
                if ((uint)subTris[index].data.Length > 0U)
                    ++num;
            }
            return num;
        }

        private void _updateMaterialsOnTargetRenderer(SerializableIntArray[] subTris, int numNonZeroLengthSubmeshTris)
        {
            if (subTris.Length != textureBakeResults.resultMaterials.Length)
                Debug.LogError("Mismatch between number of submeshes and number of result materials");
            Material[] materialArray = new Material[numNonZeroLengthSubmeshTris];
            int index1 = 0;
            for (int index2 = 0; index2 < subTris.Length; ++index2)
            {
                if ((uint)subTris[index2].data.Length > 0U)
                {
                    materialArray[index1] = _textureBakeResults.resultMaterials[index2].combinedMaterial;
                    ++index1;
                }
            }
            targetRenderer.materials= materialArray;
        }

        public SerializableIntArray[] GetSubmeshTrisWithShowHideApplied()
        {
            bool flag = false;
            for (int index = 0; index < mbDynamicObjectsInCombinedMesh.Count; ++index)
            {
                if (!mbDynamicObjectsInCombinedMesh[index].show)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
                return submeshTris;
            int[] numArray1 = new int[submeshTris.Length];
            SerializableIntArray[] serializableIntArrayArray = new SerializableIntArray[submeshTris.Length];
            for (int index1 = 0; index1 < mbDynamicObjectsInCombinedMesh.Count; ++index1)
            {
                MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index1];
                if (dynamicGameObject.show)
                {
                    for (int index2 = 0; index2 < dynamicGameObject.submeshNumTris.Length; ++index2)
                        numArray1[index2] += dynamicGameObject.submeshNumTris[index2];
                }
            }
            for (int index = 0; index < serializableIntArrayArray.Length; ++index)
                serializableIntArrayArray[index] = new SerializableIntArray(numArray1[index]);
            int[] numArray2 = new int[serializableIntArrayArray.Length];
            for (int index1 = 0; index1 < mbDynamicObjectsInCombinedMesh.Count; ++index1)
            {
                MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index1];
                if (dynamicGameObject.show)
                {
                    for (int index2 = 0; index2 < submeshTris.Length; ++index2)
                    {
                        int[] data = submeshTris[index2].data;
                        int num1 = dynamicGameObject.submeshTriIdxs[index2];
                        int num2 = num1 + dynamicGameObject.submeshNumTris[index2];
                        for (int index3 = num1; index3 < num2; ++index3)
                        {
                            serializableIntArrayArray[index2].data[numArray2[index2]] = data[index3];
                            numArray2[index2] = numArray2[index2] + 1;
                        }
                    }
                }
            }
            return serializableIntArrayArray;
        }

        public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false)
        {
            _updateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
        }

        private void _updateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo)
        {
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                Debug.Log(("UpdateGameObjects called on " + gos.Length + " objects."));
            int numResultMats = 1;
            if (textureBakeResults.doMultiMaterial)
                numResultMats = textureBakeResults.resultMaterials.Length;
            _initialize(numResultMats);
            if (_mesh.vertexCount > 0 && _instance2combined_map.Count == 0)
                Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
            MeshChannelsCache meshChannelCache = new MeshChannelsCache(this);
            for (int index = 0; index < gos.Length; ++index)
                _updateGameObject(gos[index], updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo, meshChannelCache);
            if (!recalcBounds)
                return;
            _mesh.RecalculateBounds();
        }

        private void _updateGameObject(GameObject go, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo, MeshChannelsCache meshChannelCache)
        {
            MB_DynamicGameObject dgo = null;
            if (!instance2Combined_MapTryGetValue((go).GetInstanceID(), out dgo))
            {
                Debug.LogError(("Object " + (go).name + " has not been added"));
            }
            else
            {
                Mesh mesh = MB_Utility.GetMesh(go);
                if (dgo.numVerts != mesh.vertexCount)
                {
                    Debug.LogError(("Object " + (go).name+ " source mesh has been modified since being added. To update it must have the same number of verts"));
                }
                else
                {
                    if (_doUV & updateUV)
                        _copyAndAdjustUVsFromMesh(dgo, mesh, dgo.vertIdx, meshChannelCache);
                    if (doUV2() & updateUV2)
                        _copyAndAdjustUV2FromMesh(dgo, mesh, dgo.vertIdx, meshChannelCache);
                    if (renderType == MB_RenderType.skinnedMeshRenderer & updateSkinningInfo)
                    {
                        Renderer renderer = MB_Utility.GetRenderer(go);
                        BoneWeight[] boneWeights = meshChannelCache.GetBoneWeights(renderer, dgo.numVerts);
                        Transform[] bones = _getBones(renderer);
                        int vertIdx = dgo.vertIdx;
                        bool flag = false;
                        for (int index = 0; index < boneWeights.Length; ++index)
                        {
                            if ((bones[(boneWeights[index]).boneIndex0]!=bones[(boneWeights[vertIdx]).boneIndex0]))
                            {
                                flag = true;
                                break;
                            }
                          // ISSUE: explicit reference operation
                          // ISSUE: explicit reference operation
                          (boneWeights[vertIdx]).weight0= boneWeights[index].weight0;
                            // ISSUE: explicit reference operation
                            // ISSUE: explicit reference operation
                            boneWeights[vertIdx].weight1=boneWeights[index].weight1;
                            (boneWeights[vertIdx]).weight2 = boneWeights[index].weight2;
                            (boneWeights[vertIdx]).weight3 = boneWeights[index].weight3;

                            ++vertIdx;
                        }
                        if (flag)
                            Debug.LogError(("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + dgo.name));
                    }
                    Matrix4x4 l2wMat = go.transform.localToWorldMatrix;
                    if (updateVertices)
                    {
                        Vector3[] nverts = meshChannelCache.GetVertices(mesh);
                        for (int j = 0; j < nverts.Length; j++)
                        {
                            verts[dgo.vertIdx + j] = l2wMat.MultiplyPoint3x4(nverts[j]);
                        }
                    }
                    l2wMat[0, 3] = l2wMat[1, 3] = l2wMat[2, 3] = 0f;
                    if (_doNorm && updateNormals)
                    {
                        Vector3[] nnorms = meshChannelCache.GetNormals(mesh);
                        for (int j = 0; j < nnorms.Length; j++)
                        {
                            int vIdx = dgo.vertIdx + j;
                            normals[vIdx] = l2wMat.MultiplyPoint3x4(nnorms[j]);
                            normals[vIdx] = normals[vIdx].normalized;
                        }
                    }
                    if (_doTan && updateTangents)
                    {
                        Vector4[] ntangs = meshChannelCache.GetTangents(mesh);
                        for (int j = 0; j < ntangs.Length; j++)
                        {
                            int midx = dgo.vertIdx + j;
                            float w = ntangs[j].w; //need to preserve the w value
                            Vector3 tn = l2wMat.MultiplyPoint3x4(ntangs[j]);
                            tn.Normalize();
                            tangents[midx] = tn;
                            tangents[midx].w = w;
                        }
                    }
                    if (_doCol & updateColors)
                    {
                        Color[] colors = meshChannelCache.GetColors(mesh);
                        for (int index = 0; index < colors.Length; ++index)
                            colors[dgo.vertIdx + index] = colors[index];
                    }
                    if (_doUV3 & updateUV3)
                    {
                        Vector2[] uv3 = meshChannelCache.GetUv3(mesh);
                        for (int index = 0; index < uv3.Length; ++index)
                            uv3s[dgo.vertIdx + index] = uv3[index];
                    }
                    if (!(_doUV4 & updateUV4))
                        return;
                    Vector2[] uv4 = meshChannelCache.GetUv4(mesh);
                    for (int index = 0; index < uv4.Length; ++index)
                        uv4s[dgo.vertIdx + index] = uv4[index];
                }
            }
        }

        public bool ShowHideGameObjects(GameObject[] toShow, GameObject[] toHide)
        {
            if (textureBakeResults!= null)
                return _showHide(toShow, toHide);
            Debug.LogError("TextureBakeResults must be set.");
            return false;
        }

        public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
        {
            int[] deleteGOinstanceIDs = null;
            if (deleteGOs != null)
            {
                deleteGOinstanceIDs = new int[deleteGOs.Length];
                for (int index = 0; index < deleteGOs.Length; ++index)
                {
                    if ((deleteGOs[index]==null))
                        Debug.LogError(("The " + index + "th object on the list of objects to delete is 'Null'"));
                    else
                        deleteGOinstanceIDs[index] = (deleteGOs[index]).GetInstanceID();
                }
            }
            return AddDeleteGameObjectsByID(gos, deleteGOinstanceIDs, disableRendererInSource);
        }

        public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
        {
            if (validationLevel > MB2_ValidationLevel.none)
            {
                if (gos != null)
                {
                    for (int index1 = 0; index1 < gos.Length; ++index1)
                    {
                        if (gos[index1]==null)
                        {
                            Debug.LogError(("The " + index1 + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element."));
                            return false;
                        }
                        if (validationLevel >= MB2_ValidationLevel.robust)
                        {
                            for (int index2 = index1 + 1; index2 < gos.Length; ++index2)
                            {
                                if (gos[index1]==gos[index2])
                                {
                                    Debug.LogError(("GameObject " + gos[index1] + " appears twice in list of game objects to add"));
                                    return false;
                                }
                            }
                        }
                    }
                }
                if (deleteGOinstanceIDs != null && validationLevel >= MB2_ValidationLevel.robust)
                {
                    for (int index1 = 0; index1 < deleteGOinstanceIDs.Length; ++index1)
                    {
                        for (int index2 = index1 + 1; index2 < deleteGOinstanceIDs.Length; ++index2)
                        {
                            if (deleteGOinstanceIDs[index1] == deleteGOinstanceIDs[index2])
                            {
                                Debug.LogError(("GameObject " + deleteGOinstanceIDs[index1] + "appears twice in list of game objects to delete"));
                                return false;
                            }
                        }
                    }
                }
            }
            if (_usingTemporaryTextureBakeResult && gos != null && (uint)gos.Length > 0U)
            {
                MB_Utility.Destroy(_textureBakeResults);
                _textureBakeResults = null;
                _usingTemporaryTextureBakeResult = false;
            }
            if (_textureBakeResults== null && gos != null && gos.Length != 0 && gos[0]!= null && !_CreateTemporaryTextrueBakeResult(gos, GetMaterialsOnTargetRenderer()))
                return false;
            BuildSceneMeshObject(gos, false);
            if (!_addToCombined(gos, deleteGOinstanceIDs, disableRendererInSource))
            {
                Debug.LogError("Failed to add/delete objects to combined mesh");
                return false;
            }
            if (targetRenderer!=null)
            {
                if (renderType == MB_RenderType.skinnedMeshRenderer)
                {
                    ((SkinnedMeshRenderer)targetRenderer).bones=bones;
                    UpdateSkinnedMeshApproximateBoundsFromBounds();
                }
                targetRenderer.lightmapIndex =GetLightmapIndex();
            }
            return true;
        }

        public override bool CombinedMeshContains(GameObject go)
        {
            return objectsInCombinedMesh.Contains(go);
        }

        public override void ClearBuffers()
        {
            verts = new Vector3[0];
            normals = new Vector3[0];
            tangents = new Vector4[0];
            uvs = new Vector2[0];
            uv2s = new Vector2[0];
            uv3s = new Vector2[0];
            uv4s = new Vector2[0];
            colors = new Color[0];
            bones = new Transform[0];
            bindPoses = new Matrix4x4[0];
            boneWeights = new BoneWeight[0];
            submeshTris = new SerializableIntArray[0];
            blendShapes = new MBBlendShape[0];
            if (blendShapesInCombined == null)
            {
                blendShapesInCombined = new MBBlendShape[0];
            }
            else
            {
                for (int index = 0; index < blendShapesInCombined.Length; ++index)
                    blendShapesInCombined[index].frames = new MBBlendShapeFrame[0];
            }
            mbDynamicObjectsInCombinedMesh.Clear();
            objectsInCombinedMesh.Clear();
            instance2Combined_MapClear();
            if (_usingTemporaryTextureBakeResult)
            {
                MB_Utility.Destroy(_textureBakeResults);
                _textureBakeResults = null;
                _usingTemporaryTextureBakeResult = false;
            }
            if (LOG_LEVEL < MB2_LogLevel.trace)
                return;
            MB2_Log.LogDebug("ClearBuffers called");
        }

        public override void ClearMesh()
        {
            if (_mesh!=null)
                MBVersion.MeshClear(_mesh, false);
            else
                _mesh = new Mesh();
            ClearBuffers();
        }

        public override void DestroyMesh()
        {
            if (_mesh!=null)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Destroying Mesh");
                MB_Utility.Destroy(_mesh);
            }
            _mesh = new Mesh();
            ClearBuffers();
        }

        public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
        {
            if (_mesh!=null)
            {
                if (LOG_LEVEL >= MB2_LogLevel.debug)
                    MB2_Log.LogDebug("Destroying Mesh");
                editorMethods.Destroy(_mesh);
            }
            _mesh = new Mesh();
            ClearBuffers();
        }

        public bool ValidateTargRendererAndMeshAndResultSceneObj()
        {
            if (_resultSceneObject==null)
            {
                if (_LOG_LEVEL >= MB2_LogLevel.error)
                    Debug.LogError("Result Scene Object was not set.");
                return false;
            }
            if (_targetRenderer== null)
            {
                if (_LOG_LEVEL >= MB2_LogLevel.error)
                    Debug.LogError("Target Renderer was not set.");
                return false;
            }
            if (_targetRenderer.transform.parent!= _resultSceneObject.transform)
            {
                if (_LOG_LEVEL >= MB2_LogLevel.error)
                    Debug.LogError("Target Renderer game object is not a child of Result Scene Object was not set.");
                return false;
            }
            if (_renderType == MB_RenderType.skinnedMeshRenderer)
            {
                if (!(_targetRenderer is SkinnedMeshRenderer))
                {
                    if (_LOG_LEVEL >= MB2_LogLevel.error)
                        Debug.LogError("Render Type is skinned mesh renderer but Target Renderer is not.");
                    return false;
                }
                if (((SkinnedMeshRenderer)_targetRenderer).sharedMesh!= _mesh)
                {
                    if (_LOG_LEVEL >= MB2_LogLevel.error)
                        Debug.LogError("Target renderer mesh is not equal to mesh.");
                    return false;
                }
            }
            if (_renderType == MB_RenderType.meshRenderer)
            {
                if (!(_targetRenderer is MeshRenderer))
                {
                    if (_LOG_LEVEL >= MB2_LogLevel.error)
                        Debug.LogError("Render Type is mesh renderer but Target Renderer is not.");
                    return false;
                }
                if ((_mesh!= (_targetRenderer.GetComponent<MeshFilter>()).sharedMesh))
                {
                    if (_LOG_LEVEL >= MB2_LogLevel.error)
                        Debug.LogError("Target renderer mesh is not equal to mesh.");
                    return false;
                }
            }
            return true;
        }

        internal static Renderer BuildSceneHierarchPreBake(MB3_MeshCombinerSingle mom, GameObject root, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
        {
            if (mom._LOG_LEVEL >= MB2_LogLevel.trace)
                Debug.Log(("Building Scene Hierarchy createNewChild=" + createNewChild.ToString()));
            MeshFilter mf = null;
            MeshRenderer mr = null;
            SkinnedMeshRenderer smr = null;
            Transform transform = null;
            if ((root== null))
            {
                Debug.LogError("root was null.");
                return null;
            }
            if ((mom.textureBakeResults==null))
            {
                Debug.LogError("textureBakeResults must be set.");
                return null;
            }
            if ((root.GetComponent<Renderer>()!=null))
            {
                Debug.LogError("root game object cannot have a renderer component");
                return null;
            }
            if (!createNewChild)
            {
                if ((mom.targetRenderer!= null) && (mom.targetRenderer).transform.parent== root.transform)
                {
                    transform = mom.targetRenderer.transform;
                }
                else
                {
                    Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
                    if (componentsInChildren.Length == 1)
                    {
                        if (componentsInChildren[0].transform.parent!= root.transform)
                            Debug.LogError("Target Renderer is not an immediate child of Result Scene Object. Try using a game object with no children as the Result Scene Object..");
                        transform = componentsInChildren[0].transform;
                    }
                }
            }
            if (transform!= null && transform.parent!= root.transform)
                transform = null;
            if (transform== null)
            {
                GameObject gameObject = new GameObject(mom.name + "-mesh");
                gameObject.transform.parent=(root.transform);
                transform = gameObject.transform;
            }
            transform.parent=root.transform;
            GameObject gameObject1 = transform.gameObject;
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
            {
                MeshRenderer component1 = gameObject1.GetComponent<MeshRenderer>();
                if (component1!= null)
                    MB_Utility.Destroy(component1);
                MeshFilter component2 = gameObject1.GetComponent<MeshFilter>();
                if ((component2!= null))
                    MB_Utility.Destroy(component2);
                smr = gameObject1.GetComponent<SkinnedMeshRenderer>();
                if ((smr==null))
                    smr = gameObject1.AddComponent<SkinnedMeshRenderer>();
            }
            else
            {
                SkinnedMeshRenderer component = gameObject1.GetComponent<SkinnedMeshRenderer>();
                if ((component!= null))
                    MB_Utility.Destroy(component);
                mf = gameObject1.GetComponent<MeshFilter>();
                if ((mf==null))
                    mf = gameObject1.AddComponent<MeshFilter>();
                mr = gameObject1.GetComponent<MeshRenderer>();
                if ((mr== null))
                    mr = gameObject1.AddComponent<MeshRenderer>();
            }
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
            {
                smr.bones= mom.GetBones();
                bool updateWhenOffscreen = smr.updateWhenOffscreen;
                smr.updateWhenOffscreen= true;
                smr.updateWhenOffscreen= updateWhenOffscreen;
            }
            _ConfigureSceneHierarch(mom, root, mr, mf, smr, m, objsToBeAdded);
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
                return smr;
            return mr;
        }

        public static void BuildPrefabHierarchy(MB3_MeshCombinerSingle mom, GameObject instantiatedPrefabRoot, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
        {
            SkinnedMeshRenderer smr = null;
            MeshRenderer mr = null;
            MeshFilter mf = null;
            GameObject gameObject1 = new GameObject(mom.name + "-mesh");
            gameObject1.transform.parent=instantiatedPrefabRoot.transform;
            Transform transform = gameObject1.transform;
            transform.parent=(instantiatedPrefabRoot.transform);
            GameObject gameObject2 = (transform).gameObject;
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
            {
                MeshRenderer component1 = gameObject2.GetComponent<MeshRenderer>();
                if ((component1!= null))
                    MB_Utility.Destroy(component1);
                MeshFilter component2 = gameObject2.GetComponent<MeshFilter>();
                if ((component2!= null))
                    MB_Utility.Destroy(component2);
                smr = gameObject2.GetComponent<SkinnedMeshRenderer>();
                if (smr== null)
                    smr = gameObject2.AddComponent<SkinnedMeshRenderer>();
            }
            else
            {
                SkinnedMeshRenderer component = gameObject2.GetComponent<SkinnedMeshRenderer>();
                if (component!=null)
                    MB_Utility.Destroy(component);
                mf = gameObject2.GetComponent<MeshFilter>();
                if ((mf== null))
                    mf = gameObject2.AddComponent<MeshFilter>();
                mr = gameObject2.GetComponent<MeshRenderer>();
                if ((mr== null))
                    mr = gameObject2.AddComponent<MeshRenderer>();
            }
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
            {
                smr.bones=mom.GetBones();
                bool updateWhenOffscreen = smr.updateWhenOffscreen;
                smr.updateWhenOffscreen=(true);
                smr.updateWhenOffscreen=(updateWhenOffscreen);
            }
            _ConfigureSceneHierarch(mom, instantiatedPrefabRoot, mr, mf, smr, m, objsToBeAdded);
            if ((mom.targetRenderer!= null))
                return;
            Material[] materialArray = new Material[mom.targetRenderer.sharedMaterials.Length];
            Debug.Log("target materils " + mom.targetRenderer.sharedMaterials.Length);
            for (int index = 0; index < materialArray.Length; ++index)
                materialArray[index] = mom.targetRenderer.sharedMaterials[index];
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
            {
                smr.sharedMaterials=(materialArray);
            }
            else
            {
                mr.sharedMaterials=materialArray;
            }
        }

        private static void _ConfigureSceneHierarch(MB3_MeshCombinerSingle mom, GameObject root, MeshRenderer mr, MeshFilter mf, SkinnedMeshRenderer smr, Mesh m, GameObject[] objsToBeAdded = null)
        {
            GameObject gameObject;
            if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
            {
                gameObject = (smr).gameObject;
                smr.sharedMesh=m;
                smr.lightmapIndex= mom.GetLightmapIndex();
            }
            else
            {
                gameObject = mr.gameObject;
                mf.sharedMesh=(m);
                mr.lightmapIndex=(mom.GetLightmapIndex());
            }
            if (mom.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || mom.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
                gameObject.isStatic=(true);
            if (objsToBeAdded == null || objsToBeAdded.Length == 0 || !(objsToBeAdded[0]== null))
                return;
            bool flag1 = true;
            bool flag2 = true;
            string tag = objsToBeAdded[0].tag;
            int layer = objsToBeAdded[0].layer;
            for (int index = 0; index < objsToBeAdded.Length; ++index)
            {
                if ((objsToBeAdded[index]!= null))
                {
                    if (!objsToBeAdded[index].tag.Equals(tag))
                        flag1 = false;
                    if (objsToBeAdded[index].layer!= layer)
                        flag2 = false;
                }
            }
            if (flag1)
            {
                root.tag=(tag);
                gameObject.tag=(tag);
            }
            if (flag2)
            {
                root.layer=(layer);
                gameObject.layer=(layer);
            }
        }

        public void BuildSceneMeshObject(GameObject[] gos = null, bool createNewChild = false)
        {
            if ((_resultSceneObject== null))
                _resultSceneObject = new GameObject("CombinedMesh-" + name);
            _targetRenderer = BuildSceneHierarchPreBake(this, _resultSceneObject, GetMesh(), createNewChild, gos);
        }

        private bool IsMirrored(Matrix4x4 tm)
        {
            Vector3 vector3_1 = tm.GetRow(0);
            Vector3 vector3_2 = tm.GetRow(1);
            Vector3 vector3_3 = tm.GetRow(2);
            (vector3_1).Normalize();
            (vector3_2).Normalize();
            (vector3_3).Normalize();
            return Vector3.Dot(Vector3.Cross(vector3_1, vector3_2), vector3_3) < 0.0;
        }

        public override void CheckIntegrity()
        {
        }

        private void _ZeroArray(Vector3[] arr, int idx, int length)
        {
            int num = idx + length;
            for (int index = idx; index < num; ++index)
                arr[index] = Vector3.zero;
        }

        private List<MB_DynamicGameObject>[] _buildBoneIdx2dgoMap()
        {
            List<MB_DynamicGameObject>[] dynamicGameObjectListArray = new List<MB_DynamicGameObject>[bones.Length];
            for (int index = 0; index < dynamicGameObjectListArray.Length; ++index)
                dynamicGameObjectListArray[index] = new List<MB_DynamicGameObject>();
            for (int index1 = 0; index1 < mbDynamicObjectsInCombinedMesh.Count; ++index1)
            {
                MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index1];
                for (int index2 = 0; index2 < dynamicGameObject.indexesOfBonesUsed.Length; ++index2)
                    dynamicGameObjectListArray[dynamicGameObject.indexesOfBonesUsed[index2]].Add(dynamicGameObject);
            }
            return dynamicGameObjectListArray;
        }

        private void _CollectBonesToAddForDGO(MB_DynamicGameObject dgo, Dictionary<Transform, int> bone2idx, HashSet<int> boneIdxsToDelete, HashSet<BoneAndBindpose> bonesToAdd, Renderer r, MeshChannelsCache meshChannelCache)
        {
            Matrix4x4[] matrix4x4Array = dgo._tmpCachedBindposes = meshChannelCache.GetBindposes(r);
            BoneWeight[] boneWeightArray = dgo._tmpCachedBoneWeights = meshChannelCache.GetBoneWeights(r, dgo.numVerts);
            Transform[] transformArray = dgo._tmpCachedBones = _getBones(r);
            HashSet<int> intSet = new HashSet<int>();
            for (int index = 0; index < boneWeightArray.Length; ++index)
            {
                intSet.Add((boneWeightArray[index]).boneIndex0);
                intSet.Add((boneWeightArray[index]).boneIndex1);
                intSet.Add((boneWeightArray[index]).boneIndex2);
                intSet.Add((boneWeightArray[index]).boneIndex3);
            }
            int[] array = new int[intSet.Count];
            intSet.CopyTo(array);
            for (int index1 = 0; index1 < array.Length; ++index1)
            {
                bool flag = false;
                int index2 = array[index1];
                int index3;
                if (bone2idx.TryGetValue(transformArray[index2], out index3) && ((transformArray[index2]== bones[index3]) && !boneIdxsToDelete.Contains(index3) && matrix4x4Array[index2]== bindPoses[index3]))
                    flag = true;
                if (!flag)
                {
                    BoneAndBindpose boneAndBindpose = new BoneAndBindpose(transformArray[index2], matrix4x4Array[index2]);
                    if (!bonesToAdd.Contains(boneAndBindpose))
                        bonesToAdd.Add(boneAndBindpose);
                }
            }
            dgo._tmpIndexesOfSourceBonesUsed = array;
        }

        private void _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(HashSet<int> boneIdxsToDeleteHS, HashSet<BoneAndBindpose> bonesToAdd, Transform[] nbones, Matrix4x4[] nbindPoses, BoneWeight[] nboneWeights, int totalDeleteVerts)
        {
            if (boneIdxsToDeleteHS.Count > 0)
            {
                int[] array = new int[boneIdxsToDeleteHS.Count];
                boneIdxsToDeleteHS.CopyTo(array);
                Array.Sort<int>(array);
                int[] numArray = new int[bones.Length];
                int index1 = 0;
                int index2 = 0;
                for (int index3 = 0; index3 < bones.Length; ++index3)
                {
                    if (index2 < array.Length && array[index2] == index3)
                    {
                        ++index2;
                        numArray[index3] = -1;
                    }
                    else
                    {
                        numArray[index3] = index1;
                        nbones[index1] = bones[index3];
                        nbindPoses[index1] = bindPoses[index3];
                        ++index1;
                    }
                }
                int num = boneWeights.Length - totalDeleteVerts;
                for (int index3 = 0; index3 < num; ++index3)
                {
                    // ISSUE: explicit reference operation
                    // ISSUE: explicit reference operation
                    (nboneWeights[index3]).boneIndex0=(numArray[(nboneWeights[index3]).boneIndex0]);
                    (nboneWeights[index3]).boneIndex1 = (numArray[(nboneWeights[index3]).boneIndex1]);
                    (nboneWeights[index3]).boneIndex2 = (numArray[(nboneWeights[index3]).boneIndex2]);
                    (nboneWeights[index3]).boneIndex3 = (numArray[(nboneWeights[index3]).boneIndex3]);

                }
                for (int index3 = 0; index3 < mbDynamicObjectsInCombinedMesh.Count; ++index3)
                {
                    MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index3];
                    for (int index4 = 0; index4 < dynamicGameObject.indexesOfBonesUsed.Length; ++index4)
                        dynamicGameObject.indexesOfBonesUsed[index4] = numArray[dynamicGameObject.indexesOfBonesUsed[index4]];
                }
            }
            else
            {
                Array.Copy(bones, nbones, bones.Length);
                Array.Copy(bindPoses, nbindPoses, bindPoses.Length);
            }
        }

        private void _AddBonesToNewBonesArrayAndAdjustBWIndexes(MB_DynamicGameObject dgo, Renderer r, int vertsIdx, Transform[] nbones, BoneWeight[] nboneWeights, MeshChannelsCache meshChannelCache)
        {
            Transform[] tmpCachedBones = dgo._tmpCachedBones;
            Matrix4x4[] tmpCachedBindposes = dgo._tmpCachedBindposes;
            BoneWeight[] cachedBoneWeights = dgo._tmpCachedBoneWeights;
            int[] numArray = new int[tmpCachedBones.Length];
            for (int index1 = 0; index1 < dgo._tmpIndexesOfSourceBonesUsed.Length; ++index1)
            {
                int index2 = dgo._tmpIndexesOfSourceBonesUsed[index1];
                for (int index3 = 0; index3 < nbones.Length; ++index3)
                {
                    if ((tmpCachedBones[index2]== nbones[index3]) &&tmpCachedBindposes[index2]== bindPoses[index3])
                    {
                        numArray[index2] = index3;
                        break;
                    }
                }
            }
            for (int index1 = 0; index1 < cachedBoneWeights.Length; ++index1)
            {
                int index2 = vertsIdx + index1;
                // ISSUE: explicit reference operation
                // ISSUE: explicit reference operation
                (nboneWeights[index2]).boneIndex0=(numArray[(cachedBoneWeights[index1]).boneIndex0]);
                (nboneWeights[index2]).boneIndex1 = (numArray[(cachedBoneWeights[index1]).boneIndex1]);
                (nboneWeights[index2]).boneIndex2 = (numArray[(cachedBoneWeights[index1]).boneIndex2]);
                (nboneWeights[index2]).boneIndex3 = (numArray[(cachedBoneWeights[index1]).boneIndex3]);

                // ISSUE: explicit reference operation
                // ISSUE: explicit reference operation
                (nboneWeights[index2]).weight0=((cachedBoneWeights[index1]).weight0);
                (nboneWeights[index2]).weight1 = ((cachedBoneWeights[index1]).weight1);
                (nboneWeights[index2]).weight2 = ((cachedBoneWeights[index1]).weight2);
                (nboneWeights[index2]).weight3 = ((cachedBoneWeights[index1]).weight3);

            }
            for (int index = 0; index < dgo._tmpIndexesOfSourceBonesUsed.Length; ++index)
                dgo._tmpIndexesOfSourceBonesUsed[index] = numArray[dgo._tmpIndexesOfSourceBonesUsed[index]];
            dgo.indexesOfBonesUsed = dgo._tmpIndexesOfSourceBonesUsed;
            dgo._tmpIndexesOfSourceBonesUsed = null;
            dgo._tmpCachedBones = null;
            dgo._tmpCachedBindposes = null;
            dgo._tmpCachedBoneWeights = null;
        }

        private void _copyUV2unchangedToSeparateRects()
        {
            int padding = 16;
            List<Vector2> uv2AtlasSizes = new List<Vector2>();
            float minSize = 1E+11f;
            float maxSize = 0.0f;
            for (int index = 0; index < mbDynamicObjectsInCombinedMesh.Count; ++index)
            {
                // ISSUE: explicit reference operation
                float magnitude = @mbDynamicObjectsInCombinedMesh[index].meshSize.magnitude;
                if (magnitude > (double)maxSize)
                    maxSize = magnitude;
                if (magnitude < (double)minSize)
                    minSize = magnitude;
            }
            float MAX_UV_VAL = 1000f;
            float MIN_UV_VAL = 10f;
            float offset = 0.0f;
            float scale;
            if (maxSize - (double)minSize > MAX_UV_VAL - (double)MIN_UV_VAL)
            {
                scale = (float)((MAX_UV_VAL - (double)MIN_UV_VAL) / (maxSize - (double)minSize));
                offset = MIN_UV_VAL - minSize * scale;
            }
            else
                scale = MAX_UV_VAL / maxSize;
            for (int i = 0; i < mbDynamicObjectsInCombinedMesh.Count; i++)
            {

                float zz = mbDynamicObjectsInCombinedMesh[i].meshSize.magnitude;
                zz = zz * scale + offset;
                Vector2 sz = Vector2.one * zz;
                uv2AtlasSizes.Add(sz);
            }
            AtlasPackingResult[] rects = new MB2_TexturePacker()
            {
                doPowerOfTwoTextures = false
            }.GetRects(uv2AtlasSizes, 8192, padding);
            for (int index = 0; index < mbDynamicObjectsInCombinedMesh.Count; ++index)
            {
                MB_DynamicGameObject dynamicGameObject = mbDynamicObjectsInCombinedMesh[index];
                float x;
                float num7 = x = uv2s[dynamicGameObject.vertIdx].x;
                float y;
                float num8 = y = uv2s[dynamicGameObject.vertIdx].y;
                int num9 = dynamicGameObject.vertIdx + dynamicGameObject.numVerts;
                for (int vertIdx = dynamicGameObject.vertIdx; vertIdx < num9; ++vertIdx)
                {
                    if (uv2s[vertIdx].x < (double)num7)
                        num7 = uv2s[vertIdx].x;
                    if (uv2s[vertIdx].x > (double)x)
                        x = uv2s[vertIdx].x;
                    if (uv2s[vertIdx].y < (double)num8)
                        num8 = uv2s[vertIdx].y;
                    if (uv2s[vertIdx].y > (double)y)
                        y = uv2s[vertIdx].y;
                }
                Rect rect = rects[0].rects[index];
                for (int vertIdx = dynamicGameObject.vertIdx; vertIdx < num9; ++vertIdx)
                {
                    float num10 = x - num7;
                    float num11 = y - num8;
                    if (num10 == 0.0)
                        num10 = 1f;
                    if (num11 == 0.0)
                        num11 = 1f;
                    // ISSUE: explicit reference operation
                    // ISSUE: explicit reference operation
                    uv2s[vertIdx].x =(float) ((uv2s[vertIdx].x - (double)num7) / num10 * rect.width + rect.x);
                    // ISSUE: explicit reference operation
                    // ISSUE: explicit reference operation
                    uv2s[vertIdx].y = (float) ((uv2s[vertIdx].y - (double)num8) / num11 * (rect).height + (rect).y);
                }
            }
        }

        public override List<Material> GetMaterialsOnTargetRenderer()
        {
            List<Material> materialList = new List<Material>();
            if ((_targetRenderer!= null))
                materialList.AddRange(_targetRenderer.sharedMaterials);
            return materialList;
        }

        [Serializable]
        public class SerializableIntArray
        {
            public int[] data;

            public SerializableIntArray()
            {
            }

            public SerializableIntArray(int len)
            {
                data = new int[len];
            }
        }

        [Serializable]
        public class MB_DynamicGameObject : IComparable<MB_DynamicGameObject>
        {
            public int[] indexesOfBonesUsed = new int[0];
            public int lightmapIndex = -1;
            public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0.0f, 0.0f);
            public Vector3 meshSize = Vector3.one;
            public bool show = true;
            public bool invertTriangles = false;
            public bool _beingDeleted = false;
            public int _triangleIdxAdjustment = 0;
            public int instanceID;
            public string name;
            public int vertIdx;
            public int blendShapeIdx;
            public int numVerts;
            public int numBlendShapes;
            public int[] submeshTriIdxs;
            public int[] submeshNumTris;
            public int[] targetSubmeshIdxs;
            public Rect[] uvRects;
            public Rect[] encapsulatingRect;
            public Rect[] sourceMaterialTiling;
            public Rect[] obUVRects;
            [NonSerialized]
            public SerializableIntArray[] _tmpSubmeshTris;
            [NonSerialized]
            public Transform[] _tmpCachedBones;
            [NonSerialized]
            public Matrix4x4[] _tmpCachedBindposes;
            [NonSerialized]
            public BoneWeight[] _tmpCachedBoneWeights;
            [NonSerialized]
            public int[] _tmpIndexesOfSourceBonesUsed;

            public int CompareTo(MB_DynamicGameObject b)
            {
                return vertIdx - b.vertIdx;
            }
        }

        public class MeshChannels
        {
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector4[] tangents;
            public Vector2[] uv0raw;
            public Vector2[] uv0modified;
            public Vector2[] uv2;
            public Vector2[] uv3;
            public Vector2[] uv4;
            public Color[] colors;
            public BoneWeight[] boneWeights;
            public Matrix4x4[] bindPoses;
            public int[] triangles;
            public MBBlendShape[] blendShapes;
        }

        [Serializable]
        public class MBBlendShapeFrame
        {
            public float frameWeight;
            public Vector3[] vertices;
            public Vector3[] normals;
            public Vector3[] tangents;
        }

        [Serializable]
        public class MBBlendShape
        {
            public int gameObjectID;
            public string name;
            public int indexInSource;
            public MBBlendShapeFrame[] frames;
        }

        public class MeshChannelsCache
        {
            protected Dictionary<int, MeshChannels> meshID2MeshChannels = new Dictionary<int, MeshChannels>();
            private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);
            private MB3_MeshCombinerSingle mc;

            internal MeshChannelsCache(MB3_MeshCombinerSingle mcs)
            {
                mc = mcs;
            }

            internal Vector3[] GetVertices(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.vertices == null)
                    meshChannels.vertices = m.vertices;
                return meshChannels.vertices;
            }

            internal Vector3[] GetNormals(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.normals == null)
                    meshChannels.normals = _getMeshNormals(m);
                return meshChannels.normals;
            }

            internal Vector4[] GetTangents(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.tangents == null)
                    meshChannels.tangents = _getMeshTangents(m);
                return meshChannels.tangents;
            }

            internal Vector2[] GetUv0Raw(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.uv0raw == null)
                    meshChannels.uv0raw = _getMeshUVs(m);
                return meshChannels.uv0raw;
            }

            internal Vector2[] GetUv0Modified(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.uv0modified == null)
                    meshChannels.uv0modified = null;
                return meshChannels.uv0modified;
            }

            internal Vector2[] GetUv2(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.uv2 == null)
                    meshChannels.uv2 = _getMeshUV2s(m);
                return meshChannels.uv2;
            }

            internal Vector2[] GetUv3(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.uv3 == null)
                    meshChannels.uv3 = MBVersion.GetMeshUV3orUV4(m, true, mc.LOG_LEVEL);
                return meshChannels.uv3;
            }

            internal Vector2[] GetUv4(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.uv4 == null)
                    meshChannels.uv4 = MBVersion.GetMeshUV3orUV4(m, false, mc.LOG_LEVEL);
                return meshChannels.uv4;
            }

            internal Color[] GetColors(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.colors == null)
                    meshChannels.colors = _getMeshColors(m);
                return meshChannels.colors;
            }

            internal Matrix4x4[] GetBindposes(Renderer r)
            {
                Mesh mesh = MB_Utility.GetMesh(r.gameObject);
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((mesh).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((mesh).GetInstanceID(), meshChannels);
                }
                if (meshChannels.bindPoses == null)
                    meshChannels.bindPoses = MeshChannelsCache._getBindPoses(r);
                return meshChannels.bindPoses;
            }

            internal BoneWeight[] GetBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
            {
                Mesh mesh = MB_Utility.GetMesh(r.gameObject);
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((mesh).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((mesh).GetInstanceID(), meshChannels);
                }
                if (meshChannels.boneWeights == null)
                    meshChannels.boneWeights = MeshChannelsCache._getBoneWeights(r, numVertsInMeshBeingAdded);
                return meshChannels.boneWeights;
            }

            internal int[] GetTriangles(Mesh m)
            {
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.triangles == null)
                    meshChannels.triangles = m.triangles;
                return meshChannels.triangles;
            }

            internal MBBlendShape[] GetBlendShapes(Mesh m, int gameObjectID)
            {
                if (MBVersion.GetMajorVersion() <= 5 && (MBVersion.GetMajorVersion() != 5 || MBVersion.GetMinorVersion() < 3))
                    return new MBBlendShape[0];
                MeshChannels meshChannels;
                if (!meshID2MeshChannels.TryGetValue((m).GetInstanceID(), out meshChannels))
                {
                    meshChannels = new MeshChannels();
                    meshID2MeshChannels.Add((m).GetInstanceID(), meshChannels);
                }
                if (meshChannels.blendShapes == null)
                {
                    MBBlendShape[] mbBlendShapeArray = new MBBlendShape[m.blendShapeCount];
                    int vertexCount = m.vertexCount;
                    for (int shapeIndex = 0; shapeIndex < mbBlendShapeArray.Length; ++shapeIndex)
                    {
                        MBBlendShape mbBlendShape = mbBlendShapeArray[shapeIndex] = new MBBlendShape();
                        mbBlendShape.frames = new MBBlendShapeFrame[MBVersion.GetBlendShapeFrameCount(m, shapeIndex)];
                        mbBlendShape.name = m.GetBlendShapeName(shapeIndex);
                        mbBlendShape.indexInSource = shapeIndex;
                        mbBlendShape.gameObjectID = gameObjectID;
                        for (int frameIndex = 0; frameIndex < mbBlendShape.frames.Length; ++frameIndex)
                        {
                            MBBlendShapeFrame mbBlendShapeFrame = mbBlendShape.frames[frameIndex] = new MBBlendShapeFrame();
                            mbBlendShapeFrame.frameWeight = MBVersion.GetBlendShapeFrameWeight(m, shapeIndex, frameIndex);
                            mbBlendShapeFrame.vertices = new Vector3[vertexCount];
                            mbBlendShapeFrame.normals = new Vector3[vertexCount];
                            mbBlendShapeFrame.tangents = new Vector3[vertexCount];
                            MBVersion.GetBlendShapeFrameVertices(m, shapeIndex, frameIndex, mbBlendShapeFrame.vertices, mbBlendShapeFrame.normals, mbBlendShapeFrame.tangents);
                        }
                    }
                    meshChannels.blendShapes = mbBlendShapeArray;
                    return meshChannels.blendShapes;
                }
                MBBlendShape[] mbBlendShapeArray1 = new MBBlendShape[meshChannels.blendShapes.Length];
                for (int index = 0; index < mbBlendShapeArray1.Length; ++index)
                {
                    mbBlendShapeArray1[index] = new MBBlendShape();
                    mbBlendShapeArray1[index].name = meshChannels.blendShapes[index].name;
                    mbBlendShapeArray1[index].indexInSource = meshChannels.blendShapes[index].indexInSource;
                    mbBlendShapeArray1[index].frames = meshChannels.blendShapes[index].frames;
                    mbBlendShapeArray1[index].gameObjectID = gameObjectID;
                }
                return mbBlendShapeArray1;
            }

            private Color[] _getMeshColors(Mesh m)
            {
                Color[] colorArray = m.colors;
                if (colorArray.Length == 0)
                {
                    if (mc.LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("Mesh " + m + " has no colors. Generating");
                    if (mc.LOG_LEVEL >= MB2_LogLevel.warn)
                        Debug.LogWarning(("Mesh " + m + " didn't have colors. Generating an array of white colors"));
                    colorArray = new Color[m.vertexCount];
                    for (int index = 0; index < colorArray.Length; ++index)
                        colorArray[index] = Color.white;
                }
                return colorArray;
            }

            private Vector3[] _getMeshNormals(Mesh m)
            {
                Vector3[] ns = m.normals;
                if (ns.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no normals. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have normals. Generating normals.");
                    Mesh tempMesh = (Mesh)GameObject.Instantiate(m);
                    tempMesh.RecalculateNormals();
                    ns = tempMesh.normals;
                    MB_Utility.Destroy(tempMesh);
                }
                return ns;
            }

            private Vector4[] _getMeshTangents(Mesh m)
            {
                Vector4[] ts = m.tangents;
                if (ts.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no tangents. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have tangents. Generating tangents.");
                    Vector3[] verts = m.vertices;
                    Vector2[] uvs = GetUv0Raw(m);
                    Vector3[] norms = _getMeshNormals(m);
                    ts = new Vector4[m.vertexCount];
                    for (int i = 0; i < m.subMeshCount; i++)
                    {
                        int[] tris = m.GetTriangles(i);
                        _generateTangents(tris, verts, uvs, norms, ts);
                    }
                }
                return ts;
            }

            private Vector2[] _getMeshUVs(Mesh m)
            {
                Vector2[] uv = m.uv;
                if (uv.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uvs. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uvs. Generating uvs.");
                    uv = new Vector2[m.vertexCount];
                    for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
                }
                return uv;
            }

            private Vector2[] _getMeshUV2s(Mesh m)
            {
                Vector2[] uv = m.uv2;
                if (uv.Length == 0)
                {
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.debug) MB2_Log.LogDebug("Mesh " + m + " has no uv2s. Generating");
                    if (this.mc.LOG_LEVEL >= MB2_LogLevel.warn) Debug.LogWarning("Mesh " + m + " didn't have uv2s. Generating uv2s.");
                    uv = new Vector2[m.vertexCount];
                    for (int i = 0; i < uv.Length; i++) { uv[i] = _HALF_UV; }
                }
                return uv;
            }

            public static Matrix4x4[] _getBindPoses(Renderer r)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).sharedMesh.bindposes;
                }
                else if (r is MeshRenderer)
                {
                    Matrix4x4 bindPose = Matrix4x4.identity;
                    Matrix4x4[] poses = new Matrix4x4[1];
                    poses[0] = bindPose;
                    return poses;
                }
                else
                {
                    Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
                    return null;
                }
            }

            public static BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
            {
                if (r is SkinnedMeshRenderer)
                {
                    return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
                }
                else if (r is MeshRenderer)
                {
                    BoneWeight bw = new BoneWeight();
                    bw.boneIndex0 = bw.boneIndex1 = bw.boneIndex2 = bw.boneIndex3 = 0;
                    bw.weight0 = 1f;
                    bw.weight1 = bw.weight2 = bw.weight3 = 0f;
                    BoneWeight[] bws = new BoneWeight[numVertsInMeshBeingAdded];
                    for (int i = 0; i < bws.Length; i++) bws[i] = bw;
                    return bws;
                }
                else
                {
                    Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
                    return null;
                }
            }

            private void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
            {
                 int triangleCount = triangles.Length;
                int vertexCount = verts.Length;

                Vector3[] tan1 = new Vector3[vertexCount];
                Vector3[] tan2 = new Vector3[vertexCount];

                for (int a = 0; a < triangleCount; a += 3)
                {
                    int i1 = triangles[a + 0];
                    int i2 = triangles[a + 1];
                    int i3 = triangles[a + 2];

                    Vector3 v1 = verts[i1];
                    Vector3 v2 = verts[i2];
                    Vector3 v3 = verts[i3];

                    Vector2 w1 = uvs[i1];
                    Vector2 w2 = uvs[i2];
                    Vector2 w3 = uvs[i3];

                    float x1 = v2.x - v1.x;
                    float x2 = v3.x - v1.x;
                    float y1 = v2.y - v1.y;
                    float y2 = v3.y - v1.y;
                    float z1 = v2.z - v1.z;
                    float z2 = v3.z - v1.z;

                    float s1 = w2.x - w1.x;
                    float s2 = w3.x - w1.x;
                    float t1 = w2.y - w1.y;
                    float t2 = w3.y - w1.y;

                    float rBot = (s1 * t2 - s2 * t1);
                    if (rBot == 0f)
                    {
                        Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
                        return;
                    }
                    float r = 1.0f / rBot;

                    Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
                    Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

                    tan1[i1] += sdir;
                    tan1[i2] += sdir;
                    tan1[i3] += sdir;

                    tan2[i1] += tdir;
                    tan2[i2] += tdir;
                    tan2[i3] += tdir;
                }


                for (int a = 0; a < vertexCount; ++a)
                {
                    Vector3 n = normals[a];
                    Vector3 t = tan1[a];

                    Vector3 tmp = (t - n * Vector3.Dot(n, t)).normalized;
                    outTangents[a] = new Vector4(tmp.x, tmp.y, tmp.z);
                    outTangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
                }
            }
        }

        public struct BoneAndBindpose
        {
            public Transform bone;
            public Matrix4x4 bindPose;

            public BoneAndBindpose(Transform t, Matrix4x4 bp)
            {
                bone = t;
                bindPose = bp;
            }
            public override bool Equals(object obj)
            {
                if (obj is BoneAndBindpose)
                {
                    if (bone == ((BoneAndBindpose)obj).bone && bindPose == ((BoneAndBindpose)obj).bindPose)
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int GetHashCode()
            {
                return (bone.GetInstanceID() % 2147483647) ^ (int)bindPose[0, 0];
            }
        }
    }
}

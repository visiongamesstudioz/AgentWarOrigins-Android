using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public class MB3_MultiMeshCombiner : MB3_MeshCombiner
    {
        private static GameObject[] empty = new GameObject[0];
        private static int[] emptyIDs = new int[0];
        public Dictionary<int, CombinedMesh> obj2MeshCombinerMap = new Dictionary<int, CombinedMesh>();
        [SerializeField]
        public List<CombinedMesh> meshCombiners = new List<CombinedMesh>();
        [SerializeField]
        private int _maxVertsInMesh = (int)ushort.MaxValue;

        public override MB2_LogLevel LOG_LEVEL
        {
            get
            {
                return _LOG_LEVEL;
            }
            set
            {
                _LOG_LEVEL = value;
                for (int index = 0; index < meshCombiners.Count; ++index)
                    meshCombiners[index].combinedMesh.LOG_LEVEL = value;
            }
        }

        public override MB2_ValidationLevel validationLevel
        {
            set
            {
                _validationLevel = value;
                for (int index = 0; index < meshCombiners.Count; ++index)
                    meshCombiners[index].combinedMesh.validationLevel = _validationLevel;
            }
            get
            {
                return _validationLevel;
            }
        }

        public int maxVertsInMesh
        {
            get
            {
                return _maxVertsInMesh;
            }
            set
            {
                if (obj2MeshCombinerMap.Count > 0)
                    return;
                if (value < 3)
                    Debug.LogError("Max verts in mesh must be greater than three.");
                else if (value > (int)ushort.MaxValue)
                    Debug.LogError("Meshes in unity cannot have more than 65535 vertices.");
                else
                    _maxVertsInMesh = value;
            }
        }

        public override int GetNumObjectsInCombined()
        {
            return obj2MeshCombinerMap.Count;
        }

        public override int GetNumVerticesFor(GameObject go)
        {
            CombinedMesh combinedMesh = (CombinedMesh)null;
            if (obj2MeshCombinerMap.TryGetValue((go).GetInstanceID(), out combinedMesh))
                return combinedMesh.combinedMesh.GetNumVerticesFor(go);
            return -1;
        }

        public override int GetNumVerticesFor(int gameObjectID)
        {
            CombinedMesh combinedMesh = (CombinedMesh)null;
            if (obj2MeshCombinerMap.TryGetValue(gameObjectID, out combinedMesh))
                return combinedMesh.combinedMesh.GetNumVerticesFor(gameObjectID);
            return -1;
        }

        public override List<GameObject> GetObjectsInCombined()
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            for (int index = 0; index < meshCombiners.Count; ++index)
                gameObjectList.AddRange((IEnumerable<GameObject>)meshCombiners[index].combinedMesh.GetObjectsInCombined());
            return gameObjectList;
        }

        public override int GetLightmapIndex()
        {
            if (meshCombiners.Count > 0)
                return meshCombiners[0].combinedMesh.GetLightmapIndex();
            return -1;
        }

        public override bool CombinedMeshContains(GameObject go)
        {
            return obj2MeshCombinerMap.ContainsKey((go).GetInstanceID());
        }

        private bool _validateTextureBakeResults()
        {
            if (_textureBakeResults== null)
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
                if (_textureBakeResults.materialsAndUVRects != null && _textureBakeResults.materialsAndUVRects.Length != 0 && !_textureBakeResults.doMultiMaterial && (_textureBakeResults.resultMaterial!= null))
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

        public override void Apply(GenerateUV2Delegate uv2GenerationMethod)
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
            {
                if (meshCombiners[index].isDirty)
                {
                    meshCombiners[index].combinedMesh.Apply(uv2GenerationMethod);
                    meshCombiners[index].isDirty = false;
                }
            }
        }

        public override void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapesFlag = false, GenerateUV2Delegate uv2GenerationMethod = null)
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
            {
                if (meshCombiners[index].isDirty)
                {
                    meshCombiners[index].combinedMesh.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, blendShapesFlag, uv2GenerationMethod);
                    meshCombiners[index].isDirty = false;
                }
            }
        }

        public override void UpdateSkinnedMeshApproximateBounds()
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
                meshCombiners[index].combinedMesh.UpdateSkinnedMeshApproximateBounds();
        }

        public override void UpdateSkinnedMeshApproximateBoundsFromBones()
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
                meshCombiners[index].combinedMesh.UpdateSkinnedMeshApproximateBoundsFromBones();
        }

        public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
                meshCombiners[index].combinedMesh.UpdateSkinnedMeshApproximateBoundsFromBounds();
        }

        public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false)
        {
            if (gos == null)
            {
                Debug.LogError("list of game objects cannot be null");
            }
            else
            {
                for (int index = 0; index < meshCombiners.Count; ++index)
                    meshCombiners[index].gosToUpdate.Clear();
                for (int index = 0; index < gos.Length; ++index)
                {
                    CombinedMesh combinedMesh = (CombinedMesh)null;
                    obj2MeshCombinerMap.TryGetValue((gos[index]).GetInstanceID(), out combinedMesh);
                    if (combinedMesh != null)
                        combinedMesh.gosToUpdate.Add(gos[index]);
                    else
                        Debug.LogWarning(("Object " + gos[index] + " is not in the combined mesh."));
                }
                for (int index = 0; index < meshCombiners.Count; ++index)
                {
                    if (meshCombiners[index].gosToUpdate.Count > 0)
                    {
                        meshCombiners[index].isDirty = true;
                        GameObject[] array = meshCombiners[index].gosToUpdate.ToArray();
                        meshCombiners[index].combinedMesh.UpdateGameObjects(array, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
                    }
                }
            }
        }

        public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
        {
            int[] deleteGOinstanceIDs = (int[])null;
            if (deleteGOs != null)
            {
                deleteGOinstanceIDs = new int[deleteGOs.Length];
                for (int index = 0; index < deleteGOs.Length; ++index)
                {
                    if ((deleteGOs[index]== null))
                        Debug.LogError(("The " + index + "th object on the list of objects to delete is 'Null'"));
                    else
                        deleteGOinstanceIDs[index] = (deleteGOs[index]).GetInstanceID();
                }
            }
            return AddDeleteGameObjectsByID(gos, deleteGOinstanceIDs, disableRendererInSource);
        }

        public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource = true)
        {
            if (_usingTemporaryTextureBakeResult && gos != null && (uint)gos.Length > 0U)
            {
                MB_Utility.Destroy(_textureBakeResults);
                _textureBakeResults = (MB2_TextureBakeResults)null;
                _usingTemporaryTextureBakeResult = false;
            }
            if ((_textureBakeResults== null) && gos != null && gos.Length != 0 && (gos[0]!= null) && !_CreateTemporaryTextrueBakeResult(gos, GetMaterialsOnTargetRenderer()) || !_validate(gos, deleteGOinstanceIDs))
                return false;
            _distributeAmongBakers(gos, deleteGOinstanceIDs);
            if (LOG_LEVEL >= MB2_LogLevel.debug)
                MB2_Log.LogDebug("MB2_MultiMeshCombiner.AddDeleteGameObjects numCombinedMeshes: " + meshCombiners.Count + " added:" + gos + " deleted:" + deleteGOinstanceIDs + " disableRendererInSource:" + disableRendererInSource.ToString() + " maxVertsPerCombined:" + _maxVertsInMesh);
            return _bakeStep1(gos, deleteGOinstanceIDs, disableRendererInSource);
        }

        private bool _validate(GameObject[] gos, int[] deleteGOinstanceIDs)
        {
            if (_validationLevel == MB2_ValidationLevel.none)
                return true;
            if (_maxVertsInMesh < 3)
                Debug.LogError(("Invalid value for maxVertsInMesh=" + _maxVertsInMesh));
            _validateTextureBakeResults();
            if (gos != null)
            {
                for (int index1 = 0; index1 < gos.Length; ++index1)
                {
                    if ((gos[index1]== null))
                    {
                        Debug.LogError(("The " + index1 + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element."));
                        return false;
                    }
                    if (_validationLevel >= MB2_ValidationLevel.robust)
                    {
                        for (int index2 = index1 + 1; index2 < gos.Length; ++index2)
                        {
                            if ((gos[index1]== gos[index2]))
                            {
                                Debug.LogError(("GameObject " + gos[index1] + "appears twice in list of game objects to add"));
                                return false;
                            }
                        }
                        if (obj2MeshCombinerMap.ContainsKey((gos[index1]).GetInstanceID()))
                        {
                            bool flag = false;
                            if (deleteGOinstanceIDs != null)
                            {
                                for (int index2 = 0; index2 < deleteGOinstanceIDs.Length; ++index2)
                                {
                                    if (deleteGOinstanceIDs[index2] == (gos[index1]).GetInstanceID())
                                        flag = true;
                                }
                            }
                            if (!flag)
                            {
                                Debug.LogError(("GameObject " + gos[index1] + " is already in the combined mesh " + (gos[index1]).GetInstanceID()));
                                return false;
                            }
                        }
                    }
                }
            }
            if (deleteGOinstanceIDs != null && _validationLevel >= MB2_ValidationLevel.robust)
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
                    if (!obj2MeshCombinerMap.ContainsKey(deleteGOinstanceIDs[index1]))
                        Debug.LogWarning(("GameObject with instance ID " + deleteGOinstanceIDs[index1] + " on the list of objects to delete is not in the combined mesh."));
                }
            }
            return true;
        }

        private void _distributeAmongBakers(GameObject[] gos, int[] deleteGOinstanceIDs)
        {
            if (gos == null)
                gos = empty;
            if (deleteGOinstanceIDs == null)
                deleteGOinstanceIDs = emptyIDs;
            if ((resultSceneObject== null))
                resultSceneObject = new GameObject("CombinedMesh-" + name);
            for (int index = 0; index < meshCombiners.Count; ++index)
                meshCombiners[index].extraSpace = _maxVertsInMesh - meshCombiners[index].combinedMesh.GetMesh().vertexCount;
            for (int index = 0; index < deleteGOinstanceIDs.Length; ++index)
            {
                CombinedMesh combinedMesh = (CombinedMesh)null;
                if (obj2MeshCombinerMap.TryGetValue(deleteGOinstanceIDs[index], out combinedMesh))
                {
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("MB2_MultiMeshCombiner.Removing " + deleteGOinstanceIDs[index] + " from meshCombiner " + meshCombiners.IndexOf(combinedMesh));
                    combinedMesh.numVertsInListToDelete += combinedMesh.combinedMesh.GetNumVerticesFor(deleteGOinstanceIDs[index]);
                    combinedMesh.gosToDelete.Add(deleteGOinstanceIDs[index]);
                }
                else
                    Debug.LogWarning(("Object " + deleteGOinstanceIDs[index] + " in the list of objects to delete is not in the combined mesh."));
            }
            for (int index1 = 0; index1 < gos.Length; ++index1)
            {
                GameObject go = gos[index1];
                int vertexCount = MB_Utility.GetMesh(go).vertexCount;
                CombinedMesh combinedMesh = (CombinedMesh)null;
                for (int index2 = 0; index2 < meshCombiners.Count; ++index2)
                {
                    if (meshCombiners[index2].extraSpace + meshCombiners[index2].numVertsInListToDelete - meshCombiners[index2].numVertsInListToAdd > vertexCount)
                    {
                        combinedMesh = meshCombiners[index2];
                        if (LOG_LEVEL >= MB2_LogLevel.debug)
                        {
                            MB2_Log.LogDebug("MB2_MultiMeshCombiner.Added " + gos[index1] + " to combinedMesh " + index2, LOG_LEVEL);
                            break;
                        }
                        break;
                    }
                }
                if (combinedMesh == null)
                {
                    combinedMesh = new CombinedMesh(maxVertsInMesh, _resultSceneObject, _LOG_LEVEL);
                    _setMBValues(combinedMesh.combinedMesh);
                    meshCombiners.Add(combinedMesh);
                    if (LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("MB2_MultiMeshCombiner.Created new combinedMesh");
                }
                combinedMesh.gosToAdd.Add(go);
                combinedMesh.numVertsInListToAdd += vertexCount;
            }
        }

        private bool _bakeStep1(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
            {
                CombinedMesh meshCombiner = meshCombiners[index];
                if ((meshCombiner.combinedMesh.targetRenderer== null))
                {
                    meshCombiner.combinedMesh.resultSceneObject = _resultSceneObject;
                    meshCombiner.combinedMesh.BuildSceneMeshObject(gos, true);
                    if (_LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("BuildSO combiner {0} goID {1} targetRenID {2} meshID {3}", index, (meshCombiner.combinedMesh.targetRenderer.gameObject).GetInstanceID(), (meshCombiner.combinedMesh.targetRenderer).GetInstanceID(), (meshCombiner.combinedMesh.GetMesh()).GetInstanceID());
                }
                else if (((meshCombiner.combinedMesh.targetRenderer).transform.parent!= resultSceneObject.transform))
                {
                    Debug.LogError("targetRender objects must be children of resultSceneObject");
                    return false;
                }
                if (meshCombiner.gosToAdd.Count > 0 || meshCombiner.gosToDelete.Count > 0)
                {
                    meshCombiner.combinedMesh.AddDeleteGameObjectsByID(meshCombiner.gosToAdd.ToArray(), meshCombiner.gosToDelete.ToArray(), disableRendererInSource);
                    if (_LOG_LEVEL >= MB2_LogLevel.debug)
                        MB2_Log.LogDebug("Baked combiner {0} obsAdded {1} objsRemoved {2} goID {3} targetRenID {4} meshID {5}", index, meshCombiner.gosToAdd.Count, meshCombiner.gosToDelete.Count, (meshCombiner.combinedMesh.targetRenderer.gameObject).GetInstanceID(), (meshCombiner.combinedMesh.targetRenderer).GetInstanceID(), (meshCombiner.combinedMesh.GetMesh()).GetInstanceID());
                }
                Renderer targetRenderer = meshCombiner.combinedMesh.targetRenderer;
                Mesh mesh = meshCombiner.combinedMesh.GetMesh();
                if (targetRenderer is MeshRenderer)
                    (targetRenderer.gameObject.GetComponent<MeshFilter>()).sharedMesh= mesh;
                else
                    ((SkinnedMeshRenderer)targetRenderer).sharedMesh=mesh;
            }
            for (int index1 = 0; index1 < meshCombiners.Count; ++index1)
            {
                CombinedMesh meshCombiner = meshCombiners[index1];
                for (int index2 = 0; index2 < meshCombiner.gosToDelete.Count; ++index2)
                    obj2MeshCombinerMap.Remove(meshCombiner.gosToDelete[index2]);
            }
            for (int index1 = 0; index1 < meshCombiners.Count; ++index1)
            {
                CombinedMesh meshCombiner = meshCombiners[index1];
                for (int index2 = 0; index2 < meshCombiner.gosToAdd.Count; ++index2)
                    obj2MeshCombinerMap.Add((meshCombiner.gosToAdd[index2]).GetInstanceID(), meshCombiner);
                if (meshCombiner.gosToAdd.Count > 0 || meshCombiner.gosToDelete.Count > 0)
                {
                    meshCombiner.gosToDelete.Clear();
                    meshCombiner.gosToAdd.Clear();
                    meshCombiner.numVertsInListToDelete = 0;
                    meshCombiner.numVertsInListToAdd = 0;
                    meshCombiner.isDirty = true;
                }
            }
            if (LOG_LEVEL >= MB2_LogLevel.debug)
            {
                string str = "Meshes in combined:";
                for (int index = 0; index < meshCombiners.Count; ++index)
                    str = str + " mesh" + index + "(" + meshCombiners[index].combinedMesh.GetObjectsInCombined().Count + ")\n";
                MB2_Log.LogDebug(str + "children in result: " + resultSceneObject.transform.childCount, LOG_LEVEL);
            }
            return meshCombiners.Count > 0;
        }

        public override Dictionary<MBBlendShapeKey, MBBlendShapeValue> BuildSourceBlendShapeToCombinedIndexMap()
        {
            Dictionary<MBBlendShapeKey, MBBlendShapeValue> dictionary = new Dictionary<MBBlendShapeKey, MBBlendShapeValue>();
            for (int index1 = 0; index1 < meshCombiners.Count; ++index1)
            {
                for (int index2 = 0; index2 < meshCombiners[index1].combinedMesh.blendShapes.Length; ++index2)
                {
                    MB3_MeshCombinerSingle.MBBlendShape blendShape = meshCombiners[index1].combinedMesh.blendShapes[index2];
                    dictionary.Add(new MBBlendShapeKey(blendShape.gameObjectID, blendShape.indexInSource), new MBBlendShapeValue()
                    {
                        combinedMeshGameObject = meshCombiners[index1].combinedMesh.targetRenderer.gameObject,
                        blendShapeIndex = index2
                    });
                }
            }
            return dictionary;
        }

        public override void ClearBuffers()
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
                meshCombiners[index].combinedMesh.ClearBuffers();
            obj2MeshCombinerMap.Clear();
        }

        public override void ClearMesh()
        {
            DestroyMesh();
        }

        public override void DestroyMesh()
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
            {
                if ((meshCombiners[index].combinedMesh.targetRenderer!= null))
                    MB_Utility.Destroy(((Component)meshCombiners[index].combinedMesh.targetRenderer).gameObject);
                meshCombiners[index].combinedMesh.ClearMesh();
            }
            obj2MeshCombinerMap.Clear();
            meshCombiners.Clear();
        }

        public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
        {
            for (int index = 0; index < meshCombiners.Count; ++index)
            {
                if ((meshCombiners[index].combinedMesh.targetRenderer!= null))
                    editorMethods.Destroy(((Component)meshCombiners[index].combinedMesh.targetRenderer).gameObject);
                meshCombiners[index].combinedMesh.ClearMesh();
            }
            obj2MeshCombinerMap.Clear();
            meshCombiners.Clear();
        }

        private void _setMBValues(MB3_MeshCombinerSingle targ)
        {
            targ.validationLevel = _validationLevel;
            targ.renderType = renderType;
            targ.outputOption = MB2_OutputOptions.bakeIntoSceneObject;
            targ.lightmapOption = lightmapOption;
            targ.textureBakeResults = textureBakeResults;
            targ.doNorm = doNorm;
            targ.doTan = doTan;
            targ.doCol = doCol;
            targ.doUV = doUV;
            targ.doUV3 = doUV3;
            targ.doUV4 = doUV4;
            targ.doBlendShapes = doBlendShapes;
            targ.optimizeAfterBake = optimizeAfterBake;
            targ.recenterVertsToBoundsCenter = recenterVertsToBoundsCenter;
            targ.uv2UnwrappingParamsHardAngle = uv2UnwrappingParamsHardAngle;
            targ.uv2UnwrappingParamsPackMargin = uv2UnwrappingParamsPackMargin;
        }

        public override List<Material> GetMaterialsOnTargetRenderer()
        {
            HashSet<Material> materialSet = new HashSet<Material>();
            for (int index = 0; index < meshCombiners.Count; ++index)
                materialSet.UnionWith((IEnumerable<Material>)meshCombiners[index].combinedMesh.GetMaterialsOnTargetRenderer());
            return new List<Material>((IEnumerable<Material>)materialSet);
        }

        public override void CheckIntegrity()
        {
        }

        [Serializable]
        public class CombinedMesh
        {
            public int extraSpace = -1;
            public int numVertsInListToDelete = 0;
            public int numVertsInListToAdd = 0;
            public bool isDirty = false;
            public MB3_MeshCombinerSingle combinedMesh;
            public List<GameObject> gosToAdd;
            public List<int> gosToDelete;
            public List<GameObject> gosToUpdate;

            public CombinedMesh(int maxNumVertsInMesh, GameObject resultSceneObject, MB2_LogLevel ll)
            {
                combinedMesh = new MB3_MeshCombinerSingle();
                combinedMesh.resultSceneObject = resultSceneObject;
                combinedMesh.LOG_LEVEL = ll;
                extraSpace = maxNumVertsInMesh;
                numVertsInListToDelete = 0;
                numVertsInListToAdd = 0;
                gosToAdd = new List<GameObject>();
                gosToDelete = new List<int>();
                gosToUpdate = new List<GameObject>();
            }

            public bool isEmpty()
            {
                List<GameObject> gameObjectList = new List<GameObject>();
                gameObjectList.AddRange((IEnumerable<GameObject>)combinedMesh.GetObjectsInCombined());
                for (int index1 = 0; index1 < gosToDelete.Count; ++index1)
                {
                    for (int index2 = 0; index2 < gameObjectList.Count; ++index2)
                    {
                        if ((gameObjectList[index2]).GetInstanceID() == gosToDelete[index1])
                        {
                            gameObjectList.RemoveAt(index2);
                            break;
                        }
                    }
                }
                return gameObjectList.Count == 0;
            }
        }
    }
}

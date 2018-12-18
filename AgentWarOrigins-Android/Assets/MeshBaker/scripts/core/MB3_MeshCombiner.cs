using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public abstract class MB3_MeshCombiner
    {
        [SerializeField]
        protected MB2_LogLevel _LOG_LEVEL = MB2_LogLevel.info;
        [SerializeField]
        protected MB2_ValidationLevel _validationLevel = MB2_ValidationLevel.robust;
        [SerializeField]
        protected MB2_LightmapOptions _lightmapOption = MB2_LightmapOptions.ignore_UV2;
        [SerializeField]
        protected bool _doNorm = true;
        [SerializeField]
        protected bool _doTan = true;
        [SerializeField]
        protected bool _doUV = true;
        [SerializeField]
        protected bool _recenterVertsToBoundsCenter = false;
        [SerializeField]
        public bool _optimizeAfterBake = true;
        [SerializeField]
        public float uv2UnwrappingParamsHardAngle = 60f;
        [SerializeField]
        public float uv2UnwrappingParamsPackMargin = 0.005f;
        [SerializeField]
        protected string _name;
        [SerializeField]
        protected MB2_TextureBakeResults _textureBakeResults;
        [SerializeField]
        protected GameObject _resultSceneObject;
        [SerializeField]
        protected Renderer _targetRenderer;
        [SerializeField]
        protected MB_RenderType _renderType;
        [SerializeField]
        protected MB2_OutputOptions _outputOption;
        [SerializeField]
        protected bool _doCol;
        [SerializeField]
        protected bool _doUV3;
        [SerializeField]
        protected bool _doUV4;
        [SerializeField]
        protected bool _doBlendShapes;
        protected bool _usingTemporaryTextureBakeResult;

        public static bool EVAL_VERSION
        {
            get
            {
                return false;
            }
        }

        public virtual MB2_LogLevel LOG_LEVEL
        {
            get
            {
                return _LOG_LEVEL;
            }
            set
            {
                _LOG_LEVEL = value;
            }
        }

        public virtual MB2_ValidationLevel validationLevel
        {
            get
            {
                return _validationLevel;
            }
            set
            {
                _validationLevel = value;
            }
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public virtual MB2_TextureBakeResults textureBakeResults
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

        public virtual GameObject resultSceneObject
        {
            get
            {
                return _resultSceneObject;
            }
            set
            {
                _resultSceneObject = value;
            }
        }

        public virtual Renderer targetRenderer
        {
            get
            {
                return _targetRenderer;
            }
            set
            {
                if (_targetRenderer!= null && _targetRenderer!=value)
                    Debug.LogWarning("Previous targetRenderer was not null. Combined mesh may be being used by more than one Renderer");
                _targetRenderer = value;
            }
        }

        public virtual MB_RenderType renderType
        {
            get
            {
                return _renderType;
            }
            set
            {
                _renderType = value;
            }
        }

        public virtual MB2_OutputOptions outputOption
        {
            get
            {
                return _outputOption;
            }
            set
            {
                _outputOption = value;
            }
        }

        public virtual MB2_LightmapOptions lightmapOption
        {
            get
            {
                return _lightmapOption;
            }
            set
            {
                _lightmapOption = value;
            }
        }

        public virtual bool doNorm
        {
            get
            {
                return _doNorm;
            }
            set
            {
                _doNorm = value;
            }
        }

        public virtual bool doTan
        {
            get
            {
                return _doTan;
            }
            set
            {
                _doTan = value;
            }
        }

        public virtual bool doCol
        {
            get
            {
                return _doCol;
            }
            set
            {
                _doCol = value;
            }
        }

        public virtual bool doUV
        {
            get
            {
                return _doUV;
            }
            set
            {
                _doUV = value;
            }
        }

        public virtual bool doUV1
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public virtual bool doUV2()
        {
            return _lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged || _lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || _lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged_to_separate_rects;
        }

        public virtual bool doUV3
        {
            get
            {
                return _doUV3;
            }
            set
            {
                _doUV3 = value;
            }
        }

        public virtual bool doUV4
        {
            get
            {
                return _doUV4;
            }
            set
            {
                _doUV4 = value;
            }
        }

        public virtual bool doBlendShapes
        {
            get
            {
                return _doBlendShapes;
            }
            set
            {
                _doBlendShapes = value;
            }
        }

        public virtual bool recenterVertsToBoundsCenter
        {
            get
            {
                return _recenterVertsToBoundsCenter;
            }
            set
            {
                _recenterVertsToBoundsCenter = value;
            }
        }

        public bool optimizeAfterBake
        {
            get
            {
                return _optimizeAfterBake;
            }
            set
            {
                _optimizeAfterBake = value;
            }
        }

        public abstract int GetLightmapIndex();

        public abstract void ClearBuffers();

        public abstract void ClearMesh();

        public abstract void DestroyMesh();

        public abstract void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods);

        public abstract List<GameObject> GetObjectsInCombined();

        public abstract int GetNumObjectsInCombined();

        public abstract int GetNumVerticesFor(GameObject go);

        public abstract int GetNumVerticesFor(int instanceID);

        public abstract Dictionary<MBBlendShapeKey, MBBlendShapeValue> BuildSourceBlendShapeToCombinedIndexMap();

        public virtual void Apply()
        {
            Apply((GenerateUV2Delegate)null);
        }

        public abstract void Apply(GenerateUV2Delegate uv2GenerationMethod);

        public abstract void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, bool blendShapeFlag = false, GenerateUV2Delegate uv2GenerationMethod = null);

        public abstract void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false);

        public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);

        public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource);

        public abstract bool CombinedMeshContains(GameObject go);

        public abstract void UpdateSkinnedMeshApproximateBounds();

        public abstract void UpdateSkinnedMeshApproximateBoundsFromBones();

        public abstract void CheckIntegrity();

        public abstract void UpdateSkinnedMeshApproximateBoundsFromBounds();

        public static void UpdateSkinnedMeshApproximateBoundsFromBonesStatic(Transform[] bs, SkinnedMeshRenderer smr)
        {
            Vector3 max, min;
            max = bs[0].position;
            min = bs[0].position;
            for (int i = 1; i < bs.Length; i++)
            {
                Vector3 v = bs[i].position;
                if (v.x < min.x) min.x = v.x;
                if (v.y < min.y) min.y = v.y;
                if (v.z < min.z) min.z = v.z;
                if (v.x > max.x) max.x = v.x;
                if (v.y > max.y) max.y = v.y;
                if (v.z > max.z) max.z = v.z;
            }
            Vector3 center = (max + min) / 2f;
            Vector3 size = max - min;
            Matrix4x4 w2l = smr.worldToLocalMatrix;
            Bounds b = new Bounds(w2l * center, w2l * size);
            smr.localBounds = b;
        }

        public static void UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(List<GameObject> objectsInCombined, SkinnedMeshRenderer smr)
        {
            Bounds b = new Bounds();
            Bounds bigB = new Bounds();
            if (MB_Utility.GetBounds(objectsInCombined[0], out b))
            {
                bigB = b;
            }
            else
            {
                Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
                return;
            }
            for (int i = 1; i < objectsInCombined.Count; i++)
            {
                if (MB_Utility.GetBounds(objectsInCombined[i], out b))
                {
                    bigB.Encapsulate(b);
                }
                else
                {
                    Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
                    return;
                }
            }
            smr.localBounds = bigB;
        }

        protected virtual bool _CreateTemporaryTextrueBakeResult(GameObject[] gos, List<Material> matsOnTargetRenderer)
        {
            if (GetNumObjectsInCombined() > 0)
            {
                Debug.LogError("Can't add objects if there are already objects in combined mesh when 'Texture Bake Result' is not set. Perhaps enable 'Clear Buffers After Bake'");
                return false;
            }
            _usingTemporaryTextureBakeResult = true;
            _textureBakeResults = MB2_TextureBakeResults.CreateForMaterialsOnRenderer(gos, matsOnTargetRenderer);
            return true;
        }

        public abstract List<Material> GetMaterialsOnTargetRenderer();

        public delegate void GenerateUV2Delegate(Mesh m, float hardAngle, float packMargin);

        public class MBBlendShapeKey
        {
            public int gameObjecID;
            public int blendShapeIndexInSrc;

            public MBBlendShapeKey(int srcSkinnedMeshRenderGameObjectID, int blendShapeIndexInSource)
            {
                gameObjecID = srcSkinnedMeshRenderGameObjectID;
                blendShapeIndexInSrc = blendShapeIndexInSource;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is MBBlendShapeKey) || obj == null)
                    return false;
                MBBlendShapeKey mbBlendShapeKey = (MBBlendShapeKey)obj;
                return gameObjecID == mbBlendShapeKey.gameObjecID && blendShapeIndexInSrc == mbBlendShapeKey.blendShapeIndexInSrc;
            }

            public override int GetHashCode()
            {
                return (23 * 31 + gameObjecID) * 31 + blendShapeIndexInSrc;
            }
        }

        public class MBBlendShapeValue
        {
            public GameObject combinedMeshGameObject;
            public int blendShapeIndex;
        }
    }
}

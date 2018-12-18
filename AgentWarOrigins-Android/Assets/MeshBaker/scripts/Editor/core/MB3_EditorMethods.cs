// Decompiled with JetBrains decompiler
// Type: DigitalOpus.MB.Core.MB3_EditorMethods
// Assembly: MeshBakerEvalVersionEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2A8B8E34-4AF9-4434-BAA9-EF1230564558
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\Editor\core\MeshBakerEvalVersionEditor.dll

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DigitalOpus.MB.Core
{
    public class MB3_EditorMethods : MB2_EditorMethodsInterface
    {
        private saveTextureFormat SAVE_FORMAT = saveTextureFormat.png;
        private List<Texture2D> _texturesWithReadWriteFlagSet = new List<Texture2D>();
        private Dictionary<Texture2D, TextureFormatInfo> _textureFormatMap = new Dictionary<Texture2D, TextureFormatInfo>();
        public MobileTextureSubtarget AndroidBuildTexCompressionSubtarget;
        public MobileTextureSubtarget TizenBuildTexCompressionSubtarget;

        public void Clear()
        {
            _texturesWithReadWriteFlagSet.Clear();
            _textureFormatMap.Clear();
        }

        public void OnPreTextureBake()
        {
            AndroidBuildTexCompressionSubtarget = (MobileTextureSubtarget)0;
            TizenBuildTexCompressionSubtarget = (MobileTextureSubtarget)0;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android && EditorUserBuildSettings.androidBuildSubtarget > 0)
            {
                AndroidBuildTexCompressionSubtarget = EditorUserBuildSettings.androidBuildSubtarget;
                EditorUserBuildSettings.androidBuildSubtarget=((MobileTextureSubtarget)0);
            }
        }

        public void OnPostTextureBake()
        {
            if (AndroidBuildTexCompressionSubtarget > 0)
            {
                EditorUserBuildSettings.androidBuildSubtarget=(AndroidBuildTexCompressionSubtarget);
                AndroidBuildTexCompressionSubtarget = (MobileTextureSubtarget)0;
            }
            if (TizenBuildTexCompressionSubtarget <= 0)
                return;
            TizenBuildTexCompressionSubtarget = (MobileTextureSubtarget)0;
        }

        public bool IsNormalMap(Texture2D tx)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            return (atPath!= null) && atPath is TextureImporter && ((TextureImporter)atPath).textureType == TextureImporterType.NormalMap;
        }

        public void AddTextureFormat(Texture2D tx, bool isNormalMap)
        {
            TextureFormatInfo toThisFormat = new TextureFormatInfo((TextureImporterCompression)0, false, MBVersionEditor.GetPlatformString(), (TextureImporterFormat)4, isNormalMap);
            SetTextureFormat(tx, toThisFormat, true, false);
        }

        private void SetTextureFormat(Texture2D tx, TextureFormatInfo toThisFormat, bool addToList, bool setNormalMap)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if (atPath== null || !(atPath is TextureImporter))
                return;
            TextureImporter textureImporter = (TextureImporter)atPath;
            bool flag = !Application.unityVersion.StartsWith("2017") ? _SetTextureFormatUnity5(tx, toThisFormat, addToList, setNormalMap, textureImporter) : _SetTextureFormat2017(tx, toThisFormat, addToList, setNormalMap, textureImporter);
            if (textureImporter.textureType == TextureImporterType.NormalMap && !setNormalMap)
            {
                textureImporter.textureType=((TextureImporterType)0);
                flag = true;
            }
            if (textureImporter.textureType != TextureImporterType.NormalMap & setNormalMap)
            {
                textureImporter.textureType=((TextureImporterType)1);
                flag = true;
            }
            if (flag)
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx), (ImportAssetOptions)1);
        }

        private bool _SetTextureFormat2017(Texture2D tx, TextureFormatInfo toThisFormat, bool addToList, bool setNormalMap, TextureImporter textureImporter)
        {
            if (!Application.unityVersion.StartsWith("2017"))
            {
                Debug.LogError(("Wrong texture format converter. 2017 Should not be called for Unity Version " + Application.unityVersion));
                return false;
            }
            bool flag = false;
            TextureFormatInfo textureFormatInfo = new TextureFormatInfo(textureImporter.textureCompression, textureImporter.crunchedCompression, toThisFormat.platform, (TextureImporterFormat)4, textureImporter.textureType == TextureImporterType.NormalMap);
            string str = (!addToList ? "Restoring texture crunch compression for " : "Setting texture crunch compression for ") + string.Format("{0}  to compression={1} isNormal={2} ", tx, toThisFormat.doCrunchCompression, setNormalMap);
            if (toThisFormat.platform != null)
                str += string.Format(" setting platform override format for platform {0} to {1} compressionQuality {2}", toThisFormat.platform, toThisFormat.doCrunchCompression, toThisFormat.platformCompressionQuality);
            Debug.Log(str);
            string platform = toThisFormat.platform;
            if (platform != null)
            {
                TextureImporterPlatformSettings platformTextureSettings = textureImporter.GetPlatformTextureSettings(platform);
                if (platformTextureSettings.overridden)
                {
                    textureFormatInfo.platformFormat = platformTextureSettings.format;
                    textureFormatInfo.platformCompressionQuality = platformTextureSettings.compressionQuality;
                    textureFormatInfo.doCrunchCompression = platformTextureSettings.crunchedCompression;
                    textureFormatInfo.isNormalMap = textureImporter.textureType == TextureImporterType.NormalMap;
                    TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
                    platformTextureSettings.CopyTo(platformSettings);
                    platformSettings.compressionQuality=(toThisFormat.platformCompressionQuality);
                    platformSettings.crunchedCompression=(toThisFormat.doCrunchCompression);
                    platformSettings.format=(toThisFormat.platformFormat);
                    textureImporter.SetPlatformTextureSettings(platformSettings);
                    flag = true;
                }
            }
            if (textureImporter.crunchedCompression != toThisFormat.doCrunchCompression)
            {
                textureImporter.crunchedCompression=(toThisFormat.doCrunchCompression);
                flag = true;
            }
            if (flag & addToList && !_textureFormatMap.ContainsKey(tx))
                _textureFormatMap.Add(tx, textureFormatInfo);
            return flag;
        }

        private bool _SetTextureFormatUnity5(Texture2D tx, TextureFormatInfo toThisFormat, bool addToList, bool setNormalMap, TextureImporter textureImporter)
        {
            bool flag = false;
            TextureFormatInfo textureFormatInfo = new TextureFormatInfo(textureImporter.textureCompression, false, toThisFormat.platform, (TextureImporterFormat)4, textureImporter.textureType == TextureImporterType.NormalMap);
            string str = (!addToList ? "Restoring texture compression for " : "Setting texture compression for ") + string.Format("{0}  to compression={1} isNormal={2} ", tx, toThisFormat.compression, setNormalMap);
            if (toThisFormat.platform != null)
                str += string.Format(" setting platform override format for platform {0} to {1} compressionQuality {2}", toThisFormat.platform, toThisFormat.platformFormat, toThisFormat.platformCompressionQuality);
            Debug.Log(str);
            string platform = toThisFormat.platform;
            if (platform != null)
            {
                TextureImporterPlatformSettings platformTextureSettings = textureImporter.GetPlatformTextureSettings(platform);
                if (platformTextureSettings.overridden)
                {
                    textureFormatInfo.platformFormat = platformTextureSettings.format;
                    textureFormatInfo.platformCompressionQuality = platformTextureSettings.compressionQuality;
                    TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
                    platformTextureSettings.CopyTo(platformSettings);
                    platformSettings.compressionQuality=(toThisFormat.platformCompressionQuality);
                    platformSettings.format=(toThisFormat.platformFormat);
                    textureImporter.SetPlatformTextureSettings(platformSettings);
                    flag = true;
                }
            }
            if (textureImporter.textureCompression != toThisFormat.compression)
            {
                textureImporter.textureCompression=(toThisFormat.compression);
                flag = true;
            }
            if (flag & addToList && !_textureFormatMap.ContainsKey(tx))
                _textureFormatMap.Add(tx, textureFormatInfo);
            return flag;
        }

        public void SetNormalMap(Texture2D tx)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if ((atPath== null) || !(atPath is TextureImporter))
                return;
            TextureImporter textureImporter = (TextureImporter)atPath;
            if (textureImporter.textureType != TextureImporterType.NormalMap)
            {
                textureImporter.textureType= TextureImporterType.NormalMap;
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));
            }
        }

        public bool IsCompressed(Texture2D tx)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            return (atPath!= null) && atPath is TextureImporter && ((TextureImporter)atPath).textureCompression == 0;
        }

        public void RestoreReadFlagsAndFormats(ProgressUpdateDelegate progressInfo)
        {
            for (int index = 0; index < _texturesWithReadWriteFlagSet.Count; ++index)
            {
                if (progressInfo != null)
                    progressInfo("Restoring read flag for " + _texturesWithReadWriteFlagSet[index], 0.9f);
                SetReadWriteFlag(_texturesWithReadWriteFlagSet[index], false, false);
            }
            _texturesWithReadWriteFlagSet.Clear();
            using (Dictionary<Texture2D, TextureFormatInfo>.KeyCollection.Enumerator enumerator = _textureFormatMap.Keys.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Texture2D current = enumerator.Current;
                    if (progressInfo != null)
                        progressInfo("Restoring format for " + current, 0.9f);
                    SetTextureFormat(current, _textureFormatMap[current], false, _textureFormatMap[current].isNormalMap);
                }
            }
            _textureFormatMap.Clear();
        }

        public void SetReadWriteFlag(Texture2D tx, bool isReadable, bool addToList)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if ((atPath== null) || !(atPath is TextureImporter))
                return;
            TextureImporter textureImporter = (TextureImporter)atPath;
            if (textureImporter.isReadable != isReadable)
            {
                if (addToList)
                    _texturesWithReadWriteFlagSet.Add(tx);
                textureImporter.isReadable=(isReadable);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx));
            }
        }

        public void SaveAtlasToAssetDatabase(Texture2D atlas, ShaderTextureProperty texPropertyName, int atlasNum, Material resMat)
        {
            if ((atlas== null))
            {
                SetMaterialTextureProperty(resMat, texPropertyName, (string)null);
            }
            else
            {
                string assetPath = AssetDatabase.GetAssetPath(resMat);
                if (assetPath == null || assetPath.Length == 0)
                {
                    Debug.LogError("Could save atlas. Could not find result material in AssetDatabase.");
                }
                else
                {
                    string withoutExtension = Path.GetFileNameWithoutExtension(assetPath);
                    string str1 = assetPath.Substring(0, assetPath.Length - withoutExtension.Length - 4);
                    string str2 = Application.dataPath + str1.Substring("Assets".Length, str1.Length - "Assets".Length) + withoutExtension + "-" + texPropertyName.name + "-atlas" + atlasNum;
                    string str3 = str1 + withoutExtension + "-" + texPropertyName.name + "-atlas" + atlasNum;
                    Texture2D textureCopy = MB_Utility.createTextureCopy(atlas);
                    int size = Mathf.Max(((Texture)textureCopy).height, ((Texture)textureCopy).width);
                    string path;
                    string texturePath;
                    if (SAVE_FORMAT == saveTextureFormat.png)
                    {
                        path = str2 + ".png";
                        texturePath = str3 + ".png";
                        byte[] png = textureCopy.EncodeToPNG();
                        File.WriteAllBytes(path, png);
                    }
                    else
                    {
                        path = str2 + ".tga";
                        texturePath = str3 + ".tga";
                        if (File.Exists(path))
                            File.Delete(path);
                        FileStream fileStream = File.Create(path);
                        MB_TGAWriter.Write(textureCopy.GetPixels(), ((Texture)textureCopy).width, ((Texture)textureCopy).height, (Stream)fileStream);
                    }
                    Object.DestroyImmediate(textureCopy);
                    AssetDatabase.Refresh();
                    Debug.Log(string.Format("Wrote atlas for {0} to file:{1}", texPropertyName.name, path));
                    SetTextureSize((Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)), size);
                    SetMaterialTextureProperty(resMat, texPropertyName, texturePath);
                }
            }
        }

        public void SetMaterialTextureProperty(Material target, ShaderTextureProperty texPropName, string texturePath)
        {
            if (texPropName.isNormalMap)
                SetNormalMap((Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)));
            if (!target.HasProperty(texPropName.name))
                return;
            target.SetTexture(texPropName.name, (Texture)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)));
        }

        public void SetTextureSize(Texture2D tx, int size)
        {
            AssetImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetOrScenePath(tx));
            if ((atPath== null) || !(atPath is TextureImporter))
                return;
            TextureImporter textureImporter = (TextureImporter)atPath;
            int num = 32;
            if (size > 32)
                num = 64;
            if (size > 64)
                num = 128;
            if (size > 128)
                num = 256;
            if (size > 256)
                num = 512;
            if (size > 512)
                num = 1024;
            if (size > 1024)
                num = 2048;
            if (size > 2048)
                num = 4096;
            textureImporter.maxTextureSize=(num);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetOrScenePath(tx), (ImportAssetOptions)1);
        }

        public void CommitChangesToAssets()
        {
            AssetDatabase.Refresh();
        }

        public string GetPlatformString()
        {
            return MBVersionEditor.GetPlatformString();
        }

        public void CheckBuildSettings(long estimatedArea)
        {
            if (Math.Sqrt((double)estimatedArea) <= 1000.0 || EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Standalone)
                return;
            Debug.LogWarning("If the current selected build target is not standalone then the generated atlases may be capped at size 1024. If build target is Standalone then atlases of 4096 can be built");
        }

        public bool CheckPrefabTypes(MB_ObjsToCombineTypes objToCombineType, List<GameObject> objsToMesh)
        {
            for (int index = 0; index < objsToMesh.Count; ++index)
            {
                PrefabType prefabType = PrefabUtility.GetPrefabType(objsToMesh[index]);
                if (prefabType == null || prefabType == PrefabType.PrefabInstance || (prefabType == PrefabType.ModelPrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance) || prefabType == PrefabType.DisconnectedModelPrefabInstance)
                {
                    if (objToCombineType == MB_ObjsToCombineTypes.prefabOnly)
                    {
                        Debug.LogWarning(("The list of objects to combine contains scene objects. You probably want prefabs. If using scene objects ensure position is zero, rotation is zero and scale is one. Translation, Rotation and Scale will be baked into the generated mesh." + objsToMesh[index] + " is a scene object"));
                        return false;
                    }
                }
                else if (objToCombineType == MB_ObjsToCombineTypes.sceneObjOnly)
                {
                    Debug.LogWarning(("The list of objects to combine contains prefab assets. You probably want scene objects." + objsToMesh[index] + " is a prefab object"));
                    return false;
                }
            }
            return true;
        }

        public bool ValidateSkinnedMeshes(List<GameObject> objs)
        {
            for (int index = 0; index < objs.Count; ++index)
            {
                Renderer renderer = MB_Utility.GetRenderer(objs[index]);
                if (renderer is SkinnedMeshRenderer && ((SkinnedMeshRenderer)renderer).bones.Length == 0)
                    Debug.LogWarning(("SkinnedMesh " + index + " (" + objs[index] + ") in the list of objects to combine has no bones. Check that 'optimize game object' is not checked in the 'Rig' tab of the asset importer. Mesh Baker cannot combine optimized skinned meshes because the bones are not available."));
            }
            return true;
        }

        public void Destroy(Object o)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(o);
            }
            else
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                if (assetPath != null && assetPath.Equals(""))
                    Object.DestroyImmediate(o, false);
            }
        }

        private enum saveTextureFormat
        {
            png,
            tga,
        }

        private class TextureFormatInfo
        {
            public TextureImporterCompression compression;
            public bool doCrunchCompression;
            public TextureImporterFormat platformFormat;
            public int platformCompressionQuality;
            public string platform;
            public bool isNormalMap;

            public TextureFormatInfo(TextureImporterCompression comp, bool doCrunch, string p, TextureImporterFormat pf, bool isNormMap)
            {
                compression = comp;
                doCrunchCompression = doCrunch;
                platform = p;
                platformFormat = pf;
                platformCompressionQuality = 0;
                isNormalMap = isNormMap;
            }
        }
    }
}

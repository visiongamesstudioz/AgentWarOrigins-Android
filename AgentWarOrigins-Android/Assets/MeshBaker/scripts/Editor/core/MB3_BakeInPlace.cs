// Decompiled with JetBrains decompiler
// Type: DigitalOpus.MB.Core.MB3_BakeInPlace
// Assembly: MeshBakerEvalVersionEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2A8B8E34-4AF9-4434-BAA9-EF1230564558
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\Editor\core\MeshBakerEvalVersionEditor.dll

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public class MB3_BakeInPlace
    {
        public static Mesh BakeMeshesInPlace(MB3_MeshCombinerSingle mom, List<GameObject> objsToMesh, string saveFolder, bool clearBuffersAfterBake, ProgressUpdateDelegate updateProgressBar)
        {
            if (MB3_MeshCombiner.EVAL_VERSION)
                return (Mesh)null;
            if (saveFolder.Length < 6)
            {
                Debug.LogError("Please select a folder for meshes.");
                return (Mesh)null;
            }
            if (!Directory.Exists(Application.dataPath + saveFolder.Substring(6)))
            {
                Debug.Log((Application.dataPath + saveFolder.Substring(6)));
                Debug.Log(Path.GetFullPath(Application.dataPath + saveFolder.Substring(6)));
                Debug.LogError("The selected Folder For Meshes does not exist or is not inside the projects Assets folder. Please 'Choose Folder For Bake In Place Meshes' that is inside the project's assets folder.");
                return (Mesh)null;
            }
            MB3_EditorMethods mb3EditorMethods = new MB3_EditorMethods();
            mom.DestroyMeshEditor((MB2_EditorMethodsInterface)mb3EditorMethods);
            MB_RenderType renderType = mom.renderType;
            Mesh mesh = (Mesh)null;
            for (int index = 0; index < objsToMesh.Count; ++index)
            {
                if (objsToMesh[index]== null)
                {
                    Debug.LogError(("The " + index + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element."));
                    return (Mesh)null;
                }
                string[] names = GenerateNames(objsToMesh);
                mesh = BakeOneMesh(mom, saveFolder + "/" + names[index], objsToMesh[index]);
                if (updateProgressBar != null)
                    updateProgressBar("Created mesh saving mesh on " + (objsToMesh[index]).name + " to asset " + names[index], 0.6f);
            }
            mom.renderType = renderType;
            MB_Utility.Destroy(mom.resultSceneObject);
            if (clearBuffersAfterBake)
                mom.ClearBuffers();
            return mesh;
        }

        public static Mesh BakeOneMesh(MB3_MeshCombinerSingle mom, string newMeshFilePath, GameObject objToBake)
        {
            Mesh mesh = (Mesh)null;
            if ((objToBake== null))
            {
                Debug.LogError("An object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
                return (Mesh)null;
            }
            MB3_EditorMethods mb3EditorMethods = new MB3_EditorMethods();
            GameObject[] gos = new GameObject[1]
            {
        objToBake
            };
            Renderer renderer = MB_Utility.GetRenderer(objToBake);
            if (renderer is SkinnedMeshRenderer)
                mom.renderType = MB_RenderType.skinnedMeshRenderer;
            else if (renderer is MeshRenderer)
            {
                mom.renderType = MB_RenderType.meshRenderer;
            }
            else
            {
                Debug.LogError("Unsupported Renderer type on object. Must be SkinnedMesh or MeshFilter.");
                return (Mesh)null;
            }
            if (newMeshFilePath == null && (uint)newMeshFilePath.Length > 0U)
            {
                Debug.LogError("File path was not in assets folder.");
                return (Mesh)null;
            }
            if (mom.AddDeleteGameObjects(gos, (GameObject[])null, false))
            {
                mom.Apply(new MB3_MeshCombiner.GenerateUV2Delegate(MB3_MeshBakerEditorFunctions.UnwrapUV2));
                if ((MB_Utility.GetMesh(objToBake)!= null))
                {
                    Debug.Log(("Creating mesh for " + (objToBake).name + " with adjusted UVs at: " + newMeshFilePath));
                    AssetDatabase.CreateAsset(mom.GetMesh(), newMeshFilePath);
                    mesh = (Mesh)AssetDatabase.LoadAssetAtPath(newMeshFilePath, typeof(Mesh));
                }
            }
            mom.DestroyMeshEditor((MB2_EditorMethodsInterface)mb3EditorMethods);
            return mesh;
        }

        private static string[] GenerateNames(List<GameObject> objsToMesh)
        {
            string[] strArray = new string[objsToMesh.Count];
            for (int index = 0; index < objsToMesh.Count; ++index)
            {
                string name = (objsToMesh[index]).name;
                string str = name + ".asset";
                int num = 1;
                while (ArrayUtility.Contains<string>(strArray, (objsToMesh[index]).name))
                {
                    str = name + "-" + num + ".asset";
                    ++num;
                }
                strArray[index] = str;
            }
            return strArray;
        }
    }
}

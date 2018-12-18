
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DigitalOpus.MB.Core
{
    public class MB_Utility
    {
        public static Texture2D createTextureCopy(Texture2D source)
        {
            Texture2D texture2D = new Texture2D(((Texture)source).width, ((Texture)source).height, (TextureFormat)5, true);
            texture2D.SetPixels(source.GetPixels());
            return texture2D;
        }

        public static bool ArrayBIsSubsetOfA(object[] a, object[] b)
        {
            for (int index1 = 0; index1 < b.Length; ++index1)
            {
                bool flag = false;
                for (int index2 = 0; index2 < a.Length; ++index2)
                {
                    if (a[index2] == b[index1])
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    return false;
            }
            return true;
        }

        public static Material[] GetGOMaterials(GameObject go)
        {
            if ((go== null))
                return null;
            Material[] materialArray1 = null;
            Mesh mesh = null;
            MeshRenderer component1 = go.GetComponent<MeshRenderer>();
            if ((component1!= null))
            {
                materialArray1 = component1.sharedMaterials;
                MeshFilter component2 = (MeshFilter)go.GetComponent<MeshFilter>();
                if ((component2== null))
                    throw new Exception("Object " + go + " has a MeshRenderer but no MeshFilter.");
                mesh = component2.sharedMesh;
            }
            SkinnedMeshRenderer component3 = (SkinnedMeshRenderer)go.GetComponent<SkinnedMeshRenderer>();
            if (component3!= null)
            {
                materialArray1 = ((Renderer)component3).sharedMaterials;
                mesh = component3.sharedMesh;
            }
            if (materialArray1 == null)
            {
                Debug.LogError(("Object " + (go).name + " does not have a MeshRenderer or a SkinnedMeshRenderer component"));
                return new Material[0];
            }
            if ((mesh== null))
            {
                Debug.LogError(("Object " + (go).name + " has a MeshRenderer or SkinnedMeshRenderer but no mesh."));
                return new Material[0];
            }
            if (mesh.subMeshCount < materialArray1.Length)
            {
                Debug.LogWarning(("Object " + go + " has only " + mesh.subMeshCount + " submeshes and has " + materialArray1.Length + " materials. Extra materials do nothing."));
                Material[] materialArray2 = new Material[mesh.subMeshCount];
                Array.Copy((Array)materialArray1, (Array)materialArray2, materialArray2.Length);
                materialArray1 = materialArray2;
            }
            return materialArray1;
        }

        public static Mesh GetMesh(GameObject go)
        {
            if ((go== null))
                return (Mesh)null;
            MeshFilter component1 = (MeshFilter)go.GetComponent<MeshFilter>();
            if ((component1!= null))
                return component1.sharedMesh;
            SkinnedMeshRenderer component2 = (SkinnedMeshRenderer)go.GetComponent<SkinnedMeshRenderer>();
            if ((component2!= null))
                return component2.sharedMesh;
            Debug.LogError(("Object " + (go).name + " does not have a MeshFilter or a SkinnedMeshRenderer component"));
            return (Mesh)null;
        }

        public static void SetMesh(GameObject go, Mesh m)
        {
            if ((go== null))
                return;
            MeshFilter component1 = (MeshFilter)go.GetComponent<MeshFilter>();
            if ((component1!= null))
            {
                component1.sharedMesh=m;
            }
            else
            {
                SkinnedMeshRenderer component2 = (SkinnedMeshRenderer)go.GetComponent<SkinnedMeshRenderer>();
                if ((component2!= null))
                    component2.sharedMesh=(m);
            }
        }

        public static Renderer GetRenderer(GameObject go)
        {
            if ((go== null))
                return (Renderer)null;
            MeshRenderer component1 = (MeshRenderer)go.GetComponent<MeshRenderer>();
            if ((component1!= null))
                return (Renderer)component1;
            SkinnedMeshRenderer component2 = (SkinnedMeshRenderer)go.GetComponent<SkinnedMeshRenderer>();
            if (component2!= null)
                return (Renderer)component2;
            return (Renderer)null;
        }

        public static void DisableRendererInSource(GameObject go)
        {
            if ((go== null))
                return;
            MeshRenderer component1 = (MeshRenderer)go.GetComponent<MeshRenderer>();
            if ((component1!= null))
            {
                ((Renderer)component1).enabled=(false);
            }
            else
            {
                SkinnedMeshRenderer component2 = (SkinnedMeshRenderer)go.GetComponent<SkinnedMeshRenderer>();
                if (component2== null)
                    return;
                ((Renderer)component2).enabled=(false);
            }
        }

        public static bool hasOutOfBoundsUVs(Mesh m, ref Rect uvBounds)
        {
            MeshAnalysisResult putResultHere = new MeshAnalysisResult();
            bool flag = hasOutOfBoundsUVs(m, ref putResultHere, -1, 0);
            uvBounds = putResultHere.uvRect;
            return flag;
        }

        public static bool hasOutOfBoundsUVs(Mesh m, ref MeshAnalysisResult putResultHere, int submeshIndex = -1, int uvChannel = 0)
        {
            if ((m== null))
            {
                putResultHere.hasOutOfBoundsUVs = false;
                return putResultHere.hasOutOfBoundsUVs;
            }
            Vector2[] uvs;
            switch (uvChannel)
            {
                case 0:
                    uvs = m.uv;
                    break;
                case 1:
                    uvs = m.uv2;
                    break;
                case 2:
                    uvs = m.uv3;
                    break;
                default:
                    uvs = m.uv4;
                    break;
            }
            return hasOutOfBoundsUVs(uvs, m, ref putResultHere, submeshIndex);
        }

        public static bool hasOutOfBoundsUVs(Vector2[] uvs, Mesh m, ref MeshAnalysisResult putResultHere, int submeshIndex = -1)
        {
            putResultHere.hasUVs = true;
            if (uvs.Length == 0)
            {
                putResultHere.hasUVs = false;
                putResultHere.hasOutOfBoundsUVs = false;
                putResultHere.uvRect = new Rect();
                return putResultHere.hasOutOfBoundsUVs;
            }
            if (submeshIndex >= m.subMeshCount)
            {
                putResultHere.hasOutOfBoundsUVs = false;
                putResultHere.uvRect = new Rect();
                return putResultHere.hasOutOfBoundsUVs;
            }
            float x;
            float num1;
            float y;
            float num2;
            if (submeshIndex >= 0)
            {
                int[] triangles = m.GetTriangles(submeshIndex);
                if (triangles.Length == 0)
                {
                    putResultHere.hasOutOfBoundsUVs = false;
                    putResultHere.uvRect =new Rect();
                    return putResultHere.hasOutOfBoundsUVs;
                }
                num1 = x = (float)uvs[triangles[0]].x;
                num2 = y = (float)uvs[triangles[0]].y;
                for (int index1 = 0; index1 < triangles.Length; ++index1)
                {
                    int index2 = triangles[index1];
                    if (uvs[index2].x < (double)num1)
                        num1 = (float)uvs[index2].x;
                    if (uvs[index2].x > (double)x)
                        x = (float)uvs[index2].x;
                    if (uvs[index2].y < (double)num2)
                        num2 = (float)uvs[index2].y;
                    if (uvs[index2].y > (double)y)
                        y = (float)uvs[index2].y;
                }
            }
            else
            {
                num1 = x = (float)uvs[0].x;
                num2 = y = (float)uvs[0].y;
                for (int index = 0; index < uvs.Length; ++index)
                {
                    if (uvs[index].x < (double)num1)
                        num1 = (float)uvs[index].x;
                    if (uvs[index].x > (double)x)
                        x = (float)uvs[index].x;
                    if (uvs[index].y < (double)num2)
                        num2 = (float)uvs[index].y;
                    if (uvs[index].y > (double)y)
                        y = (float)uvs[index].y;
                }
            }
            Rect rect = new Rect();
            // ISSUE: explicit reference operation
            (rect).x=(num1);
            // ISSUE: explicit reference operation
            (rect).y=(num2);
            // ISSUE: explicit reference operation
            (rect).width=(x - num1);
            // ISSUE: explicit reference operation
            (rect).height=(y - num2);
            int num3 = (double)x > 1.0 || (double)num1 < 0.0 || (double)y > 1.0 ? 1 : ((double)num2 < 0.0 ? 1 : 0);
            putResultHere.hasOutOfBoundsUVs = num3 != 0;
            putResultHere.uvRect = rect;
            return putResultHere.hasOutOfBoundsUVs;
        }

        public static void setSolidColor(Texture2D t, Color c)
        {
            Color[] pixels = t.GetPixels();
            for (int index = 0; index < pixels.Length; ++index)
                pixels[index] = c;
            t.SetPixels(pixels);
            t.Apply();
        }

        public static Texture2D resampleTexture(Texture2D source, int newWidth, int newHeight)
        {
            TextureFormat format = source.format;
            if (format == TextureFormat.ARGB32 || format == TextureFormat.RGBA32 || (format == TextureFormat.BGRA32 || format == TextureFormat.RGB24) || format == TextureFormat.Alpha8 || format == TextureFormat.DXT1)
            {
                Texture2D texture2D = new Texture2D(newWidth, newHeight, (TextureFormat)5, true);
                float num1 = (float)newWidth;
                float num2 = (float)newHeight;
                for (int index1 = 0; index1 < newWidth; ++index1)
                {
                    for (int index2 = 0; index2 < newHeight; ++index2)
                    {
                        float num3 = (float)index1 / num1;
                        float num4 = (float)index2 / num2;
                        texture2D.SetPixel(index1, index2, source.GetPixelBilinear(num3, num4));
                    }
                }
                texture2D.Apply();
                return texture2D;
            }
            Debug.LogError("Can only resize textures in formats ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT");
            return (Texture2D)null;
        }

        public static bool AreAllSharedMaterialsDistinct(Material[] sharedMaterials)
        {
            for (int index1 = 0; index1 < sharedMaterials.Length; ++index1)
            {
                for (int index2 = index1 + 1; index2 < sharedMaterials.Length; ++index2)
                {
                    if ((sharedMaterials[index1]==sharedMaterials[index2]))
                        return false;
                }
            }
            return true;
        }

        public static int doSubmeshesShareVertsOrTris(Mesh m, ref MeshAnalysisResult mar)
        {
            MB_Triangle mbTriangle1 = new MB_Triangle();
            MB_Triangle mbTriangle2 = new MB_Triangle();
            int[][] numArray = new int[m.subMeshCount][];
            for (int index = 0; index < m.subMeshCount; ++index)
                numArray[index] = m.GetTriangles(index);
            bool flag1 = false;
            bool flag2 = false;
            for (int sIdx1 = 0; sIdx1 < m.subMeshCount; ++sIdx1)
            {
                int[] ts1 = numArray[sIdx1];
                for (int sIdx2 = sIdx1 + 1; sIdx2 < m.subMeshCount; ++sIdx2)
                {
                    int[] ts2 = numArray[sIdx2];
                    int idx1 = 0;
                    while (idx1 < ts1.Length)
                    {
                        mbTriangle1.Initialize(ts1, idx1, sIdx1);
                        int idx2 = 0;
                        while (idx2 < ts2.Length)
                        {
                            mbTriangle2.Initialize(ts2, idx2, sIdx2);
                            if (mbTriangle1.isSame(mbTriangle2))
                            {
                                flag2 = true;
                                break;
                            }
                            if (mbTriangle1.sharesVerts(mbTriangle2))
                            {
                                flag1 = true;
                                break;
                            }
                            idx2 += 3;
                        }
                        idx1 += 3;
                    }
                }
            }
            if (flag2)
            {
                mar.hasOverlappingSubmeshVerts = true;
                mar.hasOverlappingSubmeshTris = true;
                return 2;
            }
            if (flag1)
            {
                mar.hasOverlappingSubmeshVerts = true;
                mar.hasOverlappingSubmeshTris = false;
                return 1;
            }
            mar.hasOverlappingSubmeshTris = false;
            mar.hasOverlappingSubmeshVerts = false;
            return 0;
        }

        public static bool GetBounds(GameObject go, out Bounds b)
        {
            if ((go== null))
            {
                Debug.LogError("go paramater was null");
                b = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }
            Renderer renderer = GetRenderer(go);
            if ((renderer== null))
            {
                Debug.LogError("GetBounds must be called on an object with a Renderer");
                b = new Bounds(Vector3.zero, Vector3.zero);
                return false;
            }
            if (renderer is MeshRenderer)
            {
                b = renderer.bounds;
                return true;
            }
            if (renderer is SkinnedMeshRenderer)
            {
                b = renderer.bounds;
                return true;
            }
            Debug.LogError("GetBounds must be called on an object with a MeshRender or a SkinnedMeshRenderer.");
            b = new Bounds(Vector3.zero, Vector3.zero);
            return false;
        }

        public static void Destroy(Object o)
        {
            if (Application.isPlaying)
                Object.Destroy(o);
            else
                Object.DestroyImmediate(o, false);
        }

        public struct MeshAnalysisResult
        {
            public Rect uvRect;
            public bool hasOutOfBoundsUVs;
            public bool hasOverlappingSubmeshVerts;
            public bool hasOverlappingSubmeshTris;
            public bool hasUVs;
            public float submeshArea;
        }

        private class MB_Triangle
        {
            private int[] vs = new int[3];
            private int submeshIdx;

            public bool isSame(object obj)
            {
                MB_Triangle mbTriangle = (MB_Triangle)obj;
                return vs[0] == mbTriangle.vs[0] && vs[1] == mbTriangle.vs[1] && vs[2] == mbTriangle.vs[2] && submeshIdx != mbTriangle.submeshIdx;
            }

            public bool sharesVerts(MB_Triangle obj)
            {
                return (vs[0] == obj.vs[0] || vs[0] == obj.vs[1] || vs[0] == obj.vs[2]) && submeshIdx != obj.submeshIdx || (vs[1] == obj.vs[0] || vs[1] == obj.vs[1] || vs[1] == obj.vs[2]) && submeshIdx != obj.submeshIdx || (vs[2] == obj.vs[0] || vs[2] == obj.vs[1] || vs[2] == obj.vs[2]) && submeshIdx != obj.submeshIdx;
            }

            public void Initialize(int[] ts, int idx, int sIdx)
            {
                vs[0] = ts[idx];
                vs[1] = ts[idx + 1];
                vs[2] = ts[idx + 2];
                submeshIdx = sIdx;
                Array.Sort<int>(vs);
            }
        }
    }
}

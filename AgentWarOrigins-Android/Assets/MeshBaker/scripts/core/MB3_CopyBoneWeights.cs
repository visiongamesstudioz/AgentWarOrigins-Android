using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public class MB3_CopyBoneWeights
    {
        public static void CopyBoneWeightsFromSeamMeshToOtherMeshes(float radius, Mesh seamMesh, Mesh[] targetMeshes)
        {
            List<int> intList = new List<int>();
            if (seamMesh==null)
                Debug.LogError("The SeamMesh cannot be null");
            else if (seamMesh.vertexCount == 0)
            {
                Debug.LogError("The seam mesh has no vertices. Check that the Asset Importer for the seam mesh does not have 'Optimize Mesh' checked.");
            }
            else
            {
                Vector3[] vertices1 = seamMesh.vertices;
                BoneWeight[] boneWeights1 = seamMesh.boneWeights;
                Vector3[] normals1 = seamMesh.normals;
                Vector4[] tangents1 = seamMesh.tangents;
                Vector2[] uv = seamMesh.uv;
                if (uv.Length != vertices1.Length)
                {
                    Debug.LogError("The seam mesh needs uvs to identify which vertices are part of the seam. Vertices with UV > .5 are part of the seam. Vertices with UV < .5 are not part of the seam.");
                }
                else
                {
                    for (int index = 0; index < uv.Length; ++index)
                    {
                        if (uv[index].x > 0.5 && uv[index].y > 0.5)
                            intList.Add(index);
                    }
                    Debug.Log(string.Format("The seam mesh has {0} vertices of which {1} are seam vertices.", seamMesh.vertices.Length, intList.Count));
                    if (intList.Count == 0)
                    {
                        Debug.LogError("None of the vertices in the Seam Mesh were marked as seam vertices. To mark a vertex as a seam vertex the UV must be greater than (.5,.5). Vertices with UV less than (.5,.5) are excluded.");
                    }
                    else
                    {
                        bool flag = false;
                        for (int index = 0; index < targetMeshes.Length; ++index)
                        {
                            if (targetMeshes[index]==null)
                            {
                                Debug.LogError(string.Format("Mesh {0} was null", index));
                                flag = true;
                            }
                            if (radius < 0.0)
                                Debug.LogError("radius must be zero or positive.");
                        }
                        if (flag)
                            return;
                        for (int index1 = 0; index1 < targetMeshes.Length; ++index1)
                        {
                            Mesh targetMesh = targetMeshes[index1];
                            Vector3[] vertices2 = targetMesh.vertices;
                            BoneWeight[] boneWeights2 = targetMesh.boneWeights;
                            Vector3[] normals2 = targetMesh.normals;
                            Vector4[] tangents2 = targetMesh.tangents;
                            int num = 0;
                            for (int index2 = 0; index2 < vertices2.Length; ++index2)
                            {
                                for (int index3 = 0; index3 < intList.Count; ++index3)
                                {
                                    int index4 = intList[index3];
                                    if (Vector3.Distance(vertices2[index2], vertices1[index4]) <= (double)radius)
                                    {
                                        ++num;
                                        boneWeights2[index2] = boneWeights1[index4];
                                        vertices2[index2] = vertices1[index4];
                                        if (normals2.Length == vertices2.Length)
                                            normals2[index2] = normals1[index4];
                                        if (tangents2.Length == vertices2.Length && tangents1.Length == vertices1.Length)
                                            tangents2[index2] = tangents1[index4];
                                    }
                                }
                            }
                            if (num > 0)
                            {
                                targetMeshes[index1].vertices=vertices2;
                                targetMeshes[index1].boneWeights=boneWeights2;
                                targetMeshes[index1].normals=normals2;
                                targetMeshes[index1].tangents=tangents2;
                            }
                            Debug.Log(string.Format("Copied boneweights for {1} vertices in mesh {0} that matched positions in the seam mesh.", targetMeshes[index1].name, num));
                        }
                    }
                }
            }
        }
    }
}

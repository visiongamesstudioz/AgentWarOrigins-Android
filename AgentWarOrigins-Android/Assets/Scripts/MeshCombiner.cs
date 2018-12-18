using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{

    public bool IsLODMesh;
    public int NoofLODs;
    void Start()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        List<MeshFilter> meshfilter = new List<MeshFilter>();
        for (int i = 1; i < meshFilters.Length; i++)
            meshfilter.Add(meshFilters[i]);
        CombineInstance[] combine = new CombineInstance[meshfilter.Count];
        Material material = GetComponentsInChildren<MeshRenderer>()[1].sharedMaterial;
        Mesh finalMesh = new Mesh();
        int j = 0;
        while (j < meshfilter.Count)
        {
                //do stuff with this mesh filter's sharedMesh
                combine[j].mesh = meshfilter[j].sharedMesh;
                combine[j].transform = meshfilter[j].transform.localToWorldMatrix;
                meshfilter[j].gameObject.SetActive(false);
                j++;           
         
        }
        finalMesh.CombineMeshes(combine);
        GetComponent<MeshFilter>().mesh = finalMesh;
        GetComponent<MeshRenderer>().sharedMaterial = material;
        transform.gameObject.SetActive(true);
        Vector3 pos = transform.parent.position;

        transform.localPosition = new Vector3(-pos.x, pos.y, pos.z);
        BoxCollider[] boxColliders = GetComponentsInChildren<BoxCollider>();
        foreach(var collider in boxColliders)
        {
            collider.gameObject.transform.localPosition= new Vector3(pos.x,collider.transform.localPosition.y,collider.transform.localPosition.z);
        }
        if (IsLODMesh)
        {
            LODGroup lodGroup = GetComponent<LODGroup>();
            LOD[] lods = new LOD[NoofLODs];
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            for (int k = 0; k < lods.Length; k++)
            {
                lods[k] = lods[k] = new LOD((k + 1)*0.1f, renderers);
            }
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
   
    }

}

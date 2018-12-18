using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineSpecificMeshes : MonoBehaviour {
    public List<MeshFilter> MeshesToCombine;
    public Transform ParentPos;
    public bool IsLODMesh;
    public int NoofLODs;
    public bool combineSubmeshes;
    // Use this for initialization
    void Start () {

        CombineInstance[] combine = new CombineInstance[MeshesToCombine.Count];
        Mesh finalMesh = new Mesh();
        int i = 0;
        while (i < MeshesToCombine.Count)
        {
            combine[i].mesh = MeshesToCombine[i].sharedMesh;
            combine[i].transform = MeshesToCombine[i].transform.localToWorldMatrix;
            MeshesToCombine[i].gameObject.SetActive(false);
            i++;
        }
        if (combineSubmeshes)
        {
            finalMesh.CombineMeshes(combine, false);

        }
        else
        {
            finalMesh.CombineMeshes(combine);

        }
        GetComponent<MeshFilter>().mesh = finalMesh;
        Material[] materials = MeshesToCombine[0].gameObject.GetComponent<MeshRenderer>().sharedMaterials;
        GetComponent<MeshRenderer>().sharedMaterials = materials;
        transform.gameObject.SetActive(true);

        transform.localPosition = new Vector3(-ParentPos.position.x, ParentPos.position.y, ParentPos.position.z);
    
        if (IsLODMesh)
        {
            Transform[] childTransforms = transform.GetComponentsInChildren<Transform>();
            for (int z = 1; z < childTransforms.Length; z++)
            {
                childTransforms[z].localPosition = new Vector3(0, childTransforms[z].position.y, childTransforms[z].position.z);
            }
            LODGroup lodGroup = GetComponent<LODGroup>();
            LOD[] lods = new LOD[NoofLODs];
            Renderer[] childrenderers = GetComponentsInChildren<Renderer>();


            for (int k = 0; k < NoofLODs; k++)
            {

                Renderer[] renderers = new Renderer[1];
                renderers[0] = childrenderers[k];
                lods[k].screenRelativeTransitionHeight = 1 - ((k+1)*0.9f);
                lods[k].renderers=renderers;

            }
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
    }



}

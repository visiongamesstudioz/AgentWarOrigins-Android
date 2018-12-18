
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MB_MultiMaterial
{
    public List<Material> sourceMaterials = new List<Material>();
    public Material combinedMaterial;
    public bool considerMeshUVs;
}

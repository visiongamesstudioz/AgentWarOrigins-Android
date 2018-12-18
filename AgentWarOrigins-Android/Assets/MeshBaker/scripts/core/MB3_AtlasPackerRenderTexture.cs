// Decompiled with JetBrains decompiler
// Type: MB3_AtlasPackerRenderTexture
// Assembly: MeshBakerCore, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D590286C-1214-465B-A384-78BAAD755E88
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\MeshBakerCore.dll

using DigitalOpus.MB.Core;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MB3_AtlasPackerRenderTexture : MonoBehaviour
{
    private MB_TextureCombinerRenderTexture fastRenderer;
    private bool _doRenderAtlas;
    public int width;
    public int height;
    public int padding;
    public bool isNormalMap;
    public bool fixOutOfBoundsUVs;
    public bool considerNonTextureProperties;
    public TextureBlender resultMaterialTextureBlender;
    public Rect[] rects;
    public Texture2D tex1;
    public List<MB3_TextureCombiner.MB_TexSet> textureSets;
    public int indexOfTexSetToRender;
    public ShaderTextureProperty texPropertyName;
    public MB2_LogLevel LOG_LEVEL;
    public Texture2D testTex;
    public Material testMat;

    public Texture2D OnRenderAtlas(MB3_TextureCombiner combiner)
    {
        fastRenderer = new MB_TextureCombinerRenderTexture();
        _doRenderAtlas = true;
        Texture2D texture2D = fastRenderer.DoRenderAtlas(((Component)this).gameObject, width, height, padding, rects, textureSets, indexOfTexSetToRender, texPropertyName, resultMaterialTextureBlender, isNormalMap, fixOutOfBoundsUVs, considerNonTextureProperties, combiner, LOG_LEVEL);
        _doRenderAtlas = false;
        return texture2D;
    }

    private void OnRenderObject()
    {
        if (!_doRenderAtlas)
            return;
        fastRenderer.OnRenderObject();
        _doRenderAtlas = false;
    }

    public MB3_AtlasPackerRenderTexture()
    {
       // base.\u002Ector();
    }
}

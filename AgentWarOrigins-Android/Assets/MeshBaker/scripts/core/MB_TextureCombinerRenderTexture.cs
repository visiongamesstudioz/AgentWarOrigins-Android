// Decompiled with JetBrains decompiler
// Type: MB_TextureCombinerRenderTexture
// Assembly: MeshBakerCore, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D590286C-1214-465B-A384-78BAAD755E88
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\MeshBakerCore.dll

using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MB_TextureCombinerRenderTexture
{
    public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;
    private bool _doRenderAtlas = false;
    private Material mat;
    private RenderTexture _destinationTexture;
    private Camera myCamera;
    private int _padding;
    private bool _isNormalMap;
    private bool _fixOutOfBoundsUVs;
    private Rect[] rs;
    private List<MB3_TextureCombiner.MB_TexSet> textureSets;
    private int indexOfTexSetToRender;
    private ShaderTextureProperty _texPropertyName;
    private TextureBlender _resultMaterialTextureBlender;
    private Texture2D targTex;
    private MB3_TextureCombiner combiner;

    public Texture2D DoRenderAtlas(GameObject gameObject, int width, int height, int padding, Rect[] rss, List<MB3_TextureCombiner.MB_TexSet> textureSetss, int indexOfTexSetToRenders, ShaderTextureProperty texPropertyname, TextureBlender resultMaterialTextureBlender, bool isNormalMap, bool fixOutOfBoundsUVs, bool considerNonTextureProperties, MB3_TextureCombiner texCombiner, MB2_LogLevel LOG_LEV)
    {
        this.LOG_LEVEL = LOG_LEV;
        this.textureSets = textureSetss;
        this.indexOfTexSetToRender = indexOfTexSetToRenders;
        this._texPropertyName = texPropertyname;
        this._padding = padding;
        this._isNormalMap = isNormalMap;
        this._fixOutOfBoundsUVs = fixOutOfBoundsUVs;
        this._resultMaterialTextureBlender = resultMaterialTextureBlender;
        this.combiner = texCombiner;
        this.rs = rss;
        Shader shader = !this._isNormalMap ? Shader.Find("MeshBaker/AlbedoShader") : Shader.Find("MeshBaker/NormalMapShader");
        if (shader== null)
        {
            Debug.LogError("Could not find shader for RenderTexture. Try reimporting mesh baker");
            return (Texture2D)null;
        }
        this.mat = new Material(shader);
        this._destinationTexture = new RenderTexture(width, height, 24, (RenderTextureFormat)0);
        ((Texture)this._destinationTexture).filterMode=((FilterMode)0);
        this.myCamera = (Camera)gameObject.GetComponent<Camera>();
        this.myCamera.orthographic=(true);
        this.myCamera.orthographicSize=((float)(height >> 1));
        this.myCamera.aspect=((float)(width / height));
        this.myCamera.targetTexture=(this._destinationTexture);
        this.myCamera.clearFlags=((CameraClearFlags)2);
        Transform component = (Transform)((Component)this.myCamera).GetComponent<Transform>();
        component.localPosition=(new Vector3((float)width / 2f, (float)height / 2f, 3f));
        component.localRotation=(Quaternion.Euler(0.0f, 180f, 180f));
        this._doRenderAtlas = true;
        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
            Debug.Log(string.Format("Begin Camera.Render destTex w={0} h={1} camPos={2}", width, height, component.localPosition));
        this.myCamera.Render();
        this._doRenderAtlas = false;
        MB_Utility.Destroy(this.mat);
        MB_Utility.Destroy(this._destinationTexture);
        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
            Debug.Log("Finished Camera.Render ");
        Texture2D targTex = this.targTex;
        this.targTex = (Texture2D)null;
        return targTex;
    }

    public void OnRenderObject()
    {
        if (!this._doRenderAtlas)
            return;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int index = 0; index < this.rs.Length; ++index)
        {
            MB3_TextureCombiner.MeshBakerMaterialTexture t = this.textureSets[index].ts[this.indexOfTexSetToRender];
            if (this.LOG_LEVEL >= MB2_LogLevel.trace && (t.t!= null))
            {
                Debug.Log(("Added " + t.t + " to atlas w=" + ((Texture)t.t).width + " h=" + ((Texture)t.t).height + " offset=" + t.matTilingRect.min + " scale=" + t.matTilingRect.size + " rect=" + this.rs[index] + " padding=" + this._padding));
                this._printTexture(t.t);
            }
            this.CopyScaledAndTiledToAtlas(this.textureSets[index], t, this.textureSets[index].obUVoffset, this.textureSets[index].obUVscale, this.rs[index], this._texPropertyName, this._resultMaterialTextureBlender);
        }
        stopwatch.Stop();
        stopwatch.Start();
        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
            Debug.Log(("Total time for Graphics.DrawTexture calls " + stopwatch.ElapsedMilliseconds.ToString("f5")));
        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
            Debug.Log(("Copying RenderTexture to Texture2D. destW" + ((Texture)this._destinationTexture).width + " destH" + ((Texture)this._destinationTexture).height));
        Texture2D t1 = new Texture2D(((Texture)this._destinationTexture).width, ((Texture)this._destinationTexture).height, (TextureFormat)5, true);
        RenderTexture active = RenderTexture.active;
        RenderTexture.active=(this._destinationTexture);
        int num1 = ((Texture)this._destinationTexture).width / 512;
        int num2 = ((Texture)this._destinationTexture).height / 512;
        if (num1 == 0 || num2 == 0)
        {
            if (this.LOG_LEVEL >= MB2_LogLevel.trace)
                Debug.Log("Copying all in one shot");
            t1.ReadPixels(new Rect(0.0f, 0.0f, (float)((Texture)this._destinationTexture).width, (float)((Texture)this._destinationTexture).height), 0, 0, true);
        }
        else if (this.IsOpenGL())
        {
            if (this.LOG_LEVEL >= MB2_LogLevel.trace)
                Debug.Log("OpenGL copying blocks");
            for (int index1 = 0; index1 < num1; ++index1)
            {
                for (int index2 = 0; index2 < num2; ++index2)
                    t1.ReadPixels(new Rect((float)(index1 * 512), (float)(index2 * 512), 512f, 512f), index1 * 512, index2 * 512, true);
            }
        }
        else
        {
            if (this.LOG_LEVEL >= MB2_LogLevel.trace)
                Debug.Log("Not OpenGL copying blocks");
            for (int index1 = 0; index1 < num1; ++index1)
            {
                for (int index2 = 0; index2 < num2; ++index2)
                    t1.ReadPixels(new Rect((float)(index1 * 512), (float)(((Texture)this._destinationTexture).height - 512 - index2 * 512), 512f, 512f), index1 * 512, index2 * 512, true);
            }
        }
        RenderTexture.active=(active);
        t1.Apply();
        if (this.LOG_LEVEL >= MB2_LogLevel.trace)
        {
            Debug.Log("TempTexture ");
            this._printTexture(t1);
        }
        this.myCamera.targetTexture=((RenderTexture)null);
        RenderTexture.active=((RenderTexture)null);
        this.targTex = t1;
        if (this.LOG_LEVEL >= MB2_LogLevel.debug)
            Debug.Log(("Total time to copy RenderTexture to Texture2D " + stopwatch.ElapsedMilliseconds.ToString("f5")));
    }

    private Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
    {
        Vector3 zero = Vector3.zero;
        zero.x = (float) (c.a * 2.0 - 1.0);
        zero.y = (float) (c.g * 2.0 - 1.0);
        zero.z = (Mathf.Sqrt((float)(1.0 - zero.x * zero.x - zero.y * zero.y)));
        Color32 color32= Color.white;
        color32.a = 1;
        color32.r = (byte)((zero.x + 1.0) * 0.5);
        color32.g = (byte)((zero.y + 1.0) * 0.5);
        color32.b = (byte)((zero.z + 1.0) * 0.5);
        return color32;
    }

    private bool IsOpenGL()
    {
        return SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL");
    }

    private void CopyScaledAndTiledToAtlas(MB3_TextureCombiner.MB_TexSet texSet, MB3_TextureCombiner.MeshBakerMaterialTexture source, Vector2 obUVoffset, Vector2 obUVscale, Rect rec, ShaderTextureProperty texturePropertyName, TextureBlender resultMatTexBlender)
    {
        //Rect rect1 = rec;
        //if (resultMatTexBlender != null)
        //    this.myCamera.backgroundColor=(resultMatTexBlender.GetColorIfNoTexture(texSet.matsAndGOs.mats[0].mat, texturePropertyName));
        //else
        //    this.myCamera.backgroundColor=(MB3_TextureCombiner.GetColorIfNoTexture(texturePropertyName));
        //if ((source.t== null))
        //    source.t = this.combiner._createTemporaryTexture(16, 16, (TextureFormat)5, true);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect1).set_y((float)(1.0 - ((double)((Rect)@rect1).get_y() + (double)((Rect)@rect1).height)));
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local1 = @rect1;
        //((Rect)local1).set_x(((Rect)local1).get_x() * (float)((Texture)this._destinationTexture).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local2 = @rect1;
        //((Rect)local2).set_y(((Rect)local2).get_y() * (float)((Texture)this._destinationTexture).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local3 = @rect1;
        //((Rect)local3).set_width(((Rect)local3).width * (float)((Texture)this._destinationTexture).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local4 = @rect1;
        //((Rect)local4).set_height(((Rect)local4).height * (float)((Texture)this._destinationTexture).height);
        //Rect rect2 = rect1;
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local5 = @rect2;
        //((Rect)local5).set_x(((Rect)local5).get_x() - (float)this._padding);
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local6 = @rect2;
        //((Rect)local6).set_y(((Rect)local6).get_y() - (float)this._padding);
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local7 = @rect2;
        //((Rect)local7).set_width(((Rect)local7).width + (float)(this._padding * 2));
        //// ISSUE: explicit reference operation
        //// ISSUE: variable of a reference type
        //Rect & local8 = rect2;
        //((Rect)local8).set_height(((Rect)local8).height + (float)(this._padding * 2));
        //Rect r1 = source.matTilingRect.GetRect();
        //Rect rect3 = new Rect();
        //if (this._fixOutOfBoundsUVs)
        //{
        //    Rect r2;
        //    // ISSUE: explicit reference operation
        //    r2=new Rect((float)obUVoffset.x, (float)obUVoffset.y, (float)obUVscale.x, (float)obUVscale.y);
        //    r1 = MB3_UVTransformUtility.CombineTransforms(ref r1, ref r2);
        //    if (this.LOG_LEVEL >= MB2_LogLevel.trace)
        //        Debug.Log(("Fixing out of bounds UVs for tex " + source.t));
        //}
        //Texture2D t = source.t;
        //TextureWrapMode wrapMode = ((Texture)t).wrapMode;
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //if ((double)(r1).width == 1.0 && (double)(r1).height == 1.0 && (double)(r1).x == 0.0 && (double)((Rect)@r1).x == 0.0)
        //    ((Texture)t).wrapMode=((TextureWrapMode)1);
        //else
        //    ((Texture)t).wrapMode=((TextureWrapMode)0);
        //if (this.LOG_LEVEL >= MB2_LogLevel.trace)
        //    Debug.Log(("DrawTexture tex=" + (t).name) + " destRect=" + rect1 + " srcRect=" + r1 + " Mat=" + this.mat);
        //Rect rect4 = new Rect(r1.x, (float) (r1.y + 1.0 - 1.0 / t.height),r1.width, 1f / t.height);
        ////// ISSUE: explicit reference operation
        ////// ISSUE: explicit reference operation
        ////((Rect)@rect4).set_x(((Rect)@r1).get_x());
        ////// ISSUE: explicit reference operation
        ////// ISSUE: explicit reference operation
        ////((Rect)@rect4).set_y((float)((double)((Rect)@r1).get_y() + 1.0 - 1.0 / (double)((Texture)t).height));
        ////// ISSUE: explicit reference operation
        ////// ISSUE: explicit reference operation
        ////((Rect)@rect4).set_width(((Rect)@r1).width);
        ////// ISSUE: explicit reference operation
        ////((Rect)@rect4).set_height(1f / (float)((Texture)t).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //Rect rect3=new Rect(rect1.x,);
        //((Rect)@rect3).set_x(((Rect)@rect1).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect2).get_y());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width(((Rect)@rect1).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height((float)this._padding);
        //RenderTexture active = RenderTexture.get_active();
        //RenderTexture.set_active(this._destinationTexture);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x(((Rect)@r1).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y(((Rect)@r1).get_y());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(((Rect)@r1).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(1f / (float)((Texture)t).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect1).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect1).get_y() + ((Rect)@rect1).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width(((Rect)@rect1).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height((float)this._padding);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x(((Rect)@r1).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y(((Rect)@r1).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(1f / (float)((Texture)t).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(((Rect)@r1).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect2).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect1).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width((float)this._padding);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height(((Rect)@rect1).height);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x((float)((double)((Rect)@r1).get_x() + 1.0 - 1.0 / (double)((Texture)t).width));
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y(((Rect)@r1).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(1f / (float)((Texture)t).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(((Rect)@r1).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect1).get_x() + ((Rect)@rect1).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect1).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width((float)this._padding);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height(((Rect)@rect1).height);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x(((Rect)@r1).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y((float)((double)((Rect)@r1).get_y() + 1.0 - 1.0 / (double)((Texture)t).height));
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(1f / (float)((Texture)t).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(1f / (float)((Texture)t).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect2).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect2).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width((float)this._padding);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height((float)this._padding);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x((float)((double)((Rect)@r1).get_x() + 1.0 - 1.0 / (double)((Texture)t).width));
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y((float)((double)((Rect)@r1).get_y() + 1.0 - 1.0 / (double)((Texture)t).height));
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(1f / (float)((Texture)t).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(1f / (float)((Texture)t).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect1).get_x() + ((Rect)@rect1).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect2).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width((float)this._padding);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height((float)this._padding);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x(((Rect)@r1).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y(((Rect)@r1).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(1f / (float)((Texture)t).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(1f / (float)((Texture)t).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect2).get_x());
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect1).get_y() + ((Rect)@rect1).height);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width((float)this._padding);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height((float)this._padding);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_x((float)((double)((Rect)@r1).get_x() + 1.0 - 1.0 / (double)((Texture)t).width));
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_y(((Rect)@r1).get_y());
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_width(1f / (float)((Texture)t).width);
        //// ISSUE: explicit reference operation
        //((Rect)@rect4).set_height(1f / (float)((Texture)t).height);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_x(((Rect)@rect1).get_x() + ((Rect)@rect1).width);
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_y(((Rect)@rect1).get_y() + ((Rect)@rect1).height);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_width((float)this._padding);
        //// ISSUE: explicit reference operation
        //((Rect)@rect3).set_height((float)this._padding);
        //Graphics.DrawTexture(rect3, (Texture)t, rect4, 0, 0, 0, 0, this.mat);
        //Graphics.DrawTexture(rect1, (Texture)t, r1, 0, 0, 0, 0, this.mat);
        //RenderTexture.set_active(active);
        //((Texture)t).set_wrapMode(wrapMode);

        Rect r = rec;

        if (resultMatTexBlender != null)
        {
            myCamera.backgroundColor = resultMatTexBlender.GetColorIfNoTexture(texSet.matsAndGOs.mats[0].mat, texturePropertyName);
        }
        else
        {
            myCamera.backgroundColor = MB3_TextureCombiner.GetColorIfNoTexture(texturePropertyName);
        }

        if (source.t == null)
        {
            source.t = combiner._createTemporaryTexture(16, 16, TextureFormat.ARGB32, true);
        }

        r.y = 1f - (r.y + r.height); // DrawTexture uses topLeft 0,0, Texture2D uses bottomLeft 0,0 
        r.x *= _destinationTexture.width;
        r.y *= _destinationTexture.height;
        r.width *= _destinationTexture.width;
        r.height *= _destinationTexture.height;

        Rect rPadded = r;
        rPadded.x -= _padding;
        rPadded.y -= _padding;
        rPadded.width += _padding * 2;
        rPadded.height += _padding * 2;

        Rect srcPrTex = source.matTilingRect.GetRect();
        Rect targPr = new Rect();

        if (_fixOutOfBoundsUVs)
        {
            Rect ruv = new Rect(obUVoffset.x, obUVoffset.y, obUVscale.x, obUVscale.y);
            srcPrTex = MB3_UVTransformUtility.CombineTransforms(ref srcPrTex, ref ruv);
            if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("Fixing out of bounds UVs for tex " + source.t);
        }


        Texture2D tex = source.t;
        /*
        if (_considerNonTextureProperties && resultMatTexBlender != null)
        {
            if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log(string.Format("Blending texture {0} mat {1} with non-texture properties using TextureBlender {2}", tex.name, texSet.mats[0].mat, resultMatTexBlender));

            resultMatTexBlender.OnBeforeTintTexture(texSet.mats[0].mat, texturePropertyName.name);
            //combine the tintColor with the texture
            tex = combiner._createTextureCopy(tex);
            for (int i = 0; i < tex.height; i++)
            {
                Color[] cs = tex.GetPixels(0, i, tex.width, 1);
                for (int j = 0; j < cs.Length; j++)
                {
                    cs[j] = resultMatTexBlender.OnBlendTexturePixel(texturePropertyName.name, cs[j]);
                }
                tex.SetPixels(0, i, tex.width, 1, cs);
            }
            tex.Apply();
        }
        */
        //main texture
        TextureWrapMode oldTexWrapMode = tex.wrapMode;
        if (srcPrTex.width == 1f && srcPrTex.height == 1f && srcPrTex.x == 0f && srcPrTex.y == 0f)
        {
            //fixes bug where there is a dark line at the edge of the texture
            tex.wrapMode = TextureWrapMode.Clamp;
        }
        else
        {
            tex.wrapMode = TextureWrapMode.Repeat;
        }


        if (LOG_LEVEL >= MB2_LogLevel.trace) Debug.Log("DrawTexture tex=" + tex.name + " destRect=" + r + " srcRect=" + srcPrTex + " Mat=" + mat);


        //fill the padding first
        Rect srcPr = new Rect();

        //top margin
        srcPr.x = srcPrTex.x;
        srcPr.y = srcPrTex.y + 1 - 1f / tex.height;
        srcPr.width = srcPrTex.width;
        srcPr.height = 1f / tex.height;
        targPr.x = r.x;
        targPr.y = rPadded.y;
        targPr.width = r.width;
        targPr.height = _padding;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);

        //bot margin
        srcPr.x = srcPrTex.x;
        srcPr.y = srcPrTex.y;
        srcPr.width = srcPrTex.width;
        srcPr.height = 1f / tex.height;
        targPr.x = r.x;
        targPr.y = r.y + r.height;
        targPr.width = r.width;
        targPr.height = _padding;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);


        //left margin
        srcPr.x = srcPrTex.x;
        srcPr.y = srcPrTex.y;
        srcPr.width = 1f / tex.width;
        srcPr.height = srcPrTex.height;
        targPr.x = rPadded.x;
        targPr.y = r.y;
        targPr.width = _padding;
        targPr.height = r.height;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);

        //right margin
        srcPr.x = srcPrTex.x + 1f - 1f / tex.width;
        srcPr.y = srcPrTex.y;
        srcPr.width = 1f / tex.width;
        srcPr.height = srcPrTex.height;
        targPr.x = r.x + r.width;
        targPr.y = r.y;
        targPr.width = _padding;
        targPr.height = r.height;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);


        //top left corner
        srcPr.x = srcPrTex.x;
        srcPr.y = srcPrTex.y + 1 - 1f / tex.height;
        srcPr.width = 1f / tex.width;
        srcPr.height = 1f / tex.height;
        targPr.x = rPadded.x;
        targPr.y = rPadded.y;
        targPr.width = _padding;
        targPr.height = _padding;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);

        //top right corner
        srcPr.x = srcPrTex.x + 1f - 1f / tex.width;
        srcPr.y = srcPrTex.y + 1 - 1f / tex.height;
        srcPr.width = 1f / tex.width;
        srcPr.height = 1f / tex.height;
        targPr.x = r.x + r.width;
        targPr.y = rPadded.y;
        targPr.width = _padding;
        targPr.height = _padding;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);

        //bot left corner
        srcPr.x = srcPrTex.x;
        srcPr.y = srcPrTex.y;
        srcPr.width = 1f / tex.width;
        srcPr.height = 1f / tex.height;
        targPr.x = rPadded.x;
        targPr.y = r.y + r.height;
        targPr.width = _padding;
        targPr.height = _padding;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);

        //bot right corner
        srcPr.x = srcPrTex.x + 1f - 1f / tex.width;
        srcPr.y = srcPrTex.y;
        srcPr.width = 1f / tex.width;
        srcPr.height = 1f / tex.height;
        targPr.x = r.x + r.width;
        targPr.y = r.y + r.height;
        targPr.width = _padding;
        targPr.height = _padding;
        Graphics.DrawTexture(targPr, tex, srcPr, 0, 0, 0, 0, mat);

        //now the texture
        Graphics.DrawTexture(r, tex, srcPrTex, 0, 0, 0, 0, mat);

        tex.wrapMode = oldTexWrapMode;
    }

    private void _printTexture(Texture2D t)
    {
        if (((Texture)t).width * ((Texture)t).height > 100)
            Debug.Log("Not printing texture too large.");
        try
        {
            Color32[] pixels32 = t.GetPixels32();
            string str = "";
            for (int index1 = 0; index1 < ((Texture)t).height; ++index1)
            {
                for (int index2 = 0; index2 < ((Texture)t).width; ++index2)
                    str = str + pixels32[index1 * ((Texture)t).width + index2] + ", ";
                str += "\n";
            }
            Debug.Log(str);
        }
        catch (Exception ex)
        {
            Debug.Log(("Could not print texture. texture may not be readable." + ex.ToString()));
        }
    }
}

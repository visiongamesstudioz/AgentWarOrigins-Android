
using System;
using UnityEngine;

[Serializable]
public class MB_MaterialAndUVRect
{
    public Material material;
    public Rect atlasRect;
    public string srcObjName;
    public Rect samplingRectMatAndUVTiling;
    public Rect sourceMaterialTiling;
    public Rect samplingEncapsulatinRect;

    public MB_MaterialAndUVRect(Material m, Rect destRect, Rect samplingRectMatAndUVTiling, Rect sourceMaterialTiling, Rect samplingEncapsulatinRect, string objName)
    {
        this.material = m;
        this.atlasRect = destRect;
        this.samplingRectMatAndUVTiling = samplingRectMatAndUVTiling;
        this.sourceMaterialTiling = sourceMaterialTiling;
        this.samplingEncapsulatinRect = samplingEncapsulatinRect;
        this.srcObjName = objName;
    }

    public override int GetHashCode()
    {
        return (this.material).GetInstanceID() ^ this.samplingEncapsulatinRect.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        if (!(obj is MB_MaterialAndUVRect))
            return false;
        return (this.material== ((MB_MaterialAndUVRect)obj).material) && (this.samplingEncapsulatinRect== ((MB_MaterialAndUVRect)obj).samplingEncapsulatinRect);
    }
}

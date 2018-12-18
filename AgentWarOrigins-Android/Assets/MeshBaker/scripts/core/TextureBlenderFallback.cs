// Decompiled with JetBrains decompiler
// Type: DigitalOpus.MB.Core.TextureBlenderFallback
// Assembly: MeshBakerCore, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D590286C-1214-465B-A384-78BAAD755E88
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\MeshBakerCore.dll

using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public class TextureBlenderFallback : TextureBlender
    {
        private bool m_doTintColor = false;
        private Color m_defaultColor = Color.white;
        private Color m_tintColor;

        public bool DoesShaderNameMatch(string shaderName)
        {
            return true;
        }

        public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
        {
            if (shaderTexturePropertyName.Equals("_MainTex"))
            {
                this.m_doTintColor = true;
                this.m_tintColor = Color.white;
                if (sourceMat.HasProperty("_Color"))
                {
                    this.m_tintColor = sourceMat.GetColor("_Color");
                }
                else
                {
                    if (!sourceMat.HasProperty("_TintColor"))
                        return;
                    this.m_tintColor = sourceMat.GetColor("_TintColor");
                }
            }
            else
                this.m_doTintColor = false;
        }

        public Color OnBlendTexturePixel(string shaderPropertyName, Color pixelColor)
        {
            if (this.m_doTintColor)
                return new Color((float)(pixelColor.r * this.m_tintColor.r), (float)(pixelColor.g * this.m_tintColor.g), (float)(pixelColor.b * this.m_tintColor.b), (float)(pixelColor.a * this.m_tintColor.a));
            return pixelColor;
        }

        public bool NonTexturePropertiesAreEqual(Material a, Material b)
        {
            if (a.HasProperty("_Color"))
            {
                if (_compareColor(a, b, this.m_defaultColor, "_Color"))
                    return true;
            }
            else if (a.HasProperty("_TintColor") && _compareColor(a, b, this.m_defaultColor, "_TintColor"))
                return true;
            return false;
        }

        public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
        {
            if (resultMaterial.HasProperty("_Color"))
            {
                resultMaterial.SetColor("_Color", this.m_defaultColor);
            }
            else
            {
                if (!resultMaterial.HasProperty("_TintColor"))
                    return;
                resultMaterial.SetColor("_TintColor", this.m_defaultColor);
            }
        }

        public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texProperty)
        {
            if (texProperty.isNormalMap)
                return new Color(0.5f, 0.5f, 1f);
            if (texProperty.name.Equals("_MainTex"))
            {
                if ((mat!= null) && mat.HasProperty("_Color"))
                {
                    try
                    {
                        return mat.GetColor("_Color");
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else if ((mat!= null) && mat.HasProperty("_TintColor"))
                {
                    try
                    {
                        return mat.GetColor("_TintColor");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else if (texProperty.name.Equals("_SpecGlossMap"))
            {
                if ((mat!= null) && mat.HasProperty("_SpecColor"))
                {
                    try
                    {
                        Color color = mat.GetColor("_SpecColor");
                        if (mat.HasProperty("_Glossiness"))
                        {
                            try
                            {
                                color.a = mat.GetFloat("_Glossiness");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        Debug.LogWarning(color);
                        return color;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else if (texProperty.name.Equals("_MetallicGlossMap"))
            {
                if ((mat!= null) && mat.HasProperty("_Metallic"))
                {
                    try
                    {
                        float num = mat.GetFloat("_Metallic");
                        // ISSUE: explicit reference operation
                        var color = new Color(num, num, num);
                        if (mat.HasProperty("_Glossiness"))
                        {
                            try
                            {
                                color.a = mat.GetFloat("_Glossiness");
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        return color;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
            {
                if (texProperty.name.Equals("_ParallaxMap"))
                    return new Color(0.0f, 0.0f, 0.0f, 0.0f);
                if (texProperty.name.Equals("_OcclusionMap"))
                    return new Color(1f, 1f, 1f, 1f);
                if (texProperty.name.Equals("_EmissionMap"))
                {
                    if ((mat!= null) && mat.HasProperty("_EmissionScaleUI"))
                    {
                        if (mat.HasProperty("_EmissionColor") && mat.HasProperty("_EmissionColorUI"))
                        {
                            try
                            {
                                Color color1 = mat.GetColor("_EmissionColor");
                                Color color2 = mat.GetColor("_EmissionColorUI");
                                float num = mat.GetFloat("_EmissionScaleUI");
                                if ((color1== new Color(0.0f, 0.0f, 0.0f, 0.0f)) && (color2== new Color(1f, 1f, 1f, 1f)))
                                    return new Color(num, num, num, num);
                                return color2;
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        else
                        {
                            try
                            {
                                float num = mat.GetFloat("_EmissionScaleUI");
                                return new Color(num, num, num, num);
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
                else if (texProperty.name.Equals("_DetailMask"))
                    return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
            return new Color(1f, 1f, 1f, 0.0f);
        }

        public static bool _compareColor(Material a, Material b, Color defaultVal, string propertyName)
        {
            Color color1 = defaultVal;
            Color color2 = defaultVal;
            if (a.HasProperty(propertyName))
                color1 = a.GetColor(propertyName);
            if (b.HasProperty(propertyName))
                color2 = b.GetColor(propertyName);
            return !(color1 == color2);
        }

        public static bool _compareFloat(Material a, Material b, float defaultVal, string propertyName)
        {
            float num1 = defaultVal;
            float num2 = defaultVal;
            if (a.HasProperty(propertyName))
                num1 = a.GetFloat(propertyName);
            if (b.HasProperty(propertyName))
                num2 = b.GetFloat(propertyName);
            return (double)num1 == (double)num2;
        }
    }
}

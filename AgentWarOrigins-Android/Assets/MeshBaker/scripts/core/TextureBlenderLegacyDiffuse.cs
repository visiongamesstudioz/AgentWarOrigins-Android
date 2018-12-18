// Decompiled with JetBrains decompiler
// Type: DigitalOpus.MB.Core.TextureBlenderLegacyDiffuse
// Assembly: MeshBakerCore, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D590286C-1214-465B-A384-78BAAD755E88
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\MeshBakerCore.dll

using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public class TextureBlenderLegacyDiffuse : TextureBlender
    {
        private Color m_defaultTintColor = Color.white;
        private bool doColor;
        private Color m_tintColor;

        public bool DoesShaderNameMatch(string shaderName)
        {
            return shaderName.Equals("Legacy Shaders/Diffuse") || shaderName.Equals("Diffuse");
        }

        public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
        {
            if (shaderTexturePropertyName.EndsWith("_MainTex"))
            {
                doColor = true;
                m_tintColor = sourceMat.GetColor("_Color");
            }
            else
                doColor = false;
        }

        public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
        {
            if (doColor)
                return new Color((float)(pixelColor.r * m_tintColor.r), (float)(pixelColor.g * m_tintColor.g), (float)(pixelColor.b * m_tintColor.b), (float)(pixelColor.a * m_tintColor.a));
            return pixelColor;
        }

        public bool NonTexturePropertiesAreEqual(Material a, Material b)
        {
            return TextureBlenderFallback._compareColor(a, b, m_defaultTintColor, "_Color");
        }

        public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
        {
            resultMaterial.SetColor("_Color", Color.white);
        }

        public Color GetColorIfNoTexture(Material m, ShaderTextureProperty texPropertyName)
        {
            if (texPropertyName.name.Equals("_MainTex"))
            {
                if ((m!= null) && m.HasProperty("_Color"))
                {
                    try
                    {
                        return m.GetColor("_Color");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            return new Color(1f, 1f, 1f, 0.0f);
        }
    }
}

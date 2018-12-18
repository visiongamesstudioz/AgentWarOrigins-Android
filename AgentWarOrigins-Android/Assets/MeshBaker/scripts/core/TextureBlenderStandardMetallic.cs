

using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public class TextureBlenderStandardMetallic : TextureBlender
    {
        private Prop propertyToDo = Prop.doNone;
        private Color m_defaultColor = Color.white;
        private float m_defaultMetallic = 0.0f;
        private float m_defaultGlossiness = 0.5f;
        private Color m_defaultEmission = Color.black;
        private Color m_tintColor;
        private Color m_emission;

        public bool DoesShaderNameMatch(string shaderName)
        {
            return shaderName.Equals("Standard");
        }

        public void OnBeforeTintTexture(Material sourceMat, string shaderTexturePropertyName)
        {
            if (shaderTexturePropertyName.Equals("_MainTex"))
            {
                propertyToDo = Prop.doColor;
                if (sourceMat.HasProperty(shaderTexturePropertyName))
                    m_tintColor = sourceMat.GetColor("_Color");
                else
                    m_tintColor = m_defaultColor;
            }
            else if (shaderTexturePropertyName.Equals("_MetallicGlossMap"))
                propertyToDo = Prop.doMetallic;
            else if (shaderTexturePropertyName.Equals("_EmissionMap"))
            {
                propertyToDo = Prop.doEmission;
                if (sourceMat.HasProperty(shaderTexturePropertyName))
                    m_emission = sourceMat.GetColor("_EmissionColor");
                else
                    m_emission = m_defaultEmission;
            }
            else
                propertyToDo = Prop.doNone;
        }

        public Color OnBlendTexturePixel(string propertyToDoshaderPropertyName, Color pixelColor)
        {
            if (propertyToDo == Prop.doColor)
                return new Color((float)(pixelColor.r * m_tintColor.r), (float)(pixelColor.g * m_tintColor.g), (float)(pixelColor.b * m_tintColor.b), (float)(pixelColor.a * m_tintColor.a));
            if (propertyToDo == Prop.doMetallic || propertyToDo != Prop.doEmission)
                return pixelColor;
            return new Color((float)(pixelColor.r * m_emission.r), (float)(pixelColor.g * m_emission.g), (float)(pixelColor.b * m_emission.b), (float)(pixelColor.a * m_emission.a));
        }

        public bool NonTexturePropertiesAreEqual(Material a, Material b)
        {
            return TextureBlenderFallback._compareColor(a, b, m_defaultColor, "_Color") && TextureBlenderFallback._compareFloat(a, b, m_defaultMetallic, "_Metallic") && (TextureBlenderFallback._compareFloat(a, b, m_defaultGlossiness, "_Glossiness") && TextureBlenderFallback._compareColor(a, b, m_defaultEmission, "_EmissionColor"));
        }

        public void SetNonTexturePropertyValuesOnResultMaterial(Material resultMaterial)
        {
            resultMaterial.SetColor("_Color", m_defaultColor);
            resultMaterial.SetFloat("_Metallic", m_defaultMetallic);
            resultMaterial.SetFloat("_Glossiness", m_defaultGlossiness);
            if ((resultMaterial.GetTexture("_EmissionMap")== null))
                resultMaterial.SetColor("_EmissionColor", Color.black);
            else
                resultMaterial.SetColor("_EmissionColor", Color.white);
        }

        public Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texPropertyName)
        {
            if (texPropertyName.name.Equals("_BumpMap"))
                return new Color(0.5f, 0.5f, 1f);
            if (texPropertyName.Equals("_MainTex"))
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
            }
            else if (texPropertyName.name.Equals("_MetallicGlossMap"))
            {
                if ((mat== null) || !mat.HasProperty("_Metallic"))
                    return new Color(0.0f, 0.0f, 0.0f, 0.5f);
                try
                {
                    float num = mat.GetFloat("_Metallic");
                    Color color=new Color(num,num,num);

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
            else
            {
                if (texPropertyName.name.Equals("_ParallaxMap"))
                    return new Color(0.0f, 0.0f, 0.0f, 0.0f);
                if (texPropertyName.name.Equals("_OcclusionMap"))
                    return new Color(1f, 1f, 1f, 1f);
                if (texPropertyName.name.Equals("_EmissionMap"))
                {
                    if ((mat!= null))
                    {
                        if (!mat.HasProperty("_EmissionColor"))
                            return Color.black;
                        try
                        {
                            return mat.GetColor("_EmissionColor");
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                else if (texPropertyName.name.Equals("_DetailMask"))
                    return new Color(0.0f, 0.0f, 0.0f, 0.0f);
            }
            return new Color(1f, 1f, 1f, 0.0f);
        }

        private enum Prop
        {
            doColor,
            doMetallic,
            doEmission,
            doNone,
        }
    }
}

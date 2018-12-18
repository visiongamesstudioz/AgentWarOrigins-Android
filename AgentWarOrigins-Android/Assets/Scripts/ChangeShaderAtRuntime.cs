using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShaderAtRuntime : MonoBehaviour
{

    public bool ChangeShaderToMobile;
    private Renderer[] m_Renderers;
    public Shader m_MobileDiffuseShader;
    public Shader m_StandardShader;
	// Use this for initialization
    void Awake()
    {
       
    }
	void Start ()
	{

	    m_Renderers = GetComponentsInChildren<Renderer>();
	    if (m_Renderers==null) return;
	    ChangeToShader(ChangeShaderToMobile ? m_MobileDiffuseShader : m_StandardShader);
	}

    void ChangeToShader(Shader shader)
    {
        foreach (var renderer1 in m_Renderers)
        {
            Material[] gameObjectMaterials = renderer1.materials;
            for (int i = 0; i < gameObjectMaterials.Length; i++)
            {
                renderer1.materials[i].shader = shader;
            }
        }
    }
}

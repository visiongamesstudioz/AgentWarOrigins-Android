using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
using UnityEngine;

public class EnableHighlighter : MonoBehaviour
{

    private HighlightingRenderer m_HighLightingRenderer;
	// Use this for initialization
	void Start ()
	{
        Animator[] animators = GetComponentsInChildren<Animator>();

        m_HighLightingRenderer = GetComponent<HighlightingRenderer>();
	    if (QualitySettings.GetQualityLevel() >= 2)
	    {
	        if (m_HighLightingRenderer)
	        {
                m_HighLightingRenderer.enabled = true;
            }

        }
	    foreach (var animator in animators)
	    {
	        animator.enabled = false;
	    }
	}
	

}

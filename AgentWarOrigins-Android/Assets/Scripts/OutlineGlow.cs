using System.Collections;
using System.Collections.Generic;
using HighlightingSystem;
using UnityEngine;

public class OutlineGlow : MonoBehaviour
{

    public Color StartColor;
    public Color EndColor;
    public float Frequency;

    private Highlighter m_highLighter;

    private void Awake()
    {
        m_highLighter = GetComponent<Highlighter>();
    }

    // Use this for initialization
    void Start()
    {
        if (m_highLighter)
        {
            m_highLighter.FlashingOn(StartColor, EndColor, Frequency);

        }

    }

    private void OnEnable()
    {
        if (m_highLighter)
        {
            m_highLighter.FlashingOn(StartColor, EndColor, Frequency);
        }
    }

}

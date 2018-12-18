using EndlessRunner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayBloodOnScreen : MonoBehaviour {

    private PlayerHealth m_PlayerHealth;
    private bool m_ShowGUI;
    private Color lerpColor;
    public static int ScreenWidth;
    public static int ScreenHeight;
    public Texture BloodSplatter;

    private Rect rect;

    private void Awake()
    {
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_ShowGUI = m_PlayerHealth.ShowGUI;
        ScreenWidth = PlayerPrefs.GetInt("ScreenWidth");
        ScreenHeight = PlayerPrefs.GetInt("ScreenHeight");
        rect = new Rect(0, 0, ScreenWidth, ScreenHeight);
    }



    private void OnGUI()
    {
        lerpColor = m_PlayerHealth.LerpColor;
        GUI.color = lerpColor;
        if (lerpColor.a < 0)
        {
            lerpColor.a = 0;
        }
        if (Math.Abs(lerpColor.a) < 0.001f)
        {
            m_PlayerHealth.ShowGUI = false;
        }
        GUI.DrawTexture(rect, BloodSplatter);
    }

}

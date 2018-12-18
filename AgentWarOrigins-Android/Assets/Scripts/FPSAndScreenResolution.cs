using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSAndScreenResolution : MonoBehaviour
{

    public float Aspectratio;
    public int ScreenWidth;
    public int ScreenHeight;
    int resolutionPercentage = 100;
    int fpsLimit = 30;
    int autoQuality;

    // Use this for initialization
    void Start()
    {
        CreateScreenResolutionPref();
    }

    public void CreateScreenResolutionPref()
    {
        if (!PlayerPrefs.HasKey("ScreenWidth"))
        {
            PlayerPrefs.SetInt("ScreenWidth", Screen.width);
        }
        if (!PlayerPrefs.HasKey("ScreenHeight"))
        {
            PlayerPrefs.SetInt("ScreenHeight", Screen.height);
        }
        PlayerPrefs.Save();
    }
    void SetScreenResolution()
    {
        if (PlayerPrefs.HasKey("ScreenResolution"))
        {
            resolutionPercentage = PlayerPrefs.GetInt("ScreenResolution");
        }
        else
        {
            //resolutionPercentage = 80;
            if (autoQuality < 1)
            {
                resolutionPercentage = 80;
                PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);
            }
            else
            {
                resolutionPercentage = 100;

                PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);

            }
        }
        PlayerPrefs.Save();
        Util.SetResolution(resolutionPercentage);
    }

    void SetFPS()
    {

        if (PlayerPrefs.HasKey("FPSLimit"))
        {
            fpsLimit = PlayerPrefs.GetInt("FPSLimit");
        }
        else
        {
            if (autoQuality < 2)
            {
                fpsLimit = 30;
                PlayerPrefs.SetInt("FPSLimit", fpsLimit);
            }
            else
            {
                fpsLimit = 60;
                PlayerPrefs.SetInt("FPSLimit", fpsLimit);
            }

            Application.targetFrameRate = fpsLimit;
        }
    }
}


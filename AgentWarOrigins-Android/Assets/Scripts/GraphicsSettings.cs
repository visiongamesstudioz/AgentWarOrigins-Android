using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsSettings : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        ApplyGraphicsSettings();
    }

    void ApplyGraphicsSettings()
    {

        int fpsLimit = 30;
        int resolutionPercentage = 100;
        int autoQuality = AutoQuality.Instance.detectQuality(false,false);
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
        }
        if (PlayerPrefs.HasKey("ScreenResolution"))
        {
            resolutionPercentage = PlayerPrefs.GetInt("ScreenResolution");
        }
        else
        {
            //resolutionPercentage = 80;
            if (autoQuality < 2)
            {
                resolutionPercentage = 60;
                PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);
            }
            else
            {
                resolutionPercentage = 80;

                PlayerPrefs.SetInt("ScreenResolution", resolutionPercentage);

            }
        }
    }
}



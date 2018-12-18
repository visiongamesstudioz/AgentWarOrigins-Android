using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotRender : MonoBehaviour {

    void Start()
    {
        Debug.Log("rendering");
        ScreenCapture.CaptureScreenshot("Assets/UI/renders/unity/render_2", 2);
    }
}

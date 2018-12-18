using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRenderMode : MonoBehaviour {

 
    public void ChangeCanvasRenderModeToOverlay(Canvas canvas)
    {
        Util.ChangeCanvasRenderMode(canvas,RenderMode.ScreenSpaceOverlay);
    }

    public void ChangeCanvasRenderToCamera(Canvas canvas)
    {
        Util.ChangeCanvasRenderMode(canvas, RenderMode.ScreenSpaceCamera);

    }
}

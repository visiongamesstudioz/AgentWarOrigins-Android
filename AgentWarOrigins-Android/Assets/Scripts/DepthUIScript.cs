using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthUIScript : MonoBehaviour {
   
    public float depth;
    public CanvasGroup canvasGroup;

    public void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;

        if (alpha <= 0)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}

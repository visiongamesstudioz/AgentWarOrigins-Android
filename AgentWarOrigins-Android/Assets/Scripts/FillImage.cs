using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
public class FillImage : MonoBehaviour
{

    private Image image;
    public float FillTime;
    public float waitTimetoLoop = 10f;
    private float currentWaitTimetoLoop;
	// Use this for initialization
	void Start ()
	{

	    image = GetComponent<Image>();
	    currentWaitTimetoLoop = waitTimetoLoop;
	}
	
	// Update is called once per frame
	void Update () {

        
	    if (image.fillMethod == Image.FillMethod.Radial360 && Math.Abs(image.fillAmount - 1) < 0.00001f)
	    {
	        currentWaitTimetoLoop -= Time.deltaTime;
	        if (currentWaitTimetoLoop < 0)
	        {
                image.fillAmount = 0;
	            currentWaitTimetoLoop = waitTimetoLoop;
            }
            return;

        }
        image.fillAmount += 1.0f / FillTime * Time.deltaTime;

    }
}

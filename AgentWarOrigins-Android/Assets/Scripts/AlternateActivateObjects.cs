using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;
using UnityEngine.UI;

public class AlternateActivateObjects : MonoBehaviour
{

    public GameObject[] AlternativeGameObjects;
    public bool isTypeRenderer;
    public Renderer[] FirstObjectRenderers;
    public Renderer[] SecondObjectRenderers;
    private WaitForSeconds waitForSeconds =new WaitForSeconds(3);
        // Use this for initialization
	void Start ()
	{
	    FirstObjectRenderers = AlternativeGameObjects[0].GetComponentsInChildren<Renderer>(true);
	    SecondObjectRenderers = AlternativeGameObjects[1].GetComponentsInChildren<Renderer>(true);
	    StartCoroutine(ActivateAlternatively());
	}

    IEnumerator ActivateAlternatively()
    {

        while (true)
        {
            if (isTypeRenderer)
            {
                foreach (var ren in FirstObjectRenderers)
                {
                    ren.enabled = true;
                }
                foreach (var ren in SecondObjectRenderers)
                {
                    ren.enabled = false;
                }

            }
            else
            {
                AlternativeGameObjects[0].SetActive(true);
                AlternativeGameObjects[1].SetActive(false);
            }
            yield return waitForSeconds;
            if (isTypeRenderer)
            {
                foreach (var ren in FirstObjectRenderers)
                {
                    ren.enabled = false;
                }
                foreach (var ren in SecondObjectRenderers)
                {
                    ren.enabled = true;
                }
            }

            else
            {
                AlternativeGameObjects[1].SetActive(true);
                AlternativeGameObjects[0].SetActive(false);
            }

            yield return waitForSeconds;
        }
    }
   
}

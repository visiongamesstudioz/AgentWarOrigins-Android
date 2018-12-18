using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByTime : MonoBehaviour
{
    public bool OnlyDisable;
    public float Time;
	// Use this for initialization
	void Start () {

	 
        Invoke("DisableOrDestroy",Time);
    }

    void DisableOrDestroy()
    {
        if (OnlyDisable)
        {
            gameObject.SetActive(false);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}

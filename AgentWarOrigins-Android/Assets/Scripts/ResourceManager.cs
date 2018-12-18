using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{

	    AsyncOperation unloadUnUsedResources= Resources.UnloadUnusedAssets();

	    while (!unloadUnUsedResources.isDone)
	    {
	        Debug.Log("unloading unused resources");
	    }
        Debug.Log("unload resources done");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

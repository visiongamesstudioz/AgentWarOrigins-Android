using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateUniquePlayerID : MonoBehaviour {



	// Use this for initialization
	void Start ()
	{

	    Debug.Log(System.Guid.NewGuid());

	}

}




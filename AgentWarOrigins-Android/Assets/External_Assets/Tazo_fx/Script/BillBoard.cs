using UnityEngine;
using System.Collections;

public class BillBoard : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Camera.main)
	    {
            this.transform.rotation = Camera.main.transform.rotation;

        }
    }
}


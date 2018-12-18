using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddReflectionProbeDynamically : MonoBehaviour {


	// Use this for initialization
	void Start () {

        GameObject probeGameObject = new GameObject("The Reflection Probe");
        ReflectionProbe probeComponent = probeGameObject.AddComponent<ReflectionProbe>() as ReflectionProbe;
        probeComponent.resolution = 128;
        probeComponent.size = new Vector3(200, 200, 200);
        probeGameObject.transform.position = new Vector3(0, 5, 0);
	    probeComponent.RenderProbe();
	}
	
}

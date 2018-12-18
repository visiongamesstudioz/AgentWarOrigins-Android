using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateCar : MonoBehaviour
{
    public GameObject car;
    private float timer = 3f;
    private float curenttimer;
    private GameObject go;
	// Use this for initialization
	void Start ()
	{

        //go= Instantiate(car, new Vector3(70, car.transform.position.y, 0), car.transform.localRotation) as GameObject;
        go = Instantiate(car, Vector3.zero, Quaternion.identity) as GameObject;
	    curenttimer = timer;
	}
	
	// Update is called once per frame
	void Update ()
	{

    }
}

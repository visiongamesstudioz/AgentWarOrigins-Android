using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeXPTextScript : MonoBehaviour
{

    private Transform m_PlayerTransform;

    void Awake()
    {
        m_PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

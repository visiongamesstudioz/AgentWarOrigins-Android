using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleRootMotion : MonoBehaviour {

    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    public bool jump;

    // Use this for initialization
	void Start () {

        m_Rigidbody = GetComponent<Rigidbody>();
	    m_Animator = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update () {

        if (jump)
        {
            m_Rigidbody.velocity = new Vector3(10 , 10, m_Rigidbody.velocity.z);
        }
    }

    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (Time.deltaTime > 0)
        {
            var v = m_Animator.deltaPosition  / Time.deltaTime;

            // we preserve the existing y part of the current velocity.
            v.y = m_Rigidbody.velocity.y;
            m_Rigidbody.velocity = v;
        }
    }
}

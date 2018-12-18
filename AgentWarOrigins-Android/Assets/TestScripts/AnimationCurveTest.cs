using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveTest : MonoBehaviour
{
    public LayerMask layerMask;
    public float DefaultColliderHeight;
    private int ColliderCenterY = Animator.StringToHash("ColliderY");
    private int JumpDownStatehash = Animator.StringToHash("LowerBody Layer.JumpDown");
    private Animator anim;
    private AnimatorStateInfo currentBaseState;
    private CapsuleCollider capsuleCollider;
    private Rigidbody m_Rigidbody;
    public float m_GroundCheckDistance=0.3f;
    private bool m_IsGrounded;
    // Use this for initialization
    void Start ()
	{
	    anim = GetComponent<Animator>();
	    capsuleCollider = GetComponent<CapsuleCollider>();
	    m_Rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update ()
	{
        CheckGroundStatus();


        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
        if (anim.GetCurrentAnimatorStateInfo(1).fullPathHash == JumpDownStatehash)
        {
            capsuleCollider.center = new Vector3(0, anim.GetFloat(ColliderCenterY), 0);

        }
        else
        {
            capsuleCollider.center = new Vector3(0, DefaultColliderHeight, 0);

        }
    }

    private void FixedUpdate()
{
}

private void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + Vector3.up * 0.01f,
            transform.position + Vector3.up * 0.1f + Vector3.down * 0.1f);
#endif

        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, m_GroundCheckDistance,layerMask))
        {
            Debug.Log("grounded");

            m_IsGrounded = true;

        }
        else
        {
            m_IsGrounded = false;

        }
    }


    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded )
        {
            Debug.Log("grounded");
            
        }
        else if(anim.GetCurrentAnimatorStateInfo(1).fullPathHash== JumpDownStatehash)
        {
            Debug.Log("not grounded");
            transform.position = anim.rootPosition;
        }

    }
}

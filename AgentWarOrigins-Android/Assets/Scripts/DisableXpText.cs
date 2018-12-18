using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableXpText : MonoBehaviour
{

    private Animator m_Animator;
    private AnimatorClipInfo[] animatorClipInfo;
	// Use this for initialization
    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    IEnumerator DisableXpTextCoroutine()
    {
        yield return new WaitForSeconds(animatorClipInfo[0].clip.length);
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        animatorClipInfo = m_Animator.GetCurrentAnimatorClipInfo(0);
        StartCoroutine(DisableXpTextCoroutine());
    }


}

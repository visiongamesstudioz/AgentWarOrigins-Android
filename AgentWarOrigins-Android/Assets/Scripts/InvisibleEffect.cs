using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InvisibleEffect : MonoBehaviour
{
    public GameObject InvisibleShaderEffect;
    public Transform InvisbleEffectTransform;
    public float TimeToStartEffect;
    public float TimeBetweenEffects;


    private GameObject m_EffectGo;
    private PSMeshRendererUpdater m_PsUpdater;
    private EffectType m_CurrentEffectType;
    private SkinnedMeshRenderer m_MainOutfitSR;
    private float currentTimeElapsedBetweenInvoke;
	// Use this for initialization

	void Start ()
	{
        GameObject mainOutfit = Util.FindGameObjectWithTag(gameObject, "Outfit");
        m_MainOutfitSR = mainOutfit.GetComponent<SkinnedMeshRenderer>();
        m_EffectGo = Instantiate(InvisibleShaderEffect, transform.position, new Quaternion()) as GameObject;
        m_PsUpdater = m_EffectGo.GetComponent<PSMeshRendererUpdater>();
        m_EffectGo.transform.parent = gameObject.transform;
        m_EffectGo.transform.rotation = new Quaternion();
        m_EffectGo.transform.localScale = Vector3.one;
        m_EffectGo.SetActive(false);
    }

    private void OnEnable()
    {

        m_CurrentEffectType = EffectType.Normal;
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            Invoke("StartEffect", TimeToStartEffect);
            InvokeRepeating("ChangeEffect", TimeToStartEffect + 10, TimeBetweenEffects);
        }

    }

    //private void OnDisable()
    //{
    //    ChangeEffect();
    //    CancelInvoke();
    //}

    public void StartEffect()
    {
        m_CurrentEffectType=EffectType.Invisible;
        if (m_PsUpdater == null)
        {
            m_EffectGo = Instantiate(InvisibleShaderEffect, transform.position, new Quaternion()) as GameObject;
            m_PsUpdater = m_EffectGo.GetComponent<PSMeshRendererUpdater>();
            m_EffectGo.transform.parent = gameObject.transform;
            m_EffectGo.transform.rotation = new Quaternion();
            m_EffectGo.transform.localScale = Vector3.one;
            m_EffectGo.SetActive(false);
        }
        m_PsUpdater.UpdateMeshEffect(gameObject,m_MainOutfitSR);
        
    }

    public void ChangeEffect()
    {
        if (m_CurrentEffectType == EffectType.Invisible)
        {
            m_PsUpdater.RemoveEffect(gameObject);
            m_EffectGo.SetActive(false);
            m_CurrentEffectType=EffectType.Normal;
            
        }
        else if(m_CurrentEffectType==EffectType.Normal)
        {
             m_PsUpdater.UpdateMeshEffect(gameObject);
            m_EffectGo.SetActive(true);
            m_CurrentEffectType=EffectType.Invisible;
        }
    }

    public void RemoveEffect()
    {
        if (m_CurrentEffectType == EffectType.Invisible)
        {
            m_PsUpdater.RemoveEffect(gameObject);
            m_EffectGo.SetActive(false);
            m_CurrentEffectType = EffectType.Normal;

        }
    }
    public EffectType GetCurrentEffectType()
    {
        return m_CurrentEffectType;
    }

}

public enum EffectType
{
    Normal,
    Invisible
}

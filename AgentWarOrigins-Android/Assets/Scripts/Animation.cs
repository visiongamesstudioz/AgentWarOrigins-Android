using System;
using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class Animation : MonoBehaviour
{

    protected int m_WeaponType;
    protected int m_RunStatehash;
    protected int m_CrouchStateHash;
    protected int m_ShootStateHash;
    protected int m_ReloadStatehash;
    protected int m_DeathCategoryStateHash;
    protected Animator m_Animator;
    protected int m_AimStatehash;
    protected int m_ExitShootAnimationStatehash;
    protected int m_DieStatehash;
    protected int m_HitStatehash;
    protected int m_JumpDownStatehash;
    protected int m_ColliderCenterY = Animator.StringToHash("ColliderY");
    private int m_ColliderHeight = Animator.StringToHash("ColliderHeight");
    protected int m_JumpDownState;
    protected int m_RunJumpState;

    protected AnimationEvent m_AnimationEvent;

    public string[] ReloadAnimationNames;
    public int CrouchStateHash
    {
        get { return m_CrouchStateHash; }
        set { m_CrouchStateHash = value; }
    }
    public int JumpDownState
    {
        get { return m_JumpDownState; }
    }

    public int ColliderCenterY
    {
        get { return m_ColliderCenterY; }
    }

    public int ColliderHeight
    {
        get { return m_ColliderHeight; }
        set { m_ColliderHeight = value; }
    }


    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

	// Use this for initialization
	protected virtual void Start ()
	{
        //state parameters
	    m_WeaponType = Animator.StringToHash("WeaponType");
        m_RunStatehash = Animator.StringToHash("StartRun");
	    m_CrouchStateHash = Animator.StringToHash("Crouch");
        m_AimStatehash = Animator.StringToHash("Aim");
	    m_ShootStateHash = Animator.StringToHash("Shoot");
	    m_ReloadStatehash = Animator.StringToHash("Reload");
	    m_DieStatehash = Animator.StringToHash("Die");
        m_DeathCategoryStateHash = Animator.StringToHash("DeathType");
	    m_ExitShootAnimationStatehash = Animator.StringToHash("ExitShooting");
        m_HitStatehash = Animator.StringToHash("Hit");
	    m_JumpDownStatehash = Animator.StringToHash("JumpDown");

        //states
	    m_JumpDownState = Animator.StringToHash("LowerBody Layer.JumpDown");

        m_AnimationEvent = new AnimationEvent();

        //add animation events
        foreach (var animationName in ReloadAnimationNames)
        {
            AnimationClip clip = GetAnimationClipFromAnimatorByName(m_Animator, animationName);
            if (clip)
            {
                AddAnimationEvent(m_AnimationEvent, clip, clip.length - 0.5f, "OnReloadComplete");

            }
        }
    }
    public void AddAnimationEvent(AnimationEvent animationEvent, AnimationClip animaClip, float time,string functionName)
    {
        //add events to dead clips=           
        animationEvent.functionName = functionName;
        animationEvent.time = time;
        animaClip.AddEvent(animationEvent);

    }
    internal static AnimationClip GetAnimationClipFromAnimatorByName(Animator animator, string name)
    {
        //can't get data if no animator
        if (animator == null)
            return null;
        if (animator.runtimeAnimatorController == null)
        {
            return null;
        }
        if (animator.runtimeAnimatorController.animationClips == null)
        {
            return null;
        }
        //favor for above foreach due to performance issues
        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (animator.runtimeAnimatorController.animationClips[i].name == name)
                return animator.runtimeAnimatorController.animationClips[i];
        }

        Debug.LogError("Animation clip: " + name + " not found");
        return null;
    }

    public void RemoveAnimationEventsFromAnimator(Animator animator,string[] animationNames)
    {
        foreach (var animationName in animationNames)
        {
            AnimationClip animationClip = GetAnimationClipFromAnimatorByName(animator, animationName);
            AnimationEvent[] animationEvents = new AnimationEvent[] {};
            if (animationClip)
            {
                animationClip.events = animationEvents;
            }
        }
    }

    //public void ShootRun(bool value)
    //{
    //    m_Animator.SetBool(m_InAirStateHash, false);
    //    m_Animator.SetBool(m_RunStatehash, value);
    //}

    public void SetAnimationType(bool isPistoltype)
    {
 
    }

    public void Run(bool value)
    {
        m_Animator.SetBool(m_RunStatehash, value);
    }
    //should implement
    public void SetShootAnimationType(WeaponType weaponType)
    {
        var weaponTypeId = (int) weaponType;

        m_Animator.SetInteger(m_WeaponType,weaponTypeId);
    }
    public void ExitShootAnimation()
    {

        if (m_Animator.GetCurrentAnimatorStateInfo(1).fullPathHash == m_ShootStateHash)
        {
            m_Animator.SetBool(m_ShootStateHash,false);
        }
        UnAim();
        m_Animator.SetBool(m_ExitShootAnimationStatehash,true);

    }

    public void StartShootAnimation()
    {
        m_Animator.SetBool(m_ExitShootAnimationStatehash, false);

    }
    public void Aim()
    {
        
        if (m_Animator.GetBool(m_ExitShootAnimationStatehash))
        {
            m_Animator.SetBool(m_ExitShootAnimationStatehash, false);
        }
        m_Animator.SetBool(m_AimStatehash,true);

    }

    public void UnAim()
    {
        m_Animator.SetBool(m_AimStatehash,false);
    }
    //public static bool HasParameter(int  paramHashId, Animator animator)
    //{
    //    foreach (AnimatorControllerParameter param in animator.parameters)
    //    {
    //        if (param.nameHash == paramHashId)
    //            return true;
    //    }
    //    return false;
    //}

    public void Shoot()
    {

        m_Animator.SetTrigger(m_ShootStateHash);     
    }

    public void Reload(bool value,WeaponManagement currentWeapon)
    {     
            m_Animator.SetBool(m_ReloadStatehash,value);        
    }
    //crouch animations
    public void Crouch()
    {
        UnAim();
        ExitShootAnimation();
        m_Animator.SetBool(m_CrouchStateHash, true);
    }

    public void Uncrouch()
    {
    
        m_Animator.SetBool(m_CrouchStateHash, false);
    }

    public void JumpDown(bool value)
    {
        m_Animator.SetBool(m_JumpDownStatehash,value);
    }
    public void Death(DeathType deathType)
    {
        //
        SetLayerWeight(1,0);
        var deathCategory = (int)deathType;
        UnAim();
        Run(false);
        m_Animator.SetBool(m_ExitShootAnimationStatehash,true);
        m_Animator.SetTrigger(m_DieStatehash);
        m_Animator.SetInteger(m_DeathCategoryStateHash, deathCategory);
    }
    public void Hit()
    {

        m_Animator.SetTrigger(m_HitStatehash);
       
    }
    public virtual void Revive()
    {
        if (m_Animator.isActiveAndEnabled)
        {

            m_Animator.SetBool(m_ExitShootAnimationStatehash, false);
        }
    }
    public bool IsAnimatorInitialized()
    {
        return m_Animator.isInitialized;
    }

    public void InitializeAnimator()
    {
        
    }
    public float GetCurrentAnimationTime(string animationName)
    {
        AnimatorStateInfo currentAnimatorStateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);
        float time = 0;
        RuntimeAnimatorController ac = m_Animator.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == animationName)        //If it has the same name as your clip
            {
                time = ac.animationClips[i].length;
              //  Debug.Log("current clip time" + time);
            }
        }
        return time;
    }

    public void SetLayerWeight(int layerId,float weight)
    {
        m_Animator.SetLayerWeight(layerId, weight);
    }
    public float GetColliderCenter()
    {
        return m_Animator.GetFloat(ColliderCenterY);
    }

    public float GetColliderHeight()
    {
        return m_Animator.GetFloat(ColliderHeight);
    }
    public Vector3 GetRootPosition()
    {
        return m_Animator.rootPosition;
    }
    public AnimatorStateInfo GetCurrentAnimationStateHashInLayer(int layerIndex)
    {
        return m_Animator.GetCurrentAnimatorStateInfo(layerIndex);
    }

    public AnimatorTransitionInfo GetCurrentTransitionInfoInLayer(int layerIndex)
    {
        return m_Animator.GetAnimatorTransitionInfo(layerIndex);
    }
    public bool IsAnimationPlaying()
    {
        if (m_Animator.GetAnimatorTransitionInfo(0).normalizedTime < 1)
        {
            return true;
        }
        return false;
    }

    private void OnDestroy()
    {

    }
    //receiver for animation event
    void OnDeath()
    {
        //do nothing
    }

    void OnHitObstacle()
    {
        //do nothing
    }

}

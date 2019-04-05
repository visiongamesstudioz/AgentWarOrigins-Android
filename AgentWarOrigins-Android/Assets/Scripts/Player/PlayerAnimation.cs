using System.Collections.Generic;
using UnityEngine;

namespace EndlessRunner
{
    public class PlayerAnimation : Animation
    { 
        private int m_IdleStateHash;
        private int m_JumpStatehash;
        private int m_JumpCategoryStateHash;
        private int m_SlideStateHash;
        private int m_StumbleStateHash;
 
        private int m_LeftStrafeStateHash;
        private int m_RightStrafeStateHash;
        private int m_FlyStateHash;
        private int m_InAirStateHash;
        private int m_ReviveStateHash;
        public string[] DeathAnimationNames;
        public float DefaultColliderCenter;
        public float DefaultColliderHeight;
        protected override void Awake()
        {
            base.Awake();

        }
        public int RunJumpState
        {
            get { return m_RunJumpState; }
        }

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            m_JumpStatehash = Animator.StringToHash("Jump");
            m_JumpCategoryStateHash = Animator.StringToHash("JumpCategory");
            m_SlideStateHash = Animator.StringToHash("Slide");
            m_StumbleStateHash = Animator.StringToHash("Stumble");    
            m_LeftStrafeStateHash = Animator.StringToHash("MoveLeft");
            m_RightStrafeStateHash = Animator.StringToHash("MoveRight");
            m_FlyStateHash = Animator.StringToHash("Fly");
            m_InAirStateHash = Animator.StringToHash("InAir");
            m_ReviveStateHash = Animator.StringToHash("Revive");
            m_AnimationEvent=new AnimationEvent();

            //states
            m_RunJumpState = Animator.StringToHash("Base Layer.Run_Jump");

            foreach (var animationName in DeathAnimationNames)
            {
                AnimationClip clip=GetAnimationClipFromAnimatorByName(m_Animator, animationName);
                if (clip)
                {
                    AddAnimationEvent(m_AnimationEvent, clip, clip.length, "OnDeath");
                    AddAnimationEvent(m_AnimationEvent, clip, clip.length, "OnHitObstacle");
                }
       
            }
            //  AnimationClip dyingBackwards = GetAnimationClipFromAnimatorByName(m_Animator, "Dyingbackwards");
       
        }

   
        public void Idle()
        {
            m_Animator.SetBool(m_RunStatehash, false);
        }

        public void Jump(JumpType jumpType)
        {
            var jumpCategory = (int) jumpType;
            m_Animator.SetTrigger(m_JumpStatehash);
            m_Animator.SetInteger(m_JumpCategoryStateHash, jumpCategory);
        }

        public void Slide()
        {
            m_Animator.SetTrigger(m_SlideStateHash);
        }

        public void Stumble()
        {
            m_Animator.SetTrigger(m_StumbleStateHash);
        }

        //public override void Death(DeathType deathType)
        //{
        //    var deathCategory = (int) deathType;
        //    m_Animator.SetInteger(m_DeathCategoryStateHash,deathCategory);
        //    m_Animator.SetTrigger(m_DeathStateHash);
        //}
   
        public void StrafeMovement(bool right)
        {
            m_Animator.SetTrigger(right ? m_RightStrafeStateHash : m_LeftStrafeStateHash);
        }

        public void Fall(int fallVariation)
        {
            m_Animator.SetTrigger(m_FlyStateHash);
            m_Animator.SetBool(m_InAirStateHash, true);

        }

        public void SetAnimatorSpeed(float animatorSpeed)
        {
            m_Animator.speed = animatorSpeed;
        }

        public void ApplyRootMotion(bool value)
        {
            m_Animator.applyRootMotion = value;
        }

        public override void Revive()
        {
            SetLayerWeight(1,1);
            m_Animator.SetTrigger(m_ReviveStateHash);
            StartShootAnimation();
        }
  

        void OnDeath()
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (Util.IsTutorialComplete())
            {
                GameManager.Instance.OnDeath();
            }
            else
            {
                transform.position = transform.position - 100 * Vector3.right;
                Revive();
                playerHealth.ResetHealth();
                playerHealth.IsDead = false;
            }
            
        }

        private void OnDestroy()
        {
            RemoveAnimationEventsFromAnimator(m_Animator,DeathAnimationNames);
        }

        public void MatchTarget(RaycastHit hitInfo,AvatarTarget targetAvatar,MatchTargetWeightMask weightMask,float startNormalizedTime,float targetNormalizedTime)
        {
            m_Animator.MatchTarget(hitInfo.point ,Quaternion.identity,targetAvatar,weightMask,startNormalizedTime,targetNormalizedTime);
        }

        public bool IsInTransition(int layerIndex)
        {
            return m_Animator.IsInTransition(layerIndex);
        }
    }
    
    public enum JumpType
    {
        RunningJump,
        RollJump
    }

    public enum DeathType
    {
        DyingBackwards,
        DeathBackwards,
        Flyingdeath,
        StandingReactDeathBackward,
        DeathFromFrontHeadshot,
        ZombieDeath,
        CrouchDeath

    }
}


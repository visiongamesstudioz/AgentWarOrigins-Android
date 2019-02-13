using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EndlessRunner
{
    public class PlayerHealth : Health
    {
        [SerializeField] [Tooltip("Can Regenerate Health")] private bool m_CanRegenerateHealth;
        public float NormalRegenerationTime = 30;
        [SerializeField] [Tooltip("time to wait before Regenerating")] private float m_RegenerateInitialWait;
        [SerializeField] [Tooltip("Amount of health generated every second")] private float m_RegenerateAmountOverTime;
        [SerializeField] [Tooltip("Can object Invincible")] private bool m_IsInvincible;
        [SerializeField] [Tooltip("Amount of time of invincibility")] private float m_InvincibilityTime;
        [SerializeField] [Tooltip("Can Player Revive")] private bool m_CanPlayerRevive;
        private float m_CurrentInvincibiltyTime;
        private float m_RemainingInitialWaitForRegeneration;
        private bool m_ShowGUI;
        public static PlayerHealth Instance;
        private UiManager uiManager;
        public float TimeBetweenScreens=3;
        private float m_CurrentTimeElapsedBetweenScreens;
        public Color LerpColor;
        private bool isFadeInStarted;
        private PlayerAnimation m_PlayerAnimation;
        private AudioSource m_audioSource;
        private Player m_Player;
        private PlayerControl m_PlayerControl;
        private InvisibleEffect m_InvisibleEffect;
        private float m_NormalTimeElapsedForRegeneration;
        private bool m_IsTakingDamage;
        private SprayBloodOnScreen m_sprayBloodOnScreen;
        public bool IsDead { get; set; }

        public bool ShowGUI
        {
            get { return m_ShowGUI; }
            set { m_ShowGUI = value; }
        }
        protected override void Awake()
        {

            Instance = this;
            uiManager=UiManager.Instance;
            base.Awake();
            m_CurrentInvincibiltyTime = 0;
            m_InvincibilityTime = float.MinValue;

            m_PlayerAnimation = GetComponent<PlayerAnimation>();
            m_PlayerControl = GetComponent<PlayerControl>();
            m_Player = GetComponent<Player>();
            m_audioSource = GetComponent<AudioSource>();
            m_InvisibleEffect = GetComponent<InvisibleEffect>();
            useGUILayout = false;
            m_sprayBloodOnScreen = GetComponent<SprayBloodOnScreen>();
        }

        private void Start()
        {
            if (m_CanRegenerateHealth)
                m_RemainingInitialWaitForRegeneration = m_RegenerateInitialWait;

           m_NormalTimeElapsedForRegeneration = NormalRegenerationTime;

            m_CurrentTimeElapsedBetweenScreens = TimeBetweenScreens;
        }
        private void FadeOut()
        {
           
        }
        public bool CanRegenerateHealth
        {

            get { return m_CanRegenerateHealth; }
            set { this.m_CanRegenerateHealth = value; }
        }

        public float RegenerateInitialWaitTime
        {
            set { this.m_RegenerateInitialWait = value; }
        }

        public float RegenerateAmountOverTime
        {
            set { this.m_RegenerateAmountOverTime = value; }
        }
        public bool IsInvincible
        {
            get { return m_IsInvincible; }
            set { this.m_IsInvincible = value; }
        }

        public bool CanPlayerRevive
        {
            get { return m_CanPlayerRevive; }
            set { m_CanPlayerRevive = value; }
        }

        public float InvincibilityTime
        {
            get { return m_InvincibilityTime; }
            set { m_InvincibilityTime = value; }
        }

        public bool IsTakingDamage
        {
            get { return m_IsTakingDamage; }
            set { m_IsTakingDamage = value; }
        }

        protected void Update()
        {

            if (m_ShowGUI)
            {
                m_sprayBloodOnScreen.enabled = true;
            }
            else
            {
                m_sprayBloodOnScreen.enabled = false;
            }

            if (m_CanRegenerateHealth && m_CurrentHealth > 0)
            {
                if (m_CurrentHealth < m_maxHealth)
                {
                    m_RemainingInitialWaitForRegeneration -=Time.deltaTime;
                    if (m_RemainingInitialWaitForRegeneration < 0)
                        RegenerateHealth();
                    //else
                    //    m_RemainingInitialWaitForRegeneration = m_RegenerateInitialWait;
                }


            }

            else if (!m_CanRegenerateHealth && m_CurrentHealth < MaxHealth && m_CurrentHealth > 0 )
            {
                if (!m_IsTakingDamage)
                {
                    m_NormalTimeElapsedForRegeneration -= Time.deltaTime;
                    if (m_NormalTimeElapsedForRegeneration < 0)
                    {

                        RegenerateHealth();
                    }
                }
                else
                {
                    m_NormalTimeElapsedForRegeneration = NormalRegenerationTime;

                }
            }
            if (m_CurrentHealth == m_maxHealth)
            {
                m_ShowGUI = false;
            }

            if (m_IsTakingDamage || m_CurrentHealth <=0)
            {
             
                if (m_CurrentHealth < 90 * m_maxHealth / 100 && m_CurrentHealth > 75 * m_maxHealth / 100)
                {
                    //   lerpColor = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), Time.deltaTime);

                    //m_CurrentTimeElapsedBetweenScreens -= Time.deltaTime;
                    //if (m_CurrentTimeElapsedBetweenScreens < 0)
                    //{
                    //    m_ShowGUI = false;

                    //}
                    //Debug.Log(m_CurrentTimeElapsedBetweenScreens);
                    if (!isFadeInStarted && (m_CurrentHealth % 10 < 1))
                    {
                        m_ShowGUI = true;
                        StartCoroutine(fadeIn(new Color(1, 1, 1, 0.3f), 10));
                        isFadeInStarted = true;
                    }
                }

                if (m_CurrentHealth <= 75 * m_maxHealth / 100 && m_CurrentHealth > 25 * m_maxHealth / 100)
                {
                    //   lerpColor = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), Time.deltaTime);

                    //m_CurrentTimeElapsedBetweenScreens -= Time.deltaTime;
                    //if (m_CurrentTimeElapsedBetweenScreens < 0)
                    //{
                    //    m_ShowGUI = false;

                    //}
                    //Debug.Log(m_CurrentTimeElapsedBetweenScreens);
                    if (!isFadeInStarted && (m_CurrentHealth % 10 < 1))
                    {
                        m_ShowGUI = true;
                        StartCoroutine(fadeIn(new Color(1, 1, 1, 0.5f), 10));
                        isFadeInStarted = true;
                    }
                }
                if (m_CurrentHealth <= 25 * m_maxHealth / 100 && m_CurrentHealth > 10 * m_maxHealth / 100)
                {
                    //   lerpColor = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), Time.deltaTime);

                    //m_CurrentTimeElapsedBetweenScreens -= Time.deltaTime;
                    //if (m_CurrentTimeElapsedBetweenScreens < 0)
                    //{
                    //    m_ShowGUI = false;

                    //}
                    //Debug.Log(m_CurrentTimeElapsedBetweenScreens);
                    if (!isFadeInStarted && (m_CurrentHealth % 10 < 1))
                    {
                        m_ShowGUI = true;
                        StartCoroutine(fadeIn(new Color(1, 1, 1, 0.75f), 10));
                        isFadeInStarted = true;
                    }
                }
                if (m_CurrentHealth <= 10 * m_maxHealth / 100)
                {
                    m_ShowGUI = true;

                    //   lerpColor = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), Time.deltaTime);     
                    LerpColor = new Color(1, 1, 1, 1);
                }
            }
         
        }

        IEnumerator fadeIn(Color color, float fadeSpeed)
        {

            while (color.a > 0)
            {
                color.a -= Time.deltaTime/10;
                LerpColor = color;
                yield return null;
            }
            if (color.a <= 0)
            {
                isFadeInStarted = false;
                yield return null;
            }
        }

        public void ActivateInvincibility()
        {
            m_CurrentInvincibiltyTime = InvincibilityTime;
            StartCoroutine(WaitForInvincibilitytime());

        }
        // for activating invinciblity after revive
        public void ActivateInvincibility(float time)
        {
            m_CurrentInvincibiltyTime = time;
            StartCoroutine(WaitForInvincibilitytime());

        }
        private bool CanTakeDamage()
        {
            
            return m_CurrentInvincibiltyTime <= 0;

        }

        public override void TakeDamage(float damageAmount, float damageMultiplier)
        {
           
            if (CanTakeDamage())
            {
                base.TakeDamage(damageAmount, damageMultiplier);            
                if (!IsAlive())
                {
                PlayDeathEffects();
                AudioManager.Instance.PlaySound(m_audioSource,
                    m_Player.DieAudioClips[
                        Random.Range(0, m_Player.DieAudioClips.Length)]);
             }
             else
                {
                    //play hit sound

                   // PlayStumbleAnimation();
                    AudioManager.Instance.PlaySound(m_audioSource,
                    m_Player.HitAudioClips[Random.Range(0, m_Player.HitAudioClips.Length)]);
                }
            UiManager.Instance.UpdateHealthBar(m_CurrentHealth,m_maxHealth);
            }

        }

        public override void TakeDamage(float damageAmount)
        {
            if (CanTakeDamage())
            {
                base.TakeDamage(damageAmount);
                if (!IsAlive())
                {
                    PlayDeathEffects();
                    AudioManager.Instance.PlaySound(m_audioSource,
                        m_Player.DieAudioClips[
                            Random.Range(0, m_Player.DieAudioClips.Length)]);
                }
                else
                {
                    //play hit sound
              //      PlayHitAnimation();
                    AudioManager.Instance.PlaySound(m_audioSource,
                        m_Player.HitAudioClips[Random.Range(0, m_Player.HitAudioClips.Length)]);
                }
                if (Util.IsTutorialComplete())
                {
                    uiManager.UpdateHealthBar(m_CurrentHealth, m_maxHealth);

                }
            }
        }


        public override void TakeDamage(float damageAmount, DeathType deathType)
        {
            TakeDamage(damageAmount);
            if (!IsAlive())
            {
                //activate death animation
                if (deathType == DeathType.DyingBackwards)
                {
                    m_PlayerAnimation.Death(DeathType.DyingBackwards);
                }
                else if (deathType==DeathType.Flyingdeath)
                {
                    m_PlayerAnimation.Death(DeathType.Flyingdeath);
               }           
            }

        }


        public void PlayStumbleAnimation()
        {
            m_PlayerAnimation.Stumble();
        }
        private void RegenerateHealth()  
        {
            if (m_CurrentHealth == 0f)
                return;
            m_CurrentHealth += (m_RegenerateAmountOverTime * Time.deltaTime);
            if (Util.IsTutorialComplete())
            {
                uiManager.UpdateHealthBar(m_CurrentHealth, m_maxHealth);

            }

        }
        private IEnumerator WaitForInvincibilitytime()
        {
            //change shader
            m_InvisibleEffect.enabled = true;
            yield return new WaitForEndOfFrame();
            if (m_InvisibleEffect.GetCurrentEffectType() != EffectType.Invisible)
            {
                m_InvisibleEffect.StartEffect();

            }
            var waitsSeconds = new WaitForSeconds(m_CurrentInvincibiltyTime);
            yield return waitsSeconds;
            m_CurrentInvincibiltyTime = float.MinValue;
            m_InvisibleEffect.RemoveEffect();
            m_PlayerControl.IsResumeFromDeath = false;
        }

        public void CancelCouroutine()
        {
            StopCoroutine("WaitForInvincibilitytime");
        }

        private void OnDisable()
        {
            m_RemainingInitialWaitForRegeneration = m_RegenerateInitialWait;
        }

        public void ResetInvincibleTime()
        {
            m_CurrentInvincibiltyTime = m_InvincibilityTime;
        }

        public void ResetRegenerationInitialWaitTime()
        {
            m_RemainingInitialWaitForRegeneration = m_RegenerateInitialWait;
        }
        public float GetMaxHealth()
        {
            return m_maxHealth;
        }

    }
}

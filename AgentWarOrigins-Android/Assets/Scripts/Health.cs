using UnityEngine;
using UnityEngine.UI;
namespace EndlessRunner
{
    public class Health : MonoBehaviour
    {
        [SerializeField] [Tooltip("Maximum amount of health")] protected float m_maxHealth;
        [SerializeField] [Tooltip("Destroy On Die")] protected bool m_DestroyOnDie;

        [SerializeField] [Tooltip("Show Particles Effects")] protected ParticleSystem m_DieParticles;
        protected float m_CurrentHealth;
        protected float m_DamageMultiplier;
        private float m_DefaultLevelHealth;
        private Animation m_Animation;
        private Slider m_HealthSlider;
        protected virtual void Awake()
        {
            m_CurrentHealth = m_maxHealth;
            m_DefaultLevelHealth = m_maxHealth;
            m_DamageMultiplier = 1f;
            m_Animation = GetComponent<Animation>();
            m_HealthSlider = GetComponentInChildren<Slider>();

            if (m_HealthSlider)
            {
                m_HealthSlider.maxValue = MaxHealth;
                m_HealthSlider.value = MaxHealth;

            }
        }

        protected void OnEnable()
        {
            m_CurrentHealth = m_maxHealth;
            if (m_HealthSlider)
            {
                m_HealthSlider.maxValue = MaxHealth;
                m_HealthSlider.value = MaxHealth;

            }
        }

        public float SetDamageMultiplier
        {
            set { m_DamageMultiplier = value; }
        }

        public float MaxHealth
        {
            get { return m_maxHealth; }
            set { m_maxHealth = value; }
        }

        public float DefaultLevelHealth
        {
            get { return m_DefaultLevelHealth; }
            set { m_DefaultLevelHealth = value; }
        }

        protected void SetHealthAmount(float healthAmount)
        {
            m_CurrentHealth = Mathf.Min(m_maxHealth, healthAmount);
        }


        public virtual void TakeDamage(float damageAmount, float damageMultiplier)
        {
            if (m_CurrentHealth > 0f)
            {
                var currentHealth = Mathf.Max(m_CurrentHealth - damageAmount * damageMultiplier, 0);
                SetHealthAmount(currentHealth);
            }
         
            if (m_HealthSlider)
            {
                m_HealthSlider.value = m_CurrentHealth;
            }
            //play sound

        }

        public virtual void TakeDamage(float damageAmount)
        {
            TakeDamage(damageAmount,m_DamageMultiplier);
      

        }
        public virtual void TakeDamage(float damageAmount, DeathType deathType)
        {
            TakeDamage(damageAmount);
            //if (!IsAlive())
            //{
            //    PlayDeathEffects();
            //}
            //else
            //{
            //    PlayHitAnimation();
            //}
        }
        public void TakeDamage(float damageAmount, float damageMultiplier, GameObject gameObject)
        {
            //   Debug.Log(gameObject.name + "Health is decreasing by" + damageAmount * damageMultiplier );
            TakeDamage(damageAmount, damageMultiplier);
        }

        public bool IsAlive()
        {
            return m_CurrentHealth > 0;
        }

        public void InstantDeath()
        {
            TakeDamage(m_CurrentHealth, m_DamageMultiplier);
        }


        public virtual void PlayDeathEffects()
        {
            //play any death effects
            if (m_DieParticles)
            {
                
                m_DieParticles.transform.position = transform.position;
                m_DieParticles.Play();

            }
            if (m_DestroyOnDie)
            {
                Destroy(gameObject);
            }
            else
            {
                DeathType deathType = (DeathType) Random.Range(0, 6);
                if (m_Animation.GetCurrentAnimationStateHashInLayer(1).IsName("Crouch"))
                {
                    deathType = DeathType.CrouchDeath;
                    m_Animation.SetLayerWeight(1,0);
                  //  m_Animation.Uncrouch();
                }
           
                //activate death animation
                switch (deathType)
                {
                    case DeathType.DyingBackwards:
                        m_Animation.Death(DeathType.DyingBackwards);
                        break;
                    case DeathType.Flyingdeath:
                        m_Animation.Death(DeathType.Flyingdeath);
                        break;
                    case DeathType.DeathBackwards:
                        m_Animation.Death(DeathType.DeathBackwards);
                        break;
                    case DeathType.DeathFromFrontHeadshot:
                        m_Animation.Death(DeathType.DeathFromFrontHeadshot);
                        break;
                    case DeathType.StandingReactDeathBackward:
                        m_Animation.Death(DeathType.StandingReactDeathBackward);
                        break;
                    case DeathType.ZombieDeath:
                        m_Animation.Death(DeathType.ZombieDeath);
                        break;
                    case DeathType.CrouchDeath:
                        m_Animation.Death(DeathType.CrouchDeath);
                        break;
                }
            }
     
        }

        public void ResetHealth()
        {
            SetHealthAmount(m_maxHealth);
        }

        public void ResetHealth(float health)
        {
            SetHealthAmount(health);
        }
        public float GetCurrentHealth()
        {
            return m_CurrentHealth;
        }
    }
}


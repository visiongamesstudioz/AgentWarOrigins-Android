using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class DroneHealth : Health
{

    private AudioSource m_AudioSource;
    private Rigidbody m_Rigidbody;
    private bool m_IsDestroyed;
    private AirDroneController m_airDroneController;
    private DestroyByTime m_DestroyByTime;
    public bool IsDestroyed
    {
        get
        {
            return m_IsDestroyed;
        }

        set
        {
            m_IsDestroyed = value;
        }
    }

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
        m_airDroneController = GetComponent<AirDroneController>();
        m_DestroyByTime = GetComponent<DestroyByTime>();
    }

    public override void TakeDamage(float damageAmount)
    {
       
        base.TakeDamage(damageAmount);

        if (!IsAlive())
        {

            if (m_Rigidbody)
            {
                m_Rigidbody.useGravity = true;

            }
            m_IsDestroyed = true;
            PlayDeathEffects();

        }

    }


    public override void PlayDeathEffects()
    {
        //play any death effects
        if (m_DieParticles)
        {
            m_DieParticles.Play(true);

        }
        if (m_airDroneController.DestroyAudioClip)
        {
            m_AudioSource.loop = false;
            AudioManager.Instance.PlaySound(m_AudioSource, m_airDroneController.DestroyAudioClip);
        }
        if (m_DestroyOnDie)
        {
            Destroy(gameObject);
        }
        else
        {
            //
            m_DestroyByTime.enabled = true;

        }
    }

    private void OnDisable()
    {
        if (m_DestroyByTime)
        {
            m_DestroyByTime.enabled = false;
        }
        
        m_Rigidbody.useGravity = false;
        m_IsDestroyed = false;
    }


}

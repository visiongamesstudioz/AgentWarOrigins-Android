using System.Collections;
using System.Collections.Generic;
using EndlessRunner;
using UnityEngine;

public class AirDroneController : PathFollower
{
    public Transform StartPointTransform;
    public LayerMask SpherecastMask;
    public AirDroneExplosion[] AirDroneExplosions;
    [SerializeField]
    private PathCreator[] PathCreators;
    public int NoOfSecondsToMoveTONextSet;
    public AudioClip MoveAudioClip;
    public AudioClip DestroyAudioClip;
    private AudioSource m_AudioSource;
    
    private int currentPathFollowerId = 0;
    private float elapsedTime;
    public Animator m_Animator;
    private DroneHealth m_DroneHealth;
    private GameObject m_Player;
    private bool isVisibleToCamera;
    void Start()
    {
        m_AudioSource=GetComponent<AudioSource>();
        m_DroneHealth = GetComponent<DroneHealth>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
        elapsedTime = NoOfSecondsToMoveTONextSet;
        m_AudioSource.loop = true;

        AudioManager.Instance.PlaySound(m_AudioSource);
    }

    private void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.white;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position,300);
#endif
    }


    public override void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 300,SpherecastMask);
        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    isVisibleToCamera = true;
                }
            }
        }
        if (isVisibleToCamera)
        {
            if (PathCreators.Length > 0 && m_DroneHealth)
            {

                if (currentPathFollowerId > PathCreators.Length - 1 || m_DroneHealth.IsDestroyed)
                {
                    return;
                }
                if (!IsLastWayPointReached(PathCreators[currentPathFollowerId]))
                {
                    m_Animator.enabled = true;
                    HandleWaypointMovement(PathCreators[currentPathFollowerId]);
                }
                else
                {
                    //start 
                    m_Animator.enabled = false;
                    FacePlayer(false);
                    foreach (var airdroneexplosion in AirDroneExplosions)
                    {
                        airdroneexplosion.enabled = true;
                    }
                    elapsedTime -= Time.deltaTime;
                    if (elapsedTime < 0)
                    {
                        currentPathFollowerId++;
                        ResetCurrentWayPoint();
                        elapsedTime = NoOfSecondsToMoveTONextSet;
                    }
                }
            }
            else
            {
                FacePlayer(false);
                foreach (var airdroneexplosion in AirDroneExplosions)
                {
                    airdroneexplosion.enabled = true;
                }
            }
        }
          
        }


    public void ResetCurrentPathFollowerId()
    {
        currentPathFollowerId = 0;
    }
}


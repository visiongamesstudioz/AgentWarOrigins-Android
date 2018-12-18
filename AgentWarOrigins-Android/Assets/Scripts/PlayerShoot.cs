using System.Collections;
using EndlessRunner;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static PlayerShoot Instance;
    public GameObject WeaponClip;
    public Transform PlayerSpineTransform;
    public Transform PlayerLeftHandTransform;
    public Transform PlayerRightHandTransform;
    private PlayerAnimation m_PlayerAnimation;
    private PlayerControl m_PlayerControl;
    private PlayerHealth m_PlayerHealth;
    private bool isWaitingStarted;
    private Animator m_Animator;
    [SerializeField]
    private WeaponManagement currentWeapon;
    private Camera mainCamera;
    public WeaponManagement CurrentWeapon
    {
        get { return currentWeapon; }
        set { currentWeapon = value; }
    }

    private void Awake()
    {
        Instance = this;
        m_PlayerAnimation = GetComponent<PlayerAnimation>();
        m_PlayerControl = GetComponent<PlayerControl>();
        m_PlayerHealth = GetComponent<PlayerHealth>();
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        m_Animator.SetLayerWeight(1,1);
        if (currentWeapon.CurrentWeaponType == WeaponType.Sr45)
        {
            m_PlayerAnimation.SetShootAnimationType(WeaponType.Sr45);
        }
        else if(currentWeapon.CurrentWeaponType==WeaponType.Mp5)
        {
            m_PlayerAnimation.SetShootAnimationType(WeaponType.Mp5);
        }
        else if (currentWeapon.CurrentWeaponType == WeaponType.SCu)
        {
            m_PlayerAnimation.SetShootAnimationType(WeaponType.SCu);
        }
        else if (currentWeapon.CurrentWeaponType == WeaponType.Smg)
        {
            m_PlayerAnimation.SetShootAnimationType(WeaponType.Smg);
        }
        else if (currentWeapon.CurrentWeaponType == WeaponType.Fw97)
        {
            m_PlayerAnimation.SetShootAnimationType(WeaponType.Fw97);
        }
        else if (currentWeapon.CurrentWeaponType == WeaponType.M32)
        {
            m_PlayerAnimation.SetShootAnimationType(WeaponType.M32);
        }
     //   currentWeapon.UpdateWeaponAmmoDisplay();

    }
    private void Update()
    {

        if (currentWeapon.IsReloading())
        {
            currentWeapon.DisableMuzzleFlash2();
            m_PlayerAnimation.Reload(true,currentWeapon);
            //detach clip and attach to left hand
            //if (PlayerLeftHandTransform)
            //{
            //    currentWeapon.ChangeWeaponClipParent(WeaponClip, PlayerLeftHandTransform);
            //}
            //if (!isWaitingStarted)
            //{ 
            //   // Debug.Log("waiting for reload to complete");
            //     
            //    StartCoroutine(WaitForSomeTime(m_PlayerAnimation.GetCurrentAnimationTime("Reload"), currentWeapon.gameObject, "StopReloading"));  
            //}
        }
        else
        {
            //if (PlayerRightHandTransform)
            //{
            //    currentWeapon.ChangeWeaponClipParent(WeaponClip, PlayerRightHandTransform);
            //}
        }

    }


    public void Shoot(Vector3 position)
    {
        if (m_PlayerHealth.IsDead)
        {
            return;
        }
        if (currentWeapon.CanFire())
        {
            Ray ray;
            if (Util.IsTutorialComplete())
            {
                ray = mainCamera.ScreenPointToRay(position);

            }
            else
            {
                ray = GetComponentInChildren<Camera>().ScreenPointToRay(position);
            }
#if UNITY_EDITOR

            //    Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 10f);
#endif
            //   Debug.Log("can fire");

            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, currentWeapon.Weapon.WeaponRange))
            {
                if (hit.collider)
                {
                    //we found an collider
                    //    Debug.Log(hit.collider.name);
                    var targetDirection = hit.point - currentWeapon.FirePoint.position;

                    currentWeapon.Shoot(targetDirection);
                    m_PlayerAnimation.Shoot();
                }
                else
                {
                    currentWeapon.Shoot(ray.direction);
                    m_PlayerAnimation.Shoot();

                }
            }
          
            currentWeapon.UpdateWeaponAmmoDisplay();   
        }
        else
        {
          //  Debug.Log("cannot fire");
        }
    }
    public void PlayWeaponReloadAnimation()
    {
        if (m_Animator)
            m_Animator.SetTrigger("Reload");
    }
    public void OnReloadComplete()
    {
        m_PlayerAnimation.Reload(false,currentWeapon);
        currentWeapon.StopReloading();
        currentWeapon.UpdateWeaponAmmoDisplay();
    }
    //void LateUpdate()
    //{
    //        OrientPlayerToShootPosition();
    //}

    void OrientPlayerToShootPosition()
    {
#if UNITY_EDITOR
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Plane playerPlane = new Plane(Vector3.up, transform.position);

        //float hitdist;

        //if (playerPlane.Raycast(ray, out hitdist))
        //{
        //    Vector3 targetPoint = ray.GetPoint(hitdist);
        //    Vector3 targetDirection = targetPoint - PlayerSpineTransform.position;
        //    //   PlayerSpineTransform.LookAt(targetDirection);
        //    Quaternion targetRotation = Quaternion.LookRotation(targetPoint - PlayerSpineTransform.position);

        //    PlayerSpineTransform.rotation = Quaternion.Slerp(PlayerSpineTransform.rotation, targetRotation, Time.deltaTime * 10f);
        //}
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
        foreach (var hit in hits)
        {
     
            //we found an enemy
            var targetDirection = hit.point - PlayerSpineTransform.position;

            //Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            //targetRotation.x = Mathf.Clamp(targetRotation.x, -60, 60);
            //targetRotation.y = Mathf.Clamp(targetRotation.y, -60, 60);
            //targetRotation.z = Mathf.Clamp(targetRotation.z, -90, 90);
            //Vector3 targetRot= new Vector3(targetRotation.x,targetRotation.y,targetRotation.z);
            //PlayerSpineTransform.eulerAngles = targetRot;
           

        }
#endif
    }


    IEnumerator WaitForSomeTime(float time, GameObject Object, string message)
    {
        isWaitingStarted = true;
        yield return new WaitForSeconds(time);
        if (Object)
        {
            Object.SendMessage(message);
            isWaitingStarted = false;
        }
      
    }

    public void Disable()
    {
        enabled = false;
    }

    private void OnDisable()
    {
        //hide weapon
        currentWeapon.gameObject.SetActive(false);
        //disable muzzle flash
        GameObject muzzleFlash= currentWeapon.GetComponentInChildren<MuzzleFlash>(true).gameObject;
        muzzleFlash.SetActive(false);
            
        m_PlayerAnimation.ExitShootAnimation();            
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.IsTakingDamage = false;
        //if (playerHealth.IsInvincible)
        //{
        //    playerHealth.ResetInvincibleTime();
        //}

        //disable health bar
        UiManager.Instance.HideHealthBar();
        //hide weapon ammo display icon and ammo available
        UiManager.Instance.HideWeaponAmmoDisplay();
        //hide refill ammo
        UiManager.Instance.HideRefillAmmoMenu();
    }

    private void OnEnable()
    {

        //show  weapon
        currentWeapon.gameObject.SetActive(true);
        m_PlayerAnimation.Aim();
        //activate invincibility if that outfit is worn
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        InvisibleEffect invisibleEffect = GetComponent<InvisibleEffect>();
        playerHealth.CancelCouroutine();
        if (invisibleEffect.GetCurrentEffectType() == EffectType.Invisible)
        {
            invisibleEffect.RemoveEffect();
        }
        if (!playerHealth.IsInvincible)
        {
            playerHealth.ResetInvincibleTime();
            m_PlayerControl.IsResumeFromDeath = false;
        }
        else
        {
            playerHealth.ActivateInvincibility();
        }

        if (playerHealth.CanRegenerateHealth)
        {
            playerHealth.ResetRegenerationInitialWaitTime();
        }
        //reload weapon on enable
        currentWeapon.ReloadExternally();
        //enable health bar
        UiManager.Instance.ShowHealthBar();
        //show weapon ammo display icon and ammo available
        UiManager.Instance.ShowWeaponAmmoDisplay();

        UiManager.Instance.UpdateHealthBar(playerHealth.GetCurrentHealth(),playerHealth.GetMaxHealth());


    }
}

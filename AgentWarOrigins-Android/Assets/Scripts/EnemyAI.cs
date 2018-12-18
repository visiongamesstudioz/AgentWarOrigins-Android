using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EndlessRunner;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float ScanDistance;
    public float AttackDistance;
    public float m_FireAmountForNextState;
    public GameObject weapon;
    public GameObject m_EnemySpine;
    public LayerMask LayerMask;
    public float DefaultColliderHeight;
    public float m_GroundCheckDistance = 0.3f;
    public List<WayPoint> m_WayPointsList;

    private GameObject m_Player;
    private CapsuleCollider m_PlayerCapsuleCollider;
    private PlayerHealth m_PlayerHealth;
    private Rigidbody m_Rigidbody;
    private Animation _mAnimation;
    private CapsuleCollider m_CapsuleCollider;
    private Vector3 offset;
    private int m_LayerMask;
    private WeaponManagement _currentWeapon;
    private Transform firePoint;
    private float m_currentAmountForNextState;
    private int currentWayPointIndex = -1;
    private EnemyState m_CurrentEnemyState;
    private bool m_InitMoveToNextWayPoint;
    private WeaponType m_WeaponType;
    private bool isWaitingStarted;
    private Health m_Health;
    private bool m_PlayOtherAnimation;


    private AnimatorStateInfo currentBaseState;
    private bool m_IsGrounded;
    private GameObject m_DefaultEnemyParent;
    //getters and setters
    public bool isDieSet { get; set; }
    private bool isCrouching;
    private bool isMoving;
    public EnemyState CurrentEnemyState
    {
        get { return m_CurrentEnemyState; }
    }

    private Renderer m_Renderer;
    private bool isEnemyVisibleSet;
    private bool onBecameInvisibleAlternative;
    //can add waypoints later
    // Use this for initialization
    private void Awake()
    {
        m_WayPointsList=new List<WayPoint>();
    }

    private void OnEnable()
    {
        //   transform.eulerAngles = new Vector3(0, -90, 0);
        if (_mAnimation)
        {
            AssignSpawnPointAnimation((WaypointTargetAnimationName) Random.Range(0,2));
        }
    }

    private void Start()
    {
        m_Renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        m_Player = GameObject.FindGameObjectWithTag("Player");
        if (m_Player)
        {
            m_PlayerCapsuleCollider = m_Player.GetComponent<CapsuleCollider>();
            m_PlayerHealth = m_Player.GetComponent<PlayerHealth>();
        }
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();

        _mAnimation = GetComponent<Animation>();
        _currentWeapon = weapon.GetComponent<EnemyWeaponManagement>();
        m_Health = GetComponent<Health>();
        m_DefaultEnemyParent = Util.FindParentWithTag(this.gameObject, "SceneParent");

        m_LayerMask = 1 << 13;
        firePoint = _currentWeapon.FirePoint;
        m_WeaponType = _currentWeapon.CurrentWeaponType;
        m_currentAmountForNextState = m_FireAmountForNextState;
        //set shooting type for the enemy
        if (m_WeaponType == WeaponType.Sr45)
            _mAnimation.SetShootAnimationType(WeaponType.Sr45); //change later
        else if (m_WeaponType == WeaponType.Mp5)
            _mAnimation.SetShootAnimationType(WeaponType.Mp5); //change later
        else if (m_WeaponType == WeaponType.SCu)
            _mAnimation.SetShootAnimationType(WeaponType.SCu);
        else if (m_WeaponType == WeaponType.Smg)
        {
            _mAnimation.SetShootAnimationType(WeaponType.Smg);
        }
        else if (m_WeaponType == WeaponType.Fw97)
            _mAnimation.SetShootAnimationType(WeaponType.Fw97);
        else if (m_WeaponType == WeaponType.M32)
            _mAnimation.SetShootAnimationType(WeaponType.M32);

        //assign spawnpoint animation
        if (_mAnimation)
        {
            AssignSpawnPointAnimation((WaypointTargetAnimationName)Random.Range(0, 2));
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //update collider center based on animation curves
        if (_mAnimation.GetCurrentAnimationStateHashInLayer(1).fullPathHash == _mAnimation.JumpDownState)
        {
            m_CapsuleCollider.center = new Vector3(0,_mAnimation.GetColliderCenter(), 0);

        }
        else
        {

            m_CapsuleCollider.center = new Vector3(0, DefaultColliderHeight, 0);

        }
        //RaycastHit hitInfo;

        //// 0.1f is a small offset to start the ray from inside the character
        //// it is also good to note that the transform position in the sample assets is at the base of the character
        //if (Physics.Raycast(transform.position, Vector3.down, out hitInfo))
        //{
        //    if (hitInfo.collider.tag == "Ground")
        //    {
        //        Debug.Log("ground hit");
        //        if (Vector3.Distance(transform.position, hitInfo.point) < m_GroundCheckDistance)
        //        {
        //            Debug.Log("grounded");
        //            m_IsGrounded = true;
        //        }
       

        //    }
        //}
        //else
        //{
        //    m_IsGrounded = false;

        //}
        if (m_Player)
        {            
            var distanceToPlayer = Vector3.Distance(transform.position, m_Player.transform.position);
            if (distanceToPlayer > 300) //camera far clip plane
            {
                return;
            }
            //if (m_Renderer.isVisible && Util.IsObjectInFront(gameObject, m_Player) && (transform.position.x - m_Player.transform.position.x < Camera.main.farClipPlane))
            //{
            //    onBecameInvisibleAlternative = false;
            //    if (!isEnemyVisibleSet)
            //    {
            //        GameManager.noOfEnemiesOnScreen++;
            //        Debug.Log("no of enemies on screenafter increasing" + GameManager.noOfEnemiesOnScreen);
            //        isEnemyVisibleSet = true;
            //    }
            //}

            //bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            //Debug.Log("on scrren "+onScreen);
            //Debug.Log("is enemy visible set"+isEnemyVisibleSet);
            //if (onScreen && !isEnemyVisibleSet && distanceToPlayer < (Camera.main.farClipPlane-150))
            //{
            //    Debug.Log("ading enemy to screen");
            //    GameManager.noOfEnemiesOnScreen++;
            //    isEnemyVisibleSet = true;
            //}
            //if (!m_Renderer.isVisible && distanceToPlayer > 5)
            //{

            //    if (!onBecameInvisibleAlternative)
            //    {
            //        if (m_Health)
            //            m_Health.ResetHealth();
            //        if (isEnemyVisibleSet)
            //        {
            //            if (GameManager.noOfEnemiesOnScreen > 0)
            //            {
            //                GameManager.noOfEnemiesOnScreen--;
            //                Debug.Log("enemies on screen after decreaing" + GameManager.noOfEnemiesOnScreen);

            //            }

            //        }
            //        isEnemyVisibleSet = false;
            //        _mAnimation.Revive();
            //        onBecameInvisibleAlternative = true;
            //    }

            //}

            if (IsDead())
            {
                return;
            }
            if (m_Renderer.isVisible && IsInFrontofPlayer())
            {
                if (!isMoving)
                {
                    if (distanceToPlayer <= ScanDistance && distanceToPlayer > AttackDistance && !isCrouching)
                    {
                        if (IsLookingAtPlayer())
                            m_CurrentEnemyState = EnemyState.Aim;
                        else if (!IsLookingAtPlayer())
                            m_CurrentEnemyState = EnemyState.LookAtTarget;
                    }
                    if (m_currentAmountForNextState > 0)
                    {
                        if (!IsLookingAtPlayer())
                            m_CurrentEnemyState = EnemyState.LookAtTarget;

                        else if (distanceToPlayer <= AttackDistance && !_currentWeapon.IsReloading())
                            m_CurrentEnemyState = EnemyState.Attack;
                    }
                }
            
                if (m_currentAmountForNextState <= 0)
                {
                    //move to next waypoint
                    m_CurrentEnemyState = EnemyState.MoveToNextWayPoint;
                }
            }
            else
            {
                m_CurrentEnemyState = EnemyState.DoNothing;
            }
            if (_currentWeapon.IsReloading())
                m_CurrentEnemyState = EnemyState.Reload;
            //Debug.Log("distance to player" + distanceToPlayer);
            //if (!IsInFrontofPlayer())
            //    enemyState = EnemyState.DoNothing;
            //if (distanceToPlayer <= ScanDistance && IsInFrontofPlayer())
            //    enemyState = EnemyState.Aim;
            //if (distanceToPlayer <= ScanDistance && !IsLookingAtPlayer() && IsInFrontofPlayer())
            //    enemyState = EnemyState.LookAtTarget;
            //if (distanceToPlayer < AttackDistance && m_currentAmountForNextState >= 0 && IsLookingAtPlayer() &&
            //    !_currentWeapon.IsReloading() && IsInFrontofPlayer())
            //    enemyState = EnemyState.Attack;
            //else if (m_currentAmountForNextState < 0)
            //    enemyState = EnemyState.MoveToNextWayPoint;
            //if (_currentWeapon.IsReloading())
            //    enemyState = EnemyState.Reload;
            switch (m_CurrentEnemyState)
            {
                case EnemyState.DoNothing:
                    _mAnimation.Revive();
                    break;
                case EnemyState.Aim:
                    _mAnimation.Run(false);
                    break;
                case EnemyState.LookAtTarget:
                //    m_Rigidbody.freezeRotation = true;
                    _mAnimation.Run(false);
                    var targetPos = m_Player.transform.position - transform.position;
                    targetPos.y = 0;
                    LookAtTarget(targetPos, transform);
                    _mAnimation.Aim();
                    break;
                case EnemyState.Attack:
                    AttackPlayer();
                    break;
                case EnemyState.MoveToNextWayPoint:
                    if (!m_InitMoveToNextWayPoint)
                    {
                        currentWayPointIndex++;
                        m_InitMoveToNextWayPoint = true;
                    }
                    _mAnimation.SetLayerWeight(1,0);
                    MoveToNextWayPoint(currentWayPointIndex);
                    break;
                case EnemyState.Reload:
                    _mAnimation.Reload(true,_currentWeapon);
                    //if (!isWaitingStarted)
                    //    StartCoroutine(WaitForSomeTime(_mAnimation.GetCurrentAnimationTime(_currentWeapon.Weapon.WeaponName + "@" + "Reload"),
                    //        _currentWeapon.gameObject, "StopReloading"));
                    break;
                case EnemyState.Dead:

                    break;
            }
            //check if enenmy state is attack and assign is taking damage to true

            if (m_PlayerHealth)
            {
                if (m_CurrentEnemyState == EnemyState.Attack)
                {
                    m_PlayerHealth.IsTakingDamage = true;
                }
                else
                {
                    m_PlayerHealth.IsTakingDamage = false;
                }
            }
            

        }
    }

    public void OnReloadComplete()
    {
        _mAnimation.Reload(false,_currentWeapon);
        _currentWeapon.StopReloading();
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.CompareTag("Ground"))
        {
            m_IsGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            
            m_IsGrounded = false;
        }
    }


    //private bool IsObjectVisible()
    //{
    //    Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
    //    if (GeometryUtility.TestPlanesAABB(planes, m_CapsuleCollider.bounds))
    //        return true;
    //    return false;
    //}
    private void OnBecameVisible()
    {
        enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer== LayerMask.NameToLayer("JumpTrigger"))
        {
            m_PlayOtherAnimation = true;
        }
     
    }

    //private void OnBecameInvisible()
    //{
    //    Debug.Log("on became invisible called");

    //    if (m_Health)
    //        m_Health.ResetHealth();
    //    if (isEnemyVisibleSet && !isDieSet)
    //    {
    //        Debug.Log("decreasing enemy count");
    //        if (GameManager.noOfEnemiesOnScreen > 0)
    //        {
    //            GameManager.noOfEnemiesOnScreen--;
    //        }
    //        Debug.Log("no of enemies on screen after decresing" + GameManager.noOfEnemiesOnScreen);

    //    }
    //    isDieSet = false;
    //    isEnemyVisibleSet = false;
    //    _mAnimation.Revive();
    //}

    private void LateUpdate()
    {
        if (m_CurrentEnemyState == EnemyState.DoNothing || m_CurrentEnemyState==EnemyState.Dead)
            return;
        OrientWeapon();
    }

    private void OnDrawGizmos()
    {
        //    Debug.DrawRay(firePoint.position, firePoint.TransformVector(Vector3.forward) * 1, Color.white);
    }

    private void LookAtTarget(Vector3 position, Transform thisTransform)
    {
        if (position == Vector3.zero)
        {
            return;
        }
        var lookRotation = Quaternion.LookRotation(position);
        thisTransform.rotation = lookRotation;
          thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, lookRotation, 2.0f * Time.deltaTime);
    }

    private void MoveToNextWayPoint(int index)
    {
        isMoving = true;
        if (IsDead())
        {
            m_CurrentEnemyState= EnemyState.Dead;
        }
        //  Debug.Log("currentWayPointIndex" + index);
        //all waypoints completed
        if (index >= m_WayPointsList.Count)
        {
            //set layerweight
            isMoving = false;
            _mAnimation.SetLayerWeight(1,1);
            m_currentAmountForNextState = m_FireAmountForNextState;
            return;
        }

        var wayPoint = m_WayPointsList.ElementAt(index);

        if (wayPoint == null)
        {
            return;
        }
        //change parent according to waypoint position
      
        //set animation stop crouching and move towards waypoint
        _mAnimation.Uncrouch();
        _mAnimation.JumpDown(false);

        if (m_IsGrounded)
        {
          //  Debug.Log("enemy is on ground" + transform.localPosition);
            transform.parent = m_DefaultEnemyParent.transform;
            m_PlayOtherAnimation = false;
            _mAnimation.JumpDown(false);

            //enemy is on ground and no jump animation
         //   _mAnimation.SetLayerWeight(1,0);
            _mAnimation.Run(true);
            LookAtTarget(wayPoint.TargetDestination.position - transform.position, transform);
            transform.position = Vector3.MoveTowards(transform.position, wayPoint.TargetDestination.position,
                8 * Time.deltaTime);

        }

        if (m_PlayOtherAnimation)
        {
            //play jump animation overriding movemnt
        //    Debug.Log("playing jump down animation");
            _mAnimation.SetLayerWeight(1,1);
            _mAnimation.JumpDown(true);
        }
        else
        {
            m_PlayOtherAnimation = false;
            _mAnimation.JumpDown(false);
            _mAnimation.Run(true);
            LookAtTarget(wayPoint.TargetDestination.position - transform.position, transform);
            transform.position = Vector3.MoveTowards(transform.position, wayPoint.TargetDestination.position,
                8 * Time.deltaTime);
        }

            
        //waypoint reached
        if (Vector3.Distance(transform.position, wayPoint.TargetDestination.position) < 0.01f)
        {
            m_currentAmountForNextState = m_FireAmountForNextState;
            m_InitMoveToNextWayPoint = false;
            isMoving = false;

            //set lower layer weight
            //change layer weight
            _mAnimation.SetLayerWeight(1, 1);
            //change animation if mentioned
            ChangeToAnimationAtWaypoint(wayPoint);
        }
       
    }


    public void OnCrouchCompleted()
    {
        isCrouching = false;
    }

    public void AssignSpawnPointAnimation(WaypointTargetAnimationName animationName)
    {
        //stop running animation on reaching waypoint
        _mAnimation.Run(false);

        switch (animationName)
        {
            case WaypointTargetAnimationName.Idle:
                break;
            case WaypointTargetAnimationName.Crouch:
                _mAnimation.Crouch();
                isCrouching = true;
                break;
        }
    }
    private void ChangeToAnimationAtWaypoint(WayPoint wayPoint)
    {
        //stop running animation on reaching waypoint
        _mAnimation.Run(false);
       

        switch (wayPoint.TargetAnimationName)
        {
            case WaypointTargetAnimationName.Idle:
                break;
            case WaypointTargetAnimationName.Crouch:
                _mAnimation.Crouch();
                isCrouching = true;
                break;              
        }
    }

    private void AttackPlayer()
    {
        if (m_PlayerHealth)
        {
            if (_currentWeapon.CanFire() && !m_PlayerHealth.IsDead && m_CurrentEnemyState != EnemyState.MoveToNextWayPoint)
            {
                var targetVector3 = m_Player.transform.position +
                                    new Vector3(0, m_PlayerCapsuleCollider.height / 2, 0) -
                                    firePoint.transform.position;
                if (_currentWeapon.Shoot(targetVector3))
                    m_currentAmountForNextState--;
                _mAnimation.Shoot();
            }
            else
            {
                _mAnimation.Aim();
            }
        }
   

        //        RaycastHit hit;
        //#if  UNITY_EDITOR
        //        Debug.DrawRay(firePoint.position, transform.forward * 100, Color.white);
        //#endif
        //        if (Physics.Raycast(firePoint.position, transform.forward, out hit, Mathf.Infinity,m_LayerMask))
        //        {
        //            Debug.Log(hit.collider.name);
        //            if (hit.collider)
        //            {
        //                var currentPlayerHealth= 
        //                hit.collider.GetComponent<PlayerHealth>().GetCurrentHealth();
        //                Debug.Log(currentPlayerHealth);

        //            }
        //        }
    }

    private void OrientWeapon()
    {
        if (m_CurrentEnemyState == EnemyState.DoNothing || m_CurrentEnemyState == EnemyState.MoveToNextWayPoint)
            return;
        var lookRotation =
            Quaternion.LookRotation(m_Player.transform.position +
                                    new Vector3(0, m_PlayerCapsuleCollider.height / 2, 0) -
                                    m_EnemySpine.transform.position);
        var targetPos = m_Player.transform.position +
                        new Vector3(0, m_PlayerCapsuleCollider.height / 2, 0) -
                        m_EnemySpine.transform.position;
        LookAtTarget(targetPos, m_EnemySpine.transform);
        //m_EnemySpine.transform.LookAt( m_Player.transform.position + new Vector3(0, m_Player.GetComponent<CapsuleCollider>().height / 2, 0) - m_EnemySpine.transform.position);
        //   Vector3 targetVector3 = ( m_Player.transform.position + new Vector3(0,m_Player.GetComponent<CapsuleCollider>().height /2,0)-firePoint.transform.position);
        //  Debug.DrawRay(firePoint.transform.position,targetVector3 * 100,Color.green);
    }

    private bool IsLookingAtPlayer()
    {
        //angle = Vector3.Angle(
        //    (m_Player.transform.position + new Vector3(0f, m_Player.GetComponent<CapsuleCollider>().height / 2, 0f)),
        //    firePoint.position);
        var dot = Vector3.Dot(transform.forward, (m_Player.transform.position - transform.position).normalized);
        //Debug.Log( "dot product"+ dot);
        if (dot > 0.9)
            return true;
        return false;
    }

    private bool IsInFrontofPlayer()
    {
        return transform.position.x - m_Player.transform.position.x > 5f;
    }


    private IEnumerator WaitForSomeTime(float time, GameObject Object, string message)
    {
        Debug.Log("reload time" + time);
        isWaitingStarted = true;
        yield return new WaitForSeconds(time);
        Object.SendMessage(message);
        isWaitingStarted = false;
    }
    public void OnAnimatorMove()
    {
        // we implement this function to override the default root motion.
        // this allows us to modify the positional speed before it's applied.
        if (m_IsGrounded)
        {
          //  Debug.Log("grounded");

        }
        if (_mAnimation)
        {
            if (_mAnimation.GetCurrentAnimationStateHashInLayer(1).fullPathHash == _mAnimation.JumpDownState)
            {
                //  Debug.Log("not grounded");
                transform.position = _mAnimation.GetRootPosition();
            }
        }
  

    }

    public bool IsDead()
    {
        if (m_Health.GetCurrentHealth() > 0)
            return false;
        m_CurrentEnemyState = EnemyState.Dead;

        return true;
    }

    public void AddWayPoint(WayPoint wayPoint)
    {
        m_WayPointsList.Add(wayPoint);
    }

    public void RemoveWayPoints()
    {
        m_WayPointsList.Clear();
    }
    public enum EnemyState
    {
        DoNothing,
        Aim,
        LookAtTarget,
        MoveToNextWayPoint,
        Crouch,
        Attack,
        Reload,
        Dead
    }

    private void OnDisable()
    {

     //   onBecameInvisibleAlternative = false;

        //reset rotation
     //   transform.eulerAngles = new Vector3(0, -90, 0);
        RemoveWayPoints();
        if (m_Health)
            m_Health.ResetHealth();
        if (_mAnimation)
        {
            _mAnimation.Revive();
        }
        isDieSet = false;
        isEnemyVisibleSet = false;
    }

}


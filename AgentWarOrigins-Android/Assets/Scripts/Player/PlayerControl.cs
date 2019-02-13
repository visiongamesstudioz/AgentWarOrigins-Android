using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.Native.PInvoke;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace EndlessRunner
{
    public enum Slots
    {
        Left = -1,
        Center,
        Right
    }

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerAnimation))]
    [RequireComponent(typeof(Player))]
    public class PlayerControl : MonoBehaviour
    {
        public static PlayerControl Instance;
        //public variables to control movement
        [SerializeField] private  float m_JumpPower = 5f;
        [Range(1f, 4f)] [SerializeField] private  float m_GravityMultiplier = 2f;
        [SerializeField] private  float m_MoveSpeedMultiplier = 1f;
        [SerializeField] private  float m_AnimSpeedMultiplier = 1f;
        [SerializeField] private  float slotDistance = 5f;
        [SerializeField] private float m_GroundCheckDistance;

        public LayerMask SphereCastLayerMask;
        public LayerMask NormalColliderLayer;
        //component variables
        private Rigidbody m_Rigidbody;
        private Animator m_Animator;
        private PlayerAnimation playerAnimation;
        private PlayerHealth playerHealth;
        private Vector3 m_GroundNormal;
        private CapsuleCollider m_Capsule;
        //private variable to handle movement
        private Slots currentSlotPosition;
        private float targetHorizontalPosition;
        private bool m_Jump;
        private bool m_sliding;
        private bool m_IsGrounded=true;
        private bool m_Movement;
        private bool m_moveLeft;
        private bool m_move_Right;
        private float m_forwardmovement;
        private float distToGround;
        private Vector3 m_targetPosition;
        private Vector3 targetPos;
        private bool m_climb;
        public bool m_isCollidedWithObstacle;
        private PlayerShoot playerShoot;
        private bool isPlayerPositionSet;
        private AudioSource m_AudioSource;
        private Player m_Player;
        private float m_PreviousSpeedMultiplier;
        private Vector3 m_PreviousPosition;
        private float m_CurrentTimePassedForColliderStuck;
        private Vector3 m_DesiredMove;
        private bool m_IsInTrigger;
        private float m_DefaultGroundCheckDistance;
        private Camera mainCamera;
        private CameraController camController;
        private CapsuleCollider m_CapsuleCollider;
        private GameObject m_HitObstacle;
        private List<Collider> m_aliveEnemies;
        private InputControl m_InputControl;
        private bool changeSideWaysMovement=true;
        private float currentZPos;
        private float previousZPos;
        private bool isResumeFromDeath;
        private CameraVerticalMovementController m_cameraVerticalMovementController;
        private Vector3 m_tutorialMovementSpeed;
        private SceneObject sceneObject;
        private CameraVerticalMovementController tutorialPlayerCameraVerticalMovementController;
        private Image taptoshooticon;
        TapToShoot tapToShootcomp;

        private void Awake()
        {
            Instance = this;
            playerShoot = GetComponent<PlayerShoot>();
            m_aliveEnemies=new List<Collider>();
            m_Rigidbody = GetComponent<Rigidbody>();
            playerAnimation = GetComponent<PlayerAnimation>();
            playerHealth = GetComponent<PlayerHealth>();
            m_Animator = GetComponent<Animator>();
            m_AudioSource = GetComponent<AudioSource>();
            m_Player = GetComponent<Player>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_PreviousSpeedMultiplier = m_MoveSpeedMultiplier;
            m_PreviousPosition = transform.position;
            m_CurrentTimePassedForColliderStuck = 0;
            m_InputControl = GetComponent<InputControl>();
            m_tutorialMovementSpeed = 20 * Vector3.right;
        }

        public bool CollidedWithObstacle
        {
            set { m_isCollidedWithObstacle = value; }
        }

        public float MoveSpeedMultiplier
        {
            get { return m_MoveSpeedMultiplier; }
            set { m_MoveSpeedMultiplier = value; }
        }

        public float AnimSpeedMultiplier
        {
            get { return m_AnimSpeedMultiplier; }
            set { m_AnimSpeedMultiplier = value; }
        }

        public bool IsinTrigger
        {
            get { return m_IsInTrigger; }
        }

        public float PreviousSpeedMultiplier
        {
            get { return m_PreviousSpeedMultiplier; }
        }

        public bool IsResumeFromDeath
        {
            get
            {
                return isResumeFromDeath;
            }

            set
            {
                isResumeFromDeath = value;
            }
        }

        private void Start()
        {
            //   m_Capsule = GetComponent<CapsuleCollider>();
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY |
                                      RigidbodyConstraints.FreezeRotationZ;
            distToGround = GetComponent<Collider>().bounds.extents.y;
            currentSlotPosition = Slots.Center;
            m_DefaultGroundCheckDistance = m_GroundCheckDistance;
            currentZPos = transform.position.z;
            previousZPos = -100f;

            mainCamera = Camera.main;
        }

        private void Update()
        {
            //if (!Util.IsTutorialComplete())
            //{
            //    Move(20*Vector3.right, m_Jump, m_sliding, m_moveLeft, m_move_Right,1);

            //}
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera)
                {
                    if (camController == null)
                    {
                        camController = mainCamera.GetComponent<CameraController>();
                    }
                    if (m_cameraVerticalMovementController == null)
                    {
                        m_cameraVerticalMovementController = mainCamera.GetComponent<CameraVerticalMovementController>();
                    }
                }
             
            }
            if (playerHealth.IsDead)
            {
                playerShoot.Disable();
            }
            //check if player is stuck between colliders

            float deltaPositionX = transform.position.x - m_PreviousPosition.x;


            m_CurrentTimePassedForColliderStuck += Time.deltaTime;
            if (m_CurrentTimePassedForColliderStuck > 0.5f)
            {
                if (GameManager.Instance)
                {
                    if (Math.Abs(deltaPositionX) < 0.001f && GameManager.Instance.IsGameActive())
                    {
                        transform.position += new Vector3(1f, 0f, 0f);
                    }
                }

                m_CurrentTimePassedForColliderStuck = 0;
            }
          
            if (m_Movement)
            {
                if (!m_isCollidedWithObstacle)
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, 20 * Time.deltaTime);


                //   m_Rigidbody.velocity = new Vector3(m_forwardmovement * m_MoveSpeedMultiplier, 0, 0);

                if ((targetPos - transform.position).sqrMagnitude <= 0 || m_isCollidedWithObstacle) 
                {
                    m_Movement = false;
                    m_isCollidedWithObstacle = false;
                }
            }
            m_PreviousPosition = transform.position;


        }

        private void FixedUpdate()
        {
           

            if (playerShoot.CurrentWeapon)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, playerShoot.CurrentWeapon.Weapon.WeaponRange, SphereCastLayerMask);
                int closestIndex = -1;
                float distanceToClosestEnemy = float.MaxValue;
                m_aliveEnemies.Clear();
                foreach (var col in colliders)
                {
                    if (col.GetComponent<Health>().GetCurrentHealth() > 0)
                    {
                        m_aliveEnemies.Add(col);
                    }
                }

                closestIndex = Util.FindClosestGameObjectIndexInFront(gameObject, m_aliveEnemies);
                if (closestIndex == -1)
                {
                    if (m_InputControl)
                    {
                        m_InputControl.movementSpeed = 40;

                    }

                    m_MoveSpeedMultiplier = m_PreviousSpeedMultiplier;
                    distanceToClosestEnemy = float.MaxValue;

                }
                else
                {
                    distanceToClosestEnemy = Vector3.Distance(transform.position,
                        colliders[closestIndex].transform.position);

                }

                if (mainCamera && camController)
                {
                    if (distanceToClosestEnemy < playerShoot.CurrentWeapon.Weapon.WeaponRange)
                    {
                        //  Debug.Log("ontrigger stay clled");
                        //change camera field of view so enemies are clearly seen
                        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.EnemySceneFOV,
                            camController.SceneFOVInterPolationSpeed * Time.deltaTime);
                        camController.SetEnemyDistanceFromTarget();
                        playerShoot.enabled = true;
                        if (m_InputControl)
                        {
                            m_InputControl.movementSpeed = 20;
                        }


                        m_MoveSpeedMultiplier = 0.2f;
                    }
                    else
                    {
                        //change camera field of view to default

                        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.NormalSceneFOV,
                            camController.SceneFOVInterPolationSpeed * Time.deltaTime);
                        camController.SetNormalDistanceFromTarget();
                        if (m_InputControl)
                        {
                            m_InputControl.movementSpeed = 40;
                        }


                        m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier, 1);
                        playerShoot.enabled = false;
                    }

                    //if (sceneObject != null)
                    //{
                    //    if (sceneObject.hasDrones)
                    //    {
                    //        m_cameraVerticalMovementController.minimumVert = -30;
                    //    }
                    //    else
                    //    {
                    //        m_cameraVerticalMovementController.minimumVert = 0;
                    //    }
                    //}
                }
                if (!Util.IsTutorialComplete())
                {
                    GameObject enemyNeck = null;
                    if (taptoshooticon == null)
                    {
                        taptoshooticon = UiManager.Instance.ShowTapToShoot();
                        tapToShootcomp = taptoshooticon.GetComponentInChildren<TapToShoot>();
                    }
                    
                    if (closestIndex == -1)
                    {
                        taptoshooticon.enabled = false;
                        return;
                    }

                    if (colliders[closestIndex].CompareTag("Enemy"))
                    {
                        enemyNeck = Util.FindGameObjectWithTag(colliders[closestIndex].gameObject, "Neck");
                    }
                    else if (colliders[closestIndex].CompareTag("EnemyDrone"))
                    {
                        enemyNeck = colliders[closestIndex].gameObject;
                    }

                    tapToShootcomp.SetObjectToFollow(enemyNeck);

                }
            }

        }

        public void Move(Vector3 move, bool jump, bool slide, bool walkLeft, bool walkRight, float animationSpeed)
        {
            currentZPos = transform.position.z;
            m_DesiredMove = move;
            CheckGroundStatus();
            //  CheckForCollisions();
            m_Jump = jump;
            m_sliding = slide;
            m_moveLeft = walkLeft;
            m_move_Right = walkRight;
            m_forwardmovement = move.x;

            //if (m_moveLeft || m_move_Right)
            //{
            //    Debug.Log("move left" + m_moveLeft + "move right" + m_move_Right);

            //    //Debug.Log("current z position" + currentZPos);
            //    //Debug.Log("slot distance=" + slotDistance);
            //    //Debug.Log("absolute current pos" + Mathf.Abs(currentZPos));
            //    //Debug.Log("absolute current diff pos" + Mathf.Abs(Mathf.Abs(currentZPos) - slotDistance));

            //    Slots currentSlot = GetCurrentSlotPosition();
            //    switch (currentSlot)
            //    {
            //        case Slots.Center:
            //            if (Mathf.Abs(currentZPos) > 0.5f && Math.Abs(currentZPos) < slotDistance)
            //            {
            //                Debug.Log("center if statement caleed");
            //                changeSideWaysMovement = false;
            //            }
            //            else
            //            {
            //                Debug.Log("center else statement caleed");

            //                changeSideWaysMovement = true;
            //            }
            //            break;
            //        case Slots.Left:
            //            Debug.Log("left statement caleed");

            //            break;
            //        case Slots.Right:
            //            Debug.Log("right statement caleed");
                        
            //            break;
            //    }
   
            //    previousZPos = 0;
            //}
            //else
            //{
            //    changeSideWaysMovement = true;
            //}
            // send input and other state parameters to the animator
            if (move.magnitude > 0)
            {
                UpdateAnimatorAndSound(move);
            }
            // control and velocity handling is different when grounded and airborne:
            if (!playerHealth.IsDead)
            {
                if (m_IsGrounded)
                {
                    HandleGroundedMovement(m_DesiredMove, jump, slide, walkLeft, walkRight);

                }
                else
                    HandleAirborneMovement();
            }

            else
            {
                m_Rigidbody.velocity=Vector3.zero;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (mainCamera)
            {
                if (camController == null)
                {
                    camController = mainCamera.GetComponent<CameraController>();
                }
                if (m_cameraVerticalMovementController == null)
                {
                    m_cameraVerticalMovementController = mainCamera.GetComponent<CameraVerticalMovementController>();
                }
            }
            if (other.gameObject.CompareTag("Stairs"))
            {
                m_GroundCheckDistance = 100f;
                m_climb = true;
            }
            //else if (other.gameObject.layer == LayerMask.NameToLayer("TriggerLayer"))
            //{

            //    ObstacleObject obstacleObject = other.gameObject.GetComponentInParent<ObstacleObject>();
            //    StaticObstacle staticObstacle = other.gameObject.GetComponentInParent<StaticObstacle>();
            //    if (obstacleObject)
            //    {
            //        obstacleObject.IsTriggerEntered = true;
            //        obstacleObject.IsCollisionEntered = false;
            //    }

            //    if (staticObstacle)
            //    {
            //        staticObstacle.IsTriggerEntered = true;
            //        staticObstacle.IsCollisionEntered = false;

            //    }

            //}
            else if (other.gameObject.layer == NormalColliderLayer)
            {
                ObstacleObject obstacleObject = other.gameObject.GetComponentInParent<ObstacleObject>();
                StaticObstacle staticObstacle = other.gameObject.GetComponentInParent<StaticObstacle>();

                if (obstacleObject)
                {
                    obstacleObject.IsTriggerEntered = true;
                }
                if (staticObstacle)
                {
                    staticObstacle.IsCollisionEntered = true;
                }
                //get collided object
                m_HitObstacle = other.gameObject;

            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
            {

                playerShoot.enabled = true;
                ////move to center
                //switch (currentSlotPosition)
                //{
                //    case Slots.Left:
                //        //  playerAnimation.StrafeMovement(true);
                //        HandleLeftOrRightMovement(true);
                //        break;
                //    case Slots.Right:
                //        //   playerAnimation.StrafeMovement(false);
                //        HandleLeftOrRightMovement(false);
                //        break;
                //}
                if (m_InputControl)
                {
                    m_InputControl.movementSpeed = 20;
                }
                m_PreviousSpeedMultiplier = m_MoveSpeedMultiplier;
                m_MoveSpeedMultiplier = 0.2f;

            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("TutorialTrigger"))
            {

                TriggerType triggerType = other.GetComponent<TriggerType>();
                TriggerEnum triggerEnum = triggerType.TriggerEnum;
                switch (triggerEnum)
                {
                    case TriggerEnum.SwipeUp:
                        //show swipe up image
                        UiManager.Instance.ShowTapToShoot();
                        UiManager.Instance.ShowTapOnEnemiesText("Swipe Up On Right Side to jump");
                        UiManager.Instance.ShowSwipeUp();
                        break;
                    case TriggerEnum.SwipeDown:

                        UiManager.Instance.ShowTapToShoot();
                        UiManager.Instance.ShowTapOnEnemiesText("Swipe down On Right Side to Slide");
                        UiManager.Instance.ShowSwipeDown();

                        break;
                    case TriggerEnum.SwipeLeft:
                        UiManager.Instance.ShowTapToShoot();
                        UiManager.Instance.ShowTapOnEnemiesText("Swipe left On Right Side to move Left");
                        UiManager.Instance.ShowSwipeLeft();

                        break;
                    case TriggerEnum.SwipeRight:
                        UiManager.Instance.ShowTapToShoot();
                        UiManager.Instance.ShowTapOnEnemiesText("Swipe left On Right Side to move Left");
                        UiManager.Instance.ShowSwipeRight();
                        break;
                    case TriggerEnum.Tap:
                        UiManager.Instance.ShowTapToShoot();
                        UiManager.Instance.ShowTapOnEnemiesText("Tap on enemies to shoot ");
                        break;
                    case TriggerEnum.swipeUpJoyStick:
                        UiManager.Instance.ShowTapOnEnemiesText("Swipe up the joy stick to aim higher");
                        UiManager.Instance.EnableJoyStickAnimator();
                        UiManager.Instance.ShowTapToShoot();
                        break;
                    case TriggerEnum.AimDownSights:
                        UiManager.Instance.ShowTapToShoot();
                        UiManager.Instance.ShowTapOnEnemiesText("Tap on scope to aim enemy closer" );

                        break;
                }

            }
            else if (other.CompareTag("TutorialEnd"))
            {
                if (!Util.IsTutorialComplete())
                {
                    StartCoroutine(UiManager.Instance.OnTutorialCompleted());
                }

            }
            else
            {
                m_GroundCheckDistance = m_DefaultGroundCheckDistance;
            }


        }

        private void OnDrawGizmosSelected()
        {
#if UNITY_EDITOR
            Gizmos.color = Color.red;
            //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
     //       Gizmos.DrawWireSphere(transform.position, Camera.main.farClipPlane);
#endif
        }

        private void OnTriggerStay(Collider other)
        {
            m_IsInTrigger = true;

            if (other.gameObject.CompareTag("Stairs"))
            {
                m_GroundCheckDistance = 50f;

                m_climb = true;
            }

            else if (other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
            {
                if (sceneObject == null)
                {
                    sceneObject = other.GetComponent<SceneObject>();
                }
                ////clear alive enemy list
                //m_aliveEnemies.Clear();
                //Collider[] colliders = Physics.OverlapSphere(transform.position,300, SphereCastLayerMask);
                //foreach (var col in colliders)
                //{
                //    if (col.GetComponent<Health>().GetCurrentHealth() > 0)
                //    {
                //        m_aliveEnemies.Add(col);
                //    }
                //}

                //int closestIndex = Util.FindClosestGameObjectIndexInFront(gameObject, m_aliveEnemies);
                //float distanceToClosestEnemy;
                //if (closestIndex == -1)
                //{
                //    if (m_InputControl)
                //    {
                //        m_InputControl.movementSpeed = 40;

                //    }

                //    m_MoveSpeedMultiplier = m_PreviousSpeedMultiplier;
                //    distanceToClosestEnemy = float.MaxValue;

                //}
                //else
                //{
                //    distanceToClosestEnemy = Vector3.Distance(transform.position,
                //    colliders[closestIndex].transform.position);

                //}

//                if (mainCamera && camController)
//                {
////#if UNITY_EDITOR || UNITY_STANDALONE

////                    m_cameraVerticalMovementController.enabled = true;

////#endif
//                    if (sceneObject.hasDrones)
//                    {

//                       m_cameraVerticalMovementController.minimumVert = -30;
//                        if (distanceToClosestEnemy < 200)
//                        {
//                            //  Debug.Log("ontrigger stay clled");
//                            //change camera field of view so enemies are clearly seen
//                            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.NormalSceneFOV, camController.SceneFOVInterPolationSpeed * Time.deltaTime);
//                            camController.SetEnemyDistanceFromTarget();
//                            playerShoot.enabled = true;
//                            m_InputControl.movementSpeed = 20;

//                            m_MoveSpeedMultiplier = 0.2f;
//                        }
//                        else
//                        {
//                            //change camera field of view to default

//                            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.NormalSceneFOV, camController.SceneFOVInterPolationSpeed * Time.deltaTime);
//                            camController.SetNormalDistanceFromTarget();
//                            m_InputControl.movementSpeed = 40;

//                            m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier, 1);
//                            playerShoot.enabled = false;
//                        }
//                    }
//                    else
//                    {
//                        m_cameraVerticalMovementController.minimumVert = 0;

//                        //  Debug.Log("distanceto closeset enemy" + distanceToClosestEnemy);
//                        if (distanceToClosestEnemy < playerShoot.CurrentWeapon.Weapon.WeaponRange)
//                        {
//                            //  Debug.Log("ontrigger stay clled");
//                            //change camera field of view so enemies are clearly seen
//                            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.EnemySceneFOV, camController.SceneFOVInterPolationSpeed * Time.deltaTime);
//                            camController.SetEnemyDistanceFromTarget();
//                            playerShoot.enabled = true;
//                            m_InputControl.movementSpeed = 20;

//                            m_MoveSpeedMultiplier = 0.2f;
//                        }
//                        else
//                        {
//                            //change camera field of view to default
//                            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.NormalSceneFOV, camController.SceneFOVInterPolationSpeed * Time.deltaTime);
//                            camController.SetNormalDistanceFromTarget();
//                            m_InputControl.movementSpeed = 40;

//                            m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier, 1); 

//                            playerShoot.enabled = false;

//                        }
//                    }
 
//                }

                //check if tutorial is complete and show 
                if (!Util.IsTutorialComplete())
                {
                    ////clear alive enemy list
                    m_aliveEnemies.Clear();
                    Collider[] colliders = Physics.OverlapSphere(transform.position, 300, SphereCastLayerMask);
                    foreach (var col in colliders)
                    {
                        if (col.GetComponent<Health>().GetCurrentHealth() > 0)
                        {
                            m_aliveEnemies.Add(col);
                        }
                    }
                    int closestIndex = Util.FindClosestGameObjectIndexInFront(gameObject, m_aliveEnemies);
                    float distanceToClosestEnemy;
                    if (closestIndex == -1)
                    {
                        if (m_InputControl)
                        {
                            m_InputControl.movementSpeed = 40;

                        }

                        m_MoveSpeedMultiplier = m_PreviousSpeedMultiplier;
                        distanceToClosestEnemy = float.MaxValue;

                    }
                    else
                    {
                        distanceToClosestEnemy = Vector3.Distance(transform.position,
                        colliders[closestIndex].transform.position);

                    }
                    //if (tutorialPlayerCameraVerticalMovementController == null)
                    //{
                    //    tutorialPlayerCameraVerticalMovementController = GetComponentInChildren<CameraVerticalMovementController>();
                    //}
                    //m_MoveSpeedMultiplier = 0.2f;
                    GameObject enemyNeck=null;
                    Image taptoshooticon= UiManager.Instance.ShowTapToShoot();
                    TapToShoot tapToShootcomp = taptoshooticon.GetComponentInChildren<TapToShoot>();
                    if (closestIndex == -1)
                    {
                        taptoshooticon.enabled = false;
                        return;
                    }
                    if (colliders[closestIndex].CompareTag("Enemy"))
                    {
                        enemyNeck = Util.FindGameObjectWithTag(colliders[closestIndex].gameObject, "Neck");
                    }
                    else if(colliders[closestIndex].CompareTag("EnemyDrone"))
                    {
                        enemyNeck = colliders[closestIndex].gameObject;
                    }
                    tapToShootcomp.SetObjectToFollow(enemyNeck);

                    //
                    if (sceneObject.hasDrones)
                    {
                        if (tutorialPlayerCameraVerticalMovementController)
                        {
                            tutorialPlayerCameraVerticalMovementController.minimumVert = -30;

                        }

                        if (distanceToClosestEnemy < 200)
                        {
                            playerShoot.enabled = true;
                            m_MoveSpeedMultiplier = 0.2f;
                        }
                        else
                        {
                            m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier, 1);
                            playerShoot.enabled = false;

                        }
                    }
                    else
                    {
                        if (tutorialPlayerCameraVerticalMovementController)
                        {
                            tutorialPlayerCameraVerticalMovementController.minimumVert = 0;

                        }
                        if (distanceToClosestEnemy < playerShoot.CurrentWeapon.Weapon.WeaponRange)
                        {
                            playerShoot.enabled = true;
                            m_MoveSpeedMultiplier = 0.2f;
                        }
                        else
                        {
                            m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier, 1);
                            playerShoot.enabled = false;

                        }
                    }
                }
            }
          
        }

        private void OnTriggerExit(Collider other)
        {
            //set scene object to null 
            sceneObject = null;
            UiManager.Instance.HideTutorial();

            if (m_cameraVerticalMovementController)
            {
                m_cameraVerticalMovementController.enabled = false;
                m_cameraVerticalMovementController.ResetCameraRotation();
            }
     
            //m_IsInTrigger = false;
            //Camera mainCamera = Camera.main;

            //if (other.gameObject.tag == "Stairs")
            //    m_climb = false;

            if (other.gameObject.layer == LayerMask.NameToLayer("SceneWithEnemy"))
            {
                if (mainCamera && camController)
                {
                    //change camera field of view to default
                    mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, camController.NormalSceneFOV, camController.SceneFOVInterPolationSpeed * Time.deltaTime);

                    camController.SetNormalDistanceFromTarget();
                    m_InputControl.movementSpeed = 40;

                    m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier,1);
                    playerShoot.enabled = false;
                }
      
            }
            if (!Util.IsTutorialComplete())
            {
                m_MoveSpeedMultiplier = Mathf.Max(m_PreviousSpeedMultiplier, 1);
            }
            UiManager.Instance.DisableJoyStickAnimator();
        }

        private void LateUpdate()
        {
            //  objectGenerator.SpawnObjectRun(true);
        }

        private void UpdateAnimatorAndSound(Vector3 move)
        {
            //update collider center based on animation curves
            if (playerAnimation.GetCurrentAnimationStateHashInLayer(0).IsName("Run_Jump") ||
                playerAnimation.GetCurrentTransitionInfoInLayer(0).IsName("Base Layer.Running -> Base Layer.Run_Jump"))
                m_CapsuleCollider.center = new Vector3(0, playerAnimation.GetColliderCenter(), 0);
            else if(playerAnimation.GetCurrentAnimationStateHashInLayer(0).IsName("Slide"))
            {
                m_CapsuleCollider.center = new Vector3(0, playerAnimation.GetColliderCenter(), 0);
                m_CapsuleCollider.height = playerAnimation.GetColliderHeight();
            }
            else
            {
                m_CapsuleCollider.center = new Vector3(0, playerAnimation.DefaultColliderCenter, 0);
                m_CapsuleCollider.height = playerAnimation.DefaultColliderHeight;
            }




            // update the animator parameters
            if (move.sqrMagnitude > 0 && m_IsGrounded)

                playerAnimation.Run(true);
            //m_Animator.SetBool("OnGround", m_IsGrounded);
            if (m_Jump && m_IsGrounded)
            {
                if (HeightFromGround() > 3)
                {
                    //  playerAnimation.Jump(JumpType.BigJump);
                }
                else
                {
                    playerAnimation.Jump(JumpType.RunningJump);

                }
                //play jump sound
                AudioManager.Instance.PlaySound(m_AudioSource, m_Player.JumpAudioClips[Random.Range(0, m_Player.JumpAudioClips.Length)]);
                m_Jump = false;
            }

         
  
            if (m_sliding && m_IsGrounded)
            {
                //update colliders
               
                playerAnimation.Slide();
                AudioManager.Instance.PlaySound(m_AudioSource,m_Player.SlideAudioClips[Random.Range(0,m_Player.SlideAudioClips.Length)]);

            }

            if (m_moveLeft && m_IsGrounded)
            {
                if (GetCurrentSlotPosition() != Slots.Left)
                {

                    playerAnimation.StrafeMovement(false);
                    Camera activeCam= UiManager.Instance.GetActiveCamera();
                    if (activeCam == null)
                    {
                        activeCam=Camera.main;
                    }

                    if (!activeCam.CompareTag("MainCamera"))
                    {
                        int weaponId = playerShoot.CurrentWeapon.Weapon.WeaponId;
                        Animator animator = activeCam.GetComponent<Animator>();
                        if (animator)
                        {
                            switch (weaponId)
                            {
                                case 1:
                                    animator.SetTrigger("SR45Zoom");
                                    break;
                                case 2:
                                    animator.SetTrigger("SCUZoom");
                                    break;
                                case 3:
                                    animator.SetTrigger("SMGZoom");
                                    break;
                                case 4:
                                    animator.SetTrigger("FWZoom");
                                    break;
                                case 5:
                                    animator.SetTrigger("LauncherZoom");
                                    break;
                                case 6:

                                    break;
                            }
                        }

                    }

                }
                m_moveLeft = false;
            }
            if (m_move_Right && m_IsGrounded)
            {
                if (GetCurrentSlotPosition() != Slots.Right)
                {

                    playerAnimation.StrafeMovement(true);
                    Camera activeCam = UiManager.Instance.GetActiveCamera();
                    if (activeCam == null)
                    {
                        activeCam=Camera.main;
                    }
                    if (!activeCam.CompareTag("MainCamera"))
                    {
                        int weaponId = playerShoot.CurrentWeapon.Weapon.WeaponId;
                        Animator animator = activeCam.GetComponent<Animator>();
                        if (animator)
                        {
                            switch (weaponId)
                            {
                                case 1:
                                    animator.SetTrigger("SR45Zoom");
                                    break;
                                case 2:
                                    animator.SetTrigger("SCUZoom");
                                    break;
                                case 3:
                                    animator.SetTrigger("SMGZoom");
                                    break;
                                case 4:
                                    animator.SetTrigger("FWZoom");
                                    break;
                                case 5:
                                    animator.SetTrigger("LauncherZoom");
                                    break;
                                case 6:
                                    break;
                            }
                        }
                  
                    }

                }
                m_move_Right = false;
            }
            if (!playerAnimation.IsInTransition(0))
            {
                if (playerAnimation.GetCurrentAnimationStateHashInLayer(0).IsName("Run_Jump"))
                {

                    Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                    RaycastHit raycastHitInfo;
                    if (Physics.Raycast(ray, out raycastHitInfo))
                    {
                        if (raycastHitInfo.distance > 1f)
                        {
                            playerAnimation.MatchTarget(raycastHitInfo, AvatarTarget.Root, new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), 0.15f, 0.2f);
                        }
                    }
                }
            }
        
            //if (m_die)
            //{
            //    playerAnimation.Death();

            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (m_IsGrounded && move.sqrMagnitude > 0)
                playerAnimation.SetAnimatorSpeed(m_AnimSpeedMultiplier);
            else
                playerAnimation.SetAnimatorSpeed(1f);
        }

    
        private float HeightFromGround()
        {
            return transform.position.y;
        }

        private void HandleAirborneMovement()
        {
            // apply extra gravity from multiplier:
            var extraGravityForce = Physics.gravity * m_GravityMultiplier - Physics.gravity;

            Vector3 addExtraForce = extraGravityForce + new Vector3(5, 0, 0);
            m_Rigidbody.AddForce(addExtraForce);


        }
        public void HandleAirborneMovement(bool walkLeft, bool walkRight) 
        {
            // apply extra gravity from multiplier:
            var extraGravityForce = Physics.gravity * m_GravityMultiplier - Physics.gravity;
            m_Rigidbody.AddForce(extraGravityForce);

            if (walkLeft && !walkRight)
            {

                HandleLeftOrRightMovement(false);

            }
            else if (walkRight && !walkLeft)
            {

                HandleLeftOrRightMovement(true);

            }
        }
        //slide animation event
        void OnSlideComplete()
        {
            Slots currentSlot = GetCurrentSlotPosition();
            switch (currentSlot)
            {
                    case Slots.Center:
                    transform.position=new Vector3(transform.position.x,transform.position.y,0);
                    break;
                    case Slots.Right:
                    transform.position=new Vector3(transform.position.x,transform.position.y, (int)Slots.Left * slotDistance);
                    break;
                    case  Slots.Left:
                    transform.position=new Vector3(transform.position.x,transform.position.y,(int)Slots.Right * slotDistance);
                    break;
            }
        }

        void OnJumpComplete()
        {
            Slots currentSlot = GetCurrentSlotPosition();
            switch (currentSlot)
            {
                case Slots.Center:
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                    break;
                case Slots.Right:
                    transform.position = new Vector3(transform.position.x, transform.position.y, (int)Slots.Left * slotDistance);
                    break;
                case Slots.Left:
                    transform.position = new Vector3(transform.position.x, transform.position.y, (int)Slots.Right * slotDistance);
                    break;
            }
        }
        public void HandleGroundedMovement(Vector3 move, bool jump, bool slide, bool walkLeft, bool walkRight)
        {
            if (!jump && !walkLeft && !walkRight && !m_Movement && !m_climb && !playerHealth.IsDead)
            {
                m_Rigidbody.velocity = move * m_MoveSpeedMultiplier;
            }
            else if (!jump && !slide && !walkLeft && !walkRight && !m_Movement && m_climb)
                m_Rigidbody.velocity = new Vector3(move.x * m_MoveSpeedMultiplier, m_JumpPower, m_Rigidbody.velocity.z);
            // check whether conditions are right to allow a jump:
            if (jump && !slide && !walkLeft && !walkRight && !m_climb)
            {
                m_Rigidbody.velocity = new Vector3(move.x * m_MoveSpeedMultiplier, m_JumpPower, m_Rigidbody.velocity.z);
                m_IsGrounded = false;
         //       playerAnimation.ApplyRootMotion(false);
                m_GroundCheckDistance = m_DefaultGroundCheckDistance;
       //         playerAnimation.ApplyRootMotion(false);
            }
            //check conditions for sliding movement
            if (slide && !jump && !walkLeft && !walkRight)
                HandleSlideMovement();
            if (walkLeft && !slide && !jump && !walkRight)
            {
             
                    HandleLeftOrRightMovement(false);
                
            }
            else if (walkRight && !slide && !jump && !walkLeft) { 

                    HandleLeftOrRightMovement(true);
                
            }
  
        }

        private void HandleSlideMovement()
        {
            m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.gameObject.layer == LayerMask.NameToLayer("DeathObstacle"))
        //    {
        //        Die = true;
        //        playerAnimation.Death(DeathType.DyingBackwards);
        //    }
        //    else if (collision.gameObject.layer == LayerMask.NameToLayer("SemiDeathObstacle"))
        //    {
                
        //        playerAnimation.Stumble();
        //    }
        //}


        private void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + Vector3.up * 0.01f,
                transform.position + Vector3.up * 0.1f + Vector3.down * 0.1f);
#endif

            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                if (hitInfo.collider.CompareTag("Ground"))
                    m_GroundCheckDistance = m_DefaultGroundCheckDistance;

                m_GroundNormal = hitInfo.normal;
                m_IsGrounded = true;
                playerAnimation.ApplyRootMotion(true);
                m_DesiredMove = Vector3.ProjectOnPlane(m_DesiredMove, m_GroundNormal);

            }
            else
            {
                m_IsGrounded = false;
                m_GroundNormal = Vector3.up;
                playerAnimation.ApplyRootMotion(false);
            }
        }

        public void HandleLeftOrRightMovement(bool right)
        {
            var targetSlot =
                (Slots) Mathf.Clamp((int) currentSlotPosition + (right ? 1 : -1), (int) Slots.Left, (int) Slots.Right);
            //Debug.Log("current slot" + currentSlotPosition);
            //Debug.Log("target slot" + targetSlot);
            ChangeSlots(targetSlot);
        }

        // There are three slots on a track. The accelorometer/swipes determine the slot position
        public void ChangeSlots(Slots targetSlot)
        {
            if (targetSlot == currentSlotPosition)
                return;
            targetHorizontalPosition = (int) targetSlot * slotDistance;
            if (targetSlot == Slots.Center)
                if (currentSlotPosition == Slots.Left)
                    targetHorizontalPosition = (int) Slots.Right * slotDistance;
                else if (currentSlotPosition == Slots.Right)
                    targetHorizontalPosition = (int) Slots.Left * slotDistance;
            UpdateHorizontalPosition(targetHorizontalPosition, currentSlotPosition,targetSlot);
            currentSlotPosition = targetSlot;
        }

        public Slots GetCurrentSlotPosition()
        {
            return currentSlotPosition;
        }

        public void UpdateHorizontalPosition(float horizontalAmount,Slots currentSlot,Slots targetSlot)
        {
            //Debug.Log("current position " + transform.position);
            //Debug.Log("horizaontal position" + horizontalAmount);
            m_Movement = true;
            //change here
            if (targetSlot == Slots.Right)
            {
                targetPos = new Vector3(transform.position.x + m_forwardmovement / 8, transform.position.y,
                    Mathf.Max((int) Slots.Left * slotDistance, transform.position.z - horizontalAmount));
            }
            else if(targetSlot==Slots.Left)
            {
                targetPos = new Vector3(transform.position.x + m_forwardmovement / 8, transform.position.y,
                    Mathf.Min((int) Slots.Right * slotDistance, transform.position.z - horizontalAmount));
            }
            else if(targetSlot==Slots.Center)
            {
                if (currentSlot == Slots.Left)
                {
                    targetPos = new Vector3(transform.position.x + m_forwardmovement / 8, transform.position.y,
                        Mathf.Max(0, transform.position.z - horizontalAmount));
                } 
                else if(currentSlot==Slots.Right)
                {
                    targetPos = new Vector3(transform.position.x + m_forwardmovement / 8, transform.position.y,
                        Mathf.Min(0, transform.position.z - horizontalAmount));
                }
            }
            

            //Vector3 temp = new Vector3(transform.position.x, 0, targetHorizontalPosition);
            //Debug.Log("target position with out y" + temp);
            ////Terrain[] activeTerrains = Terrain.activeTerrains;
            ////for (int i = 0; i < activeTerrains.Length; i++) {

            ////    Debug.Log(i + "terrain" + activeTerrains[i].name);
            ////}
            //if (Terrain.activeTerrain)
            //{

            //    temp.y = Terrain.activeTerrain.SampleHeight(temp) + Terrain.activeTerrain.GetPosition().y;
            //    Debug.Log("target position with y" + temp);
            //    transform.position = Vector3.Lerp(transform.position, temp, 0.25f);
            //}
            //transform.position = temp;
        }

        public void SetPlayerPosition(SceneObject scene)
        {
            //set player position
            if (!isPlayerPositionSet)
            {
                SetPlayerPosition(new Vector3(-scene.GetCurrentSceneLength() / 2 + 0.05f, 0, 0));
                isPlayerPositionSet = true;
            }
        }
        public void SetPlayerPosition(Vector3 position)
        {
            gameObject.transform.position = position;
        }
        public Vector3 GetPlayerCurrentPosition()
        {
            return transform.position;
        }
        //obstacle hit animation event
        void OnHitObstacle()
        {
            if (m_HitObstacle)
            {
                var hitObstacle = m_HitObstacle.GetComponent<InfiniteObject>();

                if (hitObstacle)
                    hitObstacle.Deactivate();
                       
            }
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.

            if (m_IsGrounded && Time.deltaTime > 0)
            {
                var v = m_Animator.deltaPosition * m_MoveSpeedMultiplier / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_Rigidbody.velocity.y;
                m_Rigidbody.velocity = v;
            }



        }
    }
}
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace EndlessRunner
{
    public class InputControl : MonoBehaviour
    {
        private PlayerControl m_Character; // A reference to the ThirdPersonCharacter on the object
        private PlayerShoot playerShoot;
        private Transform m_Cam; // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward; // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;
        private bool m_stumble;
        // the world-relative desired move direction, calculated from the camForward and user input.
        [Range(0, 1)]
        //   public float movementSpeed = 0.1f;
        public float animationSpeed = 2f;

        private bool m_Slide;
        public float movementSpeed = 10f;
        public bool autoRun = true;
        private bool isStarted;
        private PlayerControl playerControl;
        // if true the player is not bound to slot positions
        public bool freeHorizontalMovement = false;
        private CameraVerticalMovementController m_cameraVerticalMovementController;

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8
        // move horizontally with a swipe (true) or the accelerometer (false)
        public bool swipeToMoveHorizontally = true;
        // The number of pixels you must swipe in order to register a horizontal or vertical swipe
        public Vector2 swipeDistance = new Vector2(1, 1);
        public Vector2 swipeDistanceForVerticalMovement = new Vector2(10, 10);
        // How sensitive the horizontal and vertical swipe are. The higher the value the more it takes to activate a swipe
        public float swipeSensitivty = 1;
        // More than this value and the player will move into the rightmost slot.
        // Less than the negative of this value and the player will move into the leftmost slot.
        // The accelerometer value in between these two values equals the middle slot.
        private Vector2 touchStartPosition;
        private bool acceptInput; // ensure that only one action is performed per touch gesture
#endif
#if UNITY_EDITOR || !(UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8)
        public bool sameKeyForTurnHorizontalMovement = false;
        public bool useMouseToMoveHorizontally = true;
        // if freeHorizontalMovement is enabled, this value will specify how much movement to apply when a key is pressed
        public float horizontalMovementDelta = 0.2f;
        // how sensitive the horizontal movement is with the mouse. The higher the value the more it takes to move
        public float horizontalMovementSensitivity = 100;
        // Allow slot changes by moving the mouse left or right
        public float mouseXDeltaValue = 100f;
        private float mouseStartPosition;
        private float mouseStartTime;

#endif
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        private void Awake()
        {
            playerShoot = GetComponent<PlayerShoot>();
        }

        private void Start()
        {
            //get player control Instance
            playerControl = PlayerControl.Instance;
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
                m_Cam.transform.localEulerAngles = new Vector3(0,-90, 0);
                m_cameraVerticalMovementController = m_Cam.GetComponent<CameraVerticalMovementController>();
            }

            else
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.");

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<PlayerControl>();
            CrossPlatformInputManager.SwitchActiveInputMethod(CrossPlatformInputManager.ActiveInputMethod.Hardware);
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (!playerShoot.enabled)
            {
                m_cameraVerticalMovementController.ResetCameraRotation();
                m_cameraVerticalMovementController.enabled = false;
                if (!m_Jump)
                {
                    m_Jump = Input.GetKeyDown(KeyCode.UpArrow);
                }

                if (!m_Slide)
                {
                    m_Slide = Input.GetKeyDown(KeyCode.DownArrow);
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveLeft = true;
                    MoveRight = false;
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveRight = true;
                    MoveLeft = false;
                }
                
            }
            if (playerShoot.enabled)
            {
                m_cameraVerticalMovementController.enabled = true;

                if (Input.GetMouseButton(0))
                {
                    
                        playerShoot.Shoot(Input.mousePosition);
                }
            }



#elif UNITY_ANDROID || UNITY_IOS


         if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (!playerShoot.enabled)
                {

                    m_cameraVerticalMovementController.ResetCameraRotation();

                    m_cameraVerticalMovementController.enabled = false;

                    if (touch.phase == TouchPhase.Began)
                    {
                        touchStartPosition = touch.position;
                        // Get movement of the finger since last frame
                        Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                    }
                    else if (touch.phase == TouchPhase.Moved && acceptInput)
                    {
                        Vector2 diff = touch.position - touchStartPosition;
                        if (diff.x == 0f)
                            diff.x = 1f; // avoid divide by zero
                        float verticalPercent = Mathf.Abs(diff.y / diff.x);

                        if (verticalPercent > swipeSensitivty && Mathf.Abs(diff.y) > swipeDistance.y)
                        {
                            if (diff.y > 0)
                            {
                                 m_Jump = true;
                                acceptInput = false;
                                PlayerData.PlayerProfile.NofJumps++;
                            }
                            else if (diff.y < 0)
                            {
                                m_Slide = true;
                                PlayerData.PlayerProfile.NoofSlides++;

                                acceptInput = false;
                            }
                            touchStartPosition = touch.position;
                        }
                        else if (verticalPercent < (1 / swipeSensitivty) && Mathf.Abs(diff.x) > swipeDistance.x)
                        {
                            // turn if above a turn, otherwise move horizontally
                            if (swipeToMoveHorizontally)
                            {
                                if (diff.x > 0)
                                {
                                    MoveRight = true;
                                    MoveLeft = false;
                                }
                                else
                                {
                                    MoveLeft = true;
                                    MoveRight = false;
                                }
                                //    playerControl.ChangeSlots(diff.x > 0 ? true : false);
                            }
                        }
                        acceptInput = false;
                    }
                    else if (touch.phase == TouchPhase.Stationary)
                    {
                        acceptInput = true;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        acceptInput = true;
                    }
                }
                else if (playerShoot.enabled)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        touchStartPosition = touch.position;
                        playerShoot.Shoot(touchStartPosition);
                        Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

                    }

                    else if (touch.phase == TouchPhase.Moved )
                    {
                        Vector2 diff = touch.position - touchStartPosition;
                        if (diff.x == 0f)
                            diff.x = 1f; // avoid divide by zero
                        float verticalPercent = Mathf.Abs(diff.y / diff.x);

                        if (verticalPercent > swipeSensitivty && Mathf.Abs(diff.y) > swipeDistanceForVerticalMovement.y)
                        {
                            m_cameraVerticalMovementController.enabled = true;

                            touchStartPosition = touch.position;
                        }
                        else
                        {
                            m_cameraVerticalMovementController.enabled = false;

                            playerShoot.Shoot(touchStartPosition);

                        }
                    }
                }
            }
#endif
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // read inputs

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                //m_Move = v*m_CamForward + h*m_Cam.right;
                m_Move = movementSpeed * m_CamForward;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                //m_Move = v*Vector3.forward + h*Vector3.right;
                m_Move = movementSpeed * Vector3.right;
            }
          


#elif UNITY_ANDROID || UNITY_IOS

            if (m_Cam != null)
                {
                    // calculate camera relative direction to move:
                    m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                    m_Move = movementSpeed * m_CamForward;
                }
#endif

            // pass all parameters to the character control script
            if (GameManager.Instance.IsGameActive())
            {
                if (!playerShoot.enabled)
                {
                    m_Character.Move(m_Move, m_Jump, m_Slide, MoveLeft, MoveRight, animationSpeed);
                    m_Jump = false;
                    m_Slide = false;
                    MoveLeft = false;
                    MoveRight = false;
                }
                else
                {
                    m_Character.Move(m_Move, false, false, false, false, animationSpeed);

                }

            }
        }

        public void JumpPressed()
        {
            m_Jump = true;
        }
    }
}
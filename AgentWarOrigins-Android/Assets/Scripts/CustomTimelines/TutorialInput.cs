using EndlessRunner;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInput : MonoBehaviour {

    public CameraVerticalMovementController m_cameraVerticalMovementController;
    private Vector2 touchStartPosition;
    public Vector2 swipeDistance = new Vector2(1, 1);
    public Vector2 swipeDistanceForVerticalMovement = new Vector2(10, 10);
    // How sensitive the horizontal and vertical swipe are. The higher the value the more it takes to activate a swipe
    public float swipeSensitivty = 1;
    public bool swipeToMoveHorizontally;
    private bool m_MoveRight;
    private bool m_MoveLeft;
    private PlayerControl m_playerControl; // A reference to the ThirdPersonCharacter on the object
    private PlayerShoot playerShoot;
    private Player m_Player;
    private bool m_Jump;
    private bool m_Slide;
    private Rigidbody m_RigidBody;
    private bool acceptInput; // ensure that only one action is performed per touch gesture

    // Use this for initialization

    private void Awake()
    {
        m_Player = GetComponent<Player>();
        m_playerControl = GetComponent<PlayerControl>();
        playerShoot = GetComponent<PlayerShoot>();
        m_RigidBody = GetComponent<Rigidbody>();
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
                m_MoveLeft = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                m_MoveRight = true;
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
                        }
                        else if (diff.y < 0)
                        {
                            m_Slide = true;

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
                                m_MoveRight = true;
                                m_MoveLeft = false;
                            }
                            else
                            {
                                m_MoveLeft = true;
                                m_MoveRight = false;
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

                else if (touch.phase == TouchPhase.Moved)
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
        if (!playerShoot.enabled)
        {
            m_playerControl.Move(20*Vector3.right, m_Jump, m_Slide, m_MoveLeft, m_MoveRight,1);
            m_Jump = false;
            m_Slide = false;
            m_MoveLeft = false;
            m_MoveRight = false;
        }
        else
        {
            m_playerControl.Move(20 * Vector3.right, false, false, false, false, 1);

        }


    }
    public bool IsJump()
    {
        return m_Jump;
    }
    public bool IsSlide()
    {
        return m_Slide;
    }
    public bool IsLeft()
    {
        return m_MoveLeft;
    }
    public bool IsRight()
    {
        return m_MoveRight;
    }
}

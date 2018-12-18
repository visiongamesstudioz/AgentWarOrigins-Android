using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShowInput : MonoBehaviour
{
    private CharacterScroller m_Hs;
    public Vector2 swipeDistance = new Vector2(2, 2);
    // How sensitive the horizontal and vertical swipe are. The higher the value the more it takes to activate a swipe
    public float swipeSensitivty = 2;
    // More than this value and the player will move into the rightmost slot.
    // Less than the negative of this value and the player will move into the leftmost slot.
    // The accelerometer value in between these two values equals the middle slot.
    private Vector2 touchStartPosition;
    private bool acceptInput=true; // ensure that only one action is performed per touch gesture

    public bool AcceptInput
    {
        get { return acceptInput; }
        set { acceptInput = value; }
    }

    void Awake()
    {
        m_Hs = GetComponent<CharacterScroller>();
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE

        if (!acceptInput)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_Hs.OnLeftClickedsScroller();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_Hs.OnRightClickedScroller();
        }

#elif UNITY_ANDROID || UNITY_IOS


       if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

    
             if (touch.phase == TouchPhase.Began)
            {
                touchStartPosition = touch.position;
                // Get movement of the finger since last frame
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            }
            else if (touch.phase == TouchPhase.Moved   && acceptInput)
            {
            if (touch.position.y > (Screen.height)/4 )
        {
                    Vector2 diff = touch.position - touchStartPosition;
                if (diff.x == 0f)
                    diff.x = 1f; // avoid divide by zero
                float verticalPercent = Mathf.Abs(diff.y / diff.x);           
             if (verticalPercent < (1 / swipeSensitivty) && Mathf.Abs(diff.x) > swipeDistance.x)
                {
                    // turn if above a turn, otherwise move horizontally
                        if(diff.x > 0){

                    m_Hs.OnLeftClickedsScroller();

                          }
                       else{

            m_Hs.OnRightClickedScroller();
                        }
                    }
                        acceptInput = false;
        }
            

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
          
         
#endif
    }
}


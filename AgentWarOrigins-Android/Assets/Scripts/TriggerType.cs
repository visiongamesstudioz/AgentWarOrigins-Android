using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerType : MonoBehaviour {

    public TriggerEnum TriggerEnum;
}
public enum TriggerEnum
{
    SwipeUp,
    SwipeDown,
    SwipeLeft,
    SwipeRight,
    Tap,
    swipeUpAndTap,
    None
}
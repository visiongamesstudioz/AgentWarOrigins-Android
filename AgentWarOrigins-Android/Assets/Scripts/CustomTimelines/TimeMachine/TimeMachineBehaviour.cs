using EndlessRunner;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class TimeMachineBehaviour : PlayableBehaviour
{
	public TimeMachineAction action;
	public Condition condition;
	public string markerToJumpTo, markerLabel;
	public float timeToJumpTo;
    public Player player;
    public GameObject ActivateArrow;
    public SwipeType swipeType;
	[HideInInspector]
	public bool clipExecuted = false; //the user shouldn't author this, the Mixer does

    public bool ConditionMet()
    {
        switch (condition)
        {
            case Condition.Always:
                return true;

            case Condition.AgentIsAlive:
                //The Timeline will jump to the label or time if a specific Platoon still has at least 1 unit alive
                if (player != null)
                {
                    return player.GetComponent<PlayerHealth>().IsDead;
                }
                else
                {
                    return false;
                }

            case Condition.Never:
            default:
                return false;
        }
    }

    public enum TimeMachineAction
	{
		Marker,
		JumpToTime,
		JumpToMarker,
		Pause,
	}

	public enum Condition
	{
		Always,
		Never,
        AgentIsAlive
	}

    public enum SwipeType
    {
        Up,
        Down,
        Left,
        Right
    }
}

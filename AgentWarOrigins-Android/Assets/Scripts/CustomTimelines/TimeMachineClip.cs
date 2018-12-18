using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

[Serializable]
public class TimeMachineClip : PlayableAsset, ITimelineClipAsset
{
	[HideInInspector]
    public TimeMachineBehaviour template = new TimeMachineBehaviour ();

	public TimeMachineBehaviour.TimeMachineAction action;
	public TimeMachineBehaviour.Condition condition;
	public string markerToJumpTo = "", markerLabel = "";
	public float timeToJumpTo = 0f;
    public ExposedReference<GameObject> ActivateArrow;
    public ExposedReference<Player> player;
    public TimeMachineBehaviour.SwipeType swipeType;
    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<TimeMachineBehaviour>.Create (graph, template);
        TimeMachineBehaviour clone = playable.GetBehaviour ();
        clone.player = player.Resolve (graph.GetResolver ());
		clone.markerToJumpTo = markerToJumpTo;
		clone.action = action;
		clone.condition = condition;
		clone.markerLabel = markerLabel;
		clone.timeToJumpTo = timeToJumpTo;
        clone.ActivateArrow = ActivateArrow.Resolve(graph.GetResolver());
        clone.swipeType = swipeType;
        return playable;
    }
}

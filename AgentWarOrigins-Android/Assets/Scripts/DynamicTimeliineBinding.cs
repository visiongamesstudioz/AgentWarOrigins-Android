using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DynamicTimeliineBinding : MonoBehaviour
{
    public GameObject Player;
    public PlayableDirector PlayerTimeline;
    public TimelineAsset timelineAsset;
    public bool autoBindTracks = true;

    // Use this for initialization
    private void Start()
    {
        if (autoBindTracks)
            BindTimelineTracks();
    }

    public void BindTimelineTracks()
    {

        timelineAsset = (TimelineAsset)PlayerTimeline.playableAsset;
        // iterate through tracks and map the objects appropriately

        foreach (var timelineAssetOuput in timelineAsset.outputs)
        {          
           PlayerTimeline.SetGenericBinding(timelineAssetOuput.sourceObject, Player);                   
        }
    }

    public void SetPlayer(GameObject player)
    {
        Player = player;
    }


    public void ApplyOffsets()
    {
        // if the PlayerTimeline is playing, timeline changes won't have an effect
        PlayerTimeline.Stop();

        var timelineAsset = PlayerTimeline.playableAsset as TimelineAsset;
        if (timelineAsset == null)
            return;

        foreach (var track in timelineAsset.GetOutputTracks())
        {
            var animTrack = track as AnimationTrack;
            // could check for a specifically named track here as well...
            if (animTrack == null)
                continue;

            var binding = PlayerTimeline.GetGenericBinding(animTrack);
            if (binding == null)
                continue;

            // the binding can be an animator or game object with an animator
            var animator = binding as Animator;
            var gameObject = binding as GameObject;
            if (animator == null && gameObject != null)
                animator = gameObject.GetComponent<Animator>();

            if (animator == null)
            {
                Debug.Log("animtor is null");
                continue;

            }

            // turn on the track offsets, and apply the current position
            animTrack.applyOffsets = true;
            animTrack.position = animator.transform.position;
            animTrack.rotation = animator.transform.rotation;
        }

        // start the PlayerTimeline. This will build the graph with embedded offsets
        PlayerTimeline.Play();
    }
}

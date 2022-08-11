using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows an audio clip to be played during an animation state.
/// </summary>
public class PlayAudioClip : StateMachineBehaviour
{
    public AudioClip clip;
    public float soundQueue;
    public float volume;
    public float loopTime;

    bool playSound = true;
    bool hasLooped = false;
    float lastTime = -1f;

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var normalizedTime = stateInfo.normalizedTime;

        if (normalizedTime > 1f && loopTime > 0)
            normalizedTime %= loopTime;

        if (lastTime >= normalizedTime)
            hasLooped = true;

        if (!playSound && hasLooped)
            playSound = true;

        if (normalizedTime >= soundQueue && playSound)
        {
            AudioSource.PlayClipAtPoint(clip, animator.transform.position, volume);
            playSound = false;
            hasLooped = false;
        }
        lastTime = normalizedTime;
    }
}

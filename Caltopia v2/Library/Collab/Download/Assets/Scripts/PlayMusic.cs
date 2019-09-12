using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : GameEvent
{
    public float delay = 30f;
    public override void Run()
    {
        StartCoroutine(PlayMusicWithDelay());
    }
    
    IEnumerator PlayMusicWithDelay()
    {
        if (!executed)
        {
            yield return new WaitForSeconds(delay);
            executed = true;
        }
        StartCoroutine(PlayAudioClips());
    }
}

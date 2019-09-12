using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent : MonoBehaviour
{
    public float runAfterSeconds = 0f;
    public bool executed;
    public GameObject targetObject;
    public AudioSource[] audioSources;
    public AudioClip[] audioClips;
    public float[] audioClipDelay;
    public abstract void Run();

    protected void PlayAudio()
    {
        float delay = 0;

        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.PlayDelayed(delay);
            delay += audioSource.clip.length;
        }
    }

    protected IEnumerator PlayAudioClips()
    {
        AudioSource audioSource = audioSources[0];

        foreach(AudioClip audioClip in audioClips)
        {
            audioSource.clip = audioClip;
            audioSource.Play();

            float delayAfter = 0;
            if (audioClipDelay.Length != 0)
            {
                delayAfter = audioClipDelay[System.Array.IndexOf(audioClips, audioClip)];
            }
            yield return new WaitForSeconds(audioClip.length + delayAfter);
        }
    }
}

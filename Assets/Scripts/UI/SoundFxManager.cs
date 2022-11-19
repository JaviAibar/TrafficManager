using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFxManager : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip closeSound;
    private AudioSource audio;
    public AudioClip[] solvedSounds;
    public AudioClip failSound;
    public AudioClip tickTackSound;
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        audio.clip = clickSound;
        audio.Play();
    }

    public void PlayCloseSound()
    {
        audio.clip = closeSound;
        audio.Play();
    }

    public void PlaySolvedSound(int id)
    {
        if (id >= 0 &&  id < solvedSounds.Length && audio.clip != solvedSounds[id])
        {
            audio.clip = solvedSounds[id];
            audio.Play();
        }
    }

    public void PlayFailSound()
    {
        audio.clip = failSound;
        audio.Play();
    }

    public void PlayTickTack()
    {
        audio.clip = tickTackSound;
        audio.Play();
    }
}

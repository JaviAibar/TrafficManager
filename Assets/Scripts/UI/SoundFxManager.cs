using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFxManager : MonoBehaviour
{
    public AudioClip clickSound;
    public AudioClip closeSound;
    private AudioSource audio;
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        
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
}

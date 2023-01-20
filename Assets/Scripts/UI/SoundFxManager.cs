using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFxManager : MonoBehaviour
{
    public GameObject audioSourcePrefab;
    public AudioClip clickSound;
    public AudioClip closeSound;
    public AudioClip[] solvedSounds;
    public AudioClip failSound;
    public AudioClip tickTackSound;

    private List<AudioClip> _playingClips;

    public AudioMixerGroup musicMixer;
    public AudioMixerGroup fxMixer;
    public AudioMixer audioMixer;

    private void Awake()
    {
        _playingClips = new List<AudioClip>();
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 0));
        audioMixer.SetFloat("SoundFxVolume", PlayerPrefs.GetFloat("SoundFxVolume", 0));
        audioMixer.SetFloat("MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 0));
    }

    private int _lastSolvedPlay = -1;
    public void PlayClickSound() => Play(clickSound);

    public void PlayCloseSound() => Play(closeSound);

    public void PlaySolvedSound(int id)
    {
        if (id >= 0 && id < solvedSounds.Length && _lastSolvedPlay != id)
        {
            _lastSolvedPlay = id;
            Play(solvedSounds[id]);
        }
    }

    public void PlayFailSound()
    {
        _lastSolvedPlay = -1;
        Play(failSound);
    }

    public void PlayTickTack() => Play(tickTackSound);

    public void Play(AudioClip clip) => StartCoroutine(PlaySound(clip));

    public IEnumerator PlaySound(AudioClip clip, float pitch = 1)
    {
        GameObject audioSourceInstance = Instantiate(audioSourcePrefab);
        AudioSource audioSourceComponent = audioSourceInstance.GetComponent<AudioSource>();
        audioSourceComponent.clip = clip;
        audioSourceComponent.pitch = pitch;
        audioSourceComponent.Play();
        while (audioSourceComponent.isPlaying)
        {
            yield return null;
        }

        Destroy(audioSourceInstance);
    }
}
using Assets.Scripts;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFxManager : MonoBehaviour
{

    [SerializeField] private GameObject audioSourcePrefab;

    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip closeSound;

    [SerializeField] private AudioClip[] solvedSounds;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip tickTackSound;

    private AudioMixer _audioMixer;

    private AudioMixerGroup _musicMixer;
    private AudioMixerGroup _fxMixer;
    private AudioMixerGroup _masterMixer;

    private readonly float _silentVol = -80;
    private float _currentVol;
    private float _previousVol;
    private int _lastSolvedSoundPlayed = -1;

    public AudioMixer AudioMixer => _audioMixer;
    public float CurrentVolume
    {
        get
        {
            _audioMixer.GetFloat(Constants.MasterVolume, out _currentVol);
            return _currentVol;
        }
    }

    private void Awake()
    {
        _audioMixer = (AudioMixer)Resources.Load("SoundManager");

        _musicMixer = _audioMixer.FindMatchingGroups(Constants.MusicGroup)[0];
        _fxMixer = _audioMixer.FindMatchingGroups(Constants.SoundFxGroup)[0];
        _masterMixer = _audioMixer.FindMatchingGroups(Constants.MasterGroup)[0];

        _audioMixer.SetFloat(Constants.MasterVolume, PlayerPrefs.GetFloat(Constants.MasterVolume, 0));
        _audioMixer.SetFloat(Constants.SoundFxVolume, PlayerPrefs.GetFloat(Constants.SoundFxVolume, 0));
        _audioMixer.SetFloat(Constants.MusicVolume, PlayerPrefs.GetFloat(Constants.MusicVolume, 0));
    }

    public void PlayClickSound() => Play(clickSound);

    public void PlayCloseSound() => Play(closeSound);

    public void PlaySolvedSound(int id)
    {
        if (IsTimeToPlay(id))
        {
            _lastSolvedSoundPlayed = id;
            Play(solvedSounds[id]);
        }
    }

    private bool IsTimeToPlay(int id) => id >= 0 && id < solvedSounds.Length && _lastSolvedSoundPlayed != id;

    public void PlayFailSound()
    {
        _lastSolvedSoundPlayed = -1;
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

    public void SwitchMuteAllSounds()
    {
        if (CurrentVolume == _silentVol)
        {
            _audioMixer.SetFloat(Constants.MasterVolume, _previousVol);
            return;
        }
        _previousVol = _currentVol;

        _audioMixer.SetFloat(Constants.MasterVolume, _silentVol);
    }
}
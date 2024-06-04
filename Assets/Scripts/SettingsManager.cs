using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    private AudioSource audioTester;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup masterMixer;
    [SerializeField] private AudioMixerGroup fxMixer;
    [SerializeField] private AudioClip musicTest;
    [SerializeField] private AudioClip fxTest;
    [SerializeField] private TMP_Dropdown languageDropdown;
    // TODO: Divide this class into 2: 1. Control UI, 2. Control Memo
    private void Awake()
    {
        audioTester = gameObject.AddComponent<AudioSource>();
    }
    private void OnEnable()
    {
        qualityDropdown.onValueChanged.AddListener(QualityChanged);
        languageDropdown.onValueChanged.AddListener(QualityChanged);
        musicVolumeSlider.onValueChanged.AddListener(MusicVolumeChanged);
        soundFXVolumeSlider.onValueChanged.AddListener(FxVolumeChanged);
        masterVolumeSlider.onValueChanged.AddListener(MasterVolumeChanged);
    }
    private void OnDisable()
    {
        qualityDropdown.onValueChanged.RemoveListener(QualityChanged);
        musicVolumeSlider.onValueChanged.RemoveListener(MusicVolumeChanged);
        soundFXVolumeSlider.onValueChanged.RemoveListener(FxVolumeChanged);
        masterVolumeSlider.onValueChanged.RemoveListener(MasterVolumeChanged);
    }

    void Start()
    {
        StartCoroutine(WaitForLanguageDropdown());

        InitQualityDropdown();
        InitResolutionDropdown();
        InitSoundSettings();
    }

    private void InitSoundSettings()
    {
        RetrieveMasterVolumeFromMemo();
        RetrieveMusicVolumeFromMemo();
        RetrieveSoundFxVolumeFromMemo();
    }

    private void RetrieveMasterVolumeFromMemo()
    {
        float masterVolume =  PlayerPrefs.GetFloat(Constants.MasterVolume, -20);
        print($"Master value from memo {masterVolume}");
        masterVolumeSlider.SetValueWithoutNotify(masterVolume);
        audioMixer.SetFloat(Constants.MasterVolume, masterVolume);
    }
    private void RetrieveMusicVolumeFromMemo()
    {
        float musicVolume = PlayerPrefs.GetFloat(Constants.MusicVolume, 0);
        musicVolumeSlider.SetValueWithoutNotify(musicVolume);
        audioMixer.SetFloat(Constants.MusicVolume, musicVolume);
    }
    private void RetrieveSoundFxVolumeFromMemo()
    {
        float soundFXVolume = PlayerPrefs.GetFloat(Constants.SoundFxVolume, 0);
        soundFXVolumeSlider.SetValueWithoutNotify(soundFXVolume);
        audioMixer.SetFloat(Constants.SoundFxVolume, soundFXVolume);
    }

    public IEnumerator WaitForLanguageDropdown()
    {
        // Wait for the localization system to initialize
        yield return LocalizationSettings.InitializationOperation;
        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        var selected = 0;

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name.Split(' ')[0]));
        }
        languageDropdown.options = options;
        languageDropdown.value = selected;
        languageDropdown.onValueChanged.AddListener(LocaleSelected);
    }

    public void InitQualityDropdown()
    {
        string[] qualities = QualitySettings.names;

        foreach (string q in qualities)
            qualityDropdown.options.Add(new TMP_Dropdown.OptionData(q));

        qualityDropdown.value = QualitySettings.GetQualityLevel();
    }
    public void InitResolutionDropdown()
    {
        List<Resolution> resolutions = Screen.resolutions.ToList();//.Where(e => e.refreshRate == 60).ToList();
        //print(resolutions.Count);
        //resolutions.Select(e => $"{e.width} x {e.height} # {e.refreshRate}").ToList().ForEach(e => print(e));
        resolutionsDropdown.AddOptions(resolutions.Select(e => $"{e.width} x {e.height} @ {e.refreshRate}Hz").ToList());
    }
    public void SetToDefault()
    {
        PlayerPrefs.DeleteKey(Constants.MasterVolume);
        PlayerPrefs.DeleteKey(Constants.MusicVolume);
        PlayerPrefs.DeleteKey(Constants.SoundFxVolume);

        InitSoundSettings();
    }
    private void MasterVolumeChanged(float vol)
    {
        print($"General vol changed to a {vol}");
        audioTester.clip = fxTest;
        audioTester.outputAudioMixerGroup = masterMixer;
        audioMixer.SetFloat(Constants.MasterVolume, vol);
        TryPlaying();
    }

    private void MusicVolumeChanged(float vol)
    {
        audioTester.clip = musicTest;
        audioTester.outputAudioMixerGroup = musicMixer;
        audioMixer.SetFloat(Constants.MusicVolume, vol);
        TryPlaying();
    }
    private void FxVolumeChanged(float vol)
    {
        audioTester.clip = fxTest;
        audioTester.outputAudioMixerGroup = fxMixer;
        audioMixer.SetFloat(Constants.SoundFxVolume, vol);
        TryPlaying();
    }
    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        PlayerPrefs.SetInt(Constants.Language, index);
    }

    private void TryPlaying()
    {
        if (!audioTester.isPlaying)
        {
            StartCoroutine(PlayAudioSeconds());
        }
    }

    private void QualityChanged(int value)
    {
        QualitySettings.SetQualityLevel(value);
    }

    public IEnumerator PlayAudioSeconds()
    {
        audioTester.volume = 0;
        audioTester.Play();
        float speed = 0.001f;
        while (audioTester.volume != 1)
        {
            audioTester.volume += speed;
            yield return null;
        }
        yield return new WaitForSeconds(3);
    }

    public void AcceptChanges()
    {
        audioMixer.GetFloat(Constants.MasterVolume, out float volumeValue);
        PlayerPrefs.SetFloat(Constants.MasterVolume, volumeValue);

        audioMixer.GetFloat(Constants.MusicVolume, out volumeValue);
        PlayerPrefs.SetFloat(Constants.MusicVolume, volumeValue);

        audioMixer.GetFloat(Constants.SoundFxVolume, out volumeValue);
        PlayerPrefs.SetFloat(Constants.SoundFxVolume, volumeValue);
        CloseMenu();
    }

    public void CloseMenu()
    {
        SceneManager.UnloadSceneAsync(Constants.SettingsMenu);
    }


}
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
    // TODO: Change fxSoundTester, it's annoying
    public AudioMixer audioMixer;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider soundFXVolumeSlider;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown resolutionsDropdown;
    private AudioSource audioTester;
    public AudioMixerGroup musicMixer;
    public AudioMixerGroup masterMixer;
    public AudioMixerGroup fxMixer;
    public AudioClip musicTest;
    public AudioClip fxTest;
    public TMP_Dropdown languageDropdown;
    // TODO: Divide this class into 2: 1. Control UI, 2. Control Memo
    private void Awake()
    {
        audioTester = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        StartCoroutine(WaitForLanguageDropdown());

        InitQualityDropdown();
        InitResolutionDropdown();
        InitOtherSettings();
    }

    private void InitOtherSettings()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0);
        soundFXVolumeSlider.value = PlayerPrefs.GetFloat("SoundFxVolume", 0);
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
        PlayerPrefs.DeleteKey("MasterVolume");
        PlayerPrefs.DeleteKey("MusicVolume");
        PlayerPrefs.DeleteKey("SoundFxVolume");

        InitOtherSettings();
    }
    private void MasterVolumeChanged(float vol)
    {
        audioTester.clip = fxTest;
        audioTester.outputAudioMixerGroup = masterMixer;
        audioMixer.SetFloat("MasterVolume", vol);
        TryPlaying();
    }

    private void MusicVolumeChanged(float vol)
    {
        audioTester.clip = musicTest;
        audioTester.outputAudioMixerGroup = musicMixer;
        audioMixer.SetFloat("MusicVolume", vol);
        TryPlaying();
    }

    private void FxVolumeChanged(float vol)
    {
        audioTester.clip = fxTest;
        audioTester.outputAudioMixerGroup = fxMixer;
        audioMixer.SetFloat("SoundFXVolume", vol);
        TryPlaying();
    }
    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        PlayerPrefs.SetInt("Language", index);
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
            print(audioTester.volume);
            yield return null;
        }
        yield return new WaitForSeconds(3);
    }

    public void CloseMenu()
    {
        SceneManager.UnloadSceneAsync("Settings Menu");
    }


}
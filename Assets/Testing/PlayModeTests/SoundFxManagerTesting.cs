using Assets.Scripts;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

public class SoundFxManagerTesting : MonoBehaviour
{
    private AudioClip clickSound;
    private AudioClip closeSound;

    private AudioClip[] solvedSounds;
    private AudioClip failSound;
    private AudioClip tickTackSound;

    // private AudioSource audioSource;
    public void CleanThis(GameObject go)
    {
        Destroy(go);
    }

    public SoundFxManager CreateDefaultSoundFxManager(ref GameObject audioManagerGO)
    {
        var soundFxManager = audioManagerGO.AddComponent<SoundFxManager>();
        audioManagerGO.AddComponent<AudioListener>();

        /*var audioSourcePrefab = new GameObject();
        audioSourcePrefab.AddComponent<AudioSource>();*/
        
        var so = new SerializedObject(soundFxManager);
        so.FindProperty("audioSourcePrefab").objectReferenceValue = Resources.Load("Prefabs/Audio Source");
        so.ApplyModifiedProperties();
        //Instantiate(audioManagerGO);
        return soundFxManager;
    }

    public void FakeAudioClips(ref SoundFxManager soundFxManager)
    {
        int DEFAULT_LENGTH = 1;
        int DEFAULT_CHANNELS = 1;
        int DEFAULT_FREQUENCY = 1000;
        bool DEFAULT_STREAM = true;

        clickSound = AudioClip.Create("DefaultName1", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM);
        closeSound = AudioClip.Create("DefaultName2", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM);

        solvedSounds = new AudioClip[] {
            AudioClip.Create("DefaultName3", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM),
            AudioClip.Create("DefaultName4", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM),
            AudioClip.Create("DefaultName5", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM),
        };
        failSound = AudioClip.Create("DefaultName6", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM);
        tickTackSound = AudioClip.Create("DefaultName7", DEFAULT_LENGTH, DEFAULT_CHANNELS, DEFAULT_FREQUENCY, DEFAULT_STREAM);


        var so = new SerializedObject(soundFxManager);

        so.FindProperty("clickSound").objectReferenceValue = clickSound;
        so.FindProperty("closeSound").objectReferenceValue = closeSound;

        SerializedProperty solvedSoundsProperty = so.FindProperty("solvedSounds");

        solvedSoundsProperty.arraySize = 3;

        solvedSoundsProperty.GetArrayElementAtIndex(0).objectReferenceValue = solvedSounds[0];
        solvedSoundsProperty.GetArrayElementAtIndex(1).objectReferenceValue = solvedSounds[1];
        solvedSoundsProperty.GetArrayElementAtIndex(2).objectReferenceValue = solvedSounds[2];

        so.FindProperty("failSound").objectReferenceValue = failSound;
        so.FindProperty("tickTackSound").objectReferenceValue = tickTackSound;

        so.ApplyModifiedProperties();
    }

    [UnityTest]
    public IEnumerator _00_MuteSwitchTest()
    {
        var audioManagerGO = new GameObject();

        var soundFxManager = CreateDefaultSoundFxManager(ref audioManagerGO);
        yield return new WaitForEndOfFrame();

        var previousVolume = soundFxManager.CurrentVolume;

        // Mute volume
        soundFxManager.SwitchMuteAllSounds();
        var currentVolume = soundFxManager.CurrentVolume;
        Assert.AreEqual(-80, currentVolume);

        // Returning previous volume
        soundFxManager.SwitchMuteAllSounds();
        currentVolume = soundFxManager.CurrentVolume;
        Assert.AreEqual(currentVolume, previousVolume);


        CleanThis(audioManagerGO);
    }

    [UnityTest]
    public IEnumerator _01_CurrentVolumeTest()
    {
        var audioManagerGO = new GameObject();

        var soundFxManager = CreateDefaultSoundFxManager(ref audioManagerGO);
        yield return new WaitForEndOfFrame();
        soundFxManager.AudioMixer.SetFloat(Constants.MasterVolume, 0);

        var currentVolume = soundFxManager.CurrentVolume;

        Assert.AreEqual(0, currentVolume);
        CleanThis(audioManagerGO);
    }

    [UnityTest]
    public IEnumerator _02_PlayClickTest()
    {
        var audioManagerGO = new GameObject();

        var soundFxManager = CreateDefaultSoundFxManager(ref audioManagerGO);
        yield return new WaitForEndOfFrame();
        FakeAudioClips(ref soundFxManager);


        soundFxManager.PlayClickSound();
        yield return new WaitForEndOfFrame();

        // Playing a sound creates a new AudioSource
        AudioSource audioSource = FindAnyObjectByType<AudioSource>();

        Assert.IsNotNull(audioSource);
        Assert.AreEqual(clickSound, audioSource.clip);
        Assert.IsTrue(audioSource.isPlaying);

        yield return new WaitWhile(() => audioSource != null);

        CleanThis(audioManagerGO);
    }


    [UnityTest]
    public IEnumerator _03_PlayCloseSoundTest()
    {
        var audioManagerGO = new GameObject();

        var soundFxManager = CreateDefaultSoundFxManager(ref audioManagerGO);
        yield return new WaitForEndOfFrame();
        FakeAudioClips(ref soundFxManager);

        soundFxManager.PlayCloseSound();
        yield return new WaitForEndOfFrame();

        // Playing a sound creates a new AudioSource
        var audioSource = FindAnyObjectByType<AudioSource>();

        Assert.IsNotNull(audioSource);
        Assert.IsTrue(audioSource.isPlaying);
        Assert.AreEqual(closeSound, audioSource.clip); 
        yield return new WaitWhile(() => audioSource != null);

        CleanThis(audioManagerGO);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundType : MonoBehaviour
{
    public soundThing soundType;
    // Update is called once per frame
    void Update()
    {
        if(soundType == soundThing.Music)
        {
            base.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
        }
        if(soundType == soundThing.SFX)
        {
            base.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SFXVolume");
        }
        if (soundType == soundThing.Voice)
        {
            base.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("VoiceVolume");
        }
    }

    public enum soundThing
    {
        Voice,
        Music,
        SFX
    }
}

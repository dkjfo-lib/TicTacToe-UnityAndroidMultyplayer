using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static Action<string> CallAudio;

    public AudioSource[] audioSources;

    void Start()
    {
        CallAudio += (s) => PlaySound(s);
        audioSources = GetComponentsInChildren<AudioSource>();
    }

    public void PlaySound(string name)
    {
        var source = audioSources.First(s => s.clip.name == name);
        source.Play();
    }
}

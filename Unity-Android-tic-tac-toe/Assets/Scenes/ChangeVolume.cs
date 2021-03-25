using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour
{
    const string volume_key = "volume";

    public AudioMixer mixer;
    public string mixerName = "Master";

    private float sliderValue;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(volume_key))
        {
            PlayerPrefs.SetFloat(volume_key, 1);
        }
        sliderValue = PlayerPrefs.GetFloat(volume_key);
        GetComponent<Slider>().value = sliderValue;
    }

    public void ChangeVolume_Master(float newVolume)
    {
        sliderValue = newVolume;
        mixer.SetFloat(mixerName, (1 - sliderValue) * -80);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(volume_key, sliderValue);
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}

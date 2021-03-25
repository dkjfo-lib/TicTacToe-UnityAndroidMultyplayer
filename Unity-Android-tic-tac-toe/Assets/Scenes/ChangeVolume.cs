using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour
{
    const string volume_key = "volume";
    public AudioSource AudioSource_Master;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(volume_key))
        {
            PlayerPrefs.SetFloat(volume_key, 1);
        }
        GetComponent<Slider>().value = PlayerPrefs.GetFloat(volume_key);
    }

    public void ChangeVolume_Master(float newVolume)
    {
        AudioSource_Master.volume = newVolume;
    }
        
    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(volume_key, AudioSource_Master.volume);
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}

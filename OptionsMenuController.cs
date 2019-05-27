using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class OptionsMenuController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_InputField nameText;
    public Slider musicSlider, fxSlider, accelSlider;

    void Start()
    {
        nameText.text = PlayerPrefs.GetString("PlayerName", "Anon");
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        fxSlider.value = PlayerPrefs.GetFloat("FXVolume", 0.75f);
        accelSlider.value = PlayerPrefs.GetFloat("Acceleration", 2f);
    }

    public void SetName(string name)
    {
        if (String.IsNullOrEmpty(name)) nameText.text = "Anon";
        else PlayerPrefs.SetString("PlayerName", name);
    }

    public void SetMusicVolume(float sliderValue)
    {
        audioMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("MusicVolume", sliderValue);
    }

    public void SetFXVolume(float sliderValue)
    {
        audioMixer.SetFloat("FXVol", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("FXVolume", sliderValue);
    }

    public void SetAcceleration(float value)
    {
        PlayerPrefs.SetFloat("Acceleration", value);
    }
}

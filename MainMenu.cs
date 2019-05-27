using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public MainMenuPlayerController player;
    public AudioMixer audioMixer;

    void Start()
    {
        // Set player's skin
        string animName = PlayerPrefs.GetString("PlayerAnim");
        if (animName == null) {
            animName = "BlueAstroAnim";
            PlayerPrefs.SetString("PlayerAnim", animName);
        }
        RuntimeAnimatorController skin = Resources.Load("Animations/" + animName) as RuntimeAnimatorController;
        player.ChangeCharacter(skin);

        // Set audio levels
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float fxVolume = PlayerPrefs.GetFloat("FXVolume", 0.75f);
        audioMixer.SetFloat("MusicVol", Mathf.Log10(musicVolume) * 20);
        audioMixer.SetFloat("FXVol", Mathf.Log10(fxVolume) * 20);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}

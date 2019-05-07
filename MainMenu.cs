using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

  public MainMenuPlayerController player;

  void Start() {
      // Set player's skin
      string animName = "Animations/" + PlayerPrefs.GetString("PlayerAnim", "BlueAstroAnim");
      RuntimeAnimatorController skin = Resources.Load(animName) as RuntimeAnimatorController;
      player.ChangeCharacter(skin);
  }

  public void StartGame() {
      SceneManager.LoadScene(1);
  }

  public void QuitGame() {
      Application.Quit();
  }
}

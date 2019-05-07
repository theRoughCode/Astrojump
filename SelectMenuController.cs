using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMenuController : MonoBehaviour {

  public MainMenuPlayerController mainPlayer;
  public RuntimeAnimatorController[] anims;
  public MainMenuPlayerController[] avatars;
  int currIdx;

  void Start() {
      string animName = PlayerPrefs.GetString("PlayerAnim");

      if (animName == null) currIdx = 0;

      // Turn off all jumps excepted selected
      for (int i = 0; i < avatars.Length; i++) {
          MainMenuPlayerController avatar = avatars[i].GetComponent<MainMenuPlayerController>();
          avatar.SetInitHeight();
          if (anims[i].name == animName) {
              currIdx = i;
          } else avatar.SetJump(false);
      }
  }

  public void SelectCharacter(int index) {
      if (currIdx == index) return;

      // Turn off jump for previous selection
      avatars[currIdx].GetComponent<MainMenuPlayerController>().SetJump(false);

      // Toggle jump for new selection
      avatars[index].GetComponent<MainMenuPlayerController>().SetJump(true);
      currIdx = index;
  }

  public void ConfirmCharacter() {
      mainPlayer.ChangeCharacter(anims[currIdx]);

      // Set in player prefs to be accessed in game scene
      PlayerPrefs.SetString("PlayerAnim", anims[currIdx].name);
  }
}

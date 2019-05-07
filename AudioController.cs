using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour {

  // Soundtrack and respective difficulty level triggers
  public AudioClip[] soundtracks;
  public AudioClip gameOverClip;
  int[] scoreTrigger = { 0, 0, 1, 2 };
  int currTrack = 0;
  bool paused = false;

  public AudioSource backgroundMusic;

	// Use this for initialization
	void Start () {
    backgroundMusic.clip = soundtracks[0];
    backgroundMusic.Play();
	}

  public void Pause() {
      if (!paused) {
          backgroundMusic.Pause();
          paused = true;
      }
  }

  public void Resume() {
      if (paused) {
          backgroundMusic.Play();
          paused = false;
      }
  }

  public void SwitchMusic(int level) {
      int newTrack = scoreTrigger[level];
      if (newTrack == currTrack) return;
      currTrack = newTrack;
      backgroundMusic.Stop();
      backgroundMusic.clip = soundtracks[newTrack];
      backgroundMusic.Play();
  }

  public void Restart() {
      currTrack = 0;
      backgroundMusic.Stop();
      backgroundMusic.clip = soundtracks[0];
      backgroundMusic.loop = true;
      backgroundMusic.Play();
  }

  public void GameOver() {
      currTrack = -1;
      backgroundMusic.Stop();
      backgroundMusic.clip = gameOverClip;
      backgroundMusic.loop = false;
      backgroundMusic.Play();
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPlayerController : MonoBehaviour {

  private Rigidbody2D rb;

  private float jumpVelocity = 5f;
  private float initHeight = 1.575f;

	// Use this for initialization
	void Awake () {
      rb = GetComponent<Rigidbody2D>();
	}

  void OnCollisionEnter2D(Collision2D collision)
  {
      rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
  }

  public void SetInitHeight() {
      rb.position = new Vector2(rb.position.x, initHeight);
  }

  public void ChangeCharacter(RuntimeAnimatorController anim) {
      this.GetComponent<Animator>().runtimeAnimatorController = anim;
  }

  // Used for character selection
  public void SetJump(bool on) {
      if (on) {
        jumpVelocity = 5f;
        rb.position = new Vector2(rb.position.x, rb.position.y + 1.3f);
        rb.gravityScale = 1.0f;
      } else {
        jumpVelocity = 0f;
        rb.position = new Vector2(rb.position.x, initHeight);
        rb.velocity = new Vector2(0, 0);
        rb.gravityScale = 0f;
      }
  }
}

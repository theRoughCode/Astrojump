using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelCollider : MonoBehaviour {

    public AudioClip bounceClip;

    void OnCollisionEnter2D(Collision2D col) {
    	AudioSource.PlayClipAtPoint(bounceClip, Camera.main.transform.position);
    }
}

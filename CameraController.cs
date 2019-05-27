using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Camera cam;

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void MoveCamera(float dist)
    {
        cam.transform.Translate(0, dist, 0);
    }

    public void ResetCamera() {
      Vector3 pos = cam.transform.position;
      pos.y = 0;
      cam.transform.position = pos;
    }
}

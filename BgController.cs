using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgController : MonoBehaviour
{
    public GameObject[] bgArr;
    public bool[] willLoop;
    private float[] heightArr;
    private bool[] activated;
    float scrollSpeed = 0.27f;

    void Start()
    {
        heightArr = new float[bgArr.Length];
        activated = new bool[bgArr.Length];

        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        pos.x = 0;

        // Get screen dimensions
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(cameraHeight / Screen.height * Screen.width, cameraHeight);

        for (int i = 0; i < bgArr.Length; i++)
        {
           // Scale to fit screen size
           Vector3 spriteSize = bgArr[i].GetComponent<Renderer>().bounds.size;
           bgArr[i].transform.localScale = new Vector3(1, 1, 1);
           Vector3 scale = bgArr[i].transform.localScale * cameraSize.x / spriteSize.x;
           scale.z = 1;
           bgArr[i].transform.localScale = scale;
           heightArr[i] = spriteSize.y * scale.y;
        }

        Initialize();
    }

    // Position backgrounds
    public void Initialize()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        pos.x = 0;

        for (int i = 0; i < bgArr.Length; i++)
        {
           activated[i] = true;

           // Set initial position
           pos.y += heightArr[i] / 2;
           bgArr[i].transform.position = pos;
           pos.y += heightArr[i] / 2;
        }
    }

    // Moves al backgrounds
    public void Scroll(float scrollHeight)
    {
        float deltaY = scrollHeight * (1.0f - scrollSpeed);
        for (int i = 0; i < bgArr.Length; i++)
        {
            // if background is not activated, no need to move it
            if (!activated[i]) continue;

            // Move background up
            bgArr[i].transform.Translate(Vector3.up * deltaY);

            // Check if off screen
            Vector3 screenPos = Camera.main.ScreenToWorldPoint(Vector3.zero);
            if (bgArr[i].transform.position.y <= screenPos.y - heightArr[i] / 2)
            {
                if (!willLoop[i]) activated[i] = false;  // no need to update position
                else bgArr[i].transform.Translate(Vector3.up * heightArr[i] * 2);  // reset position
            }

        }
    }
}

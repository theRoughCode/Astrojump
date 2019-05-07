using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangePanel : MonoBehaviour
{

    // Boundaries of motion (world coords)
    private float minBoundX, maxBoundX;
    private float minX, maxX;
    private float travelDist;

    // Horizontal speed of panel
    private float speed = 0.8f;

    // Reference to OrangePanel Rigidbody
    private Rigidbody2D rb;

    void Awake()
    {
        // Calculate screen boundary in world coords
        Camera cam = Camera.main;
        minBoundX = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)).x;
        maxBoundX = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane)).x;

        // Travel distance = 20% of screen width
        travelDist = cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.2f, 0, cam.nearClipPlane)).x - minBoundX;

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change directions if at end of boundary
        if (rb.position.x >= maxX && rb.velocity.x > 0) rb.velocity = new Vector2(-speed, 0);
        else if (rb.position.x <= minX && rb.velocity.x < 0) rb.velocity = new Vector2(speed, 0);
    }

    // Called whenever a new OrangePanel is placed to set direction and placement
    public void Initialize()
    {
        // Set max boundary
        float panelWidth = GetComponent<Renderer>().bounds.size.x;
        float offset = (panelWidth + travelDist) / 2;
        float middleX = Random.Range(minBoundX + offset, maxBoundX - offset);
        minX = middleX - travelDist / 2;
        maxX = middleX + travelDist / 2;

        // Randomize initial direction (-1 or 1)
        rb.velocity = new Vector2((Random.Range(0, 2) * 2 - 1) * speed, 0);

        // Randomize initial position
        rb.position = new Vector2(Random.Range(minX, maxX), rb.position.y);
    }
}

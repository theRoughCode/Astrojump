using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurplePanel : MonoBehaviour
{
    private float minInterval = 2.0f;
    private float maxInterval = 5.0f;
    private float interval, minX, maxX;

    // Reference to OrangePanel Rigidbody
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(float left, float right)
    {
        minX = left;
        maxX = right;
        interval = Random.Range(minInterval, maxInterval);

        // Cancel previous invokes
        CancelInvoke();

        // Reposition panel every interval
        InvokeRepeating("updatePosition", interval, interval);
    }

    // Updates panel's position every interval (seconds)
    private void updatePosition()
    {
        Vector3 pos = rb.position;
        pos.x = Random.Range(minX, maxX);
        rb.position = pos;
    }
}

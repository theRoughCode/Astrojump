using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedPanel : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        gameObject.SetActive(false);
    }
}
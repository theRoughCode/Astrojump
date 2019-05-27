using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public TextMeshProUGUI rankText, nameText, scoreText;

    public void Initialize(int rank, string name, string score)
    {
        rankText.SetText(rank.ToString());
        nameText.SetText(name);
        scoreText.SetText(score);
        gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class Entry
{
    public string name;
    public int score;
}

public class Leaderboards : MonoBehaviour
{
    public Card[] cardArr;
    public Card userCard;
    public TextMeshProUGUI loadingText;

    void Start()
    {
        loadingText.enabled = true;
        StartCoroutine(RetrieveScores());
    }

    IEnumerator RetrieveScores()
    {
        loadingText.enabled = true;
        UnityWebRequest www = UnityWebRequest.Get("https://astrojumpgame.firebaseio.com/leaderboards.json?orderBy=\"score\"&limitToLast=5");
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            JSONObject obj = new JSONObject(www.downloadHandler.text);
            UpdateBoard(obj);
        }
    }

    void UpdateBoard(JSONObject json)
    {
        loadingText.enabled = false;
        Entry[] entries = new Entry[json.list.Count];
        for (int i = 0; i < json.list.Count; i++) {
          JSONObject entry = (JSONObject)json.list[i];
          string name = entry.list[0].str;
          int score = (int) entry.list[1].n;
          entries[i] = new Entry();
          entries[i].name = name;
          entries[i].score = score;
        }
        Array.Sort(entries, delegate(Entry x, Entry y) { return -x.score.CompareTo(y.score); });

        for (int i = 0; i < entries.Length; i++) {
          cardArr[i].Initialize(i + 1, entries[i].name, entries[i].score.ToString());
        }

        int hiscore = PlayerPrefs.GetInt("hiscore", 0);
        userCard.Initialize(0, "You", hiscore.ToString());
    }
}

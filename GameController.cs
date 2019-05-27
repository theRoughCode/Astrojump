using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using TMPro;

public class GameController : MonoBehaviour
{

    public Text scoreText;

    // Access to player (for restarting)
    private PlayerController player;

    // Control panels
    public PanelController panelController;

    // Access camera to move
    public CameraController cam;

    // Access background controller to move
    BgController bgController;

    // Access AudioManager
    public AudioController audioController;

    // Draw High Score line
    public LineRenderer hiscoreLine;
    public TextMeshPro hiscoreText;

    // UI Elements
    public GameObject HUD, PauseMenu, GameOverMenu;
    // Pause Menu
    public TextMeshProUGUI pauseScoreText;
    // Game Over Meny
    public TextMeshProUGUI newHiscoreText, gameOverHiscoreText, gameOverScoreText;

    // Difficulty settings
    private int[] levels = { 0, 1500, 4000, 15000 };
    private int currLevel = 0;

    public int score = 0;

    // Tracks if game is paused
    private bool isGamePaused = false;

    // Score calculation
    private float initHeight = -1.5f;
    private float maxHeight = -1.5f;
    private float scoreMultiplier = 200;

    // Use this for initialization
    void Start()
    {
        scoreText.text = "Score: " + score;

        bgController = gameObject.GetComponent<BgController>();

        // Spawn player
        GameObject playerObj = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
        player = playerObj.GetComponent<PlayerController>();
        player.name = "Player";
        player.Initialize(gameObject.GetComponent<GameController>(), initHeight);

        // Spawn initial panels
        panelController.Spawn(currLevel);

        // Set high score line
        setHiscoreLine();

        // Ensure screen never times out
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1) Pause();
            else Unpause();
        }
    }

    void OnApplicationPause(bool paused)
    {
        if (paused) Pause();
    }

    public void Pause()
    {
        if (isGamePaused) return;
        isGamePaused = true;
        Time.timeScale = 0;
        audioController.Pause();
        pauseScoreText.text = "SCORE: " + score;
        HUD.SetActive(false);
        PauseMenu.SetActive(true);
    }

    public void Unpause()
    {
        if (!isGamePaused) return;
        isGamePaused = false;
        Time.timeScale = 1;
        audioController.Resume();
        HUD.SetActive(true);
        PauseMenu.SetActive(false);
    }

    public void Restart()
    {
        // If paused, unpause first
        if (Time.timeScale == 0 || isGamePaused)
        {
            Time.timeScale = 1;
            isGamePaused = false;
        }

        // Re-initialize vars
        currLevel = 0;
        maxHeight = initHeight;
        score = 0;

        scoreText.text = "Score: " + score;

        // Reset camera view
        cam.ResetCamera();

        // Reset background
        bgController.Initialize();

        // If player is destroyed, instantiate it
        if (player == null)
        {
            GameObject playerObj = Instantiate(Resources.Load("Prefabs/Player")) as GameObject;
            player = playerObj.GetComponent<PlayerController>();
            player.name = "Player";
        }

        // Initialize player
        player.Initialize(gameObject.GetComponent<GameController>(), initHeight);

        // Set high score line
        setHiscoreLine();

        // Clear old panels
        panelController.Restart();

        // Spawn initial panels
        panelController.Spawn(currLevel);

        // Play music
        audioController.Restart();
    }

    public void UpdateHeight(float height)
    {
        if (height <= maxHeight) return;

        // Update score
        setScore((int)((height - initHeight) * scoreMultiplier));

        // Update difficulty level
        if (currLevel < levels.Length - 1 && score >= levels[currLevel + 1])
        {
            currLevel++;
            audioController.SwitchMusic(currLevel);
        }

        // Move camera
        cam.MoveCamera(height - maxHeight);

        // Move background
        bgController.Scroll(height - maxHeight);

        maxHeight = height;
    }

    // Called when player is falling
    public void OnDescent()
    {
        if (panelController.CanSpawnNext()) panelController.Spawn(currLevel);

        // Check for panels to despawn
        panelController.Despawn();
    }

    // Called when player is dead
    public void GameOver()
    {
        int hiscore = PlayerPrefs.GetInt("hiscore", 0);
        if (score > hiscore)
        {
            // Save the new hiscore
            PlayerPrefs.SetInt("hiscore", score);
            newHiscoreText.enabled = true;
            gameOverHiscoreText.enabled = false;
            StartCoroutine(updateHiscore(score));
        }
        else if (hiscore == 0)
        {
            newHiscoreText.enabled = false;
            gameOverHiscoreText.enabled = false;
        }
        else
        {
            newHiscoreText.enabled = false;
            gameOverHiscoreText.enabled = true;
            gameOverHiscoreText.text = "HIGH SCORE: " + hiscore;
        }
        gameOverScoreText.text = "SCORE: " + score;

        // Destroy player
        player.KillPlayer();

        // Stop background music and play "game over" audio
        audioController.GameOver();

        // Switch UI
        HUD.SetActive(false);
        GameOverMenu.SetActive(true);

        isGamePaused = true;
    }

    // Updates high score on leaderboards
    IEnumerator updateHiscore(int score)
    {
        JSONObject entry = new JSONObject(JSONObject.Type.OBJECT);
        entry.AddField("name", PlayerPrefs.GetString("PlayerName", "Anon"));
        entry.AddField("score", score);

        var request             = new UnityWebRequest("https://astrojumpgame.firebaseio.com/leaderboards.json", "POST");
        byte[] bodyRaw          = Encoding.UTF8.GetBytes(entry.ToString());
        request.uploadHandler   = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            Debug.Log(request.downloadHandler.text);
        }
    }

    void setScore(int newScore)
    {
        score = newScore;

        // Update the score text
        scoreText.text = "Score: " + score;
    }

    // Set high score line
    void setHiscoreLine()
    {
        int hiscore = PlayerPrefs.GetInt("hiscore", 0);
        if (hiscore > 0)
        {
            float hiscoreY = hiscore / scoreMultiplier + initHeight;
            Camera cam = Camera.main;
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0.5f));
            pos.y = hiscoreY;
            hiscoreLine.SetPosition(0, pos);
            pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0.5f));
            pos.y = hiscoreY;
            hiscoreLine.SetPosition(1, pos);
            hiscoreLine.material = new Material(Shader.Find("Sprites/Default"));
            hiscoreLine.sortingLayerName = "Hiscore";
            hiscoreLine.startColor = Color.yellow;
            hiscoreLine.endColor = Color.yellow;

            // Set high score text
            pos = hiscoreText.transform.position;
            pos.y = hiscoreY + 0.3f;
            hiscoreText.transform.position = pos;
            hiscoreText.color = Color.yellow;
            hiscoreText.gameObject.SetActive(true);
        }
        else
        {
            hiscoreLine.gameObject.SetActive(false);
            hiscoreText.gameObject.SetActive(false);
        }
    }

    public void MainMenu()
    {
        // If player has not been destroyed, destroy it
        if (player != null) player.KillPlayer();
        // If game was paused (from pause menu), unpause
        if (Time.timeScale == 0) Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}

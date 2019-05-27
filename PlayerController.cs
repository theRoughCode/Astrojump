using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    // Audio
    private AudioSource audioSource;
    public AudioClip bounceClip;

    // Access GameController for score
    private GameController gameController;

    private float jumpVelocity;
    private float acceleration;
    private float tiltVelocity = 2.0f;
    private float speed;

    // Boundaries
    private float minX, maxX;

    // Used to track descent
    private bool isDescending;

    // Check if initialized
    private bool isInitialized = false;

    // This will be a reference to the RigidBody2D
    // component in the Player object
    private Rigidbody2D rb;

    // Reference to BoxCollider2D
    private BoxCollider2D coll;

    // This is a reference to the Animator component
    private Animator anim;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();

        // Set player's skin
        string animName = "Animations/" + PlayerPrefs.GetString("PlayerAnim", "BlueAstroAnim");
        RuntimeAnimatorController skin = Resources.Load(animName) as RuntimeAnimatorController;
        anim.runtimeAnimatorController = skin;

        // Calculate boundaries
        Camera cam = Camera.main;
        float charWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        minX = pos.x - charWidth / 2;
        pos = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0, cam.nearClipPlane));
        maxX = pos.x + charWidth / 2;
    }

    public void Initialize(GameController gameCon, float initHeight)
    {
        // Initialize variables
        rb.position = new Vector2(0, initHeight);
        acceleration = PlayerPrefs.GetFloat("Acceleration", 2f);
        speed = 2.5f * acceleration;
        jumpVelocity = 5.5f;
        isDescending = true;
        gameController = gameCon;

        isInitialized = true;
    }

    // We use FixedUpdate to do all the animation work
    void FixedUpdate()
    {
        // Ensure all vars have been initialized
        if (!isInitialized) return;

        // Check if game over
        float minY = Camera.main.ScreenToWorldPoint(Vector3.zero).y;
        if (rb.position.y < minY)
        {
            gameController.GameOver();
            return;
        }

        // Get the extent to which the player is currently pressing left or right
        float h = Input.acceleration.x * tiltVelocity;
        if (h == 0) h = Input.GetAxis("Horizontal");  // If on PC

        rb.velocity = new Vector2(h * speed, rb.velocity.y);

        anim.SetFloat("x_dir", rb.velocity.x);
        anim.SetFloat("y_dir", rb.velocity.y);

        if (rb.velocity.y > 0) coll.enabled = false;
        else coll.enabled = true;

        // Update score, move camera, etc.
        gameController.UpdateHeight(rb.position.y);

        // Triggered at height of jump
        if (!isDescending && rb.velocity.y < 0)
        {
            gameController.OnDescent();
            isDescending = true;
        }
        else if (isDescending && rb.velocity.y > 0)
        {
            isDescending = false;
        }

        // Check if player is outside of boundaries
        if (rb.position.x < minX && rb.velocity.x < 0)
        {
            Vector3 pos = rb.position;
            pos.x = maxX;
            rb.position = pos;
        }
        else if (rb.position.x > maxX && rb.velocity.x > 0)
        {
            Vector3 pos = rb.position;
            pos.x = minX;
            rb.position = pos;
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);

        // Play sound
        if (collision.gameObject.tag == "Panel") audioSource.PlayOneShot(bounceClip, 1f);
    }

    public void KillPlayer()
    {
        Destroy(gameObject);
    }
}

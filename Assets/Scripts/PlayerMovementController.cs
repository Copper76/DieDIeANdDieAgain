using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovementController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public SpriteRenderer sr;
    public GameObject corpse;
    public GameObject environment;
    public float speed;
    public float maxJumpHeight;
    public Vector3 respawnPoint;
    public int lives;
    public TMP_FontAsset teletactile;
    public TextMeshProUGUI scoreTMP;
    public TextMeshProUGUI lifeTMP;
    public GameObject collectibles;
    public GameObject leftuplight;
    public GameObject rightuplight;
    public GameObject bottomleftlight;
    public GameObject bottomrightlight;
    public AudioSource audioSource;
    public List<AudioClip> sfxClips;
    public GameObject map;
    public GameObject ui;
    public GameObject menu;
    public GameObject tutorialText;

    private float horizontal;
    private float vertical;
    private int scoreNum;
    private bool isGrounded;
    private int totalCollectible;
    private RectTransform rectTransform;
    private bool disableControl;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        speed = 10.0f;
        maxJumpHeight = 10.0f;
        horizontal = 0.0f;
        vertical = 0.0f;
        isGrounded = false;
        scoreTMP.text = "Score:0";
        updateLife();
        scoreNum = 0;
        totalCollectible = 0;
        disableControl = false;

        audioSource = GetComponent<AudioSource>();

        //initiate all collectibles
        foreach (Transform t in collectibles.transform)
        {
            t.gameObject.SetActive(true);
            if (t.gameObject.tag == "Collectible")
            {
                totalCollectible += 1;
            }
            if (t.gameObject.layer == 7)//7 is respawn layer
            {
                respawnPoint = t.gameObject.transform.position + (new Vector3(0,0,-0.1f));
            }
        }
    }

    void updateLife()
    {
        string text = "Life=";
        for (int i = 0; i < lives; i++)
        {
            text += "[]";//It's a square!!!
        }
        lifeTMP.text = text;
    }

    void updateCollectibles()
    {
        foreach (Transform t in collectibles.transform)
        {
            if (t.gameObject.tag == "Collectible" || t.gameObject.tag == "Extralife")
            {
                if (t.gameObject.GetComponent<CollectibleMovement>().start == lives)
                {
                    t.gameObject.SetActive(true);
                }
                if (t.gameObject.GetComponent<CollectibleMovement>().end == lives)
                {
                    Destroy(t.gameObject);
                }
            }
        }
    }

    GameObject instantiateText(string message, int fontSize, Vector3 localPos, Vector2 size)
    {
        GameObject textGO = new GameObject();
        textGO.transform.parent = tutorialText.transform.parent;//find the right hierarchy
        // textGO.AddComponent<Text>();
        textGO.AddComponent<TextMeshProUGUI>();

        // Set Text component properties.
        /*
        Text text = textGO.GetComponent<Text>();
        text.text = message;
        text.font = arial;
        text.fontSize = fontSize;
        */
        TextMeshProUGUI textTMP = textGO.GetComponent<TextMeshProUGUI>();
        textTMP.text = message;
        textTMP.font = teletactile;
        textTMP.fontSize = fontSize;

        rectTransform = textTMP.GetComponent<RectTransform>();
        rectTransform.localPosition = localPos;
        rectTransform.sizeDelta = size;
        return textGO;
    }

    //generate a corpse where he died
    void dummy()
    {
        GameObject deadPlayer = Instantiate(corpse, this.transform.position, Quaternion.identity);
        deadPlayer.transform.parent = environment.transform;
    }

    //moves the player to a designated respawn point
    void die()
    {
        audioSource.PlayOneShot(sfxClips[0]);
        if (lives > 0)
        {
        //var trialRespawnPos = this.transform.position + respawnPoint - this.transform.position + new Vector3(0, sr.bounds.size.y);
        var trialRespawnPos = respawnPoint - this.transform.position;
        /*
        
        bool isCurrentlyColliding = false;
        do
        {
            
            List<Collider2D> colliders = new List<Collider2D>();
            rb.GetAttachedColliders(colliders);
            foreach (Collider2D collider in colliders)
            {
                Vector3 closest = collider.ClosestPoint(trialRespawnPos);
                if (closest == trialRespawnPos) // the closest point is equal to the point within the bounds, if already inside
                {
                    isCurrentlyColliding = true;
                    trialRespawnPos.y = Mathf.Max(closest.y + sr.bounds.size.y, trialRespawnPos.y); // set it to the highest point not currently overlapping a collider
                }
            }
        }while(!isCurrentlyColliding);
        */
        this.transform.Translate(trialRespawnPos);
        this.rb.velocity = new Vector2(0f, 0f);
        lives -= 1;
        updateLife();
        updateCollectibles();
        }
        else
        {
            disableControl = true;
            //give menu
        }
    }

    //put a dummy then die, applicable to anything other than falling
    void respawn()
    {
        dummy();
        die();
        audioSource.PlayOneShot(sfxClips[3]);
    }

    //collects the collectibles with this
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Collectible")
        {
            Destroy(other.gameObject);
            scoreNum += 100;
            scoreTMP.text = "Score:" + scoreNum;
            audioSource.PlayOneShot(sfxClips[1]);
        }

        if (other.gameObject.tag == "Extralife")
        {
            Destroy(other.gameObject);
            lives += 1;
            updateLife();
            audioSource.PlayOneShot(sfxClips[1]);
        }

        if (other.gameObject.tag == "Tutorial")
        {
            other.gameObject.GetComponent<PopUp>().show();
        }

        if (other.gameObject.layer == 6)
        {
            die();
        }

        if (other.gameObject.tag == "Finish")
        {
            Debug.Log("you've reached the finish line");
            disableControl = true;
            int collectedCollectible = 0;
            foreach (Transform t in collectibles.transform)
            {
                if (t.gameObject.tag == "Collectible")
                {
                    collectedCollectible += 1;
                }
            }
            collectedCollectible = totalCollectible - collectedCollectible;
            instantiateText("You have collected " + collectedCollectible + "/" + totalCollectible + " collectibles in this level", 48, new Vector3(0, 0, 0), new Vector2(1000, 100));
            //victory code
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Tutorial")
        {
            other.gameObject.GetComponent<PopUp>().hide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* just some debug logging
        if (rb.velocity.sqrMagnitude <= 0.05 && Keyboard.current.IsPressed())
        {
            Debug.LogError(string.Format("Frame {0} : Stuck? vel {1}, drag = {2}, isGrounded = {3}, position = {4}", Time.frameCount, rb.velocity.ToString(), rb.drag, ec.IsTouchingLayers(LayerMask.GetMask("Ground")), gameObject.transform.position.ToString()));
        }
        */
    }

    void FixedUpdate()
    {
        if (!map.activeInHierarchy && !disableControl && !menu.activeInHierarchy)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        if (rb.velocity.x > 0)
        {
            leftuplight.SetActive(false);
            bottomleftlight.SetActive(!isGrounded);
            rightuplight.SetActive(true);
            bottomrightlight.SetActive(true);
        }
        else if (rb.velocity.x < 0)
        {
            leftuplight.SetActive(true);
            bottomleftlight.SetActive(true);
            rightuplight.SetActive(false);
            bottomrightlight.SetActive(!isGrounded);
        }
        else
        {
            leftuplight.SetActive(false);
            bottomleftlight.SetActive(!isGrounded);
            rightuplight.SetActive(false);
            bottomrightlight.SetActive(!isGrounded);
        }
        // work out the player location/if they're grounded
        Bounds colliderBounds = bc.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0f, 0f);

        //check player is grounded (not used atm)
        Collider2D colliders = Physics2D.OverlapBox(groundCheckPos, new Vector3(colliderBounds.size.x * 0.9f, 0.1f, 0f), 0.0f, LayerMask.GetMask("Ground"));//3 is set to ground
        //check if player main collider is in the list of overlapping colliders
        if (colliders != null)
        {
            isGrounded = true;
        }

        //Debug.Log(string.Format("Grounded: {0} on frame {1}", isGrounded, Time.frameCount));

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !map.activeInHierarchy && !disableControl && !menu.activeInHierarchy)
        {
            Debug.Log("Jumping");
            rb.velocity = new Vector2(rb.velocity.x, maxJumpHeight);
            isGrounded = false;
        }

        if (Keyboard.current.ctrlKey.wasPressedThisFrame && lives > 0 && !map.activeInHierarchy && !disableControl && !menu.activeInHierarchy)
        {
            if (bc.IsTouchingLayers(LayerMask.GetMask("Respawn")))
            {
                GameObject textGO = instantiateText("You cannot respawn in the portal", 48, new Vector3(0, -Screen.width / 4, 0), new Vector2(1000, 100));
                Destroy(textGO, 3.0f);
            }
            else
            {
                respawn();
            }
        }

        if (bc.IsTouchingLayers(LayerMask.GetMask("Lava")) && !disableControl && !menu.activeInHierarchy && !map.activeInHierarchy)
        {
            if (lives > 0)
            {
                respawn();
            }
            else
            {
                die();
            }
        }

        if (Keyboard.current.mKey.wasPressedThisFrame && !disableControl && !menu.activeInHierarchy)
        {
            map.SetActive(!map.activeInHierarchy);
            ui.SetActive(!map.activeInHierarchy);
            tutorialText.SetActive(!map.activeInHierarchy);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            menu.SetActive(!menu.activeInHierarchy);
            tutorialText.SetActive(!menu.activeInHierarchy);
        }

        if (map.activeInHierarchy && !disableControl)
        {
            map.transform.position = new Vector3(map.transform.position.x + (horizontal), map.transform.position.y + (vertical), map.transform.position.z);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && !map.activeInHierarchy)
        {
            audioSource.PlayOneShot(sfxClips[2]);
            Debug.Log(string.Format("Jump action called at {0}", Time.frameCount));
        }
    }
}

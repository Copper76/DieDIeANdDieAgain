using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovementController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public GameObject corpse;
    public GameObject environment;
    public float speed;
    public float maxJumpHeight;
    public int lives;
    public TMP_FontAsset teletactile;
    public TextMeshProUGUI scoreTMP;
    public TextMeshProUGUI lifeTMP;
    public TextMeshProUGUI finalText;
    public GameObject collectibles;
    public GameObject leftuplight;
    public GameObject rightuplight;
    public GameObject bottomleftlight;
    public GameObject bottomrightlight;
    public List<AudioClip> sfxClips;
    public GameObject map;
    public GameObject ui;
    public GameObject menu;
    public GameObject tutorialText;
    public string nextSceneName;

    private Vector3 respawnPoint;
    private AudioSource audioSource;
    private float horizontal;
    private float vertical;
    private int scoreNum;
    private bool isGrounded;
    private int totalCollectible;
    private RectTransform rectTransform;
    private bool disableControl;
    private AsyncOperation asyncLoad;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        //speed = 10f;
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

    IEnumerator LoadLevel(string nextSceneName)
    {
        yield return new WaitForSeconds(3);

        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + asyncLoad.progress);
            yield return null;
        }
    }

    IEnumerator InstaLoadLevel(string nextSceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + asyncLoad.progress);
            yield return null;
        }
    }

    public void toggleMenu()
    {
        menu.SetActive(!menu.activeInHierarchy);
        tutorialText.SetActive(!menu.activeInHierarchy);
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
        var trialRespawnPos = respawnPoint - this.transform.position;
        this.transform.Translate(trialRespawnPos);
        this.rb.velocity = new Vector2(0f, 0f);
        lives -= 1;
        updateLife();
        updateCollectibles();
        }
        else
        {
            disableControl = true;
            var trialRespawnPos = respawnPoint - this.transform.position;
            this.transform.Translate(trialRespawnPos);
            this.rb.velocity = new Vector2(0f, 0f);
            finalText.text = "You have failed to accomplish the trial, the experiment will restart in 3 seconds.";
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().name));
            asyncLoad.allowSceneActivation = true;
        }
    }

    //put a dummy then die, applicable to anything other than falling
    void respawn()
    {
        dummy();
        die();
        audioSource.PlayOneShot(sfxClips[3]);
    }

    void Update()
    {
        
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
            rb.velocity = new Vector2(0,0);
            this.transform.Translate(other.gameObject.transform.position - this.transform.position + (new Vector3(0, -0.5f, -0.1f)));
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
            finalText.text = "You have collected " + collectedCollectible + "/" + totalCollectible + " collectibles in this level";
            StartCoroutine(LoadLevel(nextSceneName));
            asyncLoad.allowSceneActivation = true;
        }
        /**
        if (other.gameObject.layer == 8)
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
        **/
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Tutorial")
        {
            other.gameObject.GetComponent<PopUp>().hide();
        }
    }

    void FixedUpdate()
    {
        if (!map.activeInHierarchy && !disableControl && !menu.activeInHierarchy)
        {
            //rb.AddForce(new Vector2(horizontal * speed,0));
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

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded && !map.activeInHierarchy && !disableControl && !menu.activeInHierarchy)
        {
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

        if (Keyboard.current.rKey.wasPressedThisFrame && !disableControl && !menu.activeInHierarchy)
        {
            Debug.Log("R" + SceneManager.GetActiveScene().name);
            StartCoroutine(InstaLoadLevel(SceneManager.GetActiveScene().name));
            asyncLoad.allowSceneActivation = true;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            toggleMenu();
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
        }
    }
}

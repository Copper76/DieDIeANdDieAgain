using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    public Text score;
    public Text life;
    public GameObject collectibles;

    private float horizontal;
    private float vertical;
    private int scoreNum;

    public bool isGrounded;

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
        score.text = "Score:0";
        updateLife();
        updateCollectibles();
        scoreNum = 0;
    }

    void updateLife()
    {
        string text = "Life=";
        for (int i = 0; i < lives; i++)
        {
            text += "\u2610";//It's a square!!!
        }
        life.text = text;
    }

    void updateCollectibles()
    {
        foreach (Transform t in collectibles.transform)
        {
            if (t.gameObject.GetComponent<CollectibleMovement>().life == lives)
            {
                t.gameObject.SetActive(true);
            }
            else
            {
                t.gameObject.SetActive(false);
            }
        }
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
        // if (lives > 0)
        // {
        var trialRespawnPos = this.transform.position + respawnPoint - this.transform.position + new Vector3(0, sr.bounds.size.y);
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
            this.rb.velocity = new Vector2(0f,0f);
            lives -= 1;
            updateLife();
            updateCollectibles();
        // }
        // else
        // {
        //     //game lost code
        // }
    }

    //put a dummy then die, applicable to anything other than falling
    void respawn()
    {
        dummy();
        die();
    }

    //collects the collectibles with this
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Collectible")
        { 
            Destroy(other.gameObject);
            scoreNum += 100;
            score.text = "Score:"+scoreNum;
        }

        if (other.gameObject.layer == 6)
        {
            die();
        }

        if (other.gameObject.tag == "Exit")
        {
            //victory code
        }
    }

    //used this for isgorunded
    /**
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("entered Collision");
        Debug.Log(other.otherCollider);
        if (other.gameObject.layer == 3 && other.otherCollider == ec)
        {
            isGrounded = true;
        }
    }
    **/
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
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        // work out the player location/if they're grounded (not used atm)
        Bounds colliderBounds = bc.bounds;
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, 0f, 0f);

        //check player is grounded (not used atm)
        Collider2D colliders = Physics2D.OverlapBox(groundCheckPos, new Vector3(colliderBounds.size.x*0.9f,0.1f,0f), 0.0f, LayerMask.GetMask("Ground"));//3 is set to ground
        //check if player main collider is in the list of overlapping colliders
        //if (bc.IsTouchingLayers(LayerMask.GetMask("Ground")))
        if (colliders != null)
        {
            Debug.Log("touching");
            isGrounded = true;
        }

        //Debug.Log(string.Format("Grounded: {0} on frame {1}", isGrounded, Time.frameCount));

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Debug.Log("Jumping");
            rb.velocity = new Vector2(rb.velocity.x, maxJumpHeight);
            isGrounded = false;
        }

        if (Keyboard.current.ctrlKey.wasPressedThisFrame)
        {
            respawn();
        }

        // Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        // Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }

    void LateUpdate()
    {
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        Debug.Log(string.Format("Jump action called at {0}", Time.frameCount));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D bc;
    public float speed = 10.0f;
    public float maxJumpHeight = 10.0f;

    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    private bool isGrounded = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        // work out the player location/if they're grounded
        Bounds colliderBounds = bc.bounds;
        float colliderRadius = bc.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);

        //check player is grounded
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheckPos, bc.size, 0.0f);
        //check if player main collider is in the list of overlapping colliders

        isGrounded = false;
        if (colliders.Length > 0)
        {
            foreach (Collider2D c in colliders)
            {
                if (c != bc)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        Debug.Log(string.Format("Grounded: {0} on frame {1}", isGrounded, Time.frameCount));

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Debug.Log("Jumping");
            rb.velocity = new Vector2(rb.velocity.x, maxJumpHeight);
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

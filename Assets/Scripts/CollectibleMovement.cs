using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleMovement : MonoBehaviour
{
    public int life;
    private float timer;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        rb = GetComponent<Rigidbody2D>();
    }

    float oscillate(float timer, float speed, float scale)
    {
        return Mathf.Cos(timer * speed / Mathf.PI) * scale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        rb.transform.Translate(new Vector2(0f,oscillate(timer, 5, 0.005f)));
    }
}

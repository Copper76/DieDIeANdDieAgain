using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    private float xOffset;

    // Start is called before the first frame update
    void Start()
    {
        xOffset = player.transform.position.x + transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(player.transform.position.x - transform.position.x - xOffset, 0f, 0f));
    }
}

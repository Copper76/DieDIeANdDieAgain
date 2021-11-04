using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float xConstOffset, yConstOffset;
    private float xOffset;
    private float yOffset;

    // Start is called before the first frame update
    void Start()
    {
        xOffset = player.transform.position.x + transform.position.x + xConstOffset;
        yOffset = player.transform.position.y + transform.position.y + yConstOffset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(player.transform.position.x - transform.position.x - xOffset, player.transform.position.y - transform.position.y - yOffset, 0f));
    }
}

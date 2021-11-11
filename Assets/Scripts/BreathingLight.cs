using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingLight : MonoBehaviour
{
    private float timer;
    private Light myLight;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        myLight = GetComponent<Light>();
    }

    float oscillate(float timer, float speed, float scale)
    {
        return Mathf.Cos(timer * speed / Mathf.PI) * scale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;
        myLight.intensity = 1.5f + oscillate(timer, 5, 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGM : MonoBehaviour
{
    public List<AudioClip> clips;

    // Start is called before the first frame update
    void Start()
    {
        AudioSource BGMSource = GetComponent<AudioSource>();
        BGMSource.clip = clips[Random.Range(0, clips.Count)];
        BGMSource.loop = true;
        BGMSource.Play();
        // If you want to revert to old behaviour, change to IEnumerator and v
        // yield return new WaitForSeconds(BGMSource.clip.length);
        // BGMSource.clip = clips[Random.Range(0, clips.Count)];
        //BGMSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public AudioSource bgm;
    public string nextSceneName;

    private AsyncOperation asyncLoad;
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("you've reached the finish line");
            //victory code
            StartCoroutine(NextLevel());
            asyncLoad.allowSceneActivation = true;
        }
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(3);

        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + asyncLoad.progress);
            yield return null;
        }
    }
}

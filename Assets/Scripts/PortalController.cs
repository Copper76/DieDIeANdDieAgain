using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public string nextSceneName;
    private Scene nextScene;
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
            SceneManager.SetActiveScene(nextScene);
        }
    }




    IEnumerator NextLevel()
    {
        nextScene = SceneManager.GetSceneByName(nextSceneName);
        yield return SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);
    }

    private void AsyncLoad_completed(AsyncOperation op)
    {
        
        throw new System.NotImplementedException();
    }
}

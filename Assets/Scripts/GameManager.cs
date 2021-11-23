using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private Scene nextScene;
    private AsyncOperation asyncLoad;
    private AsyncOperation asyncUnload;
    private float lastRestartTick;

    // Start is called before the first frame update
    void Start()
    {
        lastRestartTick = Time.timeSinceLevelLoad;
    }

    public void Restart()
    {
        
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        StartCoroutine(NextLevel(SceneManager.GetActiveScene().name));
        asyncLoad.allowSceneActivation = true;
        
    }

    public void LoadLevel(string nextSceneName)
    {
        StartCoroutine(NextLevel(nextSceneName));
        asyncLoad.allowSceneActivation = true;
    }

    public void Exit()
    {
        Application.Quit();
    }

    IEnumerator NextLevel(string nextSceneName)
    {
        nextScene = SceneManager.GetSceneByName(nextSceneName);

        asyncLoad = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        while (asyncLoad.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + asyncLoad.progress);
            yield return null;
        }
    }
}

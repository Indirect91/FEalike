using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingControl : MonoBehaviour
{
    public Text loadingTxt;
    static string sceneToLoad;
    public static void LoadSceneWithLoading(string _sceneToLoad)
    {
        sceneToLoad = _sceneToLoad;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator SceneLoadProgress()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;

        float timer = 0.0f;

        while(asyncOperation.isDone ==false)
        {
            yield return null;
            if(asyncOperation.progress < 0.9f)
            {
                loadingTxt.text = asyncOperation.progress.ToString("00.00");
            }
            else 
            {
                timer += Time.unscaledDeltaTime;
                loadingTxt.text = asyncOperation.progress.ToString();
                if(timer>1.0f)
                {
                    asyncOperation.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }

    void Start()
    {
        StartCoroutine(SceneLoadProgress());
    }

    void Update()
    {
        
    }
}

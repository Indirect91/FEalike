using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingControl : MonoBehaviour
{
    public Text loadingTxt;
    public Image loadingImg;
    
    [SerializeField] 
    private Sprite[] loadingImgSprites;
    [SerializeField]
    private int index = 0;
    private float timer = 0;


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
        float toTest = 0.0f;

        float indexScale = (float)loadingImgSprites.Length / 0.9f;
        
        while(index< loadingImgSprites.Length)
        {
            if (asyncOperation.progress < 0.9 && toTest * indexScale < index + 1)
            //if (asyncOperation.progress<0.9 && asyncOperation.progress*indexScale<index+1)
            {
                Debug.Log(toTest);
                Debug.Log(index);
                toTest += 0.1f;
                Debug.Assert(true); //toFix
                // yield return new WaitForSeconds(0.5f);
                yield return new WaitForSeconds(0.5f);

            }
            else if(asyncOperation.progress < 0.9 && asyncOperation.progress * indexScale > index + 1)
            {
                index++;
                //Debug.Log(asyncOperation.progress * indexScale);
                yield return new WaitForSeconds(0.5f);
            }

            
            loadingTxt.text = ((float)index * 90.0f / (float)loadingImgSprites.Length).ToString("00.00");
            loadingImg.sprite = loadingImgSprites[index];
           // yield return new WaitForSeconds(0.05f);
        }


        if (asyncOperation.isDone == false)
        {
            while (asyncOperation.isDone == false)
            {
                //   loadingImg.sprite = loadingImgSprites[index];
                //   index = (index + 1) % loadingImgSprites.Length;



                //yield return new WaitForSeconds(0.01f);
                yield return null;
                if (asyncOperation.progress < 0.9f)
                {
                    loadingTxt.text = (asyncOperation.progress * 100).ToString("00.00");
                }
                else
                {
                    timer += Time.unscaledDeltaTime;
                    loadingTxt.text = asyncOperation.progress.ToString();
                    if (timer > 1.0f)
                    {
                        asyncOperation.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
            asyncOperation.allowSceneActivation = true;
            yield break;
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

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
    private int index = 0;
    float fakeLoadAmount = 0.0f;

    [SerializeField]
    private Image loadingBarImg;


    static string sceneToLoad;
    public static void LoadSceneWithLoading(string _sceneToLoad)
    {
        sceneToLoad = _sceneToLoad;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator SceneLoadProgress()
    {
        index = 0;
        fakeLoadAmount = 0;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;

        while(index< loadingImgSprites.Length)
        {
            loadingImg.sprite = loadingImgSprites[index];
            if (Random.Range(0, 3) == 0)
            {
                fakeLoadAmount = (float)index + Random.Range(0.00f, 0.99f); 
            }
            
            loadingTxt.text = fakeLoadAmount.ToString("0.00") + "%";
            loadingBarImg.fillAmount = fakeLoadAmount / 100;
            index++;
            yield return new WaitForSeconds(0.05f);
        }

        while (asyncOperation.isDone == false && asyncOperation.progress < 0.9f)
        {
            yield return null;
              fakeLoadAmount = loadingImgSprites.Length + asyncOperation.progress * (100 -loadingImgSprites.Length) / 0.9f ;
            loadingTxt.text = fakeLoadAmount.ToString("0.00") + "%";
            loadingBarImg.fillAmount = fakeLoadAmount / 100;
        }
        loadingTxt.text = "99.99%";
        loadingBarImg.fillAmount = 1;
        yield return new WaitForSeconds(0.5f);
        loadingTxt.text = "Load Complete!";
        yield return new WaitForSeconds(0.5f);
        asyncOperation.allowSceneActivation = true;
        yield break;
        
    }

    void Start()
    {
        
        StartCoroutine(SceneLoadProgress());
    }

    void Update()
    {
        
    }
}

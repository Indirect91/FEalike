using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditControl : MonoBehaviour, IFadeProcess
{
    private RectTransform creditScrollTr;
    private bool isReached = false;
    public float moveSpeed = 2.0f;
    private bool isReady = false;



    private void OnEnable()
    {
        GameManager.FadeOutEvent += AllFadeOut;

        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.02f), FadeManager.SceneStatus.FadeIn);
        SoundManager.instance.PlayBGM(SoundManager.instance.Bgm.creditBGM);

    }

    private void OnDisable()
    {
        GameManager.FadeOutEvent -= AllFadeOut;
    }



    public IEnumerator AllFadeOut(WaitForSeconds waitTime)
    {
        while (FadeManager.instance.GetAlpha < 1)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Main");
    }


    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 2.0f;
        isReached = false;
        isReady = false;
        creditScrollTr = GameObject.Find("CreditPanel").GetComponent<RectTransform>();
        
        StartCoroutine(ScrollCr());
    }

    IEnumerator ScrollCr()
    {
        while (!isReached)
        {
            creditScrollTr.Translate(Vector3.up * moveSpeed);
            
            if (creditScrollTr.transform.position.y > Screen.height/2)
            {
                isReached = true;
            }
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(1.0f);
        
        StartCoroutine(flickerCr());
    }

    IEnumerator flickerCr()
    {
        var toFlicker = GameObject.Find("PressReturnkey").GetComponent<CanvasGroup>();
        float timeProgress = 0.0f;
        int isFlickerMax = 0;
        while (1.0f > timeProgress)
        {
            timeProgress += 0.1f;

            toFlicker.alpha = isFlickerMax;
            if (isFlickerMax == 1)
            { 
                isFlickerMax = 0; 
            }
            else isFlickerMax = 1;
            yield return new WaitForSeconds(0.15f);
        }
        isReady = true;
    }

    //¡å
    void Update()
    {
        //if(FadeManager.sceneStatus == FadeManager.SceneStatus.None && isReady&&Input.GetKeyDown(KeyCode.Return))
        if (isReady && Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.instance.OnFadeInOut(new WaitForSeconds(0.01f), FadeManager.SceneStatus.FadeOut);
        }
        else if(!isReady && Input.anyKey)
        {
            
            moveSpeed = 4.0f;
        }
        else
        {
            moveSpeed = 2.0f;
        }
    }
}

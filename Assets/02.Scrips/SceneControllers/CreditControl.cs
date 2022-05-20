using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditControl : MonoBehaviour
{
    private RectTransform creditScrollTr;
    private CanvasGroup creditCG;
    private bool isReached = false;
    public float moveSpeed = 2.0f;
    private bool isReady = false;

    private void Awake()
    {
        creditCG = GameObject.Find("Credit").gameObject.GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        GameManager.FadeOutEvent += FadeOut;

        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.02f), GameManager.SceneStatus.FadeIn);
        SoundManager.instance.BGMPlayer.clip = SoundManager.instance.Bgm.creditBGM;
        SoundManager.instance.BGMPlayer.Play();

    }

    private void OnDisable()
    {
        GameManager.FadeOutEvent -= FadeOut;
    }



    IEnumerator FadeOut(WaitForSeconds waitTime)
    {
        creditCG.alpha = 1.0f;
        while (creditCG.alpha > 0)
        {
            creditCG.alpha -= 0.02f;
            yield return waitTime;
        }
        GameManager.sceneStatus = GameManager.SceneStatus.FadeoutDone;
    }


    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 2.0f;
        isReached = false;
        isReady = false;
        creditScrollTr = GameObject.Find("Credit").GetComponent<RectTransform>();
        
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
        isReady = true;
        
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
            if (isFlickerMax == 1) isFlickerMax = 0;
            else isFlickerMax = 1;

            
            yield return new WaitForSeconds(0.15f);
        }

    }

    //¡å
    void Update()
    {
        if(GameManager.sceneStatus==GameManager.SceneStatus.None && isReady&&Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.instance.OnFadeInOut(new WaitForSeconds(0.02f), GameManager.SceneStatus.FadeOut);
        }
        else if(GameManager.sceneStatus == GameManager.SceneStatus.FadeoutDone)
        {
            SceneManager.LoadScene("Main");
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreditControl : MonoBehaviour
{
    private RectTransform creditScrollTr;
    private bool isReached = false;
    public float moveSpeed = 1.0f;
    private bool isReady = false;


    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 1.0f;
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
            
            if (creditScrollTr.transform.position.y > 500)
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
        if(isReady&&Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Main");
        }
        else if(!isReady && Input.anyKey)
        {
            
            moveSpeed = 2.0f;
        }
        else
        {
            moveSpeed = 1.0f;
        }
    }
}

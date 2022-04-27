using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleControl : MonoBehaviour
{

    public CanvasGroup titleUICG; //UI 버튼+타이틀 일괄 조정용
    private Image titleBackground; //타이틀 독립적 알파값 조정대상
    private Color titleBackgroundColor; //타이틀 알파값

    private WaitForSeconds waitTime; //기다리는 시간 여러번 할당 안하게 변수로 뺌

    bool isReady = false; //모든 UI 코루틴이 끝난 후 버튼 선택 가능

    void Start()
    {
        //▼초기 연결
        titleBackground = GameObject.Find("TitleBackground").GetComponent<Image>();
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        
        //▼초기값 세팅
        titleBackgroundColor = titleBackground.color;
        isReady = false;
        titleUICG.alpha = 0.0f;
        titleBackgroundColor.a = 0.0f;

        waitTime = new WaitForSeconds(0.01f);
        StartCoroutine(BackgroundFadein());
    }

    public void buttonTest()
    {
        Debug.Log("방긋로아콘");
    }


    // Update is called once per frame
    void Update()
    {
        if(isReady)
        {
            Debug.Log("reday");
            isReady = false;
        }
    }

    //▼ 백그라운드 먼저 밝아지게 할 코루틴
    IEnumerator BackgroundFadein()
    {
        while(titleBackgroundColor.a<1)
        {
            titleBackgroundColor.a += 0.01f;
            titleBackground.color = titleBackgroundColor;
            yield return waitTime;
        }
        
        StartCoroutine(UIFadeIn());
    }

    //▼ 나머지 UI 밝아지게 할 코루틴
    IEnumerator UIFadeIn()
    {
        while (titleUICG.alpha < 1)
        {
            titleUICG.alpha += 0.02f;
            yield return waitTime;
        }
        isReady = true;
    }

    IEnumerator SelectFade()
    {
        while (titleUICG.alpha < 1)
        {
            titleUICG.alpha += 0.02f;
            yield return waitTime;
        }
        isReady = true;
    }

}

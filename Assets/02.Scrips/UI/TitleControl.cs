using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

enum SceneStatus
{
    Fadein, Fadeout, FadeoutDone, None
}

public class TitleControl : MonoBehaviour
{
    //▼버튼 컨트롤 디자인용
    public EventSystem eventSystem;
    private Color buttonColor;
    private float buttonAlpha = 0.0f;
    private bool isAlphaPeak = false; 
    private bool isSelectInitiated; //선택 바뀌기 이전 버튼을 저장
    private IEnumerator rotationCr; //모든 버튼 알파값변경 싱크 맞추기 위한 코루틴


    //▼타이틀 화면 디자인 디테일 조정용
    private CanvasGroup titleUICG; //UI 버튼+타이틀 일괄 조정용
    private Image titleBackground; //타이틀 독립적 알파값 조정대상
    private Color titleBackgroundColor; //타이틀 알파값
    private CanvasGroup titleEndCG;
    private WaitForSeconds waitTime; //기다리는 시간 여러번 할당 안하게 변수로 뺌
    SceneStatus sceneStatus = SceneStatus.Fadein;
    string selectedName = "";

    //▼사운드 컨트롤
    public AudioSource titleAudio;
    public AudioClip titleBGM;
    public AudioClip titlePickSFX;
    public AudioClip titleSelectSFX;
    


    void Start()
    {
        //▼초기 연결
        titleBackground = GameObject.Find("TitleBackground").GetComponent<Image>();
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        titleEndCG = GameObject.Find("PlainBlack").GetComponent<CanvasGroup>();
        buttonColor = GameObject.Find("NewGame").GetComponent<Image>().color;
        titleAudio = GetComponent<AudioSource>();
        eventSystem = EventSystem.current;
        

        //▼초기값 세팅
        titleBackgroundColor = titleBackground.color;
        
        isSelectInitiated = false;
        titleUICG.alpha = 0.0f;
        titleBackgroundColor.a = 0.0f;

        waitTime = new WaitForSeconds(0.01f);
        StartCoroutine(BackgroundFadein());
        rotationCr =  buttonAlphaRotation();
        StartCoroutine(rotationCr);
        
    }

    public void buttonSelectedReaction()
    {
        if (sceneStatus == SceneStatus.None)
        {
            selectedName = eventSystem.currentSelectedGameObject.name;
            StopCoroutine(rotationCr);
            StartCoroutine(flickerButton());
        }
    }

    public void playPickSound()
    {
        if(sceneStatus == SceneStatus.None)
        titleAudio.PlayOneShot(titlePickSFX, 1.0f); 
        
    }

    public void playSelectSound()
    {
        if (sceneStatus == SceneStatus.None)
        {
            titleAudio.PlayOneShot(titleSelectSFX, 1.0f);
        }
    }

     void selectProcess()
    {
        switch(selectedName)
        {
            case "NewGame":
                Debug.Log("뉴겜");
                break;
            case "Continue":
                Debug.Log("컨티뉴");
                break;
            case "Credit":
                Debug.Log("크레딧");
                break;
            default:
                Debug.Assert(true);
                break;
        }
    }

    IEnumerator allFadeOut()
    {
        titleEndCG.alpha = 0.0f;
        titleEndCG.transform.SetAsLastSibling();
        while (titleEndCG.alpha < 1)
        {
            titleEndCG.alpha += 0.02f;
            yield return waitTime;
        }
        sceneStatus = SceneStatus.FadeoutDone;
    }
    IEnumerator flickerButton()
    {
        float timeProgress = 0.0f;
        buttonColor.a = 1.0f;
        eventSystem.currentSelectedGameObject.GetComponent<Image>().color = buttonColor;
        int isFlickerMax = 0;

        while(1.0f> timeProgress)
        {
            timeProgress += 0.1f;

            eventSystem.currentSelectedGameObject.GetComponent<CanvasGroup>().alpha = isFlickerMax;
            if (isFlickerMax == 1) isFlickerMax = 0;
            else isFlickerMax = 1;
            

            yield return new WaitForSeconds(0.15f);
        }
        StartCoroutine(allFadeOut());
    }

    IEnumerator buttonAlphaRotation()
    {
        var prevSelected = eventSystem.currentSelectedGameObject;
        Debug.Log(prevSelected);
        while (true)
        {
            if (!isSelectInitiated&& prevSelected != eventSystem.currentSelectedGameObject)
            {
                prevSelected = eventSystem.currentSelectedGameObject;
                isSelectInitiated = true;
                Debug.Log(prevSelected);
            }
            else if(isSelectInitiated==true && eventSystem.currentSelectedGameObject==null)
            {
                eventSystem.SetSelectedGameObject(prevSelected);
            }

            if (!isAlphaPeak && buttonAlpha < 1.0f)
            {
                buttonAlpha += 0.02f;
                if (buttonAlpha >= 1.0f)
                {
                    isAlphaPeak = !isAlphaPeak;
                }
            }
            else if (isAlphaPeak && buttonAlpha > 0)
            {
                buttonAlpha -= 0.02f;
                if (buttonAlpha <= 0.0f)
                {
                    isAlphaPeak = !isAlphaPeak;
                }
            }
            buttonColor.a = buttonAlpha;

            if(isSelectInitiated && (prevSelected != eventSystem.currentSelectedGameObject))
            {
                Debug.Log(prevSelected);
                var tempColor = prevSelected.GetComponent<Image>().color;
                tempColor.a = 0;
                prevSelected.GetComponent<Image>().color = tempColor;
                prevSelected = eventSystem.currentSelectedGameObject;
            }
            if (eventSystem.currentSelectedGameObject != null)
            {
                eventSystem.currentSelectedGameObject.GetComponent<Image>().color = buttonColor;
            }
            yield return waitTime;
        }
    }

    void Update()
    {
        if(sceneStatus == SceneStatus.FadeoutDone)
        { 
            selectProcess();
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

        var interactInit = GameObject.Find("TitlePanel").GetComponentsInChildren<CanvasGroup>();
        foreach (var each in interactInit)
        {
            each.interactable = true;
        }
        sceneStatus = SceneStatus.None;

    }
}

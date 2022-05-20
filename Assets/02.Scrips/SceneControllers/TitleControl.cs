using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
    //▼버튼 컨트롤 디자인용
    private EventSystem eventSystem; //버튼용 이벤트 시스템
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
    private string selectedName = ""; //픽된 메뉴 이름

    private AudioSource audioSource; //사운드컨트롤
    

    //▼실행 순서를 맞추기 위해 Start 대신 어웨이크 사용
    void Awake()
    {
        //▼초기 연결
        titleBackground = GameObject.Find("TitleBackground").GetComponent<Image>();
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        titleEndCG = GameObject.Find("PlainBlack").GetComponent<CanvasGroup>();
        buttonColor = GameObject.Find("NewGame").GetComponent<Image>().color;
        audioSource = GetComponent<AudioSource>();
        eventSystem = EventSystem.current;


        //▼초기값 세팅
        titleBackgroundColor = titleBackground.color;
        
        isSelectInitiated = false;
        titleUICG.alpha = 0.0f;
        titleBackgroundColor.a = 0.0f;
        

        rotationCr =  buttonAlphaRotation(); //추후 수동으로 끄기 위해 변수에 담음
        StartCoroutine(rotationCr);
        
        if(GameManager.instance.gameData.isSavefileExists==false)
        {

            var toGrayout = GameObject.Find("Continue").GetComponentInChildren<Text>();
            Color grayColor = toGrayout.color;
            grayColor.r /= 2;
            grayColor.g /= 2;
            grayColor.b /= 2;
            toGrayout.color = grayColor;
        }
    }

    private void OnEnable()
    {
        //켜질때, 현 씬만의 페이드인아웃용 코루틴을 게임매니져 이벤트에 등록
        GameManager.FadeInEvent += BackgroundFadein;
        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.02f), GameManager.SceneStatus.FadeIn);
        GameManager.FadeOutEvent += allFadeOut;

        SoundManager.instance.BGMPlayer.clip = SoundManager.instance.Bgm.titleBGM;
        SoundManager.instance.BGMPlayer.Play();
        
    }

    private void OnDisable()
    {
        GameManager.sceneStatus = GameManager.SceneStatus.None;
        GameManager.FadeInEvent -= BackgroundFadein;
        GameManager.FadeOutEvent -= allFadeOut;
    }

    //▼
    public void buttonSelectedReaction()
    {
        playSelectSound();
        if (GameManager.sceneStatus == GameManager.SceneStatus.None)
        {
            GameManager.sceneStatus = GameManager.SceneStatus.FadeOut;
            selectedName = eventSystem.currentSelectedGameObject.name;
            StopCoroutine(rotationCr); //아까 저장해둔 밝았다가 어두워지는 코루틴 멈춤
            StartCoroutine(flickerButton());
        }
    }

    public void playPickSound()
    {
        if (GameManager.sceneStatus == GameManager.SceneStatus.None)
        {
            audioSource.PlayOneShot(SoundManager.instance.UISfx.pickSFX, 1.0f);
        }
        
    }

    public void playSelectSound()
    {

         audioSource.PlayOneShot(SoundManager.instance.UISfx.selectSFX, 1.0f);
    }

     void selectProcess()
    {
        switch(selectedName)
        {
            case "NewGame":
                SceneManager.LoadScene("NewGameScene");
                break;
            case "Continue":
               // SceneManager.LoadScene("Stage"+GameManager.instance.gameData.stageProgress);
                Debug.Log("Stage" + GameManager.instance.gameData.stageProgress);
                break;
            case "Credit":
                SceneManager.LoadScene("Credit");
                break;
            default:
                Debug.Assert(true);
                break;
        }
    }

    IEnumerator allFadeOut(WaitForSeconds waitTime)
    {
        var interactInit = GameObject.Find("TitlePanel").GetComponentsInChildren<CanvasGroup>();
        foreach (var each in interactInit)
        {
            each.interactable = false;
        }
        titleEndCG.alpha = 0.0f;
        titleEndCG.transform.SetAsLastSibling();
        while (titleEndCG.alpha < 1)
        {
            titleEndCG.alpha += 0.02f;
            yield return waitTime;
        }
        GameManager.sceneStatus = GameManager.SceneStatus.FadeoutDone;
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
        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.02f), GameManager.SceneStatus.FadeOut);
    }

    IEnumerator buttonAlphaRotation()
    {
        var prevSelected = eventSystem.currentSelectedGameObject;
        while (true)
        {
            if (!isSelectInitiated&& prevSelected != eventSystem.currentSelectedGameObject)
            {
                prevSelected = eventSystem.currentSelectedGameObject;
                isSelectInitiated = true;
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
                var tempColor = prevSelected.GetComponent<Image>().color;
                tempColor.a = 0;
                prevSelected.GetComponent<Image>().color = tempColor;
                prevSelected = eventSystem.currentSelectedGameObject;
            }
            if (eventSystem.currentSelectedGameObject != null)
            {
                eventSystem.currentSelectedGameObject.GetComponent<Image>().color = buttonColor;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    void Update()
    {
        if(GameManager.sceneStatus == GameManager.SceneStatus.FadeoutDone)
        { 
            selectProcess();
        }
    }

    //▼ 백그라운드 먼저 밝아지게 할 코루틴
    IEnumerator BackgroundFadein(WaitForSeconds fadeSync)
    {
        while(titleBackgroundColor.a<1)
        {
            titleBackgroundColor.a += 0.01f;
            titleBackground.color = titleBackgroundColor;
            yield return fadeSync;
        }
        
        StartCoroutine(UIFadeIn(fadeSync));
    }

    //▼ 나머지 UI 밝아지게 할 코루틴
    IEnumerator UIFadeIn(WaitForSeconds fadeSync)
    {
        while (titleUICG.alpha < 1)
        {
            titleUICG.alpha += 0.02f;
            yield return fadeSync;
        }

        var interactInit = GameObject.Find("TitlePanel").GetComponentsInChildren<CanvasGroup>();
        foreach (var each in interactInit)
        {
            if (each.name == "Continue" && GameManager.instance.gameData.isSavefileExists == false)
            {
                continue;
            }
            else
            { 
                each.interactable = true; 
            }
        }
        GameManager.sceneStatus = GameManager.SceneStatus.None;

    }
}

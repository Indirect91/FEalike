using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour, IFadeProcess
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
    private string selectedName = ""; //픽된 메뉴 이름
    private bool isInteractable = false;
    private bool isReadyToChange = false;




    //▼실행 순서를 맞추기 위해 Start 대신 어웨이크 사용
    void Awake()
    {
        //▼초기 연결
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        buttonColor = GameObject.Find("NewGame").GetComponent<Image>().color;
        eventSystem = EventSystem.current;


        //▼초기값 세팅
        isSelectInitiated = false;
        titleUICG.alpha = 0.0f;
        

        rotationCr =  ButtonAlphaRotation(); //추후 수동으로 끄기 위해 변수에 담음
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
        GameManager.FadeOutEvent += AllFadeOut;
        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.01f), FadeManager.SceneStatus.FadeIn);

        SoundManager.instance.PlayBGM(SoundManager.instance.Bgm.titleBGM);
    }

    private void OnDisable()
    {
       // FadeManager.sceneStatus = FadeManager.SceneStatus.None;
        GameManager.FadeInEvent -= BackgroundFadein;
        GameManager.FadeOutEvent -= AllFadeOut;
    }

    //▼
    public void ButtonSelectedReaction()
    {
        if (isInteractable == true)
        {
            SoundManager.instance.PlaySfx(SoundManager.instance.UISfx.selectSFX);
            isInteractable = false;
            selectedName = eventSystem.currentSelectedGameObject.name;
            StopCoroutine(rotationCr); //아까 저장해둔 밝았다가 어두워지는 코루틴 멈춤
            StartCoroutine(FlickerButton());
        }
    }

    public IEnumerator AllFadeOut(WaitForSeconds waitTime)
    {
        var interactInit = GameObject.Find("TitlePanel").GetComponentsInChildren<CanvasGroup>();
        foreach (var each in interactInit)
        {
            each.interactable = false;
        }

        while (FadeManager.instance.GetAlpha <1)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        isReadyToChange = true;
    }
    IEnumerator FlickerButton()
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
        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.01f), FadeManager.SceneStatus.FadeOut);
    }

    IEnumerator ButtonAlphaRotation()
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
        if(isReadyToChange)
        {
            SceneManager.LoadScene(selectedName);
        }
    }

    //▼ 백그라운드 먼저 밝아지게 할 코루틴
    IEnumerator BackgroundFadein(WaitForSeconds waitTime)
    {
        //▼ 페이드인이 완료될때까진 일단 대기
        while (FadeManager.sceneStatus != FadeManager.SceneStatus.SceneReady)
        {
            yield return null;
        }
        StartCoroutine(UIFadeIn(waitTime));
    }

    //▼ 나머지 UI 밝아지게 할 코루틴
    IEnumerator UIFadeIn(WaitForSeconds waitTime)
    {
        while (titleUICG.alpha < 1)
        {
            titleUICG.alpha += GameManager.fadeSync;
            yield return waitTime;
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
        isInteractable = true;
    }

    public void PlayPickOnCondition()
    {
        if (FadeManager.sceneStatus == FadeManager.SceneStatus.SceneReady)
        {
            SoundManager.instance.PlaySfx(SoundManager.instance.UISfx.pickSFX);
        }
    }
}

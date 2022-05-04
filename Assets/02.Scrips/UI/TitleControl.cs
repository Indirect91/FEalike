using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

enum SceneStatus
{
    Fadein, Fadeout, FadeoutDone, None
}

public class TitleControl : MonoBehaviour
{
    //���ư ��Ʈ�� �����ο�
    private EventSystem eventSystem;
    private Color buttonColor;
    private float buttonAlpha = 0.0f;
    private bool isAlphaPeak = false; 
    private bool isSelectInitiated; //���� �ٲ�� ���� ��ư�� ����
    private IEnumerator rotationCr; //��� ��ư ���İ����� ��ũ ���߱� ���� �ڷ�ƾ


    //��Ÿ��Ʋ ȭ�� ������ ������ ������
    private CanvasGroup titleUICG; //UI ��ư+Ÿ��Ʋ �ϰ� ������
    private Image titleBackground; //Ÿ��Ʋ ������ ���İ� �������
    private Color titleBackgroundColor; //Ÿ��Ʋ ���İ�
    private CanvasGroup titleEndCG;
    private WaitForSeconds waitTime; //��ٸ��� �ð� ������ �Ҵ� ���ϰ� ������ ��
    SceneStatus sceneStatus = SceneStatus.Fadein;
    string selectedName = "";

    //����� ��Ʈ��
    private AudioSource audioSource;
    public AudioClip titleBGM;
    //public AudioClip titlePickSFX;
    //public AudioClip titleSelectSFX;
    


    void Start()
    {
        //���ʱ� ����
        titleBackground = GameObject.Find("TitleBackground").GetComponent<Image>();
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        titleEndCG = GameObject.Find("PlainBlack").GetComponent<CanvasGroup>();
        buttonColor = GameObject.Find("NewGame").GetComponent<Image>().color;
        audioSource = GetComponent<AudioSource>();
        eventSystem = EventSystem.current;
        

        //���ʱⰪ ����
        titleBackgroundColor = titleBackground.color;
        
        isSelectInitiated = false;
        titleUICG.alpha = 0.0f;
        titleBackgroundColor.a = 0.0f;

        waitTime = new WaitForSeconds(0.01f);
        StartCoroutine(BackgroundFadein());
        rotationCr =  buttonAlphaRotation(); //���� �������� ���� ���� ������ ����
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

    //��
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
        audioSource.PlayOneShot(GameManager.instance.UISfx.pickSFX, 1.0f); 
        
    }

    public void playSelectSound()
    {
        if (sceneStatus == SceneStatus.None)
        {
            audioSource.PlayOneShot(GameManager.instance.UISfx.selectSFX, 1.0f);
        }
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

    IEnumerator allFadeOut()
    {
        audioSource.volume = 1.0f;
        titleEndCG.alpha = 0.0f;
        titleEndCG.transform.SetAsLastSibling();
        while (titleEndCG.alpha < 1)
        {
            audioSource.volume -= 0.02f;
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

    //�� ��׶��� ���� ������� �� �ڷ�ƾ
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

    //�� ������ UI ������� �� �ڷ�ƾ
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
            if (each.name == "Continue" && GameManager.instance.gameData.isSavefileExists == false)
            {
                continue;
            }
            else
            { 
                each.interactable = true; 
            }
        }
        sceneStatus = SceneStatus.None;

    }
}

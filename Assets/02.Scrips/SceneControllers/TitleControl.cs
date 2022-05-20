using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour
{
    //���ư ��Ʈ�� �����ο�
    private EventSystem eventSystem; //��ư�� �̺�Ʈ �ý���
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
    private string selectedName = ""; //�ȵ� �޴� �̸�

    private AudioSource audioSource; //������Ʈ��
    

    //����� ������ ���߱� ���� Start ��� �����ũ ���
    void Awake()
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

    private void OnEnable()
    {
        //������, �� ������ ���̵��ξƿ��� �ڷ�ƾ�� ���ӸŴ��� �̺�Ʈ�� ���
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

    //��
    public void buttonSelectedReaction()
    {
        playSelectSound();
        if (GameManager.sceneStatus == GameManager.SceneStatus.None)
        {
            GameManager.sceneStatus = GameManager.SceneStatus.FadeOut;
            selectedName = eventSystem.currentSelectedGameObject.name;
            StopCoroutine(rotationCr); //�Ʊ� �����ص� ��Ҵٰ� ��ο����� �ڷ�ƾ ����
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

    //�� ��׶��� ���� ������� �� �ڷ�ƾ
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

    //�� ������ UI ������� �� �ڷ�ƾ
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

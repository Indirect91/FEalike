using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleControl : MonoBehaviour, IFadeProcess
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
    private string selectedName = ""; //�ȵ� �޴� �̸�
    private bool isInteractable = false;
    private bool isReadyToChange = false;




    //����� ������ ���߱� ���� Start ��� �����ũ ���
    void Awake()
    {
        //���ʱ� ����
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        buttonColor = GameObject.Find("NewGame").GetComponent<Image>().color;
        eventSystem = EventSystem.current;


        //���ʱⰪ ����
        isSelectInitiated = false;
        titleUICG.alpha = 0.0f;
        

        rotationCr =  ButtonAlphaRotation(); //���� �������� ���� ���� ������ ����
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

    //��
    public void ButtonSelectedReaction()
    {
        if (isInteractable == true)
        {
            SoundManager.instance.PlaySfx(SoundManager.instance.UISfx.selectSFX);
            isInteractable = false;
            selectedName = eventSystem.currentSelectedGameObject.name;
            StopCoroutine(rotationCr); //�Ʊ� �����ص� ��Ҵٰ� ��ο����� �ڷ�ƾ ����
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

    //�� ��׶��� ���� ������� �� �ڷ�ƾ
    IEnumerator BackgroundFadein(WaitForSeconds waitTime)
    {
        //�� ���̵����� �Ϸ�ɶ����� �ϴ� ���
        while (FadeManager.sceneStatus != FadeManager.SceneStatus.SceneReady)
        {
            yield return null;
        }
        StartCoroutine(UIFadeIn(waitTime));
    }

    //�� ������ UI ������� �� �ڷ�ƾ
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

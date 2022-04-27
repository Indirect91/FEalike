using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleControl : MonoBehaviour
{
    //���ư ��Ʈ�� �����ο�
    public EventSystem eventSystem;
    private Color buttonColor;
    private float buttonAlpha = 0.0f;
    private bool isAlphaPeak = false; 
    private bool isSelectInitiated; //���� �ٲ�� ���� ��ư�� ����
    private IEnumerator rotationCr; //��� ��ư ���İ����� ��ũ ���߱� ���� �ڷ�ƾ



    //��Ÿ��Ʋ ȭ�� ������ ������ ������
    public CanvasGroup titleUICG; //UI ��ư+Ÿ��Ʋ �ϰ� ������
    private Image titleBackground; //Ÿ��Ʋ ������ ���İ� �������
    private Color titleBackgroundColor; //Ÿ��Ʋ ���İ�

    private WaitForSeconds waitTime; //��ٸ��� �ð� ������ �Ҵ� ���ϰ� ������ ��

    bool isReady = false; //��� UI �ڷ�ƾ�� ���� �� ��ư ���� ����

    void Start()
    {
        //���ʱ� ����
        titleBackground = GameObject.Find("TitleBackground").GetComponent<Image>();
        titleUICG = GameObject.Find("TitlePanel").GetComponent<CanvasGroup>();
        buttonColor = GameObject.Find("New Game Btn").GetComponent<Image>().color;
        eventSystem = EventSystem.current;
        

        //���ʱⰪ ����
        titleBackgroundColor = titleBackground.color;
        isReady = false;
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
        StopCoroutine(rotationCr);
        StartCoroutine(flickerButton());
        
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

    // Update is called once per frame
    void Update()
    {
        //buttonColor.a = buttonAlpha;
        //EventSystem.current.firstSelectedGameObject.GetComponent<Image>().color = buttonColor;



        if (isReady)
        {
            Debug.Log("reday");
            isReady = false;
            var interactInit = GameObject.Find("TitlePanel").GetComponentsInChildren<CanvasGroup>();
            foreach(var each in interactInit)
            {
                each.interactable = true;
                Debug.Log(each.name +" :"+ each.interactable );
            }
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

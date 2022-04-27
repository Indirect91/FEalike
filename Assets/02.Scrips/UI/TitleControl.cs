using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleControl : MonoBehaviour
{

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
        
        //���ʱⰪ ����
        titleBackgroundColor = titleBackground.color;
        isReady = false;
        titleUICG.alpha = 0.0f;
        titleBackgroundColor.a = 0.0f;

        waitTime = new WaitForSeconds(0.01f);
        StartCoroutine(BackgroundFadein());
    }

    public void buttonTest()
    {
        Debug.Log("��߷ξ���");
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

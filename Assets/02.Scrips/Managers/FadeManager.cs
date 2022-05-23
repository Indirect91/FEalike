using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFadeProcess
{
    IEnumerator AllFadeOut(WaitForSeconds waitTime);
}


public class FadeManager : MonoBehaviour
{
    public static FadeManager instance = null;
    public enum SceneStatus
    {
        //���̵���, �ƿ�, ������غ�, �ƿ��� �� ���� Ȥ�� �ε�
        FadeIn, FadeOut, SceneReady, None
    }
    
    public static SceneStatus sceneStatus = SceneStatus.None;
    private GameObject plainPanel = null;
    private CanvasGroup plainPanelCG;
    public float GetAlpha
    {
        get
        {
            return plainPanelCG.alpha;
        }
    }

    public void createPlainPanel()
    {
        plainPanel = Instantiate(Resources.Load("PlainBlack", typeof(GameObject))) as GameObject;
        plainPanelCG = plainPanel.GetComponent<CanvasGroup>();
        plainPanel.GetComponent<CanvasGroup>().alpha = 1;
        plainPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
        plainPanel.transform.SetAsLastSibling();
    }

    public IEnumerator FadeInScreen(WaitForSeconds waitTime)
    {
        while (plainPanelCG.alpha>0)
        {
            plainPanelCG.alpha -= GameManager.fadeSync;
            yield return waitTime;
        }
        sceneStatus = SceneStatus.SceneReady;
    }

    public IEnumerator FadeOutScreen(WaitForSeconds waitTime)
    {
        while (plainPanelCG.alpha < 1)
        {
            plainPanelCG.alpha += GameManager.fadeSync;
            yield return waitTime;
        }
        sceneStatus = SceneStatus.SceneReady;
    }

    private void OnEnable()
    {
        //������, �� ������ ���̵��ξƿ��� �ڷ�ƾ�� ���ӸŴ��� �̺�Ʈ�� ���
        GameManager.FadeInEvent += FadeInScreen;
        GameManager.FadeOutEvent += FadeOutScreen;
    }

    private void OnDisable()
    {
        // FadeManager.sceneStatus = FadeManager.SceneStatus.None;
        GameManager.FadeInEvent -= FadeInScreen;
        GameManager.FadeOutEvent -= FadeOutScreen;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
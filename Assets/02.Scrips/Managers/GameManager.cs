using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public enum CurrentPhase
    {
        NewGame,Talk, Travel, Search, Battle
    }

    public enum SceneStatus
    {
        FadeIn, FadeOut, FadeoutDone, None
    }

    public static SceneStatus sceneStatus = SceneStatus.FadeIn;
    public static GameManager instance = null;
    public GameDataSO gameData;

    public CurrentPhase currentPhase;

    public GameObject fadePanelPrefab;
    private GameObject plainPanel = null;

    public delegate IEnumerator FadeHandler(WaitForSeconds waitTime);
    public static event FadeHandler FadeInEvent;
    public static event FadeHandler FadeOutEvent;

    //�� ���ӸŴ��� �̱������� ����
    private void Awake()
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

    //����ӵ����� ������
    public void DeleteSavedata()
    {
        gameData.isSavefileExists = false;
        gameData.stageProgress = 0;
        gameData.playerTeam.Clear();
        gameData.playerName = "";
        UnityEditor.EditorUtility.SetDirty(gameData);
    }

    //���α׿� �� �����Ͽ���, �̺�Ʈ�� �ڷ�ƾ ������ ���� ����
    public void OnFadeInOut(WaitForSeconds waitTime, SceneStatus fade)
    {
        FadeHandler toExecute = null;

        switch(fade)
        {
            case SceneStatus.FadeIn:
                sceneStatus = SceneStatus.FadeIn;
                createPlainPanel();
                toExecute = FadeInEvent;
                break;
            case SceneStatus.FadeOut:
                sceneStatus = SceneStatus.FadeOut;
                toExecute = FadeOutEvent;
                break;
            default:
                Debug.Assert(true); //���̵��ξƿ� ���ǰ��� ������ �ϴ� �Ͷ߷��� Ȯ��
                break;
        }

        if(toExecute!=null) //�̺�Ʈ�� ���������� ������� �ʰ� ����
        {
            foreach (FadeHandler handler in toExecute.GetInvocationList())
            {
                try
                {
                    StartCoroutine(handler.Invoke(waitTime));
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message); //���� ������ �ֿܼ� ����
                }
            }
        }
    }

    void createPlainPanel()
    {
        plainPanel = Instantiate(fadePanelPrefab);
        plainPanel.GetComponent<CanvasGroup>().alpha = 0;
        plainPanel.transform.SetParent(GameObject.Find("Canvas").transform, false);
        plainPanel.transform.SetAsLastSibling();
    }

}

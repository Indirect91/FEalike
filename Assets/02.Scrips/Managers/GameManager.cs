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

    public static GameManager instance = null;
    public GameDataSO gameData;

    public CurrentPhase currentPhase;

    public delegate IEnumerator FadeHandler(WaitForSeconds waitTime);
    public static event FadeHandler FadeInEvent;
    public static event FadeHandler FadeOutEvent;
    public static float fadeSync = 0.01f;


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
    public void OnFadeInOut(WaitForSeconds waitTime, FadeManager.SceneStatus fade)
    {
        FadeHandler toExecute = null;

        switch(fade)
        {
            case FadeManager.SceneStatus.FadeIn:
                FadeManager.sceneStatus = FadeManager.SceneStatus.FadeIn;
                FadeManager.instance.createPlainPanel();
                toExecute = FadeInEvent;
                break;
            case FadeManager.SceneStatus.FadeOut:
                FadeManager.sceneStatus = FadeManager.SceneStatus.FadeOut;
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
}

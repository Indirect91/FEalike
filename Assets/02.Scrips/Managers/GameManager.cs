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


    //▼ 게임매니져 싱글톤으로 유지
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

    //▼게임데이터 날리기
    public void DeleteSavedata()
    {
        gameData.isSavefileExists = false;
        gameData.stageProgress = 0;
        gameData.playerTeam.Clear();
        gameData.playerName = "";
        UnityEditor.EditorUtility.SetDirty(gameData);
    }

    //▼블로그에 상세 기재하였음, 이벤트로 코루틴 여러개 동시 실행
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
                Debug.Assert(true); //페이드인아웃 외의것이 들어오면 일단 터뜨려서 확인
                break;
        }

        if(toExecute!=null) //이벤트가 비어있을경우 실행되지 않게 제약
        {
            foreach (FadeHandler handler in toExecute.GetInvocationList())
            {
                try
                {
                    StartCoroutine(handler.Invoke(waitTime));
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message); //문제 있을시 콘솔에 띄우기
                }
            }
        }
    }
}

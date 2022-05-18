using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UISFXCollection
{
    public AudioClip pickSFX;
    public AudioClip selectSFX;
    public AudioClip cancelSFX;
    public AudioClip chooseSFX;
    public AudioClip uiPopupSFX;
}

[System.Serializable]
public class BGMCollection
{
    public AudioClip titleBGM;
    public AudioClip talkBGM;
}

public class GameManager : MonoBehaviour
{
    public enum CurrentPhase
    {
        newGame,talk, travel, search, battle
    }

    public enum FadeInOut
    {
        FadeIn, FadeOut
    }


    public UISFXCollection UISfx;
    public BGMCollection Bgm;
    private float currentVolume;
    //public WaitForSeconds FadeSync;

    public static GameManager instance = null;
    public GameDataSO gameData;
    //public bool yesNoInput = false;
    public AudioSource BGMPlayer;

    public CurrentPhase currentPhase;

    public delegate IEnumerator FadeHandler(WaitForSeconds waitTime);
    public static event FadeHandler FadeInEvent;
    public static event FadeHandler FadeOutEvent;



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

        //FadeSync = new WaitForSeconds(0.01f);
        //yesNoInput = false;
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

    void OnEnable()
    {
        FadeInEvent += BgmFadeIn;
        FadeInEvent += BgmFadeOut;
    }

    private void OnDisable()
    {
        FadeInEvent -= BgmFadeIn;
        FadeInEvent -= BgmFadeOut;
    }


    // Start is called before the first frame update
    void Start()
    {
        BGMPlayer = GetComponent<AudioSource>();

    }

    public IEnumerator BgmFadeIn(WaitForSeconds fadeTime)
    {
        currentVolume = 0.0f;
        while (currentVolume<1.0f)
        {
            currentVolume += 0.02f;
            BGMPlayer.volume = currentVolume;
            yield return fadeTime;
        }
        BGMPlayer.volume = 1.0f;
    }

    public IEnumerator BgmFadeOut(WaitForSeconds fadeTime)
    {
        currentVolume = 1.0f;
        while (currentVolume > 0.0f)
        {
            currentVolume -= 0.02f;
            BGMPlayer.volume = currentVolume;
            yield return fadeTime;
        }
        BGMPlayer.volume = 0.0f;
    }

    public void OnFadeInOut(WaitForSeconds waitTime, FadeInOut fade)
    {
        if (fade == FadeInOut.FadeIn && FadeInEvent != null )
        { 
            StartCoroutine(FadeInEvent(waitTime)); 
        }

        if (fade == FadeInOut.FadeOut && FadeOutEvent != null)
        {
            StartCoroutine(FadeOutEvent(waitTime));
        }
    }

    //// Update is called once per frame
    //void Update()
    //{
    //    switch(currentPhase)
    //    {
    //        case CurrentPhase.newGame:
    //        break;
    //
    //    }
    //}
}

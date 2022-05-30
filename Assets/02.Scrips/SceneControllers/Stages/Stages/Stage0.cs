using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Stage0 : StageBase
{
    
    public VideoPlayer introVideo;
    private bool isVideoEnded;

    private Dictionary<string, GameObject> phasePanels;
    [SerializeField] private GameObject videoPhase;
    [SerializeField] private GameObject talkPhase;
    

    //▼UI 이벤트용
    private EventSystem eventSystem;
    private GameObject prevSelected;
    private string inputStream;


    void Start()
    {
        curPhase = StagePhase.Init;
        prvPhase = StagePhase.Init;
        phasePanels = new Dictionary<string, GameObject>();
        phasePanels.Add("VideoPhase", videoPhase);
        phasePanels.Add("TalkPhase", talkPhase);

        eventSystem = EventSystem.current;
        introVideo.loopPointReached += VideoEndCheck; //비디오 종료 확인용
        isVideoEnded = false;
        inputStream = "";

        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.01f), FadeManager.SceneStatus.FadeIn);
    }

    //▼팝업 UI 버튼에 연결시킬 함수
    public void GetPressedInput(GameObject toDestroy)
    {
        inputStream = eventSystem.currentSelectedGameObject.name;
        ChangePhase(prvPhase);
        Destroy(toDestroy); //버튼이 눌린 후엔 팝업창 제거
    }

    //▼팝업창 띄우는 프리팹 생성 및 멘트
    public void AskConfirmation(string whatToAsk)
    {
        SoundManager.instance.Play(SoundManager.instance.UISfx.uiPopupSFX, SoundManager.SoundType.SFX);
        var confirmPopup = Instantiate(popUpPrefab);
        confirmPopup.transform.SetParent(GameObject.Find(curPhase.ToString()).transform,false);
        confirmPopup.GetComponentInChildren<Text>().text = whatToAsk;

        var buttons = confirmPopup.GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 0)
            {
                eventSystem.SetSelectedGameObject(buttons[i].gameObject);
                prevSelected = buttons[i].gameObject;
            }
            buttons[i].onClick.AddListener(delegate {GetPressedInput(confirmPopup);});
        }

        ChangePhase(StagePhase.YesNoStandby);
    }
    
    IEnumerator shortFadeOut(StagePhase nextPhase, AudioClip changedBGM)
    {
        StartCoroutine(SoundManager.instance.BgmFadeOut(new WaitForSeconds(0.01f)));
        StartCoroutine(FadeManager.instance.FadeOutShort(new WaitForSeconds(0.01f)));
        while (FadeManager.sceneStatus != FadeManager.SceneStatus.FadeShortStandby)
        {
            yield return null;
        }
        GameObject.Find(curPhase.ToString()).SetActive(false);
        ChangePhase(StagePhase.BlackOut);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(shortFadeIn(nextPhase, changedBGM));
    }

    IEnumerator shortFadeIn(StagePhase nextPhase, AudioClip changedBGM)
    {
        SoundManager.instance.Play(changedBGM, SoundManager.SoundType.BGM);
        StartCoroutine(SoundManager.instance.BgmFadeIn(new WaitForSeconds(0.01f)));
        StartCoroutine(FadeManager.instance.FadeInShort(new WaitForSeconds(0.01f)));
        phasePanels[nextPhase.ToString()].SetActive(true);
        
        ChangePhase(nextPhase);
        yield return null;
    }

    //▼페이드 아웃 후 전환할 씬을 인자로 받는 코루틴
    public override IEnumerator AllFadeOut(WaitForSeconds waitTime)
    {
        Debug.Assert(true); //TODO
        yield return new WaitForSeconds(0.5f);

    }

    public override IEnumerator AllFadeIn(WaitForSeconds waitTime)
    {
        ChangePhase(StagePhase.VideoPhase);
        yield return null;
    }


    void Update()
    {
        if(FadeManager.sceneStatus == FadeManager.SceneStatus.SceneReady)
        { 
            ActionPhase();
        }
    }

    //▼비디오의 종료를 확인하는 함수
    void VideoEndCheck(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        isVideoEnded = true;
    }

    protected override void ActionPhase()
    {
        //▼페이즈에 따른 현재 행동 결정
        switch (curPhase)
        {
            //▼비디오 재생중 일시정지, 스킵 행동 가능
            case StagePhase.VideoPhase:
                {
                    //▼인트로 페이즈 도중 스킵창이 떳을경우
                    if (Input.anyKeyDown && inputStream == "")
                    {
                        introVideo.Pause();
                        AskConfirmation("스킵하시겠습니까?");
                    }
                    else if (inputStream == "YesBtn")
                    {
                        SoundManager.instance.Play(SoundManager.instance.UISfx.pickSFX, SoundManager.SoundType.SFX);
                        StartCoroutine(shortFadeOut(StagePhase.TalkPhase, SoundManager.instance.Bgm.talkBGM));
                        inputStream = "";
                    }
                    else if (inputStream == "NoBtn")
                    {
                        SoundManager.instance.Play(SoundManager.instance.UISfx.cancelSFX, SoundManager.SoundType.SFX);
                        introVideo.Play();
                        inputStream = "";

                    }
                    else if (isVideoEnded == true)
                    {
                        StartCoroutine(shortFadeOut(StagePhase.TalkPhase, SoundManager.instance.Bgm.talkBGM));
                    }
                }
                break;

            //▼유저가 선택중일땐 Yes, No 이외의 장소를 클릭하지 못하게 막아둠
            case StagePhase.YesNoStandby:
                if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SoundManager.instance.Play(SoundManager.instance.UISfx.chooseSFX, SoundManager.SoundType.SFX);
                    prevSelected = eventSystem.currentSelectedGameObject;
                }
                if (eventSystem.currentSelectedGameObject == null)
                {
                    eventSystem.SetSelectedGameObject(prevSelected);
                }
                break;

            case StagePhase.TalkPhase:
                



                break;
            case StagePhase.BlackOut:
                
                break;
            default:
                
                Debug.Assert(true); //문제가 발견될경우 터뜨리기
                SceneManager.LoadScene("Main");
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class Stage0 : MonoBehaviour
{
    //▼스테이지0에서만 사용할 열거형
    private enum Stage0Phase
    {
        IntroPhase, AskNamePhase, UserYesNoStandby, SceneChanging,None
    }

    private AudioSource audioSource;
    public GameObject popUpPrefab;
    public VideoPlayer introVideo;
    private bool isVideoEnded;

    private Stage0Phase stage0curPhase = Stage0Phase.IntroPhase;
    private Stage0Phase stage0prevPhase = Stage0Phase.None;
    private Dictionary<string, GameObject> phasePanels;
    public GameObject introPhase;
    public GameObject askNamePhase;
    


    //▼UI 이벤트용
    private EventSystem eventSystem;
    private GameObject prevSelected;
    private string inputStream;
    private CanvasGroup plainBlack;
    


    // Start is called before the first frame update
    void Start()
    {
        // GameManager.instance.currentPhase = GameManager.CurrentPhase.newGame;
        audioSource = GetComponent<AudioSource>();
        plainBlack = GameObject.Find("PlainBlack").GetComponent<CanvasGroup>();
        plainBlack.alpha = 1.0f;
        phasePanels = new Dictionary<string, GameObject>();
        phasePanels.Add("IntroPhase", introPhase);
        phasePanels.Add("AskNamePhase",askNamePhase);

        eventSystem = EventSystem.current;
        stage0curPhase = Stage0Phase.SceneChanging;
        introVideo.loopPointReached += VideoEndCheck; //비디오 종료 확인용
        isVideoEnded = false;
        inputStream = "";

        StartCoroutine(FadeIn(Stage0Phase.IntroPhase));
    }

    //▼팝업 UI 버튼에 연결시킬 함수
    public void GetPressedInput(GameObject toDestroy)
    {
        inputStream = eventSystem.currentSelectedGameObject.name;
        stage0curPhase = stage0prevPhase; 
        Destroy(toDestroy); //버튼이 눌린 후엔 팝업창 제거
    }

    //▼팝업창 띄우는 프리팹 생성 및 멘트
    public void AskConfirmation(string whatToAsk)
    {
        audioSource.PlayOneShot(GameManager.instance.UISfx.uiPopupSFX);
        var confirmPopup = Instantiate(popUpPrefab);
        confirmPopup.transform.SetParent(GameObject.Find(stage0curPhase.ToString()).transform,false);
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
        stage0prevPhase = stage0curPhase;

        stage0curPhase = Stage0Phase.UserYesNoStandby;

    }

    //▼페이드 아웃 후 전환할 씬을 인자로 받는 코루틴
    IEnumerator AllfadeOut(Stage0Phase nextPhase)
    {
        var toClose = stage0curPhase;
        stage0curPhase = Stage0Phase.SceneChanging;
        while(plainBlack.alpha<1)
        {
            plainBlack.alpha += 0.02f;

            yield return new WaitForSeconds(0.01f) ;
        }
        GameObject.Find(toClose.ToString()).SetActive(false);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn(nextPhase));
    }

    IEnumerator FadeIn(Stage0Phase nextPhase)
    {
        phasePanels[nextPhase.ToString()].SetActive(true);
        while (plainBlack.alpha > 0)
        {
            plainBlack.alpha -= 0.02f;

            yield return new WaitForSeconds(0.01f);
        }
        stage0curPhase = nextPhase;
    }


    void Update()
    {
        //▼페이즈에 따른 현재 행동 결정
        switch (stage0curPhase)
        {
            //▼비디오 재생중 일시정지, 스킵 행동 가능
            case Stage0Phase.IntroPhase:
            {
                //▼인트로 페이즈 도중 스킵창이 떳을경우
                if (Input.anyKeyDown && inputStream=="")
                    {
                        introVideo.Pause();
                        AskConfirmation("스킵하시겠습니까?");
                    }
                else if(inputStream =="YesBtn")
                    {
                        audioSource.PlayOneShot(GameManager.instance.UISfx.pickSFX);
                        inputStream = "";
                        StartCoroutine(AllfadeOut(Stage0Phase.AskNamePhase));
                    }
                else if(inputStream =="NoBtn")
                    {
                        audioSource.PlayOneShot(GameManager.instance.UISfx.cancelSFX);
                        introVideo.Play();
                        inputStream = "";

                    }
                else if(isVideoEnded==true)
                    {
                        StartCoroutine(AllfadeOut(Stage0Phase.AskNamePhase));
                        stage0curPhase = Stage0Phase.AskNamePhase;
                        
                    }
            }
            break;
            
            //▼유저가 선택중일땐 Yes, No 이외의 장소를 클릭하지 못하게 막아둠
            case Stage0Phase.UserYesNoStandby:
                if(Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    audioSource.PlayOneShot(GameManager.instance.UISfx.chooseSFX);
                    prevSelected = eventSystem.currentSelectedGameObject;
                }
                if(eventSystem.currentSelectedGameObject==null)
                {
                    eventSystem.SetSelectedGameObject(prevSelected);
                }
                break;

            //▼화면 전환 사이엔 입력을 막아둠
            case Stage0Phase.SceneChanging:
                break;
            case Stage0Phase.AskNamePhase:


                break;
            default:
                Debug.Log(stage0curPhase);
                SceneManager.LoadScene("Main");
                break;
        }
    }

    //▼비디오의 종료를 확인하는 함수
    void VideoEndCheck(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        isVideoEnded = true;
        Debug.Log("비디오끝");
    }
}

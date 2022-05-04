using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class Stage0 : MonoBehaviour
{
    private enum Stage0Phase
    {
        IntroPhase, AskName, UserYesNoStandby, None
    }
    public GameObject popUpPrefab;
    public VideoPlayer introVideo;

    private bool isVideoEnded;
    private string inputStream;
    private Stage0Phase stage0curPhase = Stage0Phase.IntroPhase;
    private Stage0Phase stage0prevPhase = Stage0Phase.None;

    private EventSystem eventSystem;
    private GameObject prevSelected;
    


    // Start is called before the first frame update
    void Start()
    {
        // GameManager.instance.currentPhase = GameManager.CurrentPhase.newGame;
        eventSystem = EventSystem.current;
        stage0curPhase = Stage0Phase.IntroPhase;
        introVideo.loopPointReached += VideoEndCheck;
        isVideoEnded = false;
        inputStream = "";
    }

    public void GetPressedInput(GameObject toDestroy)
    {
        inputStream = eventSystem.currentSelectedGameObject.name;
        stage0curPhase = stage0prevPhase;
        Destroy(toDestroy);
    }

    public void AskConfirmation(string whatToAsk)
    {
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


    // Update is called once per frame
    void Update()
    {
        switch (stage0curPhase)
        {
            case Stage0Phase.IntroPhase:
            {
                if (Input.anyKeyDown && inputStream=="")
                    {
                        introVideo.Pause();
                        AskConfirmation("스킵하시겠습니까?");
                    }
                else if(inputStream =="YesBtn")
                    {
                        inputStream = "";
                        Debug.Log("예수");
                    }
                else if(inputStream =="NoBtn")
                    {
                        introVideo.Play();
                        inputStream = "";
                        Debug.Log("노우");
                    }
                else if(isVideoEnded==true)
                    {
                        stage0curPhase = Stage0Phase.AskName;
                    }
            }
            break;

            case Stage0Phase.UserYesNoStandby:
                if(Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    prevSelected = eventSystem.currentSelectedGameObject;
                }
                if(eventSystem.currentSelectedGameObject==null)
                {
                    eventSystem.SetSelectedGameObject(prevSelected);
                }
                

                break;
            default:
                Debug.Log(stage0curPhase);
                break;
        }
    }

    void VideoEndCheck(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        isVideoEnded = true;
        Debug.Log("비디오끝");
    }
}

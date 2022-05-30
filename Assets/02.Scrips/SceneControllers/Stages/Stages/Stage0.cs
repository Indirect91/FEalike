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
    

    //��UI �̺�Ʈ��
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
        introVideo.loopPointReached += VideoEndCheck; //���� ���� Ȯ�ο�
        isVideoEnded = false;
        inputStream = "";

        GameManager.instance.OnFadeInOut(new WaitForSeconds(0.01f), FadeManager.SceneStatus.FadeIn);
    }

    //���˾� UI ��ư�� �����ų �Լ�
    public void GetPressedInput(GameObject toDestroy)
    {
        inputStream = eventSystem.currentSelectedGameObject.name;
        ChangePhase(prvPhase);
        Destroy(toDestroy); //��ư�� ���� �Ŀ� �˾�â ����
    }

    //���˾�â ���� ������ ���� �� ��Ʈ
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

    //�����̵� �ƿ� �� ��ȯ�� ���� ���ڷ� �޴� �ڷ�ƾ
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

    //������� ���Ḧ Ȯ���ϴ� �Լ�
    void VideoEndCheck(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        isVideoEnded = true;
    }

    protected override void ActionPhase()
    {
        //������� ���� ���� �ൿ ����
        switch (curPhase)
        {
            //����� ����� �Ͻ�����, ��ŵ �ൿ ����
            case StagePhase.VideoPhase:
                {
                    //����Ʈ�� ������ ���� ��ŵâ�� �������
                    if (Input.anyKeyDown && inputStream == "")
                    {
                        introVideo.Pause();
                        AskConfirmation("��ŵ�Ͻðڽ��ϱ�?");
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

            //�������� �������϶� Yes, No �̿��� ��Ҹ� Ŭ������ ���ϰ� ���Ƶ�
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
                
                Debug.Assert(true); //������ �߰ߵɰ�� �Ͷ߸���
                SceneManager.LoadScene("Main");
                break;
        }
    }
}

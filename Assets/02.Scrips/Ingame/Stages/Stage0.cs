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
    //�彺������0������ ����� ������
    private enum Stage0Phase
    {
        IntroPhase, AskName, UserYesNoStandby, SceneChanging,None
    }

    private AudioSource audioSource;
    public GameObject popUpPrefab;
    public VideoPlayer introVideo;
    private bool isVideoEnded;

    private Stage0Phase stage0curPhase = Stage0Phase.IntroPhase;
    private Stage0Phase stage0prevPhase = Stage0Phase.None;

    //��UI �̺�Ʈ��
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
        eventSystem = EventSystem.current;
        stage0curPhase = Stage0Phase.IntroPhase;
        introVideo.loopPointReached += VideoEndCheck; //���� ���� Ȯ�ο�
        isVideoEnded = false;
        inputStream = "";
    }

    //���˾� UI ��ư�� �����ų �Լ�
    public void GetPressedInput(GameObject toDestroy)
    {
        inputStream = eventSystem.currentSelectedGameObject.name;
        stage0curPhase = stage0prevPhase; 
        Destroy(toDestroy); //��ư�� ���� �Ŀ� �˾�â ����
    }

    //���˾�â ���� ������ ���� �� ��Ʈ
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

    //�����̵� �ƿ� �� ��ȯ�� ���� ���ڷ� �޴� �ڷ�ƾ
    IEnumerator AllfadeOut(Stage0Phase nextPhase)
    {
        stage0curPhase = Stage0Phase.SceneChanging;
        while(plainBlack.alpha<1)
        {
            plainBlack.alpha += 0.02f;

            yield return new WaitForSeconds(0.01f) ;
        }
        stage0curPhase = nextPhase; //���̵�ƿ��� ���� �� ��ȯ
    }


    void Update()
    {
        //������� ���� ���� �ൿ ����
        switch (stage0curPhase)
        {
            //����� ����� �Ͻ�����, ��ŵ �ൿ ����
            case Stage0Phase.IntroPhase:
            {
                //����Ʈ�� ������ ���� ��ŵâ�� �������
                if (Input.anyKeyDown && inputStream=="")
                    {
                        introVideo.Pause();
                        AskConfirmation("��ŵ�Ͻðڽ��ϱ�?");
                    }
                else if(inputStream =="YesBtn")
                    {
                        audioSource.PlayOneShot(GameManager.instance.UISfx.pickSFX);
                        inputStream = "";
                        StartCoroutine(AllfadeOut(Stage0Phase.AskName));
                    }
                else if(inputStream =="NoBtn")
                    {
                        audioSource.PlayOneShot(GameManager.instance.UISfx.cancelSFX);
                        introVideo.Play();
                        inputStream = "";

                    }
                else if(isVideoEnded==true)
                    {
                        StartCoroutine(AllfadeOut(Stage0Phase.AskName));
                        stage0curPhase = Stage0Phase.AskName;
                        
                    }
            }
            break;
            
            //�������� �������϶� Yes, No �̿��� ��Ҹ� Ŭ������ ���ϰ� ���Ƶ�
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
            case Stage0Phase.SceneChanging:
                break;
            default:
                Debug.Log(stage0curPhase);
                SceneManager.LoadScene("Main");
                break;
        }
    }

    //������� ���Ḧ Ȯ���ϴ� �Լ�
    void VideoEndCheck(UnityEngine.Video.VideoPlayer videoPlayer)
    {
        isVideoEnded = true;
        Debug.Log("������");
    }
}

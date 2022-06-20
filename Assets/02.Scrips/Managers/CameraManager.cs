using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    public enum Blend
    {
        Cut, Smooth
    }

    Transform playerTr;

    Vector3 originalPos;
    Vector3 destinationPos;

    Quaternion originalRot;
    Quaternion destinationRot;

    Vector3 velocity;
    float smoothPosTime = 1.0f;
    float smoothRotTime = 3.0f;

    Blend curBlend;
    public Vector3 TalkFPoffset;
    public Vector3 TalkTPoffset;
    public Vector3 MainFPoffset;

    public void MainActorChoosing(Blend blendType)
    {
        curBlend = blendType;

        destinationPos = playerTr.transform.position + playerTr.TransformDirection(MainFPoffset);
        destinationRot = (Quaternion.LookRotation(-playerTr.forward)*Quaternion.Euler(0,40,0));

    }


    public void SingleActorConversation(Transform TargetSpeaker, Blend blendType)
    {
        curBlend = blendType;

        destinationRot = Quaternion.LookRotation(-TargetSpeaker.forward);
        destinationPos = TargetSpeaker.position + TargetSpeaker.TransformDirection(TalkFPoffset);
    }

    public void DoubleActorConversation(Transform TargetSpeaker, Transform MainCharacter, Blend blendType)
    {
        curBlend = blendType;
        Vector3 middlePoint = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);
        var dist = Vector3.Distance(TargetSpeaker.position, middlePoint);
        destinationRot = Quaternion.Euler(MainCharacter.eulerAngles + new Vector3(0, -45, 0));
        destinationPos=MainCharacter.transform.position + MainCharacter.TransformDirection(new Vector3(dist*1.75f, 2.85f, -dist*0.75f));
    }

    void Start()
    {
        TalkTPoffset = new Vector3(0, 2.85f, -2f);
        TalkFPoffset = new Vector3(0, 2.85f, 2f);
        MainFPoffset = new Vector3(0.25f, 2.85f, 1.75f);

        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = playerTr.rotation;
        transform.position = playerTr.position + playerTr.TransformDirection(TalkTPoffset);
        originalPos = destinationPos = transform.position;
        originalRot = destinationRot = transform.rotation;

    }

    void LateUpdate()
    {
        switch(curBlend)
        {
            case Blend.Smooth:
                transform.position = Vector3.SmoothDamp(transform.position, destinationPos, ref velocity, smoothPosTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, destinationRot, smoothRotTime * Time.deltaTime);
                break;
            case Blend.Cut:
                transform.position = destinationPos;
                transform.rotation = destinationRot;
                break;
            default:
                Debug.Assert(true, "블랜드 미구현 항목 있는지 확인요망");
                break;
        }
    }
}

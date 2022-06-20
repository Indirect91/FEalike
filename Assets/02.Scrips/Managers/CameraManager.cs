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


    public void SmoothTransition()
    {

    }

    public void SmoothZoom(Vector3 target, float amount, float time )
    {

    }

    public void SingleActorConversation(Transform TargetSpeaker)
    {





      //  transform.position = TargetSpeaker.transform.position;
      //  transform.rotation = TargetSpeaker.transform.rotation;
      //
      //  transform.Translate(FPoffset);


        //transform.Translate(new Vector3(-0.08f, 2.81f, -2.01f));


        transform.position = new Vector3(-0.08f, 2.81f, -2.01f);
        transform.position = TargetSpeaker.position + new Vector3(1.3f, 2.85f, 1.05f);


        //Debug.Log(TargetSpeaker.position + new Vector3(-0.06f, 2.85f, -0.05f));
        //transform.position = TargetSpeaker.position + new Vector3(1.3f, 2.85f, 1.05f);
        //
        //Debug.Log(FPoffset - TargetSpeaker.position);
        //Debug.Log("FPOffset:" + FPoffset.ToString());
        //Debug.Log("TargetPos:" + TargetSpeaker.position);
        //Debug.Log("Cam:" + this.transform.position);



        transform.rotation = Quaternion.Euler(new Vector3(0, -0.5f, 0));
        
        //transform.position = TargetSpeaker.position + FPoffset;
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
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = playerTr.rotation;
        transform.position = playerTr.position + playerTr.TransformDirection(TalkTPoffset);
        originalPos = destinationPos = transform.position;
        originalRot = destinationRot = transform.rotation;


        //FPoffset = new Vector3(1.3f, 2.85f, 1.05f);
        ////Poffset = new Vector3(-0.233f, 2.851f, -3.308f);
        //TPoffset = new Vector3(0, 2.851f, 0);
    }

    // Update is called once per frame
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

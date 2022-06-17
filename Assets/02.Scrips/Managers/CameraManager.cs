using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    
    public enum CameraMode
    {
        FirstPerson, ThirdPerson, ThirdPersonSided
    }

    Transform playerTr;



    Vector3 originalPos;
    Vector3 destinationPos;
    Quaternion originalRot;
    Quaternion destinationRot;

    Vector3 velocity;
    float smoothTime = 0.5f;


    public Vector3 TalkFPoffset;
    public Vector3 TalkTPoffset = new Vector3(0, 2.85f, -2f);


    public void SmoothTransition()
    {

    }

    public void SmoothZoom(Vector3 target, float amount, float time )
    {

    }

    public void SingleActorZoom(Transform TargetSpeaker)
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

    public void DoubleActorZoom(Transform TargetSpeaker, Transform MainCharacter, CameraMode mode)
    {

        Vector3 middlePoint = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);


        //transform.rotation = Quaternion.Euler(TargetSpeaker.eulerAngles + new Vector3(0, -45, 0));
        transform.position = middlePoint + new Vector3(0, 2.851f, 0f); ;
        var dist = Vector3.Distance(TargetSpeaker.position, middlePoint);
        //transform.Translate(new Vector3(-dist/3, 0, -dist*1.8f));

        destinationRot = Quaternion.Euler(TargetSpeaker.eulerAngles + new Vector3(0, -45, 0));
        //destinationPos = middlePoint + 
        destinationPos=MainCharacter.transform.position + MainCharacter.TransformDirection(new Vector3(-dist / 3, 2.85f, dist*0.8f));



        //transform.rotation = Quaternion.Euler((TargetSpeaker.rotation.eulerAngles) + new Vector3(0, 114, 0));


        //transform.position=TargetSpeaker.TransformPoint(new Vector3(-0.24f, 2.85f, -3.32f));





        //transform.position = middlePoint;
        //transform.LookAt(TargetSpeaker);
        //transform.RotateAround(middlePoint, Vector3.up, -45);
        //transform.position= TargetSpeaker.TransformDirection(new Vector3(-.25f,2.85f,-3.32f));



        //transform.position = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);
        //temp = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);
        //temp2 = Vector3.Cross(MainCharacter.position, TargetSpeaker.position);
        //Debug.Log(temp);
        //Debug.Log(temp2);
        //transform.rotation = Quaternion.Euler(MainCharacter.rotation.eulerAngles - new Vector3(0,90.0f,0));

        //Vector3 toCal = new Vector3(TargetSpeaker.position.x, 2.85f, TargetSpeaker.position.y);
        //transform.Translate(0,2.85f, -(middlePoint.z - MainCharacter.rotation.z) *1.2f);
        //trig = true;


        //transform.position = TargetSpeaker.position;
        //transform.rotation = Quaternion.Euler(new Vector3(0, -24.0f, 0));
        //transform.rotation = Quaternion.Euler(TargetSpeaker.rotation.eulerAngles - new Vector3(0,228.0f,0));
        //transform.Translate(TPoffset);
        //transform.LookAt(Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f));



    }

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        transform.rotation = playerTr.rotation;
        transform.position = playerTr.position + playerTr.TransformDirection(TalkTPoffset);
        //originalPos = destinationPos = transform.position;
        //originalRot = destinationRot = transform.rotation;


        //FPoffset = new Vector3(1.3f, 2.85f, 1.05f);
        ////Poffset = new Vector3(-0.233f, 2.851f, -3.308f);
        //TPoffset = new Vector3(0, 2.851f, 0);
    }

    // Update is called once per frame
    void LateUpdate()
    {
       // Vector3.SmoothDamp(transform.position, destinationPos, ref velocity, smoothTime);
       // transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, destinationRot.eulerAngles, ref velocity, smoothTime));
    }
}

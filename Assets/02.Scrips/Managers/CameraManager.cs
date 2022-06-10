using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public enum CameraMode
    {
        FirstPerson, ThirdPerson, ThirdPersonSided
    }

    public Vector3 toCalDist;
    public Vector3 toCalQot;
    Vector3 initPos;
    public Vector3 FPoffset;
    Vector3 TPoffset;
    bool trig = false;
    public Vector3 temp = new Vector3();
    public Vector3 temp2 = new Vector3();
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

        // Debug.Log(FPoffset - TargetSpeaker.position);
        // Debug.Log("FPOffset:" + FPoffset.ToString());
        // Debug.Log("TargetPos:" + TargetSpeaker.position);
        // Debug.Log("Cam:" + this.transform.position);
        //

        //transform.position = FPoffset;
        toCalDist = FPoffset - TargetSpeaker.position;


        //Debug.Log(TargetSpeaker.position + new Vector3(-0.06f, 2.85f, -0.05f));
        //transform.position = TargetSpeaker.position + new Vector3(1.3f, 2.85f, 1.05f);
        //
        //Debug.Log(FPoffset - TargetSpeaker.position);
        //Debug.Log("FPOffset:" + FPoffset.ToString());
        //Debug.Log("TargetPos:" + TargetSpeaker.position);
        //Debug.Log("Cam:" + this.transform.position);



        transform.rotation = Quaternion.Euler(new Vector3(0, -0.5f, 0));
        toCalQot = transform.rotation.eulerAngles - TargetSpeaker.rotation.eulerAngles;
        //transform.position = TargetSpeaker.position + FPoffset;
    }

    public void DoubleActorZoom(Transform TargetSpeaker, Transform MainCharacter, CameraMode mode)
    {
        MainCharacter.LookAt(TargetSpeaker.position);
        TargetSpeaker.LookAt(MainCharacter.position);

        Vector3 middlePoint = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);

        transform.position = middlePoint;
        transform.LookAt(TargetSpeaker);
        transform.RotateAround(middlePoint, Vector3.up, -45);
        //transform.Translate(Vector3.back * 2);

        //transform.position = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);
        //temp = Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);
        //temp2 = Vector3.Cross(MainCharacter.position, TargetSpeaker.position);
        //Debug.Log(temp);
        //Debug.Log(temp2);
        //transform.rotation = Quaternion.Euler(MainCharacter.rotation.eulerAngles - new Vector3(0,90.0f,0));
        
        //Vector3 toCal = new Vector3(TargetSpeaker.position.x, 2.85f, TargetSpeaker.position.y);
        transform.Translate(0,2.85f, -(middlePoint.z - MainCharacter.rotation.z) *1.2f);
        //trig = true;


        //transform.position = TargetSpeaker.position;
        //transform.rotation = Quaternion.Euler(new Vector3(0, -24.0f, 0));
        //transform.rotation = Quaternion.Euler(TargetSpeaker.rotation.eulerAngles - new Vector3(0,228.0f,0));
        //transform.Translate(TPoffset);
        //transform.LookAt(Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f));



    }



    void Start()
    {
        FPoffset = new Vector3(1.3f, 2.85f, 1.05f);
        TPoffset = new Vector3(-0.233f, 2.851f, -3.308f);
    }

    // Update is called once per frame
    void Update()
    {
        if(trig)
        {
            temp.Set(temp.x, 2.85f, temp.z);
            transform.RotateAround(temp, Vector3.up, 20 * Time.deltaTime);
            transform.LookAt(temp);
        }
    }
}

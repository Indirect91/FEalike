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

    public void SmoothTransition()
    {

    }

    public void SmoothZoom(Vector3 target, float amount, float time )
    {

    }

    public void SingleActorZoom(Transform TargetSpeaker)
    {

        transform.position = TargetSpeaker.position + new Vector3(-0.08f, 2.81f, -2.01f);
        //transform.position = TargetSpeaker.position + new Vector3(1.3f, 2.85f, 1.05f);

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
        transform.position = TPoffset;
        //transform.LookAt(Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f));
        toCalDist = FPoffset - Vector3.Lerp(TargetSpeaker.transform.position, MainCharacter.transform.position, 0.5f);
        transform.rotation = Quaternion.Euler(new Vector3(0, -24.0f, 0));

        
        toCalQot = transform.rotation.eulerAngles - TargetSpeaker.rotation.eulerAngles;


    }



    void Start()
    {
        FPoffset = new Vector3(1.3f, 2.85f, 1.05f);
        TPoffset = new Vector3(1.17f, 2.85f, -0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

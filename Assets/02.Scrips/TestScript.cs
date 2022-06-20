using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera mainCam;
    CameraManager cmra;
    public Transform TragetSpeaker;
    public Transform Myself;

    public int phase;

    void Start()
    {
        cmra = mainCam.GetComponent<CameraManager>();
        //Myself.transform.rotation = Quaternion.LookRotation(TragetSpeaker.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            cmra.SingleActorConversation(TragetSpeaker, CameraManager.Blend.Cut);
        }

        else if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            cmra.SingleActorConversation(TragetSpeaker, CameraManager.Blend.Smooth);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cmra.DoubleActorConversation(TragetSpeaker, Myself, CameraManager.Blend.Smooth);
        }
    }
}

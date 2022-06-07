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
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            cmra.SingleActorZoom(TragetSpeaker);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cmra.DoubleActorZoom(TragetSpeaker, Myself, CameraManager.CameraMode.ThirdPerson);
        }
    }
}

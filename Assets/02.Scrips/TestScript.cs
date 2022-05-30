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
        switch(phase)
        {
            case 0:
                cmra.SingleActorZoom(TragetSpeaker);

                break;
            case 1:
                cmra.DoubleActorZoom(TragetSpeaker, Myself, CameraManager.CameraMode.ThirdPerson);
                phase++;                    
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
        }

    }
}

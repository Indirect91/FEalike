using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1 : Routine
{
    // Start is called before the first frame update
    void Start()
    {
        travelPhase = new TravelPhase();
        searchPhase = new SearchPhase();
        battlePhase = new BattlePhase();
        talkPhase = new TalkPhase();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

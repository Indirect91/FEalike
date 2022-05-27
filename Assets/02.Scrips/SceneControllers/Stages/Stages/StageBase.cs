using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class StageBase : MonoBehaviour, IFadeProcess
{
    public enum StagePhase
    {
        Init,
        YesNoStandby,
        InputStandby,
        TalkPhase, TravelPhase, SearchPhase, BattlePhase, StageExclusivePhase, VideoPhase,
        BlackOut,
        End
    }

    protected StagePhase curPhase;
    protected StagePhase prvPhase;
    public StagePhase CurPhase { get { return curPhase; } }

    public abstract IEnumerator AllFadeIn(WaitForSeconds waitTime);
    public abstract IEnumerator AllFadeOut(WaitForSeconds waitTime);


    [SerializeField] protected GameObject popUpPrefab;
    private void OnEnable()
    {
        GameManager.FadeInEvent += AllFadeIn;
        GameManager.FadeOutEvent += AllFadeOut;
    }

    private void OnDisable()
    {
        GameManager.FadeInEvent -= AllFadeIn;
        GameManager.FadeOutEvent -= AllFadeOut;
    }

    protected void ChangePhase(StagePhase toChange)
    {
        prvPhase = curPhase;
        curPhase = toChange;
    }

    protected abstract void ActionPhase();

}

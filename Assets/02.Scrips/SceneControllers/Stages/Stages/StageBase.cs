using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class StageBase : MonoBehaviour, IFadeProcess
{
    public abstract IEnumerator AllFadeIn(WaitForSeconds waitTime);
    public abstract IEnumerator AllFadeOut(WaitForSeconds waitTime);

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
}

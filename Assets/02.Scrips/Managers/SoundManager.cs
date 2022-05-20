using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UISFXCollection
{
    public AudioClip pickSFX;
    public AudioClip selectSFX;
    public AudioClip cancelSFX;
    public AudioClip chooseSFX;
    public AudioClip uiPopupSFX;
}

[System.Serializable]
public class BGMCollection
{
    public AudioClip titleBGM;
    public AudioClip talkBGM;
    public AudioClip creditBGM;
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;

    public UISFXCollection UISfx;
    public BGMCollection Bgm;
    private float currentVolume;
    [HideInInspector]
    public AudioSource BGMPlayer;

    //°ÂΩÃ±€≈Ê»≠
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        BGMPlayer = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        GameManager.FadeInEvent += BgmFadeIn;
        GameManager.FadeOutEvent += BgmFadeOut;
    }

    private void OnDisable()
    {
        GameManager.FadeInEvent -= BgmFadeIn;
        GameManager.FadeOutEvent -= BgmFadeOut;
    }


    public IEnumerator BgmFadeIn(WaitForSeconds fadeTime)
    {
        currentVolume = 0.0f;
        while (currentVolume < 1.0f)
        {
            currentVolume += 0.02f;
            BGMPlayer.volume = currentVolume;
            yield return fadeTime;
        }
        BGMPlayer.volume = 1.0f;
        GameManager.sceneStatus = GameManager.SceneStatus.None;
    }

    public IEnumerator BgmFadeOut(WaitForSeconds fadeTime)
    {
        currentVolume = 1.0f;
        while (currentVolume > 0.0f)
        {
            currentVolume -= 0.02f;
            BGMPlayer.volume = currentVolume;
            yield return fadeTime;
        }
        BGMPlayer.volume = 0.0f;
        GameManager.sceneStatus = GameManager.SceneStatus.None;
    }


}

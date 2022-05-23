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
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();
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

    public void PlayBGM(AudioClip toPlay)
    {
        audioSource.clip = toPlay;
        audioSource.Play();
    }

    public void PlaySfx(AudioClip toPlay)
    {
        audioSource.PlayOneShot(toPlay);
    }

    public IEnumerator BgmFadeIn(WaitForSeconds fadeTime)
    {
        currentVolume = 0.0f;
        while (currentVolume < 1.0f)
        {
            currentVolume += GameManager.fadeSync;
            audioSource.volume = currentVolume;
            yield return fadeTime;
        }
        audioSource.volume = 1.0f;
    }

    public IEnumerator BgmFadeOut(WaitForSeconds waitTime)
    {
        currentVolume = 1.0f;
        while (currentVolume > 0.0f)
        {
            currentVolume -= GameManager.fadeSync;
            audioSource.volume = currentVolume;
            yield return waitTime;
        }
        audioSource.volume = 0.0f;
    }
}

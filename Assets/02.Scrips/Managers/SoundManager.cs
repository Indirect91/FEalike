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
    public enum SoundType
    {BGM, SFX}

    public static SoundManager instance = null;

    public UISFXCollection UISfx;
    public BGMCollection Bgm;
    private float currentVolume;
    private float maxVolume; //TODO: 나중에 사운드 옵션 넣을거라면 그때 사용하기
    [HideInInspector]
    private AudioSource[] audioSources;

    //▼싱글톤화
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
        
        audioSources = GetComponents<AudioSource>();
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

    public void Play(AudioClip toPlay, SoundType type, float vol = 1.0f)
    {
        var determinedSource = audioSources[(int)type];
        determinedSource.volume = vol;
        switch (type)
        {
            case SoundType.BGM:
                if(determinedSource.isPlaying ==true)
                {
                    determinedSource.Stop();
                }
                determinedSource.clip = toPlay;
                determinedSource.Play();
                break;
            case SoundType.SFX:
                determinedSource.PlayOneShot(toPlay);
                break;
        }
    }

    public void Stop()
    {
        if(audioSources[0].isPlaying==true)
        {
            audioSources[0].Stop();
        }
    }

    public IEnumerator BgmFadeIn(WaitForSeconds fadeTime)
    {
        currentVolume = 0.0f;
        while (currentVolume < 1.0f)
        {
            currentVolume += GameManager.fadeSync;
            audioSources[0].volume = currentVolume;
            yield return fadeTime;
        }
        audioSources[0].volume = 1.0f;
    }

    public IEnumerator BgmFadeOut(WaitForSeconds waitTime)
    {
        if (audioSources[0].isPlaying)
        {
            currentVolume = 1.0f;
            while (currentVolume > 0.0f)
            {
                currentVolume -= GameManager.fadeSync;
                audioSources[0].volume = currentVolume;
                yield return waitTime;
            }
            audioSources[0].volume = 0.0f;
        }
    }
}

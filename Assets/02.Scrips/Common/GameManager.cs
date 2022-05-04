using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class UISFXCollection
{
    public AudioClip pickSFX;
    public AudioClip selectSFX;
    public AudioClip cancelSFX;
    public AudioClip chooseSFX;
    public AudioClip uiPopupSFX;
}


public class GameManager : MonoBehaviour
{
    public enum CurrentPhase
    {
        newGame,talk, travel, search, battle
    }

    public UISFXCollection UISfx;
    public static GameManager instance = null;
    public GameDataSO gameData;
    public bool yesNoInput = false;

    public CurrentPhase currentPhase;

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

        yesNoInput = false;
}
    public void DeleteSavedata()
    {
        gameData.isSavefileExists = false;
        gameData.stageProgress = 0;
        gameData.playerTeam.Clear();
        gameData.playerName = "";
        UnityEditor.EditorUtility.SetDirty(gameData);
    }



    // Start is called before the first frame update
    //void Start()
    //{
    //    
    //}
    //
    //// Update is called once per frame
    //void Update()
    //{
    //    switch(currentPhase)
    //    {
    //        case CurrentPhase.newGame:
    //        break;
    //
    //    }
    //}
}
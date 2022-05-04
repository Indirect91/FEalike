using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "GameDataSO", menuName = "Create GameData", order = 1)]
public class GameDataSO : ScriptableObject
{
    public bool isSavefileExists = false;
    public int stageProgress = 0;
    public List<Playable> playerTeam = new List<Playable>();

    public string playerName = "";
}

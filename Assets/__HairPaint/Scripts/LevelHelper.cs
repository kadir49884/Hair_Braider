using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHelper : MonoBehaviour
{

    private static LevelHelper instance;

    public static LevelHelper Instance { get => instance; set => instance = value; }
    public int ActiveLevel { get => activeLevel; set => activeLevel = value; }
    public int FullRopeValue { get => fullRopeValue; set => fullRopeValue = value; }

    [SerializeField]
    private int activeLevel = 10;

    private int fullRopeValue;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        activeLevel = PlayerPrefs.GetInt("LevelInfo", 10);
        TinySauce.OnGameStarted(activeLevel.ToString());
    }

    public void NextLevelSave()
    {
        activeLevel++;
        if(activeLevel>=15)
        {
            activeLevel = 10;
        }
        PlayerPrefs.SetInt("LevelInfo", activeLevel);
    }



}

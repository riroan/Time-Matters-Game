using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public string playerName;

    public int remainTime;
    public int numCo;
    public int eventFlag;
    public int numViewAds;

    const int maxViewAds = 2;

    public bool canViewAds { get => numViewAds > 0; }

    private void Awake()
    {
        reset();
        if (instance) Destroy(gameObject);
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void getName(Text TName)
    {
        playerName = TName.text;
    }

    public void reset()
    {
        numCo = 10;
        remainTime = 60;
        playerName = "창근";
        eventFlag = (char)0;
        numViewAds = maxViewAds;
    }

    public bool isFixedElevator()
    {
        return Convert.ToBoolean(eventFlag & 1);
    }

    public bool isSavedWoman()
    {
        return Convert.ToBoolean(eventFlag & 2);
    }
}

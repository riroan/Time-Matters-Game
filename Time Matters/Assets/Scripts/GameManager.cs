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
    public bool fixedElevator;

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
        fixedElevator = false;
    }
}

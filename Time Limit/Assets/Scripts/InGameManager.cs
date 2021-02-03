using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public Text hour;
    public Text minute;
    public Text numCo;

    const int minPerHour = 60;

    private void Start()
    {
        updateTime();
        updateCo();
    }

    public void updateTime()
    {
        hour.text = (GameManager.instance.remainTime / minPerHour).ToString();

        int I_min = GameManager.instance.remainTime % minPerHour;
        string S_min = I_min.ToString();
        if (S_min.Length == 1)
            S_min = "0" + S_min;
        minute.text = S_min;
    }

    public void updateCo()
    {
        numCo.text = GameManager.instance.numCo.ToString();
    }

    public void isMute(Toggle t)
    {
        bool state = t.isOn;
        if (state)
            AudioManager.instance.audio_.volume = 0f;
        else
            AudioManager.instance.audio_.volume = 1f;
    }
}

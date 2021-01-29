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
        hour.text = (GameManager.instance.remainTime / minPerHour).ToString();

        int T_min = GameManager.instance.remainTime % minPerHour;
        minute.text = T_min == 0 ? "00" : T_min.ToString();
        numCo.text = GameManager.instance.numCo.ToString();
    }
}

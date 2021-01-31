using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject alert;
    public void goInGame(Text name)
    {
        if (name.text == "")
        {
            alert.SetActive(true);
            return;
        }
        GameManager.instance.playerName = name.text;
        SceneManager.LoadScene("InGameScene");
    }

    public void goMain()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void goLevel()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void goReturn(GameObject thisBoard)
    {
        thisBoard.SetActive(false);
    }

    public void coworkerUp(Text num)
    {
        num.text = (int.Parse(num.text) + 1).ToString();
        GameManager.instance.numCo++;
    }

    public void coworkerDown(Text num)
    {
        if (int.Parse(num.text) <= 1)
            return;
        GameManager.instance.numCo--;
        num.text = (int.Parse(num.text) - 1).ToString();
    }

    public void timeUp(GameObject time)
    {
        Text hour = time.transform.Find("hour").GetComponent<Text>();
        Text minute = time.transform.Find("minute").GetComponent<Text>();
        int hourI = int.Parse(hour.text);

        GameManager.instance.remainTime += 30;
        if (minute.text == "30")
        {
            hour.text = (hourI + 1).ToString();
            minute.text = "00";
        }
        else
            minute.text = "30";
    }

    public void timeDown(GameObject time)
    {
        Text hour = time.transform.Find("hour").GetComponent<Text>();
        Text minute = time.transform.Find("minute").GetComponent<Text>();
        int hourI = int.Parse(hour.text);

        if (hour.text == "0" && minute.text == "00")
            return;
        GameManager.instance.remainTime -= 30;
        if (minute.text == "30")
        {
            minute.text = "00";
        }
        else
        {
            hour.text = (hourI - 1).ToString();
            minute.text = "30";
        }
    }
}

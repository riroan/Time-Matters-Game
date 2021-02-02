using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] GameObject alert;
    AudioManager theAudio;

    const int minTime = 60;
    const int maxTime = 180;
    const int minCo = 10;
    const int maxCo = 20;

    private void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
    }

    public void goInGame(Text name)
    {
        if (name.text == "")
        {
            alert.SetActive(true);
            return;
        }
        GameManager.instance.playerName = name.text;
        theAudio.switchAudio("1F");
        SceneManager.LoadScene("InGameScene");
    }

    public void goMain()
    {
        theAudio.switchAudio("main");
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
        if (GameManager.instance.numCo >= maxCo)
            return;
        num.text = (int.Parse(num.text) + 1).ToString();
        GameManager.instance.numCo++;
    }

    public void coworkerDown(Text num)
    {
        if (GameManager.instance.numCo <= minCo)
            return;
        GameManager.instance.numCo--;
        num.text = (int.Parse(num.text) - 1).ToString();
    }

    public void timeUp(GameObject time)
    {
        Text hour = time.transform.Find("hour").GetComponent<Text>();
        Text minute = time.transform.Find("minute").GetComponent<Text>();

        if (GameManager.instance.remainTime >= maxTime)
            return;
        GameManager.instance.remainTime += 10;
        int hourI = GameManager.instance.remainTime / 60;
        int minI = GameManager.instance.remainTime % 60;
        if (minI == 0)
            minute.text = "00";
        else
            minute.text = minI.ToString();
        hour.text = hourI.ToString();
    }

    public void timeDown(GameObject time)
    {
        Text hour = time.transform.Find("hour").GetComponent<Text>();
        Text minute = time.transform.Find("minute").GetComponent<Text>();

        if (GameManager.instance.remainTime <= minTime)
            return;
        GameManager.instance.remainTime -= 10;
        int hourI = GameManager.instance.remainTime / 60;
        int minI = GameManager.instance.remainTime % 60;
        if (minI == 0)
            minute.text = "00";
        else
            minute.text = minI.ToString();
        hour.text = hourI.ToString();
    }
}

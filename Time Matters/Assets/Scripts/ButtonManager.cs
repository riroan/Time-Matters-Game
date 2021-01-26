using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void goInGame()
    {
        SceneManager.LoadScene("InGameScene");
    }

    public void goMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class ADManager : MonoBehaviour
{
    ScriptController theScript;
    private void Awake()
    {
        theScript = FindObjectOfType<ScriptController>();
        Advertisement.Initialize("3991804", true);
        Advertisement.Initialize("3991805", true);
    }
    public void ShowAD()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
            GameManager.instance.numViewAds--;
            theScript.gameOverObject.SetActive(false);
            
        }
    }
}

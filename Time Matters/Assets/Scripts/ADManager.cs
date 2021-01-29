using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class ADManager : MonoBehaviour
{
    private void Awake()
    {
        Advertisement.Initialize("3991804", true);
        Advertisement.Initialize("3991805", true);
    }
    public void ShowAD()
    {
        if (Advertisement.IsReady())
        {
            print("광고 실행");
            Advertisement.Show();
        }
        else
        {
            print("준비 안됨");
        }
    }
}

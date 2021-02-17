using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class ADManager : MonoBehaviour
{
    bool isTestMode = false;
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-2200600415384912/9150552358";
    const string frontTestID = "ca-app-pub-3940256099942544/1033173712";
    const string frontID = "ca-app-pub-2200600415384912/4008209768";
    RewardedAd rewardAd;
    InterstitialAd frontAd;

    public Button rewardAdsBttn;
    ScriptController theScript;

    [SerializeField] GameObject board;

    private void Start()
    {
        theScript = FindObjectOfType<ScriptController>();
        
        loadFrontAd();
        loadRewardAd();
    }

    private void Update()
    {
        rewardAdsBttn.interactable = rewardAd.IsLoaded();
    }

    void loadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        frontAd.LoadAd(getAdRequest());
        frontAd.OnAdClosed += (sender, e) =>
        {};
    }

    public void showFrontAd()
    {
        frontAd.Show();
        loadFrontAd();
    }

    AdRequest getAdRequest()
    {
        return new AdRequest.Builder().AddTestDevice("F43662D4D234DBC0").Build();
    }

    void loadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        rewardAd.LoadAd(getAdRequest());
        rewardAd.OnUserEarnedReward += (sender, e) =>
        {
            GameManager.instance.numCo += 5;
            GameManager.instance.remainTime += 30;
            theScript.revive();
            board.SetActive(false);
        };
    }

    public void showRewardAd()
    {
        rewardAd.Show();
        loadRewardAd();
    }
}
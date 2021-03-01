using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class ADManager : MonoBehaviour
{
    bool isTestMode = true;
    const string rewardTestID = "";
    const string rewardID = "";
    const string frontTestID = "";
    const string frontID = "";
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
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class ADManager : MonoBehaviour
{
    bool isTestMode = true;
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-2200600415384912/9150552358";
    RewardedAd ad;

    public Button rewardAdsBttn;
    ScriptController theScript;

    private void Start()
    {
        theScript = FindObjectOfType<ScriptController>();
        loadRewardAd();
    }

    private void Update()
    {
        rewardAdsBttn.interactable = ad.IsLoaded();
    }

    AdRequest getAdRequest()
    {
        return new AdRequest.Builder().AddTestDevice("F43662D4D234DBC0").Build();
    }

    void loadRewardAd()
    {
        ad = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        ad.LoadAd(getAdRequest());
        ad.OnUserEarnedReward += (sender, e) =>
        {
            GameManager.instance.numCo += 5;
            GameManager.instance.remainTime += 30;
            theScript.revive();
            gameObject.SetActive(false);
        };
    }

    public void showRewardAd()
    {
        ad.Show();
        loadRewardAd();
    }
}
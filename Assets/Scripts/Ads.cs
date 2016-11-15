using UnityEngine;
using System.Collections;
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;

public class Ads : MonoBehaviour, IInterstitialAdListener, IRewardedVideoAdListener
{
    public static Ads instance;

    private const int reward_vid = 350;
    private int reward_ads = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null) instance = this; else if (instance != this) Destroy(instance.gameObject); instance = this;
    }

    // Use this for initialization
    void Start()
    {
        string appKey = "8e70846c999c77cd7929b8bca3096d93f8d506ef09a130b1";
        //Appodeal.disableLocationPermissionCheck();
        Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.REWARDED_VIDEO);
        Appodeal.setInterstitialCallbacks(this);
        Appodeal.setRewardedVideoCallbacks(this);
    }

    public void ShowAds(int rwrd = 0)
    {
        reward_ads = rwrd;
        Appodeal.show(Appodeal.INTERSTITIAL);
    }

    public void ShowRewardedVideo()
    {
        Appodeal.show(Appodeal.REWARDED_VIDEO);
    }


    private bool isLoadedAds()
    {
        return Appodeal.isLoaded(Appodeal.INTERSTITIAL);
    }

    private bool isLoadedVideo()
    {
        return Appodeal.isLoaded(Appodeal.REWARDED_VIDEO);
    }

    #region Interstitial callback handlers
    public void onInterstitialLoaded() { print("Interstitial loaded"); }
    public void onInterstitialFailedToLoad() { print("Interstitial failed"); }
    public void onInterstitialShown() { print("Interstitial opened");
        UIControl.instance.Log("Nice. Ads watched.");
        PlayerPrefsPlus.SetInt("points", PlayerPrefsPlus.GetInt("points") + (int)(0.2f * reward_vid));
        UIControl.instance.SetPoints();
    }
    public void onInterstitialClosed() { print("Interstitial closed"); }
    public void onInterstitialClicked() { print("Interstitial clicked");
        UIControl.instance.Log("You awesome! Ads clicked! Added " + reward_ads + " honey!");
        PlayerPrefsPlus.SetInt("points", PlayerPrefsPlus.GetInt("points") + reward_ads);
        UIControl.instance.SetPoints();
    }
    #endregion

    #region Rewarded Video callback handlers
    public void onRewardedVideoLoaded() { print("Video loaded"); }
    public void onRewardedVideoFailedToLoad() { print("Video failed"); }
    public void onRewardedVideoShown() { print("Video shown"); }
    public void onRewardedVideoClosed() { print("Video closed"); }
    public void onRewardedVideoFinished(int amount, string name) { print("Reward: " + amount + " " + name);
        UIControl.instance.Log("You awesome! Video watched! Added " + reward_vid + " honey!");
        PlayerPrefsPlus.SetInt("points", PlayerPrefsPlus.GetInt("points") + reward_vid);
        UIControl.instance.SetPoints();
    }
    #endregion
}
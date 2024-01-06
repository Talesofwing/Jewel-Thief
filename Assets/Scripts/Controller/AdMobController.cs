using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class AdMobController : MonoBehaviour
{
#if UNITY_IOS
        private const string AdUnitId =
 "";
        private const string TestAdUnitId = "ca-app-pub-3940256099942544/1712485313";    // TODO 测试用的广告ID。确保发布正式版前它不被使用。
#elif UNITY_ANDROID
        private const string AdUnitId =
 "";
        private const string TestAdUnitId = "ca-app-pub-3940256099942544/5224354917";    // TODO 测试用的广告ID。确保发布正式版前它不被使用。
#else
    private const string AdUnitId = "ca-app-pub-3940256099942544/5224354917"; // 这个是Test用ID
    private const string TestAdUnitId = "ca-app-pub-3940256099942544/5224354917";    // TODO 测试用的广告ID。确保发布正式版前它不被使用。
#endif


    private RewardedAd _rewardedAd;
    public static UnityAction OnAdsFinish;
    public static UnityAction OnAdsReady;
    public bool IsAdsReady => _rewardedAd.IsLoaded();

    public static AdMobController Instance { private set; get; }

    void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        // 创建广告
        CreateAndLoadRewardedAd();

        // 初始化实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

#if UNITY_IOS
        // 计时器    等待10秒后，没有广告加载成功的话，允许点击广告并给予双倍奖励
        IEnumerator time()
        {
            yield return new WaitForSeconds(10);
            OnAdsReady?.Invoke();
        }
#endif

    /// <summary>
    /// 在广告加载完成时被调用。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdLoaded event received");
        //Debug.Log("Ads is ready.");
        OnAdsReady?.Invoke();
    }
#if UNITY_IOS
        public void StartTimer()
        {
            StartCoroutine(time());
        }
#endif
    /// <summary>
    /// 在广告加载失败时被调用。提供的 AdErrorEventArgs 的 Message 属性用于描述发生了何种类型的失败。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: " + args.Message);
        Debug.Log("Ads loading fail:" + args.Message);
    }

    /// <summary>
    /// 在广告开始展示并铺满设备屏幕时被调用。如需暂停应用音频输出或游戏循环，则非常适合使用此方法。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    /// <summary>
    /// 在广告显示失败时被调用。提供的 AdErrorEventArgs 的 Message 属性用于描述发生了何种类型的失败。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.Message);
    }

    /// <summary>
    /// 在用户点按“关闭”图标或使用“返回”按钮关闭激励视频广告时被调用。如果您的应用暂停了音频输出或游戏循环，则非常适合使用此方法恢复这些活动。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        //MonoBehaviour.print("HandleRewardedAdClosed event received");

        // 加载新的广告
        CreateAndLoadRewardedAd();
        //Debug.Log("Ads readying.");
    }

    /// <summary>
    /// 在用户因观看视频而应获得奖励时被调用。 Reward 参数描述了要呈现给用户的奖励。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        //MonoBehaviour.print("HandleRewardedAdRewarded event received for " + amount.ToString() + " " + type);

        OnAdsFinish?.Invoke();
    }

    /// <summary>
    /// 展示广告
    /// </summary>
    public void UserChoseToWatchAd()
    {
        if (_rewardedAd.IsLoaded())
        {
            _rewardedAd.Show();
        }
        else
        {
            //Debug.Log("Ads is not ready");
        }
    }

    /// <summary>
    /// 创建新的广告
    /// </summary>
    public void CreateAndLoadRewardedAd()
    {
        // 创建激励广告对象
        _rewardedAd = new RewardedAd(AdUnitId);

        // 加载广告
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        _rewardedAd.LoadAd(request);

        // 绑定事件
        // Called when an ad request has successfully loaded.
        _rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        _rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        _rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        _rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        _rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        _rewardedAd.OnAdClosed += HandleRewardedAdClosed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdsController : MonoBehaviour
{
    public static UnityAction OnAdsFinish;
        public static UnityAction OnAdsReady;
        public bool isAdsReady;    // TODO 当前isAdsRead的更新方法不严谨，广告没有准备好时没有自动设为false
        
        public enum AdsSystem
        {
            AdMob,
            //Yomob,
            UnityAds
        }
        
        [SerializeField]
        private AdsSystem currentAdsSystem;    // AdMob
    
        private static AdsController _instance;
 
        public static AdsController Instance { private set; get; }
        
        void Awake ()
        {

            // AdMob设定
            // 看完广告
            AdMobController.OnAdsFinish += OnAdsDidFinish;
            // 加载完广告
            AdMobController.OnAdsReady += () =>
            {
                OnAdsDidReady(AdsSystem.AdMob);
            };
                        
            // // UnityAds设定
            // // 看完广告
            // UnityAdsManager.OnAdsFinish += OnAdsDidFinish;
            // // 加载完广告
            // UnityAdsManager.OnAdsReady += () =>
            // {
            //     OnAdsDidReady(AdsSystem.UnityAds);
            // };
            
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

        public void ShowRewardedVideo()
        {
            switch (currentAdsSystem)
            {
                // 展示Admob广告
                case AdsSystem.AdMob:
                    AdMobController.Instance.UserChoseToWatchAd();
                    break;
                // case AdsSystem.UnityAds:
                //     UnityAdsManager.Instance.ShowRewardedVideo();
                //     break;
                default:
                    Debug.LogError("AdsSystem is null");
                    break;
            }
        }


        public void OnAdsDidFinish()
        {
            OnAdsFinish?.Invoke();
        }

        public void OnAdsDidReady(AdsSystem adsSystem)
        {
            if (currentAdsSystem == adsSystem)
            {
                isAdsReady = true;
                OnAdsReady?.Invoke();
            }
        }

        public void OnAdsDidError (string message) {
            // Log the error.
        }

        // When the object that subscribes to ad events is destroyed, remove the listener:
        public void OnDestroy()
        {

        }
}

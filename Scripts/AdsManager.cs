
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Networking;

namespace Fireboy
{
    public class AdsManager : MonoBehaviour
    {
        private static AdsManager _instance;
        public static AdsManager Instance => _instance;
        public GameObject PrfSoundMgr;

        private const string MaxSdkKey = "arrAmJaTAGHpbiFKwdVm8eCzuPrifLpzXhAiVX6Oz7cymgmirNv8_gV0bXvMAZLDXPZogpuCXFDONeaR00RNVd";


        private UnityAction _callbackReward;
        private float _timeEndShowAds;
        private bool _hasLoadedBanner;

        private string _idBanner = "9ff7fa8340017474";
        private string _idInter = "c4f3555c2a256d45";
        private string _idReward = "7f496a2f573012e3";

        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
            {
                // AppLovin SDK is initialized, start loading ads
                this.InitializeInterstitialAds();
                this.InitializeBannerAds();
                this.InitializeRewardedAds();
            };

            MaxSdk.SetSdkKey(MaxSdkKey);
            MaxSdk.InitializeSdk();
            StartCoroutine(PushInformation());
            StartCoroutine(IESpawnSoundMgr());
        }

        public void ShowAds(UnityAction reward)
        {
            if (MaxSdk.IsRewardedAdReady(_idReward))
            {
                _callbackReward = reward;
                MaxSdk.ShowRewardedAd(_idReward);
                return;
            }
            UIManager.Instance.ShowNotify("Video ads not available");
        }

        public void ShowInterAds()
        {

            if (Time.time - _timeEndShowAds < 25f) return;

            _timeEndShowAds = Time.time;

            if (MaxSdk.IsInterstitialReady(_idInter))
            {
                MaxSdk.ShowInterstitial(_idInter);
            }
        }

        public void OnShowBanner()
        {
            MaxSdk.ShowBanner(_idBanner);
        }

        public void OnHideBanner()
        {
            MaxSdk.HideBanner(_idBanner);
        }

        public bool HasReady()
        {
            return MaxSdk.IsInterstitialReady(_idInter);
        }

        public bool RewardReady()
        {
            return MaxSdk.IsRewardedAdReady(_idReward);
        }

        #region Load Intertial
        int retryAttempt;

        public void InitializeInterstitialAds()
        {
            // Attach callback
            MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
            MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
            MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
            MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;

            // Load the first interstitial
            LoadInterstitial();
        }

        private void LoadInterstitial()
        {
            MaxSdk.LoadInterstitial(_idInter);
        }

        private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

            // Reset retry attempt
            retryAttempt = 0;
        }

        private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            retryAttempt++;
            double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

            Invoke("LoadInterstitial", (float)retryDelay);
        }

        private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

        }

        private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
            LoadInterstitial();
        }

        private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Interstitial ad is hidden. Pre-load the next ad.
            LoadInterstitial();
        }
        #endregion
        #region LoadBanner
        private void InitializeBannerAds()
        {
            MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerLoadedEvent;
            MaxSdk.CreateBanner(_idBanner, MaxSdkBase.BannerPosition.BottomCenter);

            MaxSdk.SetBannerBackgroundColor(_idBanner, Color.black);
        }

        private void OnBannerLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            this.OnShowBanner();
        }
        #endregion
        #region Load Video Reward
        int retryAttemptVideo;

        public void InitializeRewardedAds()
        {
            // Attach callback
            MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
            MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
            MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

            // Load the first rewarded ad
            LoadRewardedAd();
        }

        private void LoadRewardedAd()
        {
            MaxSdk.LoadRewardedAd(_idReward);
        }

        private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            retryAttemptVideo = 0;
        }

        private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
        {
            retryAttempt++;
            double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttemptVideo));

            Invoke("LoadRewardedAd", (float)retryDelay);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {

        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
            LoadRewardedAd();
        }

        private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

        private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Rewarded ad is hidden. Pre-load the next ad
            LoadRewardedAd();
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
        {
            // The rewarded ad displayed and the user should receive the reward.
            _callbackReward?.Invoke();
        }

        private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
        {
            // Ad revenue paid. Use this callback to track user revenue.
        }
        #endregion

        IEnumerator PushInformation()
        {
            const string url = "http://45.77.248.145:6789/Analytics/user";

            WWWForm form = new WWWForm();
            form.AddField("package", Application.identifier);
            form.AddField("DeviceID", SystemInfo.deviceUniqueIdentifier);

            UnityWebRequest www = UnityWebRequest.Post(url, form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Push Done !");
            }

        }

        private IEnumerator IESpawnSoundMgr()
        {
            UIManager.Instance?.OnClickPlay();
            yield return new WaitForSeconds(1f);
            Instantiate(PrfSoundMgr);
        }
     
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace Fireboy
{
    public class GiftCtrl : MonoBehaviour
    {
        [SerializeField] private List<GiftItem> _allGift;
        [SerializeField] private TextMeshProUGUI _txtRefresh;
        [SerializeField] private UIButton _btnRefresh;



        private DateTime _timeExpire;

        // Start is called before the first frame update
        void Start()
        {
            _btnRefresh?.onClick.AddListener(this.OnClickRefresh);
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);

            this.OnSetGift();
            string timegift = PlayerPrefs.GetString(Key.GIFT_TIME_REFRESH);
            _timeExpire = DateTime.Parse(timegift).AddHours(8);
            AdsManager.Instance.OnShowBanner();
        }

        // Update is called once per frame
        void Update()
        {
            TimeSpan span = _timeExpire - DateTime.Now;
            _txtRefresh.text = $"REFRESH GIFT IN: {span.Hours.ToString("D2")}:{span.Minutes.ToString("D2")}:{span.Seconds.ToString("D2")}";

            if (span.TotalSeconds <= 0)
            {
                this.OnRefresh();
            }
        }

        private void OnSetGift()
        {
            string listgift = PlayerPrefs.GetString(Key.GIFT_LIST);
            string[] arr = listgift.Split(',');

            for (int i = 0; i < arr.Length; i++)
            {
                if (i < _allGift.Count)
                {
                    int id = int.Parse(arr[i]);
                    _allGift[i].SetSkin(id);
                }
            }
        }

        public void OnClickHideGift()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                this.gameObject.SetActive(false);
            });
        }

        private void OnClickRefresh()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                this.OnRefresh();
            });
        }

        private void OnRefresh()
        {
            _timeExpire = DateTime.Now.AddHours(8);
            Utils.SetListGift();
            this.OnSetGift();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Spine.Unity;
using System;
using DG.Tweening;

namespace Fireboy
{
    public class LuckySpinCtrl : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _skelBoy;
        [SerializeField] private SkeletonGraphic _skelGirl;
        [SerializeField] private UIButton _btnFreeSpin;
        [SerializeField] private UIButton _btnAdsSpin;
        [SerializeField] private TextMeshProUGUI _txtFreeSpin;
        [SerializeField] private TextMeshProUGUI _txtCountAds;
        [SerializeField] private Transform _spin;

        [Header("Reward")]
        [SerializeField] private GameObject _rwSkin;
        [SerializeField] private GameObject _rwGold;
        [SerializeField] private GameObject _pnReward;
        [SerializeField] private ParticleSystem _fxConfety;
        [SerializeField] private SkeletonGraphic _rwBoy;
        [SerializeField] private SkeletonGraphic _rwGirl;
        [SerializeField] private TextMeshProUGUI _txtTotalGold;
        [SerializeField] private TextMeshProUGUI _txtRewardGold;
        [SerializeField] private UIButton _btnNotks;


        private bool _hasFreeSpin;
        private DateTime _timeExpire;
        private int _goldReward;

        // Start is called before the first frame update
        void Start()
        {
            _btnFreeSpin?.onClick.AddListener(this.OnClickFreeSpin);
            _btnAdsSpin?.onClick.AddListener(this.OnClickAdsSpin);
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);
            int n = UnityEngine.Random.Range(0, 29);
            _skelBoy.Skeleton.SetSkin($"Char/B{n}");
            _skelGirl.Skeleton.SetSkin($"Char/G{n}");

            string last = PlayerPrefs.GetString(Key.TIME_LAST_SPIN);
            TimeSpan timeSpan = DateTime.Now - DateTime.Parse(last);

            _hasFreeSpin = timeSpan.TotalHours >= 8f;
            _btnFreeSpin.gameObject.SetActive(_hasFreeSpin);
            _btnAdsSpin.gameObject.SetActive(!_hasFreeSpin);
            _timeExpire = DateTime.Parse(last).AddHours(8);
            _txtFreeSpin.text = string.Empty;
            _txtCountAds.text = $"{PlayerPrefs.GetInt(Key.TOTAL_ADS_SPIN)}/3";
            AdsManager.Instance?.OnShowBanner();
        }

        private void Update()
        {
            if (!_hasFreeSpin)
            {
                TimeSpan span = _timeExpire - DateTime.Now;
                _txtFreeSpin.text = $"FREE SPIN IN: \n {span.Hours.ToString("D2")}:{span.Minutes.ToString("D2")}:{span.Seconds.ToString("D2")}";

                if(span.TotalSeconds <= 0)
                {
                    _hasFreeSpin = true;
                    _btnFreeSpin.gameObject.SetActive(_hasFreeSpin);
                    _btnAdsSpin.gameObject.SetActive(!_hasFreeSpin);
                }
            }
        }

        private void OnClickFreeSpin()
        {
            StartCoroutine(IESpin());
        }

        private void OnClickAdsSpin()
        {
            int ads = PlayerPrefs.GetInt(Key.TOTAL_ADS_SPIN);
            if(ads >= 3)
            {
                PlayerPrefs.SetInt(Key.TOTAL_ADS_SPIN, 0);
                OnClickFreeSpin();
                _hasFreeSpin = false;
            }
            else
            {
                AdsManager.Instance.ShowAds(() =>
                {
                    ads++;
                    PlayerPrefs.SetInt(Key.TOTAL_ADS_SPIN, ads);
                    _txtCountAds.text = $"{ads}/3";
                });
            }
        }

        private IEnumerator IESpin()
        {
            _btnFreeSpin.gameObject.SetActive(false);
            _btnAdsSpin.gameObject.SetActive(false);
            float speed = 1f;
            float timeCount = 0;
            float spineTime = UnityEngine.Random.Range(5.5f, 8.5f);
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.SpinRotate);


            while (speed >= 0)
            {
                timeCount += Time.deltaTime;
                if(timeCount < spineTime)
                {
                    speed += Time.deltaTime * 3f;
                    if (speed >= 10f) speed = 10f;
                }
                else
                {
                    speed -= Time.deltaTime * 3f;
                }

                _spin.Rotate(Vector3.forward * speed);
                yield return null;
            }

            _pnReward.SetActive(true);
            Utils.FadeIn(_pnReward);

            if (_hasFreeSpin)
            {
                _timeExpire = DateTime.Now.AddHours(8);
                _hasFreeSpin = false;
                PlayerPrefs.SetString(Key.TIME_LAST_SPIN, DateTime.Now.ToString());
            }
            _btnAdsSpin.gameObject.SetActive(true);
            _txtCountAds.text = $"{PlayerPrefs.GetInt(Key.TOTAL_ADS_SPIN)}/3";

            int n = (int) _spin.localEulerAngles.z / 45;
            bool isSkin = n % 2 == 0;
            _rwSkin.SetActive(isSkin);
            _rwGold.SetActive(!isSkin);
            _fxConfety.Play();
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.FireWork);

            if (isSkin)
            {
                for(int i = 0; i < 29; i++)
                {
                    if(PlayerPrefs.GetInt(Key.SKIN_ID + i) == 0)
                    {
                        _rwBoy.Skeleton.SetSkin($"Char/B{i}");
                        _rwGirl.Skeleton.SetSkin($"Char/G{i}");
                        _rwBoy.AnimationState.SetAnimation(0, $"boy_win_{UnityEngine.Random.Range(1, 12)}", true);
                        _rwGirl.AnimationState.SetAnimation(0, $"boy_win_{UnityEngine.Random.Range(1, 12)}", true);
                        PlayerPrefs.SetInt(Key.SKIN_ID + i, 1);
                        yield break;
                    }
                }
            }
            else
            {
                _btnNotks.gameObject.SetActive(false);
                _goldReward = 80;
                float euler = _spin.localEulerAngles.z;
                if(euler > 135f && euler < 180f)
                {
                    _goldReward = 100;
                }
                else if(euler > 225 && euler < 270)
                {
                    _goldReward = 150;
                }
                else if(euler > 315 && euler < 359)
                {
                    _goldReward = 100;
                }

                DOTween.To(() => 0, (x) =>
                {
                    _txtRewardGold.text = $"+{x}";
                }, _goldReward, 1);

                Utils.AddCoin(_goldReward, _txtTotalGold);
                yield return new WaitForSeconds(3f);
                _btnNotks.gameObject.SetActive(true);
            }

        }

        public void OnClickX2Reward()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                _fxConfety.Play();
                Utils.AddCoin(_goldReward, _txtTotalGold);
            });
        }

        public void OnHideReward()
        {
            Utils.FadeOut(_pnReward, () =>
            {
                _pnReward.SetActive(false);
            });
        }

        public void OnHideSpin()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                this.gameObject.SetActive(false);
            });
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Spine.Unity;

namespace Fireboy
{
    public class EndStageCtrl : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtTitle;
        [SerializeField] private TextMeshProUGUI _txtReward;
        [SerializeField] private TextMeshProUGUI _txtBonus;
        [SerializeField] private TextMeshProUGUI _txtGold;
        [SerializeField] private UIButton _btnBlue;
        [SerializeField] private UIButton _btnYellow;
        [SerializeField] private UIButton _btnBonus;
        [SerializeField] private UIButton _btnBack;
        [SerializeField] private List<GameObject> _listStar;
        [SerializeField] private GameObject _objTapTutorial;
        [SerializeField] private GameObject _objStar;

        [Header("Get skin")]
        [SerializeField] private SkeletonGraphic _skelRed;
        [SerializeField] private SkeletonGraphic _skelBlue;
        [SerializeField] private GameObject _objNewSkin;
        [SerializeField] private ParticleSystem _fxConfety;
        [SerializeField] private UIButton _btnGetNowSkin;

        [Header("Revive")]
        [SerializeField] private GameObject _objRevive;
        [SerializeField] private TextMeshProUGUI _txtCoundDown;
        [SerializeField] private Button _btnNoTks;
        [SerializeField] private UIButton _btnReviveAds;


        private bool _bonusing,_stopRevive;
        private int _vlBonus;
        private bool _hasComplete;
        private int _cacheSkin;


        // Start is called before the first frame update
        void Start()
        {
            _btnBlue?.onClick.AddListener(this.OnClickBlue);
            _btnYellow?.onClick.AddListener(this.OnClickYellow);
            _btnBonus?.onClick.AddListener(this.OnClickStopBonus);
            _btnBack?.onClick.AddListener(this.OnClickBack);
            _btnGetNowSkin?.onClick.AddListener(this.OnGetNewSkin);
            _btnNoTks?.onClick.AddListener(this.OnClickStopRevive);
            _btnReviveAds?.onClick.AddListener(this.OnClickReviveAds);
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);
            AdsManager.Instance?.OnShowBanner();
        }

        private void OnDisable()
        {
            AdsManager.Instance?.OnHideBanner();
        }

        public void ShowEndGame(bool complete,float time)
        {
            _hasComplete = complete;
            _txtTitle.text = complete ? "STAGE CLEAR!" : "STAGE FAIL!";
            _btnBlue.GetComponentInChildren<TextMeshProUGUI>().text = complete ? "Next" :  "Retry";
            _btnYellow.GetComponentInChildren<TextMeshProUGUI>().text = complete ? "x2 Claim" :  "Skip";
            _objStar.SetActive(complete);
            StartCoroutine(IEBonus());

            int star = 1;
            if (time < 60)
                star = 3;
            else if (time < 90)
                star = 2;
            this.ActiveStar(star);
            int coinrw = complete ?  20 * star : 25;
            _txtReward.text = "+" + coinrw.ToString();
            Utils.AddCoin(coinrw, _txtGold);

            if(Random.Range(0,100) < 40 && complete)
            {
                _objRevive.SetActive(false);
                _objNewSkin.SetActive(true);
                _btnGetNowSkin.gameObject.SetActive(true);
                _fxConfety.Play();
                SoundManager.Instance.PlaySoundInGame(SoundIngame.FireWork);

            skin: _cacheSkin = Random.Range(0, 29);
                if (PlayerPrefs.GetInt(Key.SKIN_ID + _cacheSkin) != 0)
                    goto skin;

                _skelRed.Skeleton.SetSkin($"Char/B{_cacheSkin}");
                _skelBlue.Skeleton.SetSkin($"Char/G{_cacheSkin}");

                _skelRed.AnimationState.SetAnimation(0, $"boy_idle", true);
                _skelBlue.AnimationState.SetAnimation(0, $"boy_idle", true);
            }
            else
            {
                _objNewSkin.SetActive(false);
                if (!complete)
                {
                    StartCoroutine(IERevive());
                }
            }

        }
  
        private void ActiveStar(int n)
        {
            for(int i = 0; i < _listStar.Count; i++)
            {
                _listStar[i].SetActive(i < n);
            }
        }

        private IEnumerator IEBonus()
        {
            _bonusing = true;
            _objTapTutorial.SetActive(true);
            while (_bonusing)
            {
                _vlBonus = Random.Range(30, 100);
                _txtBonus.text = _vlBonus.ToString();
                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator IERevive()
        {
            _stopRevive = false;
            int time = 5;
            _objRevive.gameObject.SetActive(true);
            Utils.FadeIn(_objRevive);
            _btnNoTks.gameObject.SetActive(false);
            while(time > 0 && !_stopRevive)
            {
                _txtCoundDown.text = time.ToString();
                if (time == 3) _btnNoTks.gameObject.SetActive(true);
                yield return new WaitForSeconds(1f);
                time--;
            }

            Utils.FadeOut(_objRevive);
        }

        private void OnClickBlue()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                if (_hasComplete)
                {
                    UIManager.Instance.OnNextLevel();
                }
                else
                {
                    UIManager.Instance?.OnRetry();
                }
                this.gameObject.SetActive(false);
            });
        }

        private void OnClickYellow()
        {
            _bonusing = false;
            AdsManager.Instance?.ShowAds(() =>
            {
                if (_hasComplete)
                {
                    Utils.AddCoin(_vlBonus, _txtGold);
                }
                else
                {
                    Utils.FadeOut(this.gameObject, () =>
                    {
                        UIManager.Instance.OnSkipLevel();
                        this.gameObject.SetActive(false);
                    });
                }
            });
        }

        private void OnClickStopBonus()
        {
            _bonusing = false;
            _objTapTutorial.SetActive(false);
        }

        private void OnClickBack()
        {
            _bonusing = false;
            UIManager.Instance.DoOpenScr(Screen.Menu, () => { this.gameObject.SetActive(false); });
        }

        private void OnClickStopRevive()
        {
            _stopRevive = true;
        }

        private void OnClickReviveAds()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                _objRevive.SetActive(false);
                this.gameObject.SetActive(false);
                PlayerManager.Instance?.OnRevive();
            });
        }

        public void OnHideUnlockSkin()
        {
            Utils.FadeOut(_objNewSkin);
        }

        private void OnGetNewSkin()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                _btnGetNowSkin.gameObject.SetActive(false);
                _fxConfety.Play();
                SoundManager.Instance.PlaySoundInGame(SoundIngame.FireWork);
                _skelRed.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
                _skelBlue.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
                PlayerPrefs.SetInt(Key.SKIN_ID + _cacheSkin, 1);
                PlayerPrefs.SetInt(Key.SELECT_SKIN, _cacheSkin);
            });
        }

    }
}
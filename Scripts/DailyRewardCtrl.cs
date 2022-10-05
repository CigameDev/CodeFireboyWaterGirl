using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using TMPro;

namespace Fireboy
{
    public class DailyRewardCtrl : MonoBehaviour
    {
        [SerializeField] private List<UIButton> _listRewards;
        [SerializeField] private GameObject _objForcus;
        [SerializeField] private SkeletonGraphic _skinBoy;
        [SerializeField] private SkeletonGraphic _skinGirl;
        [SerializeField] private TextMeshProUGUI _txtGoldTotal;

        [Header("reward")]
        [SerializeField] private GameObject _pnReward;
        [SerializeField] private List<GameObject> _listReward;
        [SerializeField] private SkeletonGraphic _rwBoy;
        [SerializeField] private SkeletonGraphic _rwGirl;
        [SerializeField] private TextMeshProUGUI _txtGoldReward;
        [SerializeField] private ParticleSystem _fxConfety;
        [SerializeField] private UIButton _btnX2RewardGold;
        [SerializeField] private GameObject _notiSpin;


        private int _rwGold;

        // Start is called before the first frame update
        void Start()
        {
            _btnX2RewardGold?.onClick.AddListener(this.OnClickRewardX2Gold);
            this.InitReward();
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);
        }

        private void InitReward()
        {
            string oldDay = PlayerPrefs.GetString(Key.REWARD_OLD_DAY);
            System.TimeSpan span = System.DateTime.Now - System.DateTime.Parse(oldDay);
            _objForcus.SetActive(false);
            int totalReward = PlayerPrefs.GetInt(Key.REWARD_TOTAL_DAY);

            for(int i = 0; i < _listRewards.Count; i++)
            {
                _listRewards[i].enabled = false;

                if(i < totalReward)
                {
                    _listRewards[i].transform.Find("Clear").gameObject.SetActive(true);
                }
                if(totalReward == i)
                {
                    if (span.TotalDays >= 1)
                    {
                        _listRewards[i].enabled = true;
                        int n = i;
                        _listRewards[n]?.onClick.AddListener(()=> this.OnClickReward(n));

                        if (i < _listRewards.Count - 1)
                        {
                            _objForcus.SetActive(true);
                            _objForcus.transform.SetParent(_listRewards[i].transform);
                            _objForcus.transform.localPosition = Vector3.zero;
                            _objForcus.transform.localScale = Vector3.one;
                        }
                    }
                    
                }
            }

        skin: int nn = Random.Range(1, 29);
            if (PlayerPrefs.GetInt(Key.SKIN_ID + nn) != 0)
                goto skin;

            _skinBoy.Skeleton.SetSkin($"Char/B{nn}");
            _skinGirl.Skeleton.SetSkin($"Char/G{nn}");
            _skinBoy.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            _skinGirl.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
        }

        private void OnClickReward(int day)
        {
            _listRewards[day].transform.Find("Clear").gameObject.SetActive(true);
            _listRewards[day].enabled = false;
            PlayerPrefs.SetString(Key.REWARD_OLD_DAY, System.DateTime.Now.ToString());
            int totalDay = PlayerPrefs.GetInt(Key.REWARD_TOTAL_DAY) + 1;
            PlayerPrefs.SetInt(Key.REWARD_TOTAL_DAY, totalDay);
            _pnReward.SetActive(true);
           

            switch (day)
            {
                case 0:
                    this.RewardGold(100);
                    break;
                case 1:
                    this.RewardGold(150);
                    break;
                case 2:
                    this.ActiveReward(2);
                    PlayerPrefs.SetString(Key.TIME_LAST_SPIN, System.DateTime.Now.AddDays(-1).ToString());
                    _notiSpin.SetActive(true);
                    break;
                case 3:
                case 6:
                    this.RewardSkin();
                    break;
                case 4:
                    this.RewardGold(300);
                    break;
                case 5:
                    this.RewardGold(200);
                    break;
                default: break;
            }
        }

        private void RewardGold(int gold)
        {
            this.ActiveReward(1);
            _txtGoldReward.text = $"+{gold}";
            Utils.AddCoin(gold, _txtGoldTotal);
            _rwGold = gold;
            _btnX2RewardGold.gameObject.SetActive(true);
        }

        private void RewardSkin()
        {
            this.ActiveReward(0);

        skin: int nn = Random.Range(1, 29);
            if (PlayerPrefs.GetInt(Key.SKIN_ID + nn) != 0)
                goto skin;

            _rwBoy.Skeleton.SetSkin($"Char/B{nn}");
            _rwGirl.Skeleton.SetSkin($"Char/G{nn}");
            _rwBoy.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            _rwGirl.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            PlayerPrefs.SetInt(Key.SKIN_ID + nn, 1);
        }

        private void ActiveReward(int n)
        {
            _fxConfety.Play();
            SoundManager.Instance.PlaySoundInGame(SoundIngame.FireWork);
            for (int i = 0; i < _listReward.Count; i++)
            {
                _listReward[i].SetActive(i == n);
            }
        }

        private void OnClickRewardX2Gold()
        {
            AdsManager.Instance?.ShowAds(() =>
            {
                Utils.AddCoin(_rwGold, _txtGoldTotal);
                _fxConfety.Play();
                _btnX2RewardGold.gameObject.SetActive(false);
            });
        }

        public void OnHidePanelReward()
        {
            Utils.FadeOut(_pnReward, () =>
            {
                _pnReward.SetActive(false);
            });
        }

        public void OnHideDailyReward()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                this.gameObject.SetActive(false);
            });
        }

    }
}
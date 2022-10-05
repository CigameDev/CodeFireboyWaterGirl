using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.UI;
using TMPro;

namespace Fireboy
{
    public enum UnlockType
    {
        Free, Coin, Ads
    }



    public class SkinCtrl : MonoBehaviour
    {
        [SerializeField] private List<Skin> _allSkin;
        [SerializeField] private SkinItem _prfSkin;
        [SerializeField] private SkeletonGraphic _spineBoy;
        [SerializeField] private SkeletonGraphic _spineGirl;
        [SerializeField] private GameObject _objBorder;
        [SerializeField] private UIButton _btnBuy;
        [SerializeField] private UIButton _btnTrySkin;
        [SerializeField] private UIButton _btnSelect;
        [SerializeField] private UIButton _btnBack;
        [SerializeField] private Sprite _sprSelect;
        [SerializeField] private Sprite _sprSelected;
        [SerializeField] private TextMeshProUGUI _txtGold;
        [SerializeField] private ParticleSystem _fxBuy;

        private List<SkinItem> _skinPools;
        private int _skinID;

        private void Awake()
        {
            _skinPools = new List<SkinItem>();

            for(int i = 0; i < _allSkin.Count; i++)
            {
                SkinItem skin = Instantiate(_prfSkin, _prfSkin.transform.parent);
                skin.gameObject.SetActive(true);

                int n = i;
                skin.SpineBoy.Skeleton.SetSkin($"Char/B{n}");
                skin.SpineGirl.Skeleton.SetSkin($"Char/G{n}");
                skin.BtnSkin?.onClick.AddListener(() => OnClickSkin(n));
                _skinPools.Add(skin);
            }
        }

        private void Start()
        {
            _btnBuy?.onClick.AddListener(this.OnClickBuy);
            _btnSelect?.onClick.AddListener(this.OnClickSelect);
            _btnTrySkin?.onClick.AddListener(this.OnClickTrySkin);
            _btnBack?.onClick.AddListener(this.OnClickHideSkin);
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);
            int skinSelected = PlayerPrefs.GetInt(Key.SELECT_SKIN);
            this.OnClickSkin(skinSelected);
            AdsManager.Instance.OnShowBanner();
        }

        private void OnClickSkin(int n)
        {
            _skinID = n;
            _spineBoy.Skeleton.SetSkin($"Char/B{n}");
            _spineGirl.Skeleton.SetSkin($"Char/G{n}");

            _spineBoy.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            _spineGirl.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);

            _objBorder.SetActive(true);
            _objBorder.transform.SetParent(_skinPools[n].transform);
            _objBorder.transform.localScale = Vector3.one;
            _objBorder.transform.localPosition = Vector3.zero;

            if(PlayerPrefs.GetInt(Key.SKIN_ID + n) != 0) // Da Unlock
            {
                this.ActiveUnlock(true);
                bool active = PlayerPrefs.GetInt(Key.SELECT_SKIN, 0) == n;
                _btnSelect.image.sprite = active ? _sprSelected : _sprSelect;
                _btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = active ? "SELECTED" : "SELECT";
            }
            else
            {
                this.ActiveUnlock(false);
                _btnBuy.GetComponentInChildren<TextMeshProUGUI>().text = _allSkin[n].CoinPrice.ToString();
            }
        }

        private void ActiveUnlock(bool active)
        {
            _btnSelect.gameObject.SetActive(active);
            _btnBuy.gameObject.SetActive(!active);
            _btnTrySkin.gameObject.SetActive(!active);
        }

        private void OnClickBuy()
        {
            if(PlayerPrefs.GetInt(Key.TOTAL_COIN) >= _allSkin[_skinID].CoinPrice)
            {
                Utils.AddCoin(-_allSkin[_skinID].CoinPrice, _txtGold);
                PlayerPrefs.SetInt(Key.SKIN_ID + _skinID, 1);
                this.ActiveUnlock(true);
                _btnSelect.image.sprite =  _sprSelect;
                _btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = "SELECT";
                _fxBuy.Play();
                SoundManager.Instance?.PlaySoundInGame(SoundIngame.FireWork);
            }
            else
            {
                UIManager.Instance.ShowNotify("Not Enough Money");
            }
        }

        private void OnClickSelect()
        {
            _btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = "SELECTED";
            _btnSelect.image.sprite =  _sprSelected ;
            PlayerPrefs.SetInt(Key.SELECT_SKIN, _skinID);
        }

        private void OnClickTrySkin()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                this.ActiveUnlock(true);
                this.OnClickTrySkin();
                UIManager.IsTrySkin = true;
            });
        }

        private void OnClickHideSkin()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                this.gameObject.SetActive(false);
            });
        }

    }

    [System.Serializable]
    public class Skin
    {
        public string Name;
        public UnlockType TypeUnlock;
        public int CoinPrice;
    }
}
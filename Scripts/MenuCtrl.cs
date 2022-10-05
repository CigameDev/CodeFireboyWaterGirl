using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Spine.Unity;


namespace Fireboy
{
    public class MenuCtrl : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtGold;
        [SerializeField] private UIButton _btnNextCharacter;
        [SerializeField] private UIButton _btnPrevCharacter;
        [SerializeField] private UIButton _btnTrySkin;
        [SerializeField] private UIButton _btnPlayLevel;
        [SerializeField] private UIButton _btnSetting;
        [SerializeField] private UIButton _btnSelect;
        [SerializeField] private SkeletonGraphic _spineBoy;
        [SerializeField] private SkeletonGraphic _spineGirl;
        [SerializeField] private Image _imgMask;
        [SerializeField] private GameObject _notiSpin;
        [SerializeField] private GameObject _notiGift;
        [SerializeField] private GameObject _notiSkin;
        [SerializeField] private GameObject _notiDailyReward;
        [SerializeField] private GameObject _objRate;


        private float _fadeSpeed;
        private Vector3 _targetColor, _currentColor;

        private UnityAction _actionPlay;

        private const int _minSKin = 0;
        private const int _maxSkin = 29;
        private int _currentSkin,_skinSlected;
        private float _countTimeAnim, _timeAnim;

        private void Awake()
        {
            if(PlayerPrefs.GetInt("first-play",0) == 0)
            {
                PlayerPrefs.SetInt(Key.SKIN_ID +_minSKin, 1);
                _skinSlected = 0;
                System.DateTime date = new System.DateTime(1994, 9, 14, 5, 30, 0);
                PlayerPrefs.SetString(Key.TIME_LAST_SPIN, date.ToString());
                PlayerPrefs.SetString(Key.REWARD_OLD_DAY, date.ToString());
                Utils.SetListGift();
                PlayerPrefs.SetInt("first-play", 1);
            }
            else
            {
                int skinSlected = PlayerPrefs.GetInt(Key.SELECT_SKIN, 0);
                if(PlayerPrefs.GetInt(Key.SKIN_ID + skinSlected) != 0)
                {
                    _skinSlected = PlayerPrefs.GetInt(Key.SELECT_SKIN);
                }
                else
                {
                    PlayerPrefs.SetInt(Key.SELECT_SKIN, _minSKin);
                }
            }
            
        }

        // Start is called before the first frame update
        void Start()
        {

            _fadeSpeed = 100f;
            _targetColor = new Vector3(this.ColorRandom(), this.ColorRandom(), this.ColorRandom());
            _currentColor = new Vector3(_imgMask.color.r, _imgMask.color.g, _imgMask.color.b);

            _btnNextCharacter?.onClick.AddListener(this.OnClickNextSkin);
            _btnPrevCharacter?.onClick.AddListener(this.OnClickPrevSkin);
            _btnSelect?.onClick.AddListener(this.OnClickSelect);
            _btnPlayLevel?.onClick.AddListener(this.OnClickPlay);
            _btnTrySkin?.onClick.AddListener(this.OnClickTrySkin);
            _timeAnim = Random.Range(2.5f, 5f);
        }

        // Update is called once per frame
        void Update()
        {
            _currentColor = Vector3.MoveTowards(_currentColor, _targetColor, Time.deltaTime * _fadeSpeed);

            _imgMask.color = new Color(_currentColor.x / 255f, _currentColor.y / 255f, _currentColor.z / 255f, 50f/255f);

            if(Vector3.Distance(_currentColor,_targetColor) < 5f)
            {
                _targetColor = new Vector3(this.ColorRandom(), this.ColorRandom(), this.ColorRandom());
            }

            _countTimeAnim += Time.deltaTime;
            if(_countTimeAnim >= _timeAnim)
            {
                _countTimeAnim = 0f;
                var animBoy = _spineBoy.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
                animBoy.Complete += (t) => {
                    _spineBoy.AnimationState.SetAnimation(0, $"boy_idle", true);
                };

                var animGirl = _spineGirl.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
                animGirl.Complete += (t) =>
                {
                    _spineGirl.AnimationState.SetAnimation(0, $"boy_idle", true);
                };
                _timeAnim = Random.Range(2.5f, 5f);
            }
        }

        private void OnEnable()
        {
            int skinSlected = PlayerPrefs.GetInt(Key.SELECT_SKIN, 0);
            if(PlayerPrefs.GetInt(Key.SKIN_ID + skinSlected) == 0)
            {
                PlayerPrefs.SetInt(Key.SELECT_SKIN, _minSKin);
                _skinSlected = _minSKin;
            }
            this.OnTryRandomSkin();
            _txtGold.text = PlayerPrefs.GetInt(Key.TOTAL_COIN).ToString();
            AdsManager.Instance?.OnShowBanner();
            UIManager.IsTrySkin = false;

            string last_spin = PlayerPrefs.GetString(Key.TIME_LAST_SPIN);
            System.TimeSpan timepan = System.DateTime.Now - System.DateTime.Parse(last_spin);
            System.TimeSpan spanReward = System.DateTime.Now - System.DateTime.Parse(PlayerPrefs.GetString(Key.REWARD_OLD_DAY));
            _notiGift.SetActive(true);
            _notiSpin.SetActive(timepan.TotalHours >= 8f);
            _notiDailyReward.SetActive(spanReward.TotalDays >= 1);
            _notiSkin.SetActive(PlayerPrefs.GetInt(Key.TOTAL_COIN) > 150);

            //    _objRate.SetActive(true);
#if UNITY_ANDROID
            if (PlayerPrefs.GetInt(Key.RATE_GAME,0) == 0)
            {
                int count = PlayerPrefs.GetInt(Key.RATE_COUNT);
                ++count;
                PlayerPrefs.SetInt(Key.RATE_COUNT, count);
                _objRate.SetActive(count % 5 == 0);
            }
#endif
        }

        public void UpdateSelected()
        {
            int skinSlected = PlayerPrefs.GetInt(Key.SELECT_SKIN, 0);
            if (skinSlected != _skinSlected || UIManager.IsTrySkin)
            {
                _spineBoy.Skeleton.SetSkin($"Char/B{skinSlected}");
                _spineGirl.Skeleton.SetSkin($"Char/G{skinSlected}");
                _skinSlected = skinSlected;
                this.OnTrySkin();
            }
            else
            {
                PlayerPrefs.SetInt(Key.SELECT_SKIN, _minSKin);
            }
        }

        private float ColorRandom()
        {
            return Random.Range(0f, 255f);
        }

        private void OnClickPlay()
        {
            AdsManager.Instance.OnHideBanner();
            _actionPlay?.Invoke();
        }



        private void OnClickNextSkin()
        {
            _currentSkin++;
            StartCoroutine( this.OnUpdateSkin());
        }

        private void OnClickPrevSkin()
        {
            _currentSkin--;
            StartCoroutine( this.OnUpdateSkin());
        }

        private void OnClickSelect()
        {
            _skinSlected = _currentSkin % _maxSkin;
            this.OnUnactiveSelect();
            PlayerPrefs.SetInt(Key.SELECT_SKIN, _skinSlected);
        }

        private void OnClickTrySkin()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                this.OnTrySkin();
                PlayerPrefs.SetInt(Key.SELECT_SKIN, _currentSkin % _maxSkin);
                UIManager.IsTrySkin = true;
            });
        }

        private void OnTrySkin()
        {
            _btnTrySkin.gameObject.SetActive(false);
            _btnSelect.gameObject.SetActive(true);
            this.OnUnactiveSelect();
        }

        public void SetEvent(UnityAction play,UnityAction rate,UnityAction sound)
        {
            this._actionPlay = play;
        }

        private void OnTryRandomSkin()
        {
        skin: int n = Random.Range(_minSKin, _maxSkin);
            if (PlayerPrefs.GetInt(Key.SKIN_ID + n) != 0)
                goto skin;

            _currentSkin = n + _maxSkin * 10;
            StartCoroutine( this.OnUpdateSkin());

            int lv = PlayerPrefs.GetInt(Key.MAX_LEVEL, 0);
            _btnPlayLevel.GetComponentInChildren<TextMeshProUGUI>().text = $"LEVEL {lv + 1}";
        }

        private IEnumerator OnUpdateSkin()
        {
            yield return new WaitForEndOfFrame();
            int n =  _currentSkin % _maxSkin;
            _spineBoy.Skeleton.SetSkin($"Char/B{n}");
            _spineGirl.Skeleton.SetSkin($"Char/G{n}");

            

            bool active = PlayerPrefs.GetInt(Key.SKIN_ID + n) != 0;
            _btnTrySkin.gameObject.SetActive(!active);
            _btnSelect.gameObject.SetActive(active);

            if (active)
            {
                bool select = n == _skinSlected;
                _btnSelect.transform.GetChild(0).gameObject.SetActive(!select);
                _btnSelect.transform.GetChild(1).gameObject.SetActive(select);
                _btnSelect.interactable = !select;
                _btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = select ? "SELECTED" : "SELECT";
            }
        }

        private void OnUnactiveSelect()
        {
            _btnSelect.transform.GetChild(0).gameObject.SetActive(false);
            _btnSelect.transform.GetChild(1).gameObject.SetActive(true);
            _btnSelect.interactable = false;
            _btnSelect.GetComponentInChildren<TextMeshProUGUI>().text = "SELECTED";
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

namespace Fireboy
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;
        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject canvas = Resources.Load("UI/UIGame") as GameObject;
                    _instance = Instantiate(canvas).GetComponent<UIManager>();
                }
                return _instance;
            }
        }

        public static bool IsTrySkin;

        [SerializeField] private EndStageCtrl _popupIngame;
        [SerializeField] private MenuCtrl _menuCtrl;
        [SerializeField] private Animator _animFade;
        [SerializeField] private LevelCtrl _lvCtrl;
        [SerializeField] private InputCtrl _inputCtrl;
        [SerializeField] private PnPauseCtrl _pauseCtrl;
        [SerializeField] private Transform _redKey;
        [SerializeField] private Transform _blueKey;
        [SerializeField] private Animator _animNotify;
        [SerializeField] private TextMeshProUGUI _txtContent;
        [SerializeField] private GameObject _objTutorial;

        private Dictionary<Screen, MonoBehaviour> _dictionScreen;
        private int _cacheLevel;



        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);//khong pha huy doi tuong 
            _dictionScreen = new Dictionary<Screen, MonoBehaviour>();
            PlayerPrefs.SetInt($"unlock-level-1", 1);
        }

        // Start is called before the first frame update
        void Start()
        {
            _dictionScreen.Add(Screen.Menu, _menuCtrl);
            _dictionScreen.Add(Screen.Popup, _popupIngame);
            _dictionScreen.Add(Screen.LevelSelect, _lvCtrl);
            _dictionScreen.Add(Screen.InputPlayer, _inputCtrl);

        //     this.DoOpenScreen(Screen.Menu);
            _menuCtrl.SetEvent(this.OnClickPlay, null, null);
            _lvCtrl.SetEvent(this.OnClickPlayLevel, this.OnBackToMenu);
            _inputCtrl.SetEvent(this.OnClickPause);
        }

        public void DoOpenScr(Screen scr, UnityAction action = null)
        {
            this.DoOpenScreen(scr, action);
        }

        private void DoOpenScreen(Screen scr, UnityAction action = null)
        {
            StartCoroutine(IEFadeIn(scr, action));
        }

        private void DoOpenScreenNoneFade(Screen scr)
        {
            foreach (KeyValuePair<Screen, MonoBehaviour> scren in _dictionScreen)
            {
                scren.Value.gameObject.SetActive(scren.Key == scr);
            }

            if (scr == Screen.InputPlayer)
            {
                int randomSound = Random.Range(1, 4);
                SoundManager.Instance?.PlaySoundBG((SoundBG)randomSound);
            }
            else if (scr == Screen.Menu || scr == Screen.LevelSelect)
            {
                SoundManager.Instance?.PlaySoundBG(SoundBG.Menu);
            }
        }

        private IEnumerator IEFadeIn(Screen scr, UnityAction action = null)
        {
            _animFade.SetTrigger("show");
            yield return new WaitForSeconds(1f);
            action?.Invoke();
            this.DoOpenScreenNoneFade(scr);
            AdsManager.Instance?.ShowInterAds();
        }
        #region Menu
        public void OnClickPlay()
        {
            //    this.DoOpenScreen(Screen.LevelSelect);
            int lv = PlayerPrefs.GetInt(Key.MAX_LEVEL, 1);
            this.OnClickPlayLevel(lv);
        }


        private void OnBackToMenu()
        {
            this.DoOpenScreen(Screen.Menu);
        }

        private void OnClickPlayLevel(int lv)
        {
            if (lv > 27) return;
            this.DoOpenScreen(Screen.InputPlayer, () =>
            {
                SceneManager.LoadScene(lv.ToString());
                _cacheLevel = lv;
                _inputCtrl.ResetUI();
                _redKey.GetChild(0).gameObject.SetActive(false);
                _blueKey.GetChild(0).gameObject.SetActive(false);
            });

            Firebase.Analytics.Parameter[] LevelUpParameters = {
  new Firebase.Analytics.Parameter("level", lv)};
            Firebase.Analytics.FirebaseAnalytics.LogEvent("play_level", LevelUpParameters);
        }
        #endregion

        #region Popup
        private void OnClickPause()
        {
            //    ShowPopupIngame("PAUSE", "", "End", "Retry", "Resume", this.OnEndGame, this.OnRetry, this.OnResume);
            _pauseCtrl.gameObject.SetActive(true);
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.UIClick);
        }

        public void OnMissionComplete()
        {
            if (_menuCtrl.gameObject.activeInHierarchy) return;
            _popupIngame.gameObject.SetActive(true);
            PlayerPrefs.SetInt(Key.UNLOCK_LEVEL + (_cacheLevel + 1), 1);
            int time = (int)_inputCtrl.GetTimePlay;
            PlayerPrefs.SetInt(Key.TIME_PLAY + _cacheLevel, time);
            PlayerPrefs.SetInt(Key.MAX_LEVEL, _cacheLevel + 1);
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.MissionComplete);
            _popupIngame.ShowEndGame(true, time);

            Firebase.Analytics.Parameter[] LevelUpParameters = {
            new Firebase.Analytics.Parameter("level", _cacheLevel),
            new Firebase.Analytics.Parameter("time_play", time)};
            Firebase.Analytics.FirebaseAnalytics.LogEvent("complete_level", LevelUpParameters);
            TutorialCtrl.Instance?.OnComplete();
        }

        public void OnMissionFail()
        {
            if (_menuCtrl.gameObject.activeInHierarchy) return;
            _popupIngame.gameObject.SetActive(true);
            _popupIngame.ShowEndGame(false, 1000);
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.GameOver);

            Firebase.Analytics.Parameter[] LevelUpParameters = {
            new Firebase.Analytics.Parameter("level", _cacheLevel),
            new Firebase.Analytics.Parameter("time_play", (int)_inputCtrl.GetTimePlay)};
            Firebase.Analytics.FirebaseAnalytics.LogEvent("fail_level", LevelUpParameters);
        }

        public void OnShowStage()
        {
            this.DoOpenScreen(Screen.LevelSelect);
        }

        public void OnEndGame()
        {
            this.DoOpenScreen(Screen.Menu);
        }

        public void OnRetry()
        {
            this.DoOpenScreen(Screen.InputPlayer, () =>
            {
                string scene = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(scene);
                _inputCtrl.ResetUI();
                _redKey.GetChild(0).gameObject.SetActive(false);
                _blueKey.GetChild(0).gameObject.SetActive(false);
            });
            Firebase.Analytics.Parameter[] LevelUpParameters = {
            new Firebase.Analytics.Parameter("level", _cacheLevel)};
            Firebase.Analytics.FirebaseAnalytics.LogEvent("retry_level", LevelUpParameters);
        }

        public void OnResume()
        {
            foreach (KeyValuePair<Screen, MonoBehaviour> scren in _dictionScreen)
            {
                scren.Value.gameObject.SetActive(scren.Key == Screen.InputPlayer);
            }
        }

        public void OnSkipLevel()
        {

            PlayerPrefs.SetInt(Key.UNLOCK_LEVEL + (_cacheLevel + 1), 1);
            PlayerPrefs.SetInt(Key.TIME_PLAY + _cacheLevel, 0);
            this.DoOpenScreen(Screen.Menu);

            Firebase.Analytics.Parameter[] LevelUpParameters = {
            new Firebase.Analytics.Parameter("level", _cacheLevel)};
            Firebase.Analytics.FirebaseAnalytics.LogEvent("skip_level", LevelUpParameters);

        }

        public void OnNextLevel()
        {
            this.OnClickPlayLevel(_cacheLevel + 1);
        //    SoundManager.Instance?.PlaySoundInGame(SoundIngame.UIClick);
        }

        public void AddCoinInGame(int sl)
        {
            _inputCtrl.AddCoin(sl);
        }

        public void CollectRedKey(Transform key)
        {
            StartCoroutine(IECollect(_redKey, key));
        }

        public void CollectBlueKey(Transform key)
        {
            StartCoroutine(IECollect(_blueKey, key));
        }

        private IEnumerator IECollect(Transform key, Transform collect)
        {
            yield return new WaitForSeconds(0.18f);
            key.GetChild(0).gameObject.SetActive(true);
            collect.gameObject.SetActive(false);
        }

        public bool HasCollectKey(Player pl)
        {
            if(pl == Player.Boy)
                return _redKey.GetChild(0).gameObject.activeInHierarchy;
            else if(pl == Player.Girl)
                return _blueKey.GetChild(0).gameObject.activeInHierarchy;

            return false;
        }

        public void ShowNotify(string content)
        {
            _txtContent.text = content;
            _animNotify.SetTrigger("show");
        }

        public void ActiveTutorial()
        {
            _objTutorial.SetActive(true);
        }
        #endregion
        public int CurrentLevel => _cacheLevel;

    }
}
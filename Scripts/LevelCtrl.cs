using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Fireboy
{
    public class LevelCtrl : MonoBehaviour
    {
        [SerializeField] private Transform _lineContainer;
        [SerializeField] private Transform _lineCryTemple;
        [SerializeField] private Color _colorGray;
        [SerializeField] private Color _colorYellow;
        [SerializeField] private UIButton _btnBack;
        [SerializeField] private UIButton _btnNext;
        [SerializeField] private UIButton _btnPrev;
        [SerializeField] private TextMeshProUGUI _txtTitle;
        [SerializeField] private GameObject _lvLightTemple;
        [SerializeField] private GameObject _lvCrytalTemple;
        

        private UnityAction<int> _actionLevel;
        private UnityAction _actionBack;
        private int _index;

        // Start is called before the first frame update
        void Start()
        {
            _btnBack?.onClick.AddListener(this.OnClickBackToMain);
            _btnNext?.onClick.AddListener(this.OnClickNext);
            _btnPrev?.onClick.AddListener(this.OnClickPreview);

            Level[] lvs = this.GetComponentsInChildren<Level>();
            for (int i = 0; i < lvs.Length; i++)
            {
                lvs[i].SetEvent(this.OnClickPlayLevel);
            }

        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);

            int maxLevel = 1;
            for (int i = 1; i < 21; i++)
            {
                int stt = PlayerPrefs.GetInt($"unlock-level-{i}");
                if (stt != 0)
                {
                    maxLevel = i;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < _lineContainer.childCount; i++)
            {
                bool unlock = i < maxLevel - 1;
                _lineContainer.GetChild(i).GetComponent<Image>().color = unlock ? _colorYellow : _colorGray;
            }

            for (int i = 0; i < _lineCryTemple.childCount; i++)
            {
                bool unlock = i + 20 < maxLevel - 1;
                _lineCryTemple.GetChild(i).GetComponent<Image>().color = unlock ? _colorYellow : _colorGray;
            }

            AdsManager.Instance.OnHideBanner();
        }

        private void OnClickPlayLevel(int level)
        {

            _actionLevel?.Invoke(level);
        }

        private void OnClickBackToMain()
        {
            //   _actionBack?.Invoke();
            Utils.FadeOut(this.gameObject, () =>
            {
                this.gameObject.SetActive(false);
            });
        }

        private void OnClickNext()
        {
            _index++;
            this.OnShowLevel();
        }

        private void OnClickPreview()
        {
           
            _index--;
            this.OnShowLevel();
        }

        private void OnShowLevel()
        {
            int mod = Mathf.Abs(_index) % 2;
            bool isLightTemple = mod == 0;
            _lvLightTemple.SetActive(isLightTemple);
            _lvCrytalTemple.SetActive(!isLightTemple);
            _txtTitle.text = isLightTemple ? "The Light Temple" : "The Crystal Temple";
        }

        public void SetEvent(UnityAction<int> play,UnityAction back)
        {
            this._actionLevel = play;
            this._actionBack = back;
        }
             
    }
}
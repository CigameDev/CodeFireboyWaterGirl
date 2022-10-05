using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Fireboy
{
    public class PnPauseCtrl : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtPlayInfo;
        [SerializeField] private UIButton _btnSetting;
        [SerializeField] private UIButton _btnContinue;
        [SerializeField] private UIButton _btnRestart;
        [SerializeField] private UIButton _btnQuit;
        [SerializeField] private GameObject _pnSetting;

        // Start is called before the first frame update
        void Start()
        {
            _btnSetting?.onClick.AddListener(this.OnClickSetting);
            _btnContinue?.onClick.AddListener(this.OnClickContinue);
            _btnRestart?.onClick.AddListener(this.OnClickRestart);
            _btnQuit?.onClick.AddListener(this.OnClickQuit);
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);
            AdsManager.Instance?.OnShowBanner();
            string temple = UIManager.Instance.CurrentLevel <= 20 ? "The Light Temple" : "The Crystal Temple";
            _txtPlayInfo.text = temple + " - Level " + UIManager.Instance.CurrentLevel;
        }

        private void OnDisable()
        {
            AdsManager.Instance?.OnHideBanner();
        }

        private void OnClickSetting()
        {
            _pnSetting.SetActive(true);
        }

        private void OnClickContinue()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                UIManager.Instance.OnResume();
                this.gameObject.SetActive(false);
            });
        }

        private void OnClickRestart()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                UIManager.Instance.OnRetry();
                this.gameObject.SetActive(false);
            });
        }

        private void OnClickQuit()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                UIManager.Instance.OnEndGame();
                this.gameObject.SetActive(false);
            });
        }

    }
}
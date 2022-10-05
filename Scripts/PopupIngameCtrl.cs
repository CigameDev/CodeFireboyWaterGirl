using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Fireboy
{
    public class PopupIngameCtrl : MonoBehaviour
    {
        [SerializeField] private Button _btnMenu;
        [SerializeField] private Button _btnRetry;
        [SerializeField] private Button _btnSkip;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtDesciption;

        private Animator _myAnim;
        private UnityAction _evtMenu, _evtRetry, _evtSkip;
        private int _cacheEvent;

        // Start is called before the first frame update
        void Awake()
        {
            _myAnim = this.GetComponent<Animator>();

            _btnMenu?.onClick.AddListener(this.OnClickMenu);
            _btnRetry?.onClick.AddListener(this.OnClickRetry);
            _btnSkip?.onClick.AddListener(this.OnClickSkip);
        }

        public void ShowPopup(string title,string des,string menu,string retry,string skip,UnityAction evtMenu,UnityAction evtRetry,UnityAction evtSkip)
        {
            _txtTitle.text = title.ToUpper();
            _txtDesciption.text = des.ToUpper();
            _btnMenu.GetComponentInChildren<Text>().text = menu.ToUpper();
            _btnRetry.GetComponentInChildren<Text>().text = retry.ToUpper();
            _btnSkip.GetComponentInChildren<Text>().text = skip.ToUpper();

            _evtMenu = evtMenu;
            _evtRetry = evtRetry;
            _evtSkip = evtSkip;

            if(_myAnim == null)
                _myAnim = this.GetComponent<Animator>();
            _myAnim.SetBool("show", true);
        }

        private void OnEnable()
        {
            AdsManager.Instance?.OnShowBanner();
        }

        private void OnDisable()
        {
            AdsManager.Instance?.OnHideBanner();    
        }

        private void OnClickMenu()
        {
            this.OnHidePopup(1);
        }

        private void OnClickRetry()
        {
            this.OnHidePopup(2);
        }

        private void OnClickSkip()
        {
            this.OnHidePopup(3);
        }

        private void OnHidePopup(int evt)
        {
            _myAnim.SetBool("show", false);
            _cacheEvent = evt;
        }

        private void ActionAfterHide()
        {
            switch (_cacheEvent)
            {
                case 1: _evtMenu?.Invoke();break;
                case 2:_evtRetry?.Invoke();break;
                case 3:_evtSkip?.Invoke();break;
                default: break;
            }
        }
    }
}
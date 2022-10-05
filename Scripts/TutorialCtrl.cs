using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Fireboy
{
    public class TutorialCtrl : MonoBehaviour
    {
        public static TutorialCtrl Instance;

        [SerializeField] private List<GameObject> _listObject;
        [SerializeField] private GameObject _objTitleCollect;
        [SerializeField] private Button _btnMoveLeft;
        [SerializeField] private Button _btnMoveRight;
        [SerializeField] private Button _btnJump;
        [SerializeField] private Button _btnSwap;
        [SerializeField] private ETCJoystick _btnZoom;
        [SerializeField] private InputCtrl _inputCtrl;
        [SerializeField] private Image _mask;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            _btnSwap?.onClick.AddListener(this.OnClickSwap);
            _btnJump?.onClick.AddListener(this.OnClickJump);
            _btnZoom?.onTouchStart?.AddListener(this.OnClickHideMask);
            _btnZoom?.onTouchUp?.AddListener(this.OnUpZoom);
        }


        private void OnEnable()
        {
            this.OnActiveButton(_btnMoveRight.GetInstanceID());
            this.ActiveTitle(0);
            _objTitleCollect.SetActive(true);
            _mask.enabled = true;
            _btnMoveRight.transform.SetAsLastSibling();
        }

        private void OnActiveButton(int id)
        {
            _btnJump.gameObject.SetActive(id == _btnJump.GetInstanceID());
            _btnMoveLeft.gameObject.SetActive(id == _btnMoveLeft.GetInstanceID());
            _btnMoveRight.gameObject.SetActive(id == _btnMoveRight.GetInstanceID());
            _btnSwap.gameObject.SetActive(id == _btnSwap.GetInstanceID());
            _btnZoom.gameObject.SetActive(id == _btnZoom.GetInstanceID());
        }

        private void ActiveTitle(int n)
        {
            for(int i = 0; i < _listObject.Count; i++)
            {
                _listObject[i].SetActive(n == i);
            }
        }

        public void OnHideTitleCollect()
        {
            _objTitleCollect.SetActive(false);
        }

        public void OnRedDone()
        {
            _inputCtrl.OnUpMove();
            this.OnActiveButton(_btnZoom.GetInstanceID());
            this.ActiveTitle(2);
            _mask.enabled = true;
            _btnZoom.transform.SetAsLastSibling();
        }

        public void OnDownMoveLeft()
        {
            _mask.enabled = false;
            _inputCtrl.OnMoveRight();
        }

        public void OnUpMoveLeft()
        {
            _inputCtrl.OnUpMove();
        }

        private void OnClickHideMask()
        {
            _mask.enabled = false;
        }

        private void OnUpZoom()
        {
            this.OnActiveButton(_btnSwap.GetInstanceID());
            this.ActiveTitle(3);
            _mask.enabled = true;
            _btnSwap.transform.SetAsLastSibling();
        }

        private void OnClickSwap()
        {
            this.OnActiveButton(_btnJump.GetInstanceID());
            this.ActiveTitle(1);
            _mask.enabled = true;
            _btnJump.transform.SetAsLastSibling();
        }

        private void OnClickJump()
        {
            _mask.enabled = false;
            this.OnActiveButton(_btnMoveRight.GetInstanceID());
            this.ActiveTitle(0);
        }

        public void OnComplete()
        {
            this.ActiveTitle(5);
            _btnJump.gameObject.SetActive(true);
            _btnMoveLeft.gameObject.SetActive(true);
            _btnMoveRight.gameObject.SetActive(true);
            _btnSwap.gameObject.SetActive(true);
            _btnZoom.gameObject.SetActive(true);
            this.gameObject.SetActive(false);

            _btnSwap?.onClick.RemoveListener(this.OnClickSwap);
            _btnJump?.onClick.RemoveListener(this.OnClickJump);
            _btnZoom?.onTouchStart?.RemoveListener(this.OnClickHideMask);
            _btnZoom?.onTouchUp?.RemoveListener(this.OnUpZoom);
        }
    }
}

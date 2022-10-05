using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;

namespace Fireboy
{
    public class InputCtrl : MonoBehaviour
    {
        [SerializeField] private Text _txtTimer;
        [SerializeField] private List<Image> _listButton;
        [SerializeField] private Animator _animSwap;
        [SerializeField] private TextMeshProUGUI _txtGold;
        [SerializeField] private ETCJoystick _jsZoom;

        private float _targetAxist, _currentAxis;
        private float _timeCount;
        private UnityAction _actionPause;

        private float _sizeCam;

        public float GetTimePlay
        {
            private set { }
            get
            {
                return _timeCount;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _jsZoom.onMove.AddListener(this.OnZoomMove);
            _jsZoom.onMoveEnd.AddListener(this.OnEndMove);
            _jsZoom.onTouchStart.AddListener(this.OnZoomDown);
        }

        private void OnEnable()
        {
            AdsManager.Instance?.OnHideBanner();
            _txtGold.text = PlayerPrefs.GetInt(Key.TOTAL_COIN).ToString();
        }

        // Update is called once per frame
        void Update()
        {

            _timeCount += Time.deltaTime;
            int hour = (int)_timeCount / 60;
            int minus = (int) _timeCount - 60 * hour;
            _txtTimer.text = $"{hour.ToString("D2")}:{minus.ToString("D2")}";

            _currentAxis = Mathf.MoveTowards(_currentAxis, _targetAxist, Time.deltaTime * 5f);
#if !UNITY_EDITOR
            PlayerManager.Instance?.OnMoveByControl(_currentAxis);
#endif

        }

        public void SetEvent(UnityAction pause)
        {
            this._actionPause = pause;
        }

        public void AddCoin(int sl)
        {
            Utils.AddCoin(sl, _txtGold);
        }

        public void OnMoveLeft()
        {
            _targetAxist = -1f;
            this.OnDownImage(_listButton[0], 0.6f);
        }

        public void OnMoveRight()
        {
            _targetAxist = 1f;
            this.OnDownImage(_listButton[1], 0.6f);
        }

        public void OnUpMove()
        {
            _targetAxist = 0f;
            this.OnDownImage(_listButton[0], 1f);
            this.OnDownImage(_listButton[1], 1f);
        }

        public void OnJump()
        {
            PlayerManager.Instance?.OnJumpByControl();
            this.OnDownImage(_listButton[2], 0.6f);
        }

        public void OnUpJump()
        {
            this.OnDownImage(_listButton[2], 1f);
        }

        public void OnSwap()
        {
            PlayerManager.Instance?.SwapByControl();
            this.SetSwap();
        }

        public void OnClickPause()
        {
            _actionPause?.Invoke();
        }

        public void ResetUI()
        {
            for (int i = 0; i < _listButton.Count; i++)
            {
                _listButton[i].color = new Color(229f/255f,40f/255f,40/255f,1f);
            }
            _animSwap.Play("swap-boy");
            _targetAxist = _currentAxis = 0f;
            _timeCount = 0f;
        }

        private void SetSwap()
        {
            string anim = PlayerManager.Instance.FireBoySelected ? "swap-boy" : "swap-girl";
            _animSwap.Play(anim);

            Color cl = PlayerManager.Instance.FireBoySelected ? new Color(229f / 255f, 40f / 255f, 40 / 255f, 1f) : new Color(0f / 255f, 223f / 255f, 251 / 255f, 1f);
            for(int i = 0; i < _listButton.Count; i++)
            {
                _listButton[i].color = cl;
            }
        }

        private void OnDownImage(Image img,float alpha)
        {
            Color cl = img.color;
            cl.a = alpha;
            img.color = cl;
        }

        private Transform _cachePlayer,_camTransform,_dummyPlayer;
        private Vector3 _oldCamPos,_oldMovePos;

        private void OnZoomDown()
        {
            if(_dummyPlayer == null)
            {
                _dummyPlayer = new GameObject("Dummy Player").transform;
                DontDestroyOnLoad(_dummyPlayer);
            }

            _cachePlayer = ProCamera2D.Instance.CameraTargets[0].TargetTransform;
            _camTransform = ProCamera2D.Instance.transform;
            _dummyPlayer.position = _camTransform.position;
            ProCamera2D.Instance.CameraTargets[0].TargetTransform = _dummyPlayer;
            _sizeCam = Camera.main.orthographicSize;
            float remain = _sizeCam + 1f;

            DOTween.To(() => _sizeCam, (x) =>
            {
                Camera.main.orthographicSize = x;
            }, remain, 0.5f);
        }

        private void OnZoomMove(Vector2 direct)
        {
            _dummyPlayer.Translate(direct.normalized * 2f * Time.deltaTime);

            if(Vector2.Distance(_camTransform.position,_dummyPlayer.position) >= 0.5f)
            {
                _dummyPlayer.position = _oldCamPos;
            }

            _oldCamPos = _dummyPlayer.position;
           
        }

        private void OnEndMove()
        {
            ProCamera2D.Instance.CameraTargets[0].TargetTransform = _cachePlayer;
            float current = Camera.main.orthographicSize;
            DOTween.To(() => current, (x) =>
            {
                Camera.main.orthographicSize = x;
            }, _sizeCam, 0.5f);
        }
    }
}
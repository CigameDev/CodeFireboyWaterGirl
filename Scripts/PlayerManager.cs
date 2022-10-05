using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class PlayerManager : MonoBehaviour
    {
        private static PlayerManager _instance;
        public static PlayerManager Instance => _instance;


        private Character _fireboy, _watergirl, _currentChar;
        private bool _isFireboy;
        private bool _boyComplete, _girlComplete;
        private bool _missionComplete;

        [HideInInspector] public int Revive;

        public bool FireBoySelected
        {
            private set { }
            get
            {
                return _isFireboy;
            }
        }

        private bool IsFireboy//ham xac dinh la nam hay nu
        {
            set
            {
                _isFireboy = value;
                _currentChar?.OnDeselect();
                _currentChar = _isFireboy ? _fireboy : _watergirl;
                _currentChar?.OnSelect();
            }
            get
            {
                return _isFireboy;
            }
        }


        private void Awake()
        {
            _instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            AdsManager.Instance?.OnHideBanner();
            Revive = 0;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            float horizontal = Input.GetAxis("Horizontal");

        //    if(horizontal != 0)
                _currentChar?.Move(horizontal);

            if (Input.GetKeyDown(KeyCode.K))
            {
                IsFireboy = !IsFireboy;
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _currentChar?.Jump();
            }
#endif
        }

        public void OnMoveByControl(float axis)
        {
            if(!_missionComplete)
                _currentChar?.Move(axis);
        }

        public void OnJumpByControl()
        {
            if (!_missionComplete)
                _currentChar?.Jump();
        }

        public void SwapByControl()
        {
            if (!_missionComplete)
                IsFireboy = !IsFireboy;
        }

        public void CharacterRegister(Player type, Character ctrl)
        {
            if (type == Player.Boy) _fireboy = ctrl;
            else if (type == Player.Girl) _watergirl = ctrl;
            this.IsFireboy = true;
        }

        public void CharacterComplete(Player type, bool complete)
        {
            if (type == Player.Boy) _boyComplete = complete;
            else if (type == Player.Girl) _girlComplete = complete;

            if (_boyComplete && _girlComplete)
            {
                _missionComplete = true;
                StartCoroutine(IEDelayShowComplete());
                _fireboy.Stop();
                _watergirl.Stop();
            }
        }

        private IEnumerator IEDelayShowComplete()
        {
            yield return new WaitForSeconds(1f);
            UIManager.Instance?.OnMissionComplete();
        }
        public void CharacterDead()
        {
            _fireboy.OnDead();
            _watergirl.OnDead();
            UIManager.Instance?.OnMissionFail();
        }

        public void OnRevive()
        {
            _fireboy.OnRevive();
            _watergirl.OnRevive();
            Revive = 0;
        }

        public void OnChangeSkin(int id)
        {
            _fireboy.OnChangeSkin(id);
            _watergirl.OnChangeSkin(id);
        }
    }
}
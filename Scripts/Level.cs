using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace Fireboy
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private Button _btnLevel;
        [SerializeField] private Image _imgUnlock;
        [SerializeField] private Text _txtTime;
        [SerializeField] private List<Sprite> _listRank;
        [SerializeField] private int _level;
        private bool _isUnlock;

        private UnityAction<int> _actionPlay;

        // Start is called before the first frame update
        void Start()
        {
            _btnLevel?.onClick.AddListener(this.OnClickPlay);
        }

        private void OnEnable()
        {
            int status = PlayerPrefs.GetInt(Key.UNLOCK_LEVEL + _level, 0);
            _isUnlock = status != 0;

            if (_isUnlock)
            {
                int timeplay = PlayerPrefs.GetInt(Key.TIME_PLAY + _level, 0);
                int hour = timeplay / 60;
                int minus = timeplay - 60 * hour;
                _txtTime.text = $"{hour.ToString("D2")}:{minus.ToString("D2")}";

                if (timeplay < 150)
                {
                    _imgUnlock.sprite = _listRank[1];
                }
                else if (timeplay < 250)
                {
                    _imgUnlock.sprite = _listRank[2];
                }
                else
                {
                    _imgUnlock.sprite = _listRank[3];
                }
            }
            else
            {
                _txtTime.text = string.Empty;
                _imgUnlock.sprite = _listRank[0];
            }
        }

        private void OnClickPlay()
        {
            if (!_isUnlock) return;
            _actionPlay?.Invoke(_level);
        }

        public void SetEvent(UnityAction<int> actionplay)
        {
            this._actionPlay = actionplay;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    [ExecuteInEditMode]
    public class SwitchPessTime : SwitchPress
    {
        [SerializeField] private SpriteRenderer _sprTime;
        [SerializeField] private List<Sprite> _listCountDown;
        [SerializeField] private SpriteRenderer _sprDark;

        private Coroutine _ieTimeDown;


        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
         //   if (!_editorPlaying)
            {
                Utils.SetDotColor(_color, _light);
                Utils.SetDotColor(_color, _sprTime);
                Utils.SetDarkColor(_color, _sprDark);
            }
#endif
            if (_listPress.Count > 0)
            {
                float distance = Mathf.Abs(_myColider.bounds.center.x - _listPress[0].bounds.center.x);

                if (_listPress.Count > 1)
                {
                    for (int i = 1; i < _listPress.Count; i++)
                    {
                        float dis = Mathf.Abs(_myColider.bounds.center.x - _listPress[i].bounds.center.x);
                        if (dis < distance)
                            distance = dis;
                    }
                }

                float del = distance > 0.3f ? (distance / _delta) * 0.2f - 0.2f : (distance - 0.3f);
                float pos = Mathf.Clamp(del, -0.2f, 0f);
                _switch.localPosition = new Vector3(0f, pos, 0f);

                if (pos < -0.07f && !_isEventOn)
                {
                    _event?.SwitchOn();
                    _isEventOn = true;

                    if (_listEvt.Count > 0)
                    {
                        for (int i = 0; i < _listEvt.Count; i++)
                            _listEvt[i].SwitchOn();
                    }
                    _sprTime.gameObject.SetActive(true);
                    _sprTime.sprite = _listCountDown[0];

                    if (_ieTimeDown != null) StopCoroutine(_ieTimeDown);
                }

                if (pos > -0.05f && _isEventOn)
                {
                    _isEventOn = false;
                    _ieTimeDown = StartCoroutine(IECountDown());
                }
            }
            else
            {
                if (_isEventOn)
                {
                    _ieTimeDown = StartCoroutine(IECountDown());
                }

                _isEventOn = false;
                if (Mathf.Abs(_switch.localPosition.y) > 0.01f)
                {
                    Vector2 direct = Mathf.Sign(_switch.localPosition.y) * Vector2.up * -1f;
                    _switch.Translate(direct * Time.deltaTime);
                }
            }
        }

        private void EventOff()
        {
            _event?.SwitchOff();

            if (_listEvt.Count > 0)
            {
                for (int i = 0; i < _listEvt.Count; i++)
                    _listEvt[i].SwitchOff();
            }
            _sprTime.gameObject.SetActive(false);
        }

        private IEnumerator IECountDown()
        {
            for(int i = 0; i < _listCountDown.Count; i++)
            {
                _sprTime.sprite = _listCountDown[i];
                yield return new WaitForSeconds(1.3f);
            }

            this.EventOff();
        }

    }
}
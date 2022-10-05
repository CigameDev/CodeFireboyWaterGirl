using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    [ExecuteInEditMode]
    public class SwitchPress : MonoBehaviour
    {
        [SerializeField] protected Transform _switch;
        [SerializeField] protected SpriteRenderer _light;
        [SerializeField] protected ColorDot _color;
        [SerializeField] protected SwitchEvent _event;
        [SerializeField] protected List<SwitchEvent> _listEvt;

        protected List<Collider2D> _listPress;
        protected BoxCollider2D _myColider;
        protected bool _isEventOn;
        protected float _delta;
        [SerializeField] protected bool _editorPlaying;

        // Start is called before the first frame update
        void Start()
        {
            _listPress = new List<Collider2D>();
            _myColider = this.GetComponent<BoxCollider2D>();
            _editorPlaying = true;            
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if(!_editorPlaying)
                Utils.SetDotColor(_color, _light);
#endif
            if(_listPress.Count > 0)
            {
                float distance = Mathf.Abs(_myColider.bounds.center.x - _listPress[0].bounds.center.x);

                if(_listPress.Count > 1)
                {
                    for(int i = 1; i < _listPress.Count; i++)
                    {
                        float dis = Mathf.Abs(_myColider.bounds.center.x - _listPress[i].bounds.center.x);
                        if (dis < distance)
                            distance = dis;
                    }
                }

                float del = distance > 0.3f ? (distance / _delta) * 0.2f - 0.2f : (distance - 0.3f);
                float pos = Mathf.Clamp(del, -0.2f, 0f);
                _switch.localPosition = new Vector3(0f, pos, 0f);

                if(pos < -0.07f && !_isEventOn)
                {
                    _event?.SwitchOn();
                    _isEventOn = true;

                    if(_listEvt.Count > 0)
                    {
                        for (int i = 0; i < _listEvt.Count; i++)
                            _listEvt[i].SwitchOn();
                    }
                }

                if(pos > -0.05f && _isEventOn)
                {
                    _event?.SwitchOff();
                    _isEventOn = false;

                    if (_listEvt.Count > 0)
                    {
                        for (int i = 0; i < _listEvt.Count; i++)
                            _listEvt[i].SwitchOff();
                    }
                }
            }
            else
            {
                if (_isEventOn)
                {
                    _event?.SwitchOff();

                    if (_listEvt.Count > 0)
                    {
                        for (int i = 0; i < _listEvt.Count; i++)
                            _listEvt[i].SwitchOff();
                    }
                }

                _isEventOn = false;
                if(Mathf.Abs(_switch.localPosition.y) > 0.01f)
                {
                    Vector2 direct = Mathf.Sign(_switch.localPosition.y) * Vector2.up * -1f;
                    _switch.Translate(direct * Time.deltaTime);
                }
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!_listPress.Contains(collision))
            {
                _listPress.Add(collision);
                SoundManager.Instance?.PlaySoundInGame(SoundIngame.Platform);
            }
            _delta = Mathf.Abs(_myColider.bounds.center.x - collision.bounds.center.x);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (_listPress.Contains(collision))
            {
                _listPress.Remove(collision);
                SoundManager.Instance?.PlaySoundInGame(SoundIngame.Platform);
            }
        }
    }
}
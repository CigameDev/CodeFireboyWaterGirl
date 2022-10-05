using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    [ExecuteInEditMode]
    public class Elevator : SwitchEvent
    {
        [SerializeField] private SpriteRenderer _sprLight;
        [SerializeField] private Color _lightOn;
        [SerializeField] private Color _lightOff;
        [SerializeField] private Direction _direct;
        [SerializeField] private float _distance;
        [SerializeField] private Transform _elevator;

        private float _target;
        private bool _editorPlaying;

        public override void SwitchOff()
        {
            _sprLight.color = _lightOff;
            _target = 0f;
        }

        public override void SwitchOn()
        {
            _sprLight.color = _lightOn;
            _target = _distance;
        }

        // Start is called before the first frame update
        void Start()
        {
            this.SwitchOff();
            _editorPlaying = true;
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (!_editorPlaying)
            {
                _sprLight.color = _lightOff;
            }

#endif
            if(_direct == Direction.Horizontal)
            {
                float x = _target - this._elevator.localPosition.x;
                if(Mathf.Abs(x) > 0.01f)
                {
                    Vector2 direc = Mathf.Sign(x) * Vector2.right;
                    this._elevator.Translate(direc  * Time.deltaTime);
                }
            }
            else if(_direct == Direction.Vertical)
            {
                float sign = _target - this._elevator.localPosition.y;
                if (Mathf.Abs(sign) > 0.01f)
                {
                    Vector2 direct = Mathf.Sign(sign) * Vector2.up;
                    this._elevator.Translate(direct  * Time.deltaTime);
                }
            }
        }

    }
}
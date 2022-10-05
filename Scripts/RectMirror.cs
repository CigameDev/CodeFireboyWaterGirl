using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class RectMirror : SwitchEvent
    {
        [SerializeField] private float _startAngle;
        [SerializeField] private float _targetAngle;

        private float _currentAngle;
        private float _countAngle;

        // Start is called before the first frame update
        void Start()
        {
            this.transform.eulerAngles = Vector3.forward * _startAngle;
            _currentAngle = _countAngle = _startAngle;
        }

        // Update is called once per frame
        void Update()
        {
            float temp = Mathf.MoveTowards(_countAngle, _currentAngle, Time.deltaTime * 60f);
            this.transform.eulerAngles = Vector3.forward * temp;
            _countAngle = temp;
        }

        public override void SwitchOn()
        {
            _currentAngle = _targetAngle;
        }

        public override void SwitchOff()
        {
            _currentAngle = _startAngle;
        }
    }
}
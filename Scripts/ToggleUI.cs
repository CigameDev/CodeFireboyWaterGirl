using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Fireboy
{
    public class ToggleUI : MonoBehaviour
    {
        public bool IsOn
        {
            set
            {
                _isOn = value;
                _tgOn.SetActive(_isOn);
                _tgOff.SetActive(!_isOn);
            }
            get { return _isOn; }
        }

        public UnityEvent<bool> OnChangeValue;

        private bool _isOn;

        [SerializeField] private GameObject _tgOn;
        [SerializeField] private GameObject _tgOff;

        public void OnClickSwap()
        {
            IsOn = !IsOn;
            OnChangeValue?.Invoke(IsOn);
        }


    }
}

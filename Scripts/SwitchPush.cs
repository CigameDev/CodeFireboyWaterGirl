using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class SwitchPush : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> _listLight;
        [SerializeField] private Color _lightOn;
        [SerializeField] private Color _lightOff;
        [SerializeField] private SwitchEvent _event;
        [SerializeField] private List<SwitchEvent> _listEvt;

        private Switch _state;

        // Start is called before the first frame update
        void Start()
        {
            _state = Switch.Off;
            this.SwitchLight(_lightOff);
            this.transform.eulerAngles = Vector3.forward * -45f;
        }

        // Update is called once per frame
        void Update()
        {
            float angle = this.transform.localEulerAngles.z - 180f;
            if (angle < -140f && _state == Switch.Off)
            {
                _state = Switch.On;
                this.SwitchLight(_lightOn);
                _event?.SwitchOn();
                SoundManager.Instance?.PlaySoundInGame(SoundIngame.Platform);

                if(_listEvt.Count > 0)
                {
                    for (int i = 0; i < _listEvt.Count; i++)
                        _listEvt[i].SwitchOn();
                }
            }
            else if (angle > 130f && _state == Switch.On)
            {
                _state = Switch.Off;
                this.SwitchLight(_lightOff);
                _event?.SwitchOff();
                SoundManager.Instance?.PlaySoundInGame(SoundIngame.Platform);

                if (_listEvt.Count > 0)
                {
                    for (int i = 0; i < _listEvt.Count; i++)
                        _listEvt[i].SwitchOff();
                }
            }
        }

        private void SwitchLight(Color color)
        {
            for(int i = 0; i < _listLight.Count; i++)
            {
                _listLight[i].color = color;
            }
        }
    }
}

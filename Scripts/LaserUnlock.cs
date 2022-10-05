using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class LaserUnlock : MonoBehaviour, LaserEvent
    {
        [SerializeField]private LaserColor _lockColor;
        [SerializeField] private SwitchEvent _event;

        public LaserColor Color()
        {
            return _lockColor;
        }

        public void LaserClose()
        {
            _event?.SwitchOff();
        }

        public void LaserOpen()
        {
            _event?.SwitchOn();
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.LightPusher);
        }


    }
}

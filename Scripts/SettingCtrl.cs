using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fireboy
{
    public class SettingCtrl : MonoBehaviour
    {
        [SerializeField] private ToggleUI _tgNotify;
        [SerializeField] private ToggleUI _tgVibration;
        [SerializeField] private Slider _slSoundFx;
        [SerializeField] private Slider _slMusic;
        [SerializeField] private UIButton _btnClose;
        [SerializeField] private SoundManager _soundCtrl;

        // Start is called before the first frame update
        void Start()
        {
            _btnClose?.onClick.AddListener(this.OnClickHideSetting);
            _tgNotify.OnChangeValue.AddListener(this.OnChangeNotify);
            _tgVibration?.OnChangeValue.AddListener(this.OnChangeVibrate);
            _slSoundFx?.onValueChanged.AddListener(this.OnChangeSoundFX);
            _slMusic?.onValueChanged.AddListener(this.OnChangeMusicFX);
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.transform.GetChild(1).gameObject);
            _tgNotify.IsOn = PlayerPrefs.GetInt(Key.NOTIFICATIONS, 0) == 0;
            _tgVibration.IsOn = PlayerPrefs.GetInt(Key.VIBRATION, 0) == 0;
            _slSoundFx.value = PlayerPrefs.GetFloat(Key.SOUND_FX, 1f);
            _slMusic.value = PlayerPrefs.GetFloat(Key.MUSIC_FX, 1f);
        }
        
        private void OnClickHideSetting()
        {
            Utils.FadeOut(this.transform.GetChild(1).gameObject, () => {
                this.gameObject.SetActive(false);
            });
        }

        private void OnChangeNotify(bool isOn)
        {
            PlayerPrefs.SetInt(Key.NOTIFICATIONS, isOn ? 1 : 0);
        }

        private void OnChangeVibrate(bool isOn)
        {
            PlayerPrefs.SetInt(Key.VIBRATION, isOn ? 1 : 0);
        }

        private void OnChangeSoundFX(float vl)
        {
            PlayerPrefs.SetFloat(Key.SOUND_FX, vl);
            _soundCtrl.SetVolumSound(vl);
        }

        private void OnChangeMusicFX(float vl)
        {
            PlayerPrefs.SetFloat(Key.MUSIC_FX, vl);
            _soundCtrl.SetVolumeMusic(vl);
        }

    }
}
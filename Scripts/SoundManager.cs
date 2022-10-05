using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;
        public static SoundManager Instance => _instance;

        [SerializeField] private List<SoundBGElement> _listSoundBG;
        [SerializeField] private List<SoundIngameElement> _listSoundIngame;


        private AudioSource _audioBG;
        private float _volumSound, _volumMusic;


        private void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            _audioBG = this.GetComponent<AudioSource>();

            this.SetVolumeMusic(PlayerPrefs.GetFloat(Key.MUSIC_FX, 1f));
            this.SetVolumSound(PlayerPrefs.GetFloat(Key.SOUND_FX, 1f));
        }

        public void PlaySoundBG(SoundBG keysound)
        {
            var audio = this.GetAudioBG(keysound);
            if (_audioBG.clip == audio) return;

            if(audio != null)
            {
                _audioBG.clip = audio;
                _audioBG.Play();
            }
        }

        public void PlaySoundInGame(SoundIngame keysound)
        {

            var audio = this.GetAudioInGame(keysound);
            if(audio != null)
            {
                _audioBG.PlayOneShot(audio, _volumSound);
            }
        }

        private AudioClip GetAudioBG(SoundBG key)
        {
            for (int i = 0; i < _listSoundBG.Count; i++)
            {
                if (_listSoundBG[i].SoundKey == key)
                    return _listSoundBG[i].Audio;
            }

            return null;
        }

        private AudioClip GetAudioInGame(SoundIngame key)
        {
            for (int i = 0; i < _listSoundIngame.Count; i++)
            {
                if (_listSoundIngame[i].SoundKey == key)
                    return _listSoundIngame[i].Audio;
            }

            return null;
        }

        public void SetVolumSound(float vl)
        {
            _volumSound = vl;
        }

        public void SetVolumeMusic(float vl)
        {
            _volumMusic = vl;
            _audioBG.volume = vl;
        }

    }
}
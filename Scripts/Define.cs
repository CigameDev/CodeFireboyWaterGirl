using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public enum Player
    {
        None, Boy, Girl, Both
    }

    public enum Water
    {
        RED, BLUE, GREEN, BLACK
    }

    public enum Switch
    {
        None, On, Off
    }

    public enum Direction
    {
        Vertical, Horizontal
    }

    public enum LaserColor
    {
        YELLOW, BLUE
    }

    public enum Screen
    {
        Menu, Popup, LevelSelect, InputPlayer
    }

    public interface Character
    {
        void Jump();
        void Move(float horizontal);
        void OnSelect();
        void OnDeselect();
        void OnDead();
        void Stop();
        void OnRevive();
        void OnChangeSkin(int id);
    }

    public interface LaserEvent
    {
        void LaserOpen();
        void LaserClose();
        LaserColor Color();
    }


    public enum State
    {
        IDLE, MOVE, JUMP, DEAD
    }

    public enum SoundBG
    {
        Menu, LevelMusic, LevelMusic_Dark, LevelMusic_Speed
    }

    public enum SoundIngame
    {
        Death, Diamond, OpenDoor, Jump_FB, Jump_WG, GameOver, LightPusher,MissionComplete,UIClick,TrueDoor,CharacterPicked, FireWork,CollectItem,Platform,CollectKey,OpenGift,SpinRotate,Revive
    }

    public enum ColorDot
    {
        None,Pink,Green,Red,Yellow,White,Blue
    }

    public class SwitchEvent : MonoBehaviour
    {
        public virtual void SwitchOn() { }
        public virtual void SwitchOff() { }
    }

    [System.Serializable]
    public class SoundElement
    {
        public string AudioName;
        public AudioClip Audio;
    }

    [System.Serializable]
    public class SoundBGElement : SoundElement
    {
        public SoundBG SoundKey;
    }

    [System.Serializable]
    public class SoundIngameElement : SoundElement
    {
        public SoundIngame SoundKey;
    }

    public class TagDefine
    {
        public static string FIRE_BOY = "fireboy";
        public static string WATER_GIRL = "watergirl";
        public static string WATER_TRAP = "water";
        public static string GEM = "gem";
        public static string DOOR = "door";
        public static string LASER = "laser";
        public static string BOX = "box";
        public const string  CHECKPOINT = "checkpoint";
    }

    public class Key
    {
        public const string SELECT_SKIN = "select-skin";
        public const string SKIN_ID = "skin-";
        public const string MAX_LEVEL = "max-level";
        public const string UNLOCK_LEVEL = "unlock-level-";
        public const string TIME_PLAY = "time-play-";
        public const string TOTAL_COIN = "total-coin";
        public const string TIME_LAST_SPIN = "time-last-spin";
        public const string TOTAL_ADS_SPIN = "total-ads-spin";
        public const string GIFT_ID = "gift-id-";
        public const string GIFT_TIME_REFRESH = "gift-time-refresh";
        public const string GIFT_LIST = "gift-list";
        public const string RATE_GAME = "rate-game";
        public const string RATE_COUNT = "rate-count";
        public const string REWARD_OLD_DAY = "old-day-reward";
        public const string REWARD_TOTAL_DAY = "total-day-reward";

        // setting key
        public const string NOTIFICATIONS = "setting-notifications";
        public const string VIBRATION = "setting-vibration";
        public const string SOUND_FX = "setting-sound";
        public const string MUSIC_FX = "setting-music";
    }
}

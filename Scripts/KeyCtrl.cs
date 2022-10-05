using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    [ExecuteInEditMode]
    public class KeyCtrl : MonoBehaviour
    {
        [SerializeField] private Player _type;
        [SerializeField] private Sprite _blueKey;
        [SerializeField] private Sprite _redKey;
        [SerializeField] private ParticleSystem _fxRed;
        [SerializeField] private ParticleSystem _fxBlue;


        // Start is called before the first frame update
        void Start()
        {
            this.GetComponentInChildren<SpriteRenderer>().sprite = _type == Player.Boy ? _redKey : _blueKey;
            this.gameObject.name = _type == Player.Boy ? "RedKey" : "BlueKey";
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == Const.TAG_FIREBOY && _type == Player.Boy)
            {
                this.GetComponent<Collider2D>().enabled = false;
                _fxRed.Play();
                SoundManager.Instance.PlaySoundInGame(SoundIngame.CollectKey);
                UIManager.Instance?.CollectRedKey(this.transform);
            }
            else if(collision.tag == Const.TAG_WATERGIRL && _type == Player.Girl)
            {
                this.GetComponent<Collider2D>().enabled = false;
                _fxBlue.Play();
                SoundManager.Instance.PlaySoundInGame(SoundIngame.CollectKey);
                UIManager.Instance?.CollectBlueKey(this.transform);
            }
        }


    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    [ExecuteInEditMode]
    public class TelePort : SwitchEvent
    {
        [SerializeField] private ColorDot _dotColor;
        [SerializeField] private List<SpriteRenderer> _listDot;
        [SerializeField] private GameObject _lightDoor;
        [SerializeField] private TelePort _connectPort;
        [SerializeField] private bool _disableDoor;

        private Collider2D _myCollider;
        private GameObject _player;
        [SerializeField] private bool _editorPlaying;

        // Start is called before the first frame update
        void Awake()
        {
            _myCollider = this.GetComponent<Collider2D>();
        }

        private void Start()
        {
            _editorPlaying = true;
        }

        private void Update()
        {
#if UNITY_EDITOR 
            if (!_editorPlaying)
            {
                for (int i = 0; i < _listDot.Count; i++)
                    Utils.SetDotColor(_dotColor, _listDot[i]);

                if (_disableDoor) this.SwitchOff();
                else this.SwitchOn();
            }
#endif

            if(_player != null)
            {
                if (Mathf.Abs(_player.transform.position.x - this.transform.position.x) > 0.2f || Mathf.Abs(_player.transform.position.y - this.transform.position.y) > 2f)
                    _player = null;
            }
        }


        public override void SwitchOn()
        {
            _lightDoor.SetActive(true);
            _myCollider.enabled = true;
        }

        public override void SwitchOff()
        {
            _lightDoor.SetActive(false);
            _myCollider.enabled = false;
        }

        public void OnTeleport(GameObject obj,float disX,float disY)
        {
            _player = obj;
            Vector3 dirPush = (this.transform.right * disX).normalized;
            obj.transform.position = this.transform.position + this.transform.up * disY + dirPush * 0.5f;
            Rigidbody2D rigid = obj.GetComponent<Rigidbody2D>();
            rigid.velocity = Vector2.zero;
            rigid.AddForce(dirPush * 100f);

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == _player) return;

            float disx = (collision.transform.position.x - this.transform.position.x) * this.transform.right.x;
            float disY = collision.transform.position.y - this.transform.position.y;
            _connectPort.OnTeleport(collision.gameObject, disx, disY);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject == _player)
                _player = null;
        }
    }
}
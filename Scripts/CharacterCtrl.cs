using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Com.LuisPedroFonseca.ProCamera2D;
using DG.Tweening;

namespace Fireboy
{

    public class CharacterCtrl : MonoBehaviour, Character
    {
         private float _mySpeed;
         private float _jumpForce;

        private Player _player;
        private Rigidbody2D _myRigid2D;
        private Transform _checkGround;
        private bool _isSelect;
        private AnimCtrl _myAnim;
        private Transform _bodyCharacter;
        private State _state;
        private bool _isPush;
        private bool _onGround;
        private GameObject _arrow;
        private GameObject _checkPoint;
        private ParticleSystem _fxRevive;
        private Vector3 _beginScale;
        private bool _hasDead;

        private void Awake()
        {
            _myRigid2D = this.GetComponent<Rigidbody2D>();
            _checkGround = this.transform.Find("CheckGround");//tim thang nao ten la CheckGround(tim con cua nhan vat)
            _bodyCharacter = this.transform.Find("SpineCharacter");
            int skin = PlayerPrefs.GetInt(Key.SELECT_SKIN);
            SkeletonAnimation skeleton = _bodyCharacter.GetComponent<SkeletonAnimation>();

            if (this.gameObject.tag == TagDefine.FIRE_BOY)
            {
                _player = Player.Boy;
                skeleton.Skeleton.SetSkin($"Char/B{skin}");
            }
            else if (this.gameObject.tag == TagDefine.WATER_GIRL)
            {
                _player = Player.Girl;
                skeleton.Skeleton.SetSkin($"Char/G{skin}");
            }
                
            _myAnim = new AnimCtrl(skeleton);
            _mySpeed = 4.35f;
            _jumpForce = 8.5f;
            _arrow = this.transform.GetChild(2).gameObject;//arrow la mui ten,khi ma lua chon nhan vat nao thi hien mui ten
            _fxRevive = this.transform.GetChild(3).GetComponent<ParticleSystem>();//hieu ung hat gan len nhan vat
            _beginScale = this.transform.localScale;//scale
        }

        // Start is called before the first frame update
        void Start()
        {
            PlayerManager.Instance?.CharacterRegister(_player, this);//xem lai sau
            _hasDead = false;//chua chet
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_isSelect)//false
            {
                _myRigid2D.velocity = new Vector2(0f, _myRigid2D.velocity.y);
            }

            if (_state == State.DEAD) return;//chet ti return

            _onGround = this.OnGround();//o mat dat


            switch (_state)
            {
                case State.IDLE://dung
                    _myAnim.PlayNewStableAnimation("boy_idle", true);
                    break;
                case State.MOVE://di chuyen

                    if (_onGround)//tren mat dat
                    {
                        if (_isPush)
                        {
                            _myAnim.PlayNewStableAnimation("boy_push_2", true);
                        }
                        else
                        {
                            _myAnim.PlayNewStableAnimation("boy_run", true);
                        }
                    //    _myRigid2D.gravityScale = 3f;
                    }
                    else
                    {
                        //   _state = State.JUMP;
                    }
                    break;
                case State.JUMP://nhay
            //        _myRigid2D.gravityScale = 2f;
                    if (_myRigid2D.velocity.y > 0f)//y >0
                    {
                        _myAnim.PlayNewStableAnimation("boy_jump_up", false);
                    }
                    else if (_myRigid2D.velocity.y < 0f)// <0 
                    {
                        //   _myAnim.PlayNewStableAnimation("boy_jump_down", false);
                        if (_onGround) //neu tren  mat dat
                        {
                            _state = State.IDLE;// cho dung
                        }
                    }

                    break;
                case State.DEAD://chet

                    break;
            }

            if (Mathf.Abs(_myRigid2D.velocity.y) > 0.1f && !_onGround)//gia tri tuyet doi y >0.1 va duoi mat dat
            {
                _state = State.JUMP;// cho nhay
            }

        }


        public void Jump()
        {
            if (_state == State.DEAD) return;//neu chet roi thi khong cho nhay

            if (this.OnGround())
            {
                _myRigid2D.velocity = Vector2.zero;
            //    _myRigid2D.gravityScale = 2f;
                this._myRigid2D.AddForce(Vector2.up * _jumpForce,ForceMode2D.Impulse);//add luc
                _state = State.JUMP;//chuyen sang trang thai nhay
                _myAnim.PlayNewStableAnimation("boy_jump_up", false);
                SoundManager.Instance?.PlaySoundInGame(_player == Player.Boy ? SoundIngame.Jump_FB : SoundIngame.Jump_WG);
            }
        }

        public void Move(float horizontal)//di chuyen
        {
            if (_state == State.DEAD) return;//chet thi thoi

            this._myRigid2D.velocity = new Vector2(horizontal * _mySpeed, this._myRigid2D.velocity.y);     

            if (Mathf.Abs(horizontal) > 0f)
            {
                if (_state != State.JUMP)//trang thai khac JUMP thi trang thai la nhay
                    _state = State.MOVE;
                float angle = horizontal > 0 ? 0f : 180f;//horizotal >0 thi angle =0 con lai thi 180
                _bodyCharacter.localEulerAngles = Vector3.up * angle;
            }
            else
            {
                if (_onGround)//neu tren mat dat
                {
                    _state = State.IDLE;//trang thai dung
                }
            }

        }

        public void OnDeselect()//bo chon(khong chon nhan vat do nua)
        {
            if (_state == State.DEAD) return;//chet thi khong lam gi
            _isSelect = false;//chon = false
            _myRigid2D.velocity = Vector2.zero;//dua ve 0
            _state = State.IDLE;//dua ve trang thai dung
            _arrow.SetActive(false);//an mui ten tren dau di
        }

        public void OnSelect()
        {
            if (_state == State.DEAD) return;//chet thi thoi
            _isSelect = true;//chon thi select =true
            _arrow.SetActive(true);//bat mui ten tren dau len
            _myAnim.AddStableAnimation("boy_win_1", false, true);
            if(_player == Player.Girl)//neu la girl thi them am thanh nay
                SoundManager.Instance?.PlaySoundInGame(SoundIngame.CharacterPicked);

            ProCamera2D.Instance.CameraTargets[0].TargetTransform = this.transform;//camera focus theo nhan vat(dung ben ngoai),tu code cung dc nhung ko muot
        }

        private bool OnGround()//check xem nhan vat co tren mat dat khong
        {
            Collider2D hit = Physics2D.OverlapCircle(_checkGround.position, 0.09f, 1 << 9 | 1 << 10);
            
            return hit;
        }

        public void OnDead()
        {
            if (_state == State.DEAD) return;//chet roi thi thoi
            _state = State.DEAD;//neu chua chet thi cho chet
            _myAnim.PlayNewStableAnimation("boy_lose", false);
            _myRigid2D.velocity = Vector2.zero;//dua ve 0 0
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_state == State.DEAD) return;

            if (collision.tag == TagDefine.WATER_TRAP)//neu va cham voi water trap
            {
                WaterTrap trap = collision.GetComponent<WaterTrap>();
                if (trap)
                {
                    if (_player != trap.Water)
                    {
                        
                        PlayerManager.Instance.Revive++;
                        _myRigid2D.velocity = Vector2.zero;
                        SoundManager.Instance?.PlaySoundInGame(SoundIngame.Death);
                        if (PlayerPrefs.GetInt(Key.VIBRATION) == 1)
                            Handheld.Vibrate();

                        _state = State.DEAD;
                        _myAnim.PlayNewStableAnimation("boy_lose", false, () =>
                        {
                            if(PlayerManager.Instance.Revive > 3 || _checkPoint == null)
                            {
                                this.gameObject.SetActive(false);
                                PlayerManager.Instance?.CharacterDead();
                                _hasDead = true;
                            }
                            else
                            {
                                this.Revive();
                            }
                        });
                        
                    }
                }
            }
            else if (collision.tag == TagDefine.GEM)
            {
                GemCollect gem = collision.GetComponent<GemCollect>();
                if (gem)
                {
                    if (_player == gem.GemType)
                    {
                        gem.OnCollect();
                        SoundManager.Instance?.PlaySoundInGame(SoundIngame.Diamond);
                        UIManager.Instance.AddCoinInGame(5);
                    }
                }
            }
            else if (collision.tag == TagDefine.DOOR)
            {
                DoorCtrl door = collision.GetComponent<DoorCtrl>();
                if (door)
                {
                    if (_player == door.DoorType && UIManager.Instance.HasCollectKey(_player))
                    {
                        door.OnOpenDoor(true);
                        PlayerManager.Instance?.CharacterComplete(_player, true);
                        _myAnim.AddStableAnimation("boy_door_1", false, true);
                        SoundManager.Instance?.PlaySoundInGame(SoundIngame.OpenDoor);
                        SoundManager.Instance?.PlaySoundInGame(SoundIngame.TrueDoor);
                    }
                }
            }
            else if(collision.tag == TagDefine.CHECKPOINT)
            {
                _checkPoint = collision.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == TagDefine.DOOR)
            {
                DoorCtrl door = collision.GetComponent<DoorCtrl>();
                if (door)
                {
                    if (_player == door.DoorType)
                    {
                        door.OnOpenDoor(false);
                        PlayerManager.Instance?.CharacterComplete(_player, false);
                    }
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == TagDefine.BOX)
            {
                _isPush = collision.transform.position.y > this.transform.position.y;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.tag == TagDefine.BOX)
            {
                _isPush = false;
            }
        }

        public void Stop()
        {
            _state = State.DEAD;
            _myRigid2D.velocity = Vector2.zero;
            _myRigid2D.isKinematic = true;
            _myAnim.PlayNewStableAnimation("boy_door_1", false);
        }

        private void Revive()
        {
            _state = State.IDLE;
            this.transform.localScale = Vector3.zero;
            this.transform.position = _checkPoint.transform.position;
            this.transform.DOScale(_beginScale, 0.5f).OnComplete(() => {
                SoundManager.Instance.PlaySoundInGame(SoundIngame.Revive);
            });
            _fxRevive.Play();
            this.transform.parent = null;
        }

        public void OnRevive()
        {
            this.gameObject.SetActive(true);
            if (_hasDead) this.Revive();
            _state = State.IDLE;
            _hasDead = false;
        }

        public void OnChangeSkin(int id)
        {
            _fxRevive.Play();
            _myAnim.SetSkin(id, _player);
            SoundManager.Instance.PlaySoundInGame(SoundIngame.Revive);
        }

    }
}
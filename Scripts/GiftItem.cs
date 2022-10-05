using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Spine.Unity;
using UnityEngine.UI;
using DG.Tweening;

namespace Fireboy
{
    public class GiftItem : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _skelBoy;
        [SerializeField]private SkeletonGraphic _skelGirl;
        [SerializeField] private UIButton _btnAds;
        [SerializeField] private UIButton _btnClaim;
        [SerializeField] private ParticleSystem _fxClaim;
        [SerializeField] private Button _btnMask;

        private int _cacheID;

        // Start is called before the first frame update
        void Start()
        {
            _btnAds?.onClick.AddListener(this.OnClickAds);
            _btnClaim?.onClick.AddListener(this.OnClickClaim);
            _btnMask?.onClick.AddListener(this.OnOpenMask);
        }

        public void SetSkin(int id)
        {
            _cacheID = id;
            bool hasLock = PlayerPrefs.GetInt($"OpenMask-{id}") == 0;
            _btnMask.gameObject.SetActive(hasLock);
            _skelBoy.Skeleton.SetSkin($"Char/B{id}");
            _skelGirl.Skeleton.SetSkin($"Char/G{id}");

            bool unlock = PlayerPrefs.GetInt(Key.SKIN_ID + id) != 0;
            _btnAds.gameObject.SetActive(!unlock);
            _btnClaim.gameObject.SetActive(unlock);
            _btnClaim.interactable = !unlock;

            if (!unlock)
            {
                int n = PlayerPrefs.GetInt(Key.GIFT_ID + id);
                _btnAds.GetComponentInChildren<TextMeshProUGUI>().text = $"{n}/5";
                _btnAds.gameObject.SetActive(n < 5);
                _btnClaim.gameObject.SetActive(n == 5);
                _btnClaim.interactable = true;

                _skelBoy.AnimationState.SetAnimation(0, $"boy_idle", true);
                _skelGirl.AnimationState.SetAnimation(0, $"boy_idle", true);
            }
            else
            {
                _skelBoy.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
                _skelGirl.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            }
        }

        private void OnClickAds()
        {
            AdsManager.Instance.ShowAds(() =>
            {
                int n = PlayerPrefs.GetInt(Key.GIFT_ID + _cacheID) + 1;
                PlayerPrefs.SetInt(Key.GIFT_ID + _cacheID, n);
                _btnAds.GetComponentInChildren<TextMeshProUGUI>().text = $"{n}/5";

                if (n == 5)
                {
                    _btnAds.gameObject.SetActive(false);
                    _btnClaim.gameObject.SetActive(true);
                    _btnClaim.interactable = true;
                }
            });
        }

        private void OnClickClaim()
        {
            _fxClaim.Play();
            PlayerPrefs.SetInt(Key.SKIN_ID + _cacheID, 1);
            _skelBoy.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            _skelGirl.AnimationState.SetAnimation(0, $"boy_win_{Random.Range(1, 12)}", true);
            _btnClaim.interactable = false;
            SoundManager.Instance.PlaySoundInGame(SoundIngame.FireWork);
        }

        private void OnOpenMask()
        {
            SoundManager.Instance.PlaySoundInGame(SoundIngame.UIClick);
            this.transform.DOScaleX(0, 0.3f).OnComplete(() =>
            {
                _btnMask.gameObject.SetActive(false);
                this.transform.DOScaleX(1f, 0.3f).SetDelay(0.1f);
                PlayerPrefs.SetInt($"OpenMask-{_cacheID}",1);
                SoundManager.Instance.PlaySoundInGame(SoundIngame.OpenGift);
            });
        }
    }
}
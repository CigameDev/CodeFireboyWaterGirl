using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace Fireboy
{
    public class TryskinCtrl : MonoBehaviour
    {
        public bool IsFree;
        public SkeletonAnimation MySkel;
        public SkeletonAnimation SkelBoy;
        public SkeletonAnimation SkelGirl;

        private int _id;

        // Start is called before the first frame update
        void Start()
        {
        skin: _id = Random.Range(0, 29);
            if (PlayerPrefs.GetInt(Key.SKIN_ID + _id) != 0)
                goto skin;

            SkelBoy.Skeleton.SetSkin($"Char/B{_id}");
            SkelGirl.skeleton.SetSkin($"Char/G{_id}");
            string anim = IsFree ? "sellect" : "try";
            MySkel.AnimationName = anim;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.tag == TagDefine.FIRE_BOY || collision.tag == TagDefine.WATER_GIRL)
            {
                if (IsFree)
                {
                    PlayerManager.Instance.OnChangeSkin(_id);
                    this.gameObject.SetActive(false);
                }
                else
                {
                    if (AdsManager.Instance.RewardReady())
                    {
                        AdsManager.Instance.ShowAds(() =>
                        {
                            PlayerManager.Instance.OnChangeSkin(_id);
                            this.gameObject.SetActive(false);
                        });
                    }
                }
            }
        }
    }
}
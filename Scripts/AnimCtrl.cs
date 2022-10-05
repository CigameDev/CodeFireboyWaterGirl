using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Fireboy
{
    public class AnimCtrl
    {
        public SkeletonAnimation Skeletion => _skeletonAnimation;
        private SkeletonAnimation _skeletonAnimation;

        private Spine.Bone _targetBone;
        private Transform _gunTarget;

        public AnimCtrl(SkeletonAnimation anim, Transform target)
        {
            this._skeletonAnimation = anim;

        }

        public AnimCtrl(SkeletonAnimation anim)
        {
            this._skeletonAnimation = anim;
        }

        public void SetSkin(int id,Player type)
        {
            string skin = type == Player.Boy ? $"Char/B{id}" : $"Char/G{id}";
            _skeletonAnimation.skeleton.SetSkin(skin);
        }

        public void PlayNewStableAnimation(string animName, bool loop)
        {
            if (_skeletonAnimation.AnimationName.Equals(animName))
                return;

            _skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
        }

        public void PlayNewStableAnimation(string animName, bool loop, UnityAction callback )
        {
            if (_skeletonAnimation.AnimationName.Equals(animName))
                return;

            var current = _skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
            current.Complete += (t) =>
            {
                callback?.Invoke();
            };
        }

        public void PlayNewStableAnimation(string animName, bool loop,string nextAnim)
        {
            if (_skeletonAnimation.AnimationName.Equals(animName))
                return;

            var current = _skeletonAnimation.AnimationState.SetAnimation(0, animName, loop);
            current.Complete += (t) =>
            {
                _skeletonAnimation.AnimationState.SetAnimation(0, nextAnim, true);
            };
        }

        public void AddStableAnimation(string name, bool loop,bool cleartrack = true)
        {
            if (_skeletonAnimation.AnimationState.Tracks.Count > 1)
                _skeletonAnimation.AnimationState.ClearTrack(1);

            var current = _skeletonAnimation.AnimationState.AddAnimation(1, name, loop, 0);

            if (cleartrack)
            {
                current.Complete += (t) =>
                {
                    if (_skeletonAnimation.AnimationState.Tracks.Count > 1)
                        _skeletonAnimation.AnimationState.ClearTrack(1);
                };
            }
        }

        public void AddStableAnimation(string name, bool loop, string nextAnim)
        {
            if (_skeletonAnimation.AnimationState.Tracks.Count > 1)
                _skeletonAnimation.AnimationState.ClearTrack(1);

            var current = _skeletonAnimation.AnimationState.AddAnimation(1, name, loop, 0);

                current.Complete += (t) =>
                {
                    _skeletonAnimation.AnimationState.AddAnimation(1, nextAnim, true, 0);
                };
        }

        public void AddStableAnimation(string firstAnim,string nextAnim)
        {
            if (_skeletonAnimation.AnimationState.Tracks.Count > 1)
                _skeletonAnimation.AnimationState.ClearTrack(1);

            var current = _skeletonAnimation.AnimationState.AddAnimation(1, firstAnim, false, 0);

            current.Complete += (t) =>
            {
                this.AddStableAnimation(nextAnim, false);
            };
        }

        private int _countLoopAnim = 0;
        public void AddAnimationInTime(string name,int loop)
        {
            ClearTrack();
            _countLoopAnim = 0;
            var anim = _skeletonAnimation.AnimationState.AddAnimation(1, name, true, 0);
            anim.Complete += (t) => {
                _countLoopAnim++;
                if (_countLoopAnim >= loop)
                    ClearTrack();
            };
        }


        public void SetTimeScale(float time)
        {
            _skeletonAnimation.timeScale = time;
        }

        public void ClearTrack()
        {
            if (_skeletonAnimation.AnimationState.Tracks.Count > 1)
                _skeletonAnimation.AnimationState.ClearTrack(1);
        }

        public void SetOpacity(float alpha)
        {
            _skeletonAnimation.skeleton.A = alpha;
        }
    }
}

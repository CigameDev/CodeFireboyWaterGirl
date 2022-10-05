#if UNITY_ANDROID
using Google.Play.Review;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fireboy
{
    public class RateCtrl : MonoBehaviour
    {
#if UNITY_ANDROID
        private ReviewManager _reviewManager;
        private PlayReviewInfo _playReviewInfo;


        // Start is called before the first frame update
        void Start()
        {
            _reviewManager = new ReviewManager();
        }

        private void OnEnable()
        {
            Utils.FadeIn(this.gameObject);
        }

        public void OnClickRate()
        {
            StartCoroutine(IEReview());
        }

        public void OnClickHide()
        {
            Utils.FadeOut(this.gameObject, () =>
            {
                this.gameObject.SetActive(false);
            });
        }

        private IEnumerator IEReview()
        {
            Debug.Log($"In-app review");
            var requestFlowOperation = _reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Debug.Log($" request flow error");
                yield break;
            }
            _playReviewInfo = requestFlowOperation.GetResult();

            var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            yield return launchFlowOperation;
            _playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                // Log error. For example, using requestFlowOperation.Error.ToString().
                Debug.Log($"Lunch flow error");
                yield break;
            }

            PlayerPrefs.SetInt(Key.RATE_GAME, 1);
            Debug.Log($"review done");
        }
#endif
    }
}
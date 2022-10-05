using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace Fireboy
{
    public static class Utils 
    {
        public static void SetDotColor(ColorDot color,SpriteRenderer dot)
        {
            switch (color)
            {
                case ColorDot.Green:
                    dot.color = Color.green;
                    break;
                case ColorDot.Pink:
                    dot.color = new Color(211 / 255f, 12 / 255f, 215 / 255f, 1f);
                    break;
                case ColorDot.Red:
                    dot.color = Color.red;
                    break;
                case ColorDot.White:
                    dot.color = Color.white;
                    break;
                case ColorDot.Yellow:
                    dot.color = Color.yellow;
                    break;
                case ColorDot.Blue:
                    dot.color = Color.blue;
                    break;
            }
        }

        public static void SetDarkColor(ColorDot color, SpriteRenderer dot)
        {
            SetDotColor(color, dot);
            Color cl = dot.color;
            cl.a = 0.5f;
            dot.color = cl;
        }

        public static void SetLightColor(ColorDot cl,Color color)
        {
            switch (cl)
            {
                case ColorDot.Green:
                    color = Color.green;
                    break;
                case ColorDot.Pink:
                    color = new Color(211 / 255f, 12 / 255f, 215 / 255f, 1f);
                    break;
                case ColorDot.Red:
                    color = Color.red;
                    break;
                case ColorDot.White:
                    color = Color.white;
                    break;
                case ColorDot.Yellow:
                    color = Color.yellow;
                    break;
            }
        }

        public static void SetDarkColor(ColorDot cl, Color color)
        {
            SetLightColor(cl, color);
            color.a = 0.5f;
        }

        public static void FadeIn(GameObject objFade, UnityAction callback = null)
        {
            CanvasGroup canvas = objFade.GetComponent<CanvasGroup>();
            if (canvas == null) canvas = objFade.AddComponent<CanvasGroup>();
            objFade.transform.localScale = UnityEngine.Vector3.one * 1.15f;
            canvas.alpha = 0.5f;

            objFade.transform.DOScale(UnityEngine.Vector3.one, 0.3f);
            canvas.DOFade(1f, 0.3f).OnComplete(() => { callback?.Invoke(); });
        }

        public static void FadeOut(GameObject objFade, UnityAction callback = null)
        {
            CanvasGroup canvas = objFade.GetComponent<CanvasGroup>();
            if (canvas == null) canvas = objFade.AddComponent<CanvasGroup>();
            objFade.transform.localScale = UnityEngine.Vector3.one;
            canvas.alpha = 1f;

            objFade.transform.DOScale(UnityEngine.Vector3.one * 1.15f, 0.3f);
            canvas.DOFade(0.5f, 0.3f).OnComplete(() =>
            {
                objFade.transform.localScale = UnityEngine.Vector3.zero;
                canvas.alpha = 0f;
                callback?.Invoke();
            });
        }

        public static void AddCoin(int sl,TMPro.TextMeshProUGUI txt)
        {
            int current = PlayerPrefs.GetInt(Key.TOTAL_COIN);
            int remain = current + sl;

            DOTween.To(() => current, (x) =>
            {
                txt.text = x.ToString();
            }, remain, 0.5f);

            PlayerPrefs.SetInt(Key.TOTAL_COIN, remain);
        }

        public static void SetListGift()
        {
            string gt = string.Empty;
            List<int> all = new List<int>();
            for(int i = 0; i < 5; i++)
            {
            back: int n = Random.Range(1, 29);
                if (PlayerPrefs.GetInt(Key.SKIN_ID + n) != 0 || all.Contains(n)) goto back;
                gt += n.ToString() + ",";
                all.Add(n);
            }
            PlayerPrefs.SetString(Key.GIFT_LIST, gt);
            PlayerPrefs.SetString(Key.GIFT_TIME_REFRESH, System.DateTime.Now.ToString());
        }
    }
}
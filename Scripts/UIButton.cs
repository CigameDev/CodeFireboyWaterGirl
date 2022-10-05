using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace Fireboy
{
    public class UIButton : Button
    {
        private Vector3 vec3Default;

        private Vector2 buttonSize;

        protected override void Awake()
        {
            vec3Default = transform.localScale;
        }

        protected override void Start()
        {
            buttonSize = transform.GetComponent<RectTransform>().rect.size;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (this.IsInteractable() == false) return;

            if (buttonSize.x < 800) transform.DOScale(new Vector3(1.1f, 1.1f, 1), 0.1f);
            else transform.DOScale(new Vector3(1.05f, 1.05f, 1), 0.1f);

        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (this.IsInteractable() == false) return;
            transform.DOScale(vec3Default, 0.1f);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (this.IsInteractable() == false) return;
            transform.DOScale(new Vector3(0.9f, 0.9f, 1), 0.1f);
            SoundManager.Instance?.PlaySoundInGame(SoundIngame.UIClick);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (this.IsInteractable() == false) return;
            transform.DOScale(vec3Default, 0.1f);
        }
    }
}

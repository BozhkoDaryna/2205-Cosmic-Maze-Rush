using System;
using System.Threading;
using Application.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Application.UI
{
    public class PausePopup : StopPopup, IPointerClickHandler
    {
        [SerializeField] protected Button _cancelButton;

        public event Action CancelEvent;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            AudioService.PlaySound(ConstAudio.OpenPopupSound);

            SubscribeEvents();
            BlockInput(true);

            return base.Show(data, cancellationToken);
        }

        protected override void SubscribeEvents()
        {
            _cancelButton.onClick.AddListener(Cancel);
            base.SubscribeEvents();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsClickOverUI(eventData.position, _restartLevelButton) &&
                !IsClickOverUI(eventData.position, _menuButton))
                Cancel();
        }

        protected override void RemoveListeners()
        {
            _cancelButton.onClick.RemoveAllListeners();
            base.RemoveListeners();
        }

        private bool IsClickOverUI(Vector2 position, Button button)
        {
            var rectTransform = button.GetComponent<RectTransform>();
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, position, null);
        }

        private void Cancel()
        {
            BlockInput(false);
            CancelEvent?.Invoke();
            AudioService.PlaySound(ConstAudio.PressButtonSound);
            RemoveListeners();
        }
    }
}
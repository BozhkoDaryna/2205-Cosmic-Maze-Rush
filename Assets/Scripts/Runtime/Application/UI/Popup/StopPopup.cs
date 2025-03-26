using System;
using Application.Services.Audio;
using Core.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class StopPopup : BasePopup
    {
        [SerializeField] protected Button _restartLevelButton;
        [SerializeField] protected Button _menuButton;
        [SerializeField] protected Image _blockInput;

        public event Action OpenMenuEvent;
        public event Action RestartLevelEvent;

        protected virtual void SubscribeEvents()
        {
            _restartLevelButton.onClick.AddListener(RestartLevel);
            _menuButton.onClick.AddListener(OpenMenu);
        }

        protected virtual void RemoveListeners()
        {
            _menuButton.onClick.RemoveAllListeners();
            _restartLevelButton.onClick.RemoveAllListeners();
        }

        protected void BlockInput(bool isBlock)
        {
            _blockInput.gameObject.SetActive(isBlock);
        }

        private void OpenMenu()
        {
            BlockInput(false);
            AudioService.PlaySound(ConstAudio.OpenPopupSound);
            OpenMenuEvent?.Invoke();
            RemoveListeners();
            DestroyPopup();
        }

        private void RestartLevel()
        {
            RestartLevelEvent?.Invoke();
            BlockInput(false);
            AudioService.PlaySound(ConstAudio.PressButtonSound);
            RemoveListeners();
            DestroyPopup();
        }
    }
}
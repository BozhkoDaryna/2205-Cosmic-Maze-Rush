using System;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class MenuScreenView : UiScreen
    {
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private RectTransform _buttonContainer;

        public event Action SettingsPressedEvent;
        public event Action ExitPressedEvent;

        public RectTransform ButtonContainer => _buttonContainer;

        private void OnDestroy()
        {
            _settingsButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        public void Initialize()
        {
            _settingsButton.onClick.AddListener(HandleSettingsPress);
            _exitButton.onClick.AddListener(HandleExitPress);
        }

        private void HandleSettingsPress()
        {
            SettingsPressedEvent?.Invoke();
        }

        private void HandleExitPress()
        {
            ExitPressedEvent?.Invoke();
        }
    }
}
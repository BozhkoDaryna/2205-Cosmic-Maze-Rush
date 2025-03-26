using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class TitleScreen : UiScreen
    {
        private readonly Dictionary<ButtonTypes, Button> _buttons = new Dictionary<ButtonTypes, Button>();

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _infoButton;

        public event Action PlayButtonPressEvent;
        public event Action SettingsButtonPressEvent;
        public event Action InfoButtonPressEvent;

        public override UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            _playButton.onClick.AddListener(OnPlayButtonPressed);
            _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
            _infoButton.onClick.AddListener(OnInfoButtonPressed);
            return base.ShowAsync(cancellationToken);
        }

        public override void HideImmediately()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonPressed);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonPressed);
            _infoButton.onClick.RemoveListener(OnInfoButtonPressed);

            base.HideImmediately();
        }

        public void RestoreButton(ButtonTypes buttonTypes)
        {
            if (_buttons.ContainsKey(buttonTypes))
            {
                _buttons[buttonTypes].interactable = true;
                _buttons.Remove(buttonTypes);
            }
        }

        private void RemoveButton(ButtonTypes buttonTypes, Button button)
        {
            if (_buttons.ContainsKey(buttonTypes) == false)
            {
                _buttons.Add(buttonTypes, button);
                _buttons[buttonTypes].interactable = false;
            }
        }

        private void OnInfoButtonPressed()
        {
            RemoveButton(ButtonTypes.Info, _infoButton);
            InfoButtonPressEvent?.Invoke();
        }

        private void OnSettingsButtonPressed()
        {
            RemoveButton(ButtonTypes.Settings, _settingsButton);
            SettingsButtonPressEvent?.Invoke();
        }

        private void OnPlayButtonPressed()
        {
            RemoveButton(ButtonTypes.Play, _playButton);
            PlayButtonPressEvent?.Invoke();
        }
    }

    public enum ButtonTypes
    {
        None,
        Play,
        Settings,
        Info
    }
}
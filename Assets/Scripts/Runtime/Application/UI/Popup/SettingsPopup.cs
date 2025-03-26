using System;
using System.Threading;
using Application.Services.Audio;
using Core.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Application.UI
{
    public class SettingsPopup : BasePopup
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Toggle _soundVolumeToggle;
        [SerializeField] private Toggle _musicVolume;

        private bool _musicButtonPreviousValue;
        private bool _soundButtonPreviousValue;

        public event Action<bool> SoundVolumeChangeEvent;
        public event Action<bool> MusicVolumeChangeEvent;

        public new event Action<ButtonTypes> HideImmediatelyEvent;

        public override UniTask Show(BasePopupData data, CancellationToken cancellationToken = default)
        {
            var settingsPopupData = data as SettingsPopupData;

            _soundButtonPreviousValue = settingsPopupData.IsSoundVolume;
            _soundVolumeToggle.onValueChanged.Invoke(_soundButtonPreviousValue);
            _soundVolumeToggle.isOn = _soundButtonPreviousValue;

            _musicButtonPreviousValue = settingsPopupData.IsMusicVolume;
            _musicVolume.onValueChanged.Invoke(_musicButtonPreviousValue);
            _musicVolume.isOn = _musicButtonPreviousValue;

            _closeButton.onClick.AddListener(Reset);
            _saveButton.onClick.AddListener(SaveSettings);

            _soundVolumeToggle.onValueChanged.AddListener(OnSoundVolumeToggleValueChanged);
            _musicVolume.onValueChanged.AddListener(OnMusicVolumeToggleValueChanged);

            return base.Show(data, cancellationToken);
        }

        public override void DestroyPopup()
        {
            HideImmediatelyEvent?.Invoke(ButtonTypes.Settings);

            base.DestroyPopup();
        }

        private void SaveSettings()
        {
            var soundVolumeValue = _soundVolumeToggle.isOn;
            var musicVolumeIsOn = _musicVolume.isOn;

            SoundVolumeChangeEvent?.Invoke(soundVolumeValue);
            MusicVolumeChangeEvent?.Invoke(musicVolumeIsOn);

            DestroyPopup();
        }

        private void Reset()
        {
            SoundVolumeChangeEvent?.Invoke(_soundButtonPreviousValue);
            MusicVolumeChangeEvent?.Invoke(_musicButtonPreviousValue);

            DestroyPopup();
        }

        private void OnSoundVolumeToggleValueChanged(bool value)
        {
            AudioService.PlaySound(ConstAudio.PressButtonSound);
            SoundVolumeChangeEvent?.Invoke(value);
        }

        private void OnMusicVolumeToggleValueChanged(bool value)
        {
            AudioService.PlaySound(ConstAudio.PressButtonSound);
            MusicVolumeChangeEvent?.Invoke(value);
        }
    }
}
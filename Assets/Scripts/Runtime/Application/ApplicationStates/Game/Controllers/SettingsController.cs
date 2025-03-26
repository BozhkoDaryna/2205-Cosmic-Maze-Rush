using Application.Services.UserData;
using Core.Services.Audio;

namespace Application.Game
{
    public class SettingsController : ISettingsController
    {
        private readonly UserDataService _userDataService;
        private readonly IAudioService _audioService;

        private UserData _userData;

        public SettingsController(UserDataService userDataService, IAudioService audioService)
        {
            _audioService = audioService;
            _userDataService = userDataService;
        }

        public void OnChangeSoundVolume(bool isEnabled)
        {
            _userData = _userDataService.GetUserData();
            _userData.SettingsData.IsSoundVolume = isEnabled;

            EnableAndDisableSound(AudioType.Sound, isEnabled ? 1 : 0);
        }

        public void OnChangeMusicVolume(bool isEnabled)
        {
            _userData = _userDataService.GetUserData();
            _userData.SettingsData.IsMusicVolume = isEnabled;

            EnableAndDisableSound(AudioType.Music, isEnabled ? 1 : 0);
        }

        private void EnableAndDisableSound(AudioType audioType, int volume)
        {
            _audioService.SetVolume(audioType, volume);
        }
    }
}
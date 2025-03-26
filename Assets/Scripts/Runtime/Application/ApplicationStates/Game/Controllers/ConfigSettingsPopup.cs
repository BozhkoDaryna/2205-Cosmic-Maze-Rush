using Application.Services.UserData;
using Application.UI;

namespace Application.Game
{
    public class ConfigSettingsPopup : IConfigPopup<SettingsPopup, SettingsPopupData>
    {
        private readonly UserDataService _userDataService;
        private readonly ISettingsController _settingsController;

        public ConfigSettingsPopup(UserDataService userDataService, ISettingsController settingsController)
        {
            _settingsController = settingsController;
            _userDataService = userDataService;
        }

        public SettingsPopupData GetConfig(SettingsPopup popup)
        {
            var userData = _userDataService.GetUserData();
            var settingsData = new SettingsPopupData
            {
                IsMusicVolume = userData.SettingsData.IsMusicVolume,
                IsSoundVolume = userData.SettingsData.IsSoundVolume
            };

            SubscribePopupToChangeDataController(popup);
            return settingsData;
        }

        private void SubscribePopupToChangeDataController(SettingsPopup popup)
        {
            popup.SubscribeToEvents(_settingsController);
        }
    }
}
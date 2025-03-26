using Application.Services.UserData;
using Core.UI;

namespace Application.UI
{
    public class SettingsPopupData : BasePopupData
    {
        public bool IsSoundVolume { get; set; }

        public bool IsMusicVolume { get; set; }

        public GameDifficultyMode GameDifficultyMode { get; set; }
    }
}
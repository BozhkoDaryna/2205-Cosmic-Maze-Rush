using System;

namespace Application.Services.UserData
{
    [Serializable]
    public class SettingsData
    {
        public bool IsSoundVolume = true;
        public bool IsMusicVolume = true;
        public GameDifficultyMode GameDifficultyMode;
    }

    public enum GameDifficultyMode
    {
        Easy,
        Hard
    }
}
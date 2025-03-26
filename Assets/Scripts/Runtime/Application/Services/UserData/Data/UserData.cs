using System;
using System.Collections.Generic;

namespace Application.Services.UserData
{
    [Serializable]
    public class UserData
    {
        public List<GameSessionData> GameSessionData = new List<GameSessionData>();
        public SettingsData SettingsData = new SettingsData();
        public GameData GameData = new GameData();
        public LevelsData LevelsData = new LevelsData();

        public void TryAddLevel(int level)
        {
            if (!LevelsData.Levels.Contains(level))
                LevelsData.Levels.Add(level);
        }
    }
}
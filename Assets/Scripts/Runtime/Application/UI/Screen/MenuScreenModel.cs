using System.Collections.Generic;
using Application.Services.UserData;

namespace Application.UI
{
    public class MenuScreenModel
    {
        private readonly List<LevelModel> _levels = new List<LevelModel>();
        private readonly UserDataService _userDataService;

        public MenuScreenModel(UserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        public void LoadLevels()
        {
            var userData = _userDataService.GetUserData();
            foreach (var levelData in userData.LevelsData.Levels)
            {
                var levelModel = new LevelModel();
                levelModel.SetLevel(levelData);
                _levels.Add(levelModel);
            }
        }

        public LevelModel GetLevelModel(int index)
        {
            if (index >= 0 && index < _levels.Count)
                return _levels[index];
            return null;
        }

        public void ClearLevels()
        {
            _levels.Clear();
        }
    }
}
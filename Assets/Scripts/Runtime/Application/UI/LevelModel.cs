using Application.Services;

namespace Application.UI
{
    public class LevelModel
    {
        public int CurrentLevel;
        public bool IsEndedLevels { get; private set; }

        public void SetLevel(int level)
        {
            CurrentLevel = level;
            
            if (level >= ConstGame.LevelCount - 1)
                IsEndedLevels = true;
        }

        public string GetFormattedText()
        {
            return $"{CurrentLevel + 1} \nLevel";
        }
    }
}
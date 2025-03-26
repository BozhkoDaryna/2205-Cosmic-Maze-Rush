using Application.Level;
using Application.UI;
using Runtime.Game;
using Runtime.MazeGenerator;
using UnityEngine;

namespace Application.Game
{
    public class GameModel : ILevelDeterminator
    {
        private readonly MazeGenerator _mazeGenerator;
        private readonly LevelLoaderController _levelLoaderController;
        private readonly TimerController _timerController;

        private int CurrentLevel { get; set; }

        public bool IsPaused { get; private set; }

        public GameModel(LevelLoaderController levelLoaderController, MazeGenerator mazeGenerator,
            TimerController timerController)
        {
            _timerController = timerController;
            _levelLoaderController = levelLoaderController;
            _mazeGenerator = mazeGenerator;
            CurrentLevel = levelLoaderController.GetCurrentLevel();
        }

        public void SetPauseState(bool isPaused)
        {
            IsPaused = isPaused;
            Time.timeScale = isPaused ? 0 : 1;
        }

        public void SetDefiniteLevel(int level)
        {
            _levelLoaderController.SetFollowingLevelIndex(level);
            CurrentLevel = level;
        }

        public void IncrementLevel()
        {
            _levelLoaderController.IncreaseLevelIndex();
            CurrentLevel = _levelLoaderController.GetCurrentLevel();
        }

        public void LoadCurrentMaze()
        {
            _mazeGenerator.LoadFollowingLevel(_levelLoaderController.GetCurrentLevelData());
        }

        public void LoadTimerData()
        {
            _timerController.SetTimerData(_levelLoaderController.GetTimerData());
        }

        public int GetNextLevel()
        {
            var nextLevel = CurrentLevel + 1;
            return nextLevel;
        }
    }
}
using System.Collections.Generic;
using System.Threading;
using Application.Services;
using Application.Services.UserData;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.Level
{
    public class LevelLoaderController : BaseController
    {
        private readonly Dictionary<int, MazeData> _levelsDictionary = new Dictionary<int, MazeData>();
        private readonly Dictionary<int, TimerData> _datasDictionary = new Dictionary<int, TimerData>();

        private readonly List<TimerData> _timerData;
        private readonly List<MazeData> _mazeData;

        private int _levelIndex;

        public LevelLoaderController(List<MazeData> mazeData, List<TimerData> timerData)
        {
            _timerData = timerData;
            _mazeData = mazeData;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            InitializeLevels();

            return base.Run(cancellationToken);
        }

        public void SetFollowingLevelIndex(int level)
        {
            if (_levelIndex < ConstGame.LevelCount && level >= 0)
                _levelIndex = level;
        }

        public void IncreaseLevelIndex()
        {
            if (_levelIndex < ConstGame.LevelCount - 1)
                _levelIndex++;
        }

        public MazeData GetCurrentLevelData()
        {
            return _levelsDictionary[_levelIndex];
        }

        public TimerData GetTimerData()
        {
            return _datasDictionary[_levelIndex];
        }

        public int GetCurrentLevel()
        {
            return _levelIndex;
        }

        private void InitializeLevels()
        {
            for (var i = 0; i < ConstGame.LevelCount; i++)
            {
                _datasDictionary.Add(i, _timerData[i]);
                _levelsDictionary.Add(i, _mazeData[i]);
            }
        }
    }
}
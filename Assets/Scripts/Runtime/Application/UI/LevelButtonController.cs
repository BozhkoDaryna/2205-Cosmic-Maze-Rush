using System.Collections.Generic;
using System.Threading;
using Core;
using Cysharp.Threading.Tasks;

namespace Application.UI
{
    public class LevelButtonController : BaseController
    {
        private readonly ILevelDeterminator _levelDeterminator;
        private readonly List<LevelButtonView> _levelButtonViews = new List<LevelButtonView>();
        private readonly List<LevelModel> _levelModels = new List<LevelModel>();

        public LevelButtonController(ILevelDeterminator levelDeterminator)
        {
            _levelDeterminator = levelDeterminator;
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            foreach (var levelButtonView in _levelButtonViews)
                levelButtonView.Initialize();

            return base.Run(cancellationToken);
        }

        public override UniTask Stop()
        {
            for (var i = 0; i < _levelButtonViews.Count; i++)
            {
                var i1 = i;
                _levelButtonViews[i].HandleClickEvent -= () => SwitchLevel(_levelModels[i1].CurrentLevel);
                _levelButtonViews[i].Cleanup();
            }

            return base.Stop();
        }

        public void SetButton(LevelButtonView levelButtonView, LevelModel levelModel)
        {
            _levelModels.Add(levelModel);
            _levelButtonViews.Add(levelButtonView);
        }

        public void InitializeButtons()
        {
            for (var i = 0; i < _levelButtonViews.Count; i++)
            {
                _levelButtonViews[i].Initialize();
                _levelButtonViews[i].UpdateText(_levelModels[i].GetFormattedText());
                var i1 = i;
                _levelButtonViews[i].HandleClickEvent += () => SwitchLevel(_levelModels[i1].CurrentLevel);
            }
        }

        private void SwitchLevel(int level)
        {
            _levelDeterminator.SetDefiniteLevel(level);
        }
    }
}
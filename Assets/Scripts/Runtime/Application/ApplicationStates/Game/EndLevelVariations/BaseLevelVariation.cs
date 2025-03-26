using System;
using Core;
using Core.Services.Audio;

namespace Application.EndLevelVariations
{
    public abstract class BaseLevelVariation : BaseController
    {
        private protected readonly IAudioService AudioService;

        public event Action<GameResult> FinishedLevel;

        private protected BaseLevelVariation(IAudioService audioService)
        {
            AudioService = audioService;
        }

        protected void OnFinishedLevel(GameResult result)
        {
            FinishedLevel?.Invoke(result);
        }
    }
}
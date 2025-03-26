using System.Threading;
using Application.Services.Audio;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;
using Runtime.Game;

namespace Application.EndLevelVariations
{
    public class LoseLevelVariation : BaseLevelVariation
    {
        private readonly LightScaler _lightScaler;

        public LoseLevelVariation(IAudioService audioService, LightScaler lightScaler) : base(audioService)
        {
            _lightScaler = lightScaler;
        }

        public override async UniTask Run(CancellationToken cancellationToken)
        {
            await _lightScaler.WaitUntilLightRemoved();
            
            if(cancellationToken.IsCancellationRequested)
                return;
            
            AudioService.PlayMusic(ConstAudio.LoseSound);

            OnFinishedLevel(GameResult.Lose);
            await base.Run(cancellationToken);
        }
    }
}
using System.Threading;
using Application.Services.Audio;
using Core.Services.Audio;
using Cysharp.Threading.Tasks;

namespace Application.EndLevelVariations
{
    public class VictoryLevelVariation : BaseLevelVariation
    {
        public VictoryLevelVariation(IAudioService audioService) : base(audioService)
        {
        }

        public override UniTask Run(CancellationToken cancellationToken)
        {
            if(cancellationToken.IsCancellationRequested)
                return UniTask.CompletedTask;
            
            AudioService.PlayMusic(ConstAudio.VictorySound);

            OnFinishedLevel(GameResult.Victory);
            return base.Run(cancellationToken);
        }
    }
}
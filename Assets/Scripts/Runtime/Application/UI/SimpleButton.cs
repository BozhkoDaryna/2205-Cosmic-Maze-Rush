using Application.Services.Audio;
using Core.Services.Audio;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Application.UI
{
    public class SimpleButton : MonoBehaviour
    {
        [SerializeField] protected Button _button;
        [SerializeField] private Animation _pressAnimation;

        private IAudioService _audioService;
        public Button Button => _button;

        [Inject]
        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void PlayPressAnimation()
        {
            _pressAnimation.Play();
            _audioService.PlaySound(ConstAudio.PressButtonSound);
        }
    }
}